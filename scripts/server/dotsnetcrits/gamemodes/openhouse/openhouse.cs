function OpenHouseGMServer::Fire(%this, %client, %unprojection)
{
  %startPos = %client.camera.position;
  %dir = VectorSub(%unprojection, %startPos);
  %dir = VectorNormalize(%dir);
  %dir = VectorScale(%dir, 1000);
  %endPos = VectorAdd(%startPos, %dir);

  %rayResult = containerRayCast(%startPos, %endPos, $TypeMasks::DynamicShapeObjectType, %client.camera);

  %objTarget = firstWord(%rayResult);

  if (!isObject(%objTarget))
  {
    return;
  }

  if (%objTarget.class !$= "OpenHouseAIClass")
  {
    return;
  }

  %objTarget.damage(%client.player, "0 0 0", 34, "hit");
}

function serverCmdFireOpenHouseGM(%client, %unprojection)
{
  if (isObject(OpenHouseGMServerSO))
  {
    OpenHouseGMServerSO.Fire(%client, %unprojection);
  }
}

function OpenHouseGMServer::RespawnNPCs(%this)
{
  %this.npcKillCount_ = 0;

  for (%x = 0; %x < %this.npcMax_; %x++)
  {
    %pos = DNCServer.GetRandomVector(%this.dummyCam_.position, 3);

    %npc = %this.npcs_.getObject(%x);
    %npc.position = %pos;
    %npc.setHidden(false);
    %npc.setAimLocation(%this.camera_.position);
    %npc.setDamageLevel(0);
    %npc.setActionThread("idle");
    //%npc.playThread(0, "idle");

    %npc.attackSchedule_ = %npc.schedule(1000, "Attack");
  }
}

function OpenHouseClass::NextNode(%this)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  if (%spawnPoint == %this.parent_.curSpawnPoint_)
  {
    %this.schedule_ = %this.schedule(1000, "NextNode");
    return;
  }

  %this.parent_.curSpawnPoint_ = %spawnPoint;

  %this.parent_.camera_.reset(%this.parent_.camSpeed_);

  %hitPos = DNCServer.GetRayHitPos("0 0 -1", 1000, %this.parent_.camHeight_, %this.parent_.rayMask_, %spawnPoint);

  %this.parent_.dummyCam_.position = %this.parent_.camera_.position;
  %this.parent_.dummyCam_.lookAt(%hitPos);
  %this.parent_.dummyCam_.position = %hitPos;

  %this.parent_.camera_.pushBack(%this.parent_.dummyCam_.getTransform(), %this.parent_.camSpeed_, "Kink", "Linear");
  %this.parent_.camera_.setState();
}

function OpenHouseClass::onNode(%this, %node)
{
  %dir = %this.getForwardVector();
  %offset = VectorScale(%dir, 3);
  %endPos = VectorAdd(%this.position, %offset);

  %this.parent_.dummyCam_.position = %endPos;
  %this.parent_.dummyCam_.lookAt(%this.position);

  %this.parent_.RespawnNPCs();

  %this.schedule_ = %this.schedule(5000, "NextNode");
}

function OpenHouseGMServer::SetupCamera(%this, %client, %npc)
{
  if (isObject(%client.camera) && isObject(%client.player))
  {
    %client.player.delete();
    %client.camera.delete();
    %client.camera = %npc;
    %client.camera.scopeToClient(%client);
    %client.setControlObject(%client.camera);
  }
}

function OpenHouseAI::onDisabled(%this, %ai, %state)
{
  cancel(%ai.attackSchedule_);
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
  %ai.setHidden(true);

  %clientCount = ClientGroup.getCount();
  for (%x = 0; %x < %clientCount; %x++)
  {
    %client = ClientGroup.getObject(%x);
    Game.incScore(%client, 1, false);
  }

  %ai.parent_.npcKillCount_++;

  if (%ai.parent_.npcKillCount_ >= %ai.parent_.npcMax_)
  {
    cancel(%ai.parent_.camera_.schedule_);
    %ai.parent_.camera_.NextNode();
  }
}

function OpenHouseAI::animationDone(%this, %ai)
{
  %clientCount = ClientGroup.getCount();
  for (%x = 0; %x < %clientCount; %x++)
  {
    %client = ClientGroup.getObject(%x);
    Game.incScore(%client, -1, false);
  }

  %ai.schedule(0, "setActionThread", "idle");
  %ai.attackSchedule_ = %ai.schedule(1000, "Attack");
}

