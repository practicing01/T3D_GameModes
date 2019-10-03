function StealthCubeClass::UseObject(%this, %player)
{
  %parent = %this.getObjectMount();

  if (%player == %parent)
  {
    %this.unmount();
    return;
  }

  %player.mountObject(%this, 1, MatrixCreate("0 3 0", "1 0 0 0"));
}

function StackStealthGMServer::CheckLOS(%this, %originObject, %destObject)
{
  %startPos = VectorAdd(%originObject.position, "0 0 1");
  %endPos = VectorAdd(%destObject.position, "0 0 1");
  %rayResult = containerRayCast(%startPos, %endPos, %this.rayMask_, %originObject);
  %objTarget = firstWord(%rayResult);

  if (%objTarget == %destObject)
  {
    return true;
  }
  else
  {
    return false;
  }
}

function StackStealthTrigger::onTickTrigger(%this, %trigger)
{
  for (%x = 0; %x < %trigger.getNumObjects(); %x++)
  {
    %obj = %trigger.getObject(%x);

    if (%obj == %trigger.legeye_)
    {
      continue;
    }

    %damageState = %obj.getDamageState();
    if (%damageState $= "Disabled" || %damageState $= "Destroyed")
    {
      continue;
    }

    if (%obj.getClassName() !$= "Player")
    {
      continue;
    }

    if (%trigger.parent_.CheckLOS(%trigger.legeye_, %obj))
    {
      %obj.damage(%trigger.legeye_, %trigger.legeye_.position, 1000, "legeyes");
    }
  }
}

function LegeyesAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function LegeyesAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function LegeyesClass::setDest(%this)
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

  %this.setAimLocation(%spawnPoint.position);
  %this.clearAim();
}

function LegeyesAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  return;
}

function StackStealthGMServer::SetExit(%this)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  %this.exitsign_.setTransform(%spawnPoint.getTransform());
  %this.exitTrigger_.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, %this.rayMask_, %this.exitsign_);
}

function StackStealthExitTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "Player")
  {
    Game.incScore(%obj.client, 10, false);
    %trigger.parent_.SetExit();
  }
}

function StackStealthGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "StackStealthGMServerQueue";

  %this.rayMask_ = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;

  %this.legeyes_ = new SimSet();
  %this.stealthCubes_ = new SimSet();
  %this.triggers_ = new SimSet();

  for (%x = 0; %x < %this.maxlegeyes_; %x++)
  {
    %legeye = new AiPlayer()
    {
       dataBlock = "LegeyesAI";
       class = "LegeyesClass";
       state_ = "idle";
       parent_ = %this;
    };

    %spawnpoint = PlayerDropPoints.getRandom();

    %transform = GameCore::pickPointInSpawnSphere(%legeye, %spawnPoint);

    %legeye.setTransform(%transform);

    %legeye.playThread(0, "run");

    %legeye.setDest();

    %this.legeyes_.add(%legeye);

    %trigger = new Trigger()
    {
      dataBlock = "StackStealthTrigger";
      polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
      position = %legeye.position;
      scale = "20 20 10";
      parent_ = %this;
      legeye_ = %legeye;
    };

    %legeye.mountObject(%trigger, 0, MatrixCreate("0 0 -5", "1 0 0 0"));

    %legeye.trigger_ = %trigger;
    %this.triggers_.add(%trigger);
  }

  for (%x = 0; %x < PlayerDropPoints.getCount(); %x++)
  {
    %stealthCube = new StaticShape()
    {
      dataBlock = StealthCubeStaticShapeData;
      class = "StealthCubeClass";
      parent_ = %this;
    };

    %spawnpoint = PlayerDropPoints.getObject(%x);
    %transform = GameCore::pickPointInSpawnSphere(%stealthCube, %spawnPoint);
    %stealthCube.setTransform(%transform);

    %this.stealthCubes_.add(%stealthCube);
  }

  %this.exitsign_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/gamemodes/stackstealth/exitsign/exitsign.dae";
  };

  %this.SetExit();

  %this.exitTrigger_ = new Trigger()
  {
    dataBlock = "StackStealthExitTrigger";
    polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
    position = %this.exitsign_.position;
    scale = "20 20 10";
    parent_ = %this;
  };

  %this.triggers_.add(%this.exitTrigger_);

  %this.exitTrigger_.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, %this.rayMask_, %this.exitsign_);
}

function StackStealthGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.legeyes_))
  {
    %this.legeyes_.deleteAllObjects();
    %this.legeyes_.delete();
  }

  if (isObject(%this.stealthCubes_))
  {
    %this.stealthCubes_.deleteAllObjects();
    %this.stealthCubes_.delete();
  }

  if (isObject(%this.triggers_))
  {
    %this.triggers_.deleteAllObjects();
    %this.triggers_.delete();
  }

  if (isObject(%this.exitsign_))
  {
    %this.exitsign_.delete();
  }

  cancel(%this.scanSchedule_);
}

function StackStealthGMServer::loadOut(%this, %player)
{
  //
}

if (isObject(StackStealthGMServerSO))
{
  StackStealthGMServerSO.delete();
}
else
{
  new ScriptObject(StackStealthGMServerSO)
  {
    class = "StackStealthGMServer";
    EventManager_ = "";
    legeyes_ = "";
    triggers_ = "";
    maxlegeyes_ = 10;
    stealthCubes_ = "";
    exitsign_ = "";
    exitTrigger_ = "";
    scanSchedule_ = "";
    rayMask_ = "";
  };

  DNCServer.loadOutListeners_.add(StackStealthGMServerSO);
  MissionCleanup.add(StackStealthGMServerSO);
}
