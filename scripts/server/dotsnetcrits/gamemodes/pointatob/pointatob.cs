function ZombiePointAtoBAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (!isObject(%collObj))
  {
    return;
  }

  if (%collObj.getClassName() $= "Player")
  {
    %collObj.damage(%obj, %vec, 1000, "zombie");
    %obj.stopThread(0);
    %obj.playThread(0, "attack");
  }
}

function PointAtoBTrigger::onTickTrigger(%this, %trigger)
{
  for (%x = 0; %x < %trigger.getNumObjects(); %x++)
  {
    %obj = %trigger.getObject(%x);

    if (%obj == %trigger.zombie_)
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

    if (!isObject(%trigger.zombie_.target_))
    {
      %trigger.zombie_.target_ = %obj;
      %trigger.zombie_.followObject(%obj, 0);
      %trigger.zombie_.stopThread(0);
      %trigger.zombie_.playThread(0, "run");
      return;
    }
  }
}

function PointAtoBTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (%obj == %trigger.zombie_)
  {
    %trigger.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, %obj.parent_.rayMask_, %obj);
    return;
  }

  if (%obj == %trigger.zombie_.target_)
  {
    %trigger.zombie_.target_ = "";
    %trigger.zombie_.stop();
    %trigger.zombie_.stopThread(0);
    %trigger.zombie_.playThread(0, "idle");
    %trigger.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, %obj.parent_.rayMask_, %trigger.zombie_);
  }
}

function KeyPointAtoBClass::UseObject(%this, %player)
{
  %player.client.hasKeyPointAtoB_ = true;
  //%this.setHidden(true);//didn't work
  %this.schedule(0,"delete");//fuck it
}

function DoorPointAtoBClass::UseObject(%this, %player)
{
  if (%player.client.hasKeyPointAtoB_ == true)
  {
    %player.client.hasKeyPointAtoB_ = false;
    %this.parent_.SpawnKey();
    %this.parent_.SpawnDoor();

    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      Game.incScore(%client, 1, false);
    }
  }
}

function PointAtoBGMServer::SpawnDoor(%this)
{
  if (!isObject(%this.door_))
  {
    %this.door_ = new StaticShape()
    {
       dataBlock = "DoorPointAtoBStaticShapeData";
       class = "DoorPointAtoBClass";
       parent_ = %this;
    };
  }

  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%this.door_, %spawnPoint);
  %this.door_.setTransform(%transform);

  %this.door_.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, %this.rayMask_, %this.door_);
}

function PointAtoBGMServer::SpawnKey(%this)
{
  if (!isObject(%this.key_))
  {
    %this.key_ = new StaticShape()
    {
       dataBlock = "KeyPointAtoBStaticShapeData";
       class = "KeyPointAtoBClass";
       parent_ = %this;
    };
  }
  else
  {
    //%this.key_.setHidden(false);//didn't work
  }

  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%this.key_, %spawnPoint);
  %this.key_.setTransform(%transform);
  %this.key_.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 1, %this.rayMask_, %this.key_);
}

function PointAtoBGMServer::SpawnZombies(%this)
{
  %zombie = new AiPlayer()
  {
     dataBlock = "ZombiePointAtoBAI";
     class = "ZombiePointAtoBAIClass";
     state_ = "idle";
     target_ = "";
     parent_ = %this;
     trigger_ = "";
     scale = "0.2 0.2 0.2";
  };

  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%zombie, %spawnPoint);

  %zombie.setTransform(%transform);

  %this.zombies_.add(%zombie);

  %zombie.stopThread(0);
  %zombie.playThread(0, "idle");

  %trigger = new Trigger()
  {
    dataBlock = "PointAtoBTrigger";
    polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
    position = %zombie.position;
    scale = "20 20 5";
    parent_ = %this;
    zombie_ = %zombie;
  };

  %trigger.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, %this.rayMask_, %zombie);

  %zombie.trigger_ = %trigger;

  %this.triggers_.add(%trigger);
}

function PointAtoBGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PointAtoBGMServerQueue";

  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.SpawnKey();

  for (%x = 0; %x < %this.zombiesMax_; %x++)
  {
    %this.SpawnZombies();
  }

  %this.SpawnDoor();
}

function PointAtoBGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.key_))
  {
    %this.key_.delete();
  }

  if (isObject(%this.zombies_))
  {
    %this.zombies_.deleteAllObjects();
    %this.zombies_.delete();
  }

  if (isObject(%this.triggers_))
  {
    %this.triggers_.deleteAllObjects();
    %this.triggers_.delete();
  }

  if (isObject(%this.door_))
  {
    %this.door_.delete();
  }
}

function ZombiePointAtoBAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
}

function PointAtoBGMServerSO::loadOut(%this, %player)
{
  if (%player.client.hasKeyPointAtoB_ == true)
  {
    %this.SpawnKey();
  }

  %player.client.hasKeyPointAtoB_ = false;
}

if (isObject(PointAtoBGMServerSO))
{
  PointAtoBGMServerSO.delete();
}
else
{
  new ScriptObject(PointAtoBGMServerSO)
  {
    class = "PointAtoBGMServer";
    EventManager_ = "";
    zombiesMax_ = 16;
    key_ = "";
    zombies_ = new SimSet();
    triggers_ = new SimSet();
    door_ = "";
    rayMask_ = "";
  };

  DNCServer.loadOutListeners_.add(PointAtoBGMServerSO);
  MissionCleanup.add(PointAtoBGMServerSO);
}
