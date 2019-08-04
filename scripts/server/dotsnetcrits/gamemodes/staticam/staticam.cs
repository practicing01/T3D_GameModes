function StaticamGMServer::Move(%this, %client, %dir)
{
  %client.player.curDir_ = %dir;

  if (%dir == 0)
  {
    %client.player.stop();
    return;
  }

  %forward = %client.player.getForwardVector();

  %newDir = VectorScale(%forward, 1000 * %dir);
  %dest = VectorAdd(%client.player.position, %newDir);

  %client.player.setMoveDestination(%dest);
  //%client.player.setPathDestination(%dest);
}

function StaticamGMServer::Rotate(%this, %client, %dir)
{
  if (%dir == 0)
  {
    cancel(%client.player.rotateSchedule_);
    %client.player.rotateSchedule_ = -1;
    return;
  }
  else if (%client.player.rotateSchedule_ != -1)
  {
    cancel(%client.player.rotateSchedule_);
  }

  %objTransform = %client.player.getTransform();
  %tryRotation = "0 0 0 0 0 1 " @ mDegToRad(%dir * 10);
  %newTransform = matrixMultiply(%objTransform, %tryRotation);
  %client.player.setTransform(%newTransform);

  %forward = %client.player.getForwardVector();
  %forward = VectorScale(%forward, 1000);
  %projection = VectorAdd(%client.player.position, %forward);
  %client.player.setAimLocation(%projection);

  if (%client.player.curDir_ != 0)
  {
    %client.player.setMoveDestination(VectorScale(%projection, %client.player.curDir_));
    //%client.player.setPathDestination(%dest);
  }

  %client.player.rotateSchedule_ = %this.schedule(100, "Rotate", %client, %dir);
}

function StaticamGMServer::Fire(%this, %client)
{
  %client.player.fire(true);
  %client.player.fire(false);
}

function serverCmdMoveStaticamGM(%client, %dir)
{
  if (isObject(StaticamGMServerSO))
  {
    StaticamGMServerSO.Move(%client, %dir);
  }
}

function serverCmdRotateStaticamGM(%client, %dir)
{
  if (isObject(StaticamGMServerSO))
  {
    StaticamGMServerSO.Rotate(%client, %dir);
  }
}

function serverCmdFireStaticamGM(%client)
{
  if (isObject(StaticamGMServerSO))
  {
    StaticamGMServerSO.Fire(%client);
  }
}

function StaticamGMServer::RandomizeCamera(%this, %client)
{
  %randyDir = "0 0 0";
  %randyDir.x = getRandom(-1, 1);
  %randyDir.y = getRandom(-1, 1);
  %randyDir.z = getRandom(0, 1);

  %startPos = VectorAdd(%client.player.position, "0 0 1");

  %endPos = VectorAdd(%startPos, VectorScale(%randyDir, 10));

  %rayResult = containerRayCast(%startPos, %endPos, %this.rayMask_, %client.player);
  %objTarget = firstWord(%rayResult);

  if (isObject(%objTarget))
  {
    %randyDir = VectorScale(%randyDir, -2);
    %endPos = getWords(%rayResult, 1, 3);
    %endPos = VectorAdd(%endPos, %randyDir);
  }

  %client.camera.position = %endPos;

  %client.camera.lookAt(VectorAdd(%client.player.position, "0 0 1"));
}

function StaticamGMServer::CheckCamera(%this, %client)
{
  if (%client.camera.checkInFoV(%client.player, 45))
  {
    %rayResult = containerRayCast(%client.player.position, %client.camera.position, %this.rayMask_, %client.player);
    %objTarget = firstWord(%rayResult);

    if (isObject(%objTarget))
    {
      %this.RandomizeCamera(%client);
    }
    else if (VectorDist(%client.player.position, %client.camera.position) >= 20)
    {
      %this.RandomizeCamera(%client);
    }
  }
  else
  {
    %this.RandomizeCamera(%client);
  }

  %client.player.camCheckSchedule_ = %this.schedule(1000, "CheckCamera", %client);
}

