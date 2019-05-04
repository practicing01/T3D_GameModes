function RastamonSnapGMServer::Fire(%this, %client)
{
  %pos = %client.camera.getPosition();

  /*%mask = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;*/

  %rayResult = %client.camera.doRaycast(1000.0, $TypeMasks::ShapeBaseObjectType);//%mask);

  %objTarget = firstWord(%rayResult);

  if (!isObject(%objTarget) || %objTarget == %client.player)
  {
    return;
  }

  %objPos = getWords(%rayResult, 1, 3);
  %objDir = getWords(%rayResult, 4, 6);

  %client.player.setAimObject(%objTarget, "0 0 1");
  %client.player.fire(true);
  %client.player.fire(false);
}

function serverCmdFireRastamonSnapGM(%client)
{
  if (isObject(RastamonSnapGMServerSO))
  {
    RastamonSnapGMServerSO.Fire(%client);
  }
}

function RastamonSnapAI::onDisabled(%this, %npc, %state)
{
  %npc.setDamageLevel(0);
  %npc.setDamageState("Enabled");
  %npc.setScale("1 1 1");
  %spawnPoint = PlayerDropPoints.getRandom();
  %npc.setTransform(%spawnPoint.getTransform());
  %npc.SetDest();
}

function RastamonSnapAIClass::setDest(%this)
{
  if (!isObject(PlayerDropPoints))
  {
    return;
  }

  %spawnPoint = PlayerDropPoints.getRandom();
  %result = %this.setPathDestination(%spawnPoint.position);

  if (!%result)
  {
    %this.schedule(1000, "setDest");
    return;
  }

  %this.setAimLocation(%spawnPoint);
  %this.clearAim();
}

function RastamonSnapGMServer::SetupCamera(%this, %client, %npc)
{
  if (isObject(%client.camera) && isObject(%client.player))
  {
    %client.player.delete();
    %client.player = %npc;
    %client.camera.setTransform(%npc.getEyeTransform());
    //%client.camera.setTrackObject(%npc);
    %client.camera.setOrbitObject(%npc, %client.camera.getRotation(), 0, 0, 0, true, "0 0 1.5", false);
    %client.camera.controlMode = "OrbitObject";
    //%client.camera.controlMode = "FreeRotate";
    %client.setControlObject(%client.camera);
    //%client.setControlObject(%npc);
  }
}

function RastamonSnapAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function RastamonSnapAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function RastamonSnapGMServer::SpawnNPC(%this, %client, %parent)
{
  %npc = new AiPlayer()
  {
    dataBlock = RastamonSnapAI;
    class = "RastamonSnapAIClass";
    client = %client;
    parent_ = %parent;
  };

  %npc.setTransform(%client.player.getTransform());

  %npc.mountImage(staplerImage, 0);
  %npc.incInventory(staplerAmmo, 2);

  MissionCleanup.add(%npc);

  return %npc;
}

function RastamonSnapGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "RastamonSnapGMServerQueue";

  %this.dictionary_ = new ArrayObject();
  %this.npcs_ = new SimSet();

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %player = %client.getControlObject();

    %npc = %this.SpawnNPC(%client, %this);

    %this.npcs_.add(%npc);

    %this.SetupCamera(%client, %npc);
    %npc.SetDest();

    %this.dictionary_.add(%client, %npc);
  }
}

function RastamonSnapGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.npcs_))
  {
    %this.npcs_.deleteAllObjects();
    %this.npcs_.delete();
  }

  if (isObject(%this.dictionary_))
  {
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

function RastamonSnapGMServerSO::loadOut(%this, %player)
{
  %client = %player.client;
  %key = %this.dictionary_.getIndexFromKey(%client);
  %npc = "";

  if (%key == -1)
  {
    %npc = %this.SpawnNPC(%client, %this);

    %this.npcs_.add(%npc);

    %this.dictionary_.add(%client, %npc);
  }
  else
  {
    %npc = %this.dictionary_.getValue(%key);
  }

  %npc.setTransform(%player.getTransform());

  %this.SetupCamera(%client, %npc);
  %npc.SetDest();
}

if (isObject(RastamonSnapGMServerSO))
{
  RastamonSnapGMServerSO.delete();
}
else
{
  new ScriptObject(RastamonSnapGMServerSO)
  {
    class = "RastamonSnapGMServer";
    EventManager_ = "";
    npcs_ = "";
    dictionary_ = "";
  };

  DNCServer.loadOutListeners_.add(RastamonSnapGMServerSO);
  MissionCleanup.add(RastamonSnapGMServerSO);
}