function OpenHouseAIClass::Attack(%this)
{
  %this.setActionThread("attack");
}

function OpenHouseAI::onRemove(%this, %ai)
{
  cancel(%ai.attackSchedule_);
}

function OpenHouseGMServer::SpawnNPC(%this)
{
  %npc = new AiPlayer()
  {
     dataBlock = "OpenHouseAI";
     class = "OpenHouseAIClass";
     state_ = "idle";
     followTarget_ = "";
     parent_ = %this;
     attackSchedule_ = "";
  };

  MissionCleanup.add(%npc);

  %this.npcs_.add(%npc);

  %npc.setHidden(true);
}

function OpenHouseGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "OpenHouseGMServerQueue";

  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.npcs_ = new SimSet();

  for (%x = 0; %x < %this.npcMax_; %x++)
  {
    %this.SpawnNPC();
  }

  %spawnPoint = PlayerDropPoints.getRandom();
  %hitPos = DNCServer.GetRayHitPos("0 0 -1", 1000, %this.camHeight_, %this.rayMask_, %spawnPoint);

  %this.camera_ = new PathCamera()
  {
    dataBlock = "OpenHouseCamDB";
    class = "OpenHouseClass";
    position = %hitPos;
    parent_ = %this;
    schedule_ = "";
  };

  %this.dummyCam_ = new Camera()
  {
    dataBlock = "DNCDummyCamDB";
    position = %hitPos;
  };

  %this.camera_.reset(%this.camSpeed_);
  %spawnPoint = PlayerDropPoints.getRandom();

  %hitPos = DNCServer.GetRayHitPos("0 0 -1", 1000, %this.camHeight_, %this.rayMask_, %spawnPoint);

  %this.dummyCam_.position = %this.camera_.position;
  %this.dummyCam_.lookAt(%hitPos);
  %this.dummyCam_.position = %hitPos;

  %this.camera_.pushBack(%this.dummyCam_.getTransform(), %this.camSpeed_, "Kink", "Linear");
  %this.camera_.setState();

  %clientCount = ClientGroup.getCount();
  for (%x = 0; %x < %clientCount; %x++)
  {
    %client = ClientGroup.getObject(%x);

    %this.SetupCamera(%client, %this.camera_);
  }
}

function OpenHouseGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.camera_))
  {
    cancel(%this.camera_.schedule_);
    %this.camera_.delete();
  }

  if (isObject(%this.dummyCam_))
  {
    %this.dummyCam_.delete();
  }

  if (isObject(%this.npcs_))
  {
    %this.npcs_.deleteAllObjects();
    %this.npcs_.delete();
  }

  DNCServer.loadOutListeners_.remove(OpenHouseGMServerSO);

  if (isObject(PlayerDropPoints))
  {
    %clientCount = ClientGroup.getCount();
    for (%x = 0; %x < %clientCount; %x++)
    {
      %spawnPoint = PlayerDropPoints.getRandom();

      %client = ClientGroup.getObject(%x);

      if (isObject(%client.player))
      {
        %client.player.delete();
      }

      if (isObject(%client.camera))
      {
        %client.camera.delete();
      }

      %client.camera = spawnObject($Game::DefaultCameraClass, $Game::DefaultCameraDataBlock);
      MissionCleanup.add(%client.camera);
      %client.camera.scopeToClient(%client);
      %client.setControlObject(%client.camera);
      %client.spawnPlayer(%spawnPoint);
    }
  }
}

function OpenHouseGMServerSO::loadOut(%this, %player)
{
  %client = %player.client;

  %this.SetupCamera(%client, %this.camera_);
}

if (isObject(OpenHouseGMServerSO))
{
  OpenHouseGMServerSO.delete();
}
else
{
  new ScriptObject(OpenHouseGMServerSO)
  {
    class = "OpenHouseGMServer";
    EventManager_ = "";
    npcMax_ = 3;
    npcKillCount_ = 0;
    npcs_ = "";
    camera_ = "";
    dummyCam_ = "";
    camSpeed_ = 10;
    camHeight_ = 0.5;
    rayMask_ = "";
    curSpawnPoint_ = "";
  };

  DNCServer.loadOutListeners_.add(OpenHouseGMServerSO);
  MissionCleanup.add(OpenHouseGMServerSO);
}