function StaticamGMServer::SetupCamera(%this, %client)
{
  if (isObject(%client.camera))
  {
    %client.camera.delete();

    %client.camera = spawnObject($Game::DefaultCameraClass, $Game::DefaultCameraDataBlock);

    MissionCleanup.add(%client.camera);
  }
  %client.camera.scopeToClient(%client);
  %client.setControlObject(%client.camera);
  %client.camera.controlMode = "Stationary";
  %client.camera.position = VectorAdd(%client.player.position, "0 -10 10");
  %client.camera.lookAt(%client.player.position);

  commandToClient(%client, 'RebindStaticamGM');

  %client.player.camCheckSchedule_ = %this.schedule(1000, "CheckCamera", %client);
}

function StaticamAIClass::onRemove(%this)
{
  cancel(%this.rotateSchedule_);
  cancel(%this.camCheckSchedule_);
}

function StaticamGMServer::SpawnNPC(%this, %client)
{
  %player = %client.getControlObject();

  %playerSO = new ScriptObject()
  {
    npc_ = "";
  };

  %playerSO.npc_ = new AiPlayer()
  {
    dataBlock = %player.dataBlock;
    class = "StaticamAIClass";
    client = %client;
    curDir_ = 0;//0 = idle, 1 = forward, -1 = backward
    rotateSchedule_ = -1;
    camCheckSchedule_ = "";
  };

  %playerSO.npc_.setTransform(%player.getTransform());
  %this.dictionary_.add(%client, %playerSO);

  %client.player.delete();
  %client.player = %playerSO.npc_;

  %forward = %client.player.getForwardVector();
  %forward = VectorScale(%forward, 10);
  %projection = VectorAdd(%client.player.position, %forward);
  %client.player.setAimLocation(%projection);

  %client.player.mountImage(fistClubImage, 0);

  %this.SetupCamera(%client);
  %this.RandomizeCamera(%client);
}

function StaticamGMServer::onAdd(%this)
{
  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "StaticamGMServerQueue";

  %this.dictionary_ = new ArrayObject();

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);

    %this.SpawnNPC(%client);
  }
}

function StaticamGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.dictionary_))
  {
    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      %key = %this.dictionary_.getIndexFromKey(%client);
      %playerSO = %this.dictionary_.getValue(%key);

      %playerSO.npc_.delete();
      cancel(%playerSO.npc_.rotateSchedule_);
      cancel(%playerSO.npc_.camCheckSchedule_);
    }
    %this.dictionary_.delete();
  }

  if (isObject(PlayerDropPoints))
  {
    %spawnPoint = PlayerDropPoints.getRandom();

    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      %client.spawnPlayer(%spawnPoint);
    }
  }
}

function StaticamGMServer::loadOut(%this, %player)
{
  %client = %player.client;
  %key = %this.dictionary_.getIndexFromKey(%client);
  %npc = "";

  if (%key == -1)
  {
    %this.SpawnNPC(%client);
    return;
  }
  else
  {
    %npcSO = %this.dictionary_.getValue(%key);

    if (isObject(%npcSO.npc_))
    {
      %npcSO.npc_.delete();
    }

    %npcSO.npc_ = new AiPlayer()
    {
      dataBlock = %player.dataBlock;
      class = "StaticamAIClass";
      client = %client;
      curDir_ = 0;//0 = idle, 1 = forward, -1 = backward
      rotateSchedule_ = -1;
      camCheckSchedule_ = "";
    };

    %npcSO.npc_.setTransform(%player.getTransform());

    %client.player.delete();
    %client.player = %npcSO.npc_;

    %forward = %client.player.getForwardVector();
    %forward = VectorScale(%forward, 10);
    %projection = VectorAdd(%client.player.position, %forward);
    %client.player.setAimLocation(%projection);

    %client.player.mountImage(fistClubImage, 0);

    %client.setControlObject(%client.player);

    %this.SetupCamera(%client);
    %this.RandomizeCamera(%client);
  }
}

if (isObject(StaticamGMServerSO))
{
  StaticamGMServerSO.delete();
}
else
{
  new ScriptObject(StaticamGMServerSO)
  {
    class = "StaticamGMServer";
    EventManager_ = "";
    dictionary_ = "";
    rayMask_ = "";
  };

  DNCServer.loadOutListeners_.add(StaticamGMServerSO);
  MissionCleanup.add(StaticamGMServerSO);
}
