function RoyalEscortPrincessAIClass::UseObject(%this, %player)
{
  if (%this.followTarget_ == %player)
  {
    %this.followTarget_ = "";
    %this.stop();
    return;
  }

  %this.followTarget_ = %player;
  %this.followObject(%player, 1);
}

function RoyalEscortPrincessAIClass::Hit(%this)
{
  %this.followTarget_ = "";
  %this.stop();
  %this.setDamageLevel(0);
  %this.setDamageState("Enabled");
  %this.setScale("1 1 1");
  %spawnPoint = PlayerDropPoints.getRandom();
  %this.setTransform(%spawnPoint.getTransform());
}

function RoyalEscortPaparazziAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (%collObj.class $= "RoyalEscortPrincessAIClass")
  {
    %collObj.Hit();
  }
}

function RoyalEscortCarAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  //parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (%collObj.class $= "RoyalEscortPrincessAIClass")
  {
    %collObj.Hit();

    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      Game.incScore(%client, 5, false);
    }

    %spawnPoint = PlayerDropPoints.getRandom();
    %transform = GameCore::pickPointInSpawnSphere(%obj, %spawnPoint);
    %obj.setTransform(%transform);
  }
}

function RoyalEscortGMServer::SpawnCar(%this)
{
  %this.car_ = new AiPlayer()
  {
     dataBlock = "RoyalEscortCarAI";
     class = "RoyalEscortCarAIClass";
     state_ = "idle";
     followTarget_ = "";
     parent_ = %this;
  };

  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%this.car_, %spawnPoint);
  %this.car_.setTransform(%transform);
}

function RoyalEscortGMServer::SpawnPaparazzi(%this)
{
  %paparazzi = new AiPlayer()
  {
     dataBlock = "RoyalEscortPaparazziAI";
     class = "RoyalEscortPaparazziAIClass";
     state_ = "idle";
     followTarget_ = "";
     parent_ = %this;
  };

  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%paparazzi, %spawnPoint);

  %paparazzi.setTransform(%transform);

  %this.paparazzi_.add(%paparazzi);

  %paparazzi.followTarget_ = %this.princess_;
  %paparazzi.followObject(%this.princess_, 0);
}

function RoyalEscortGMServer::SpawnPrincess(%this)
{
  %this.princess_ = new AiPlayer()
  {
     dataBlock = "RoyalEscortPrincessAI";
     class = "RoyalEscortPrincessAIClass";
     state_ = "idle";
     followTarget_ = "";
     parent_ = %this;
  };

  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%this.princess_, %spawnPoint);

  %this.princess_.setTransform(%transform);
}

function RoyalEscortGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "RoyalEscortGMServerQueue";

  %this.SpawnPrincess();

  for (%x = 0; %x < %this.paparazziMax_; %x++)
  {
    %this.SpawnPaparazzi();
  }

  %this.SpawnCar();
}

function RoyalEscortGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.princess_))
  {
    %this.princess_.delete();
  }

  if (isObject(%this.paparazzi_))
  {
    %this.paparazzi_.deleteAllObjects();
    %this.paparazzi_.delete();
  }

  if (isObject(%this.car_))
  {
    %this.car_.delete();
  }
}

function RoyalEscortPrincessAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
  %ai.setScale("1 1 1");
  %spawnPoint = PlayerDropPoints.getRandom();
  %ai.setTransform(%spawnPoint.getTransform());
}

function RoyalEscortPaparazziAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
  %ai.setScale("1 1 1");
  %spawnPoint = PlayerDropPoints.getRandom();
  %ai.setTransform(%spawnPoint.getTransform());
}

function RoyalEscortGMServer::loadOut(%this, %player)
{
  //
}

if (isObject(RoyalEscortGMServerSO))
{
  RoyalEscortGMServerSO.delete();
}
else
{
  new ScriptObject(RoyalEscortGMServerSO)
  {
    class = "RoyalEscortGMServer";
    EventManager_ = "";
    paparazziMax_ = 8;
    princess_ = "";
    paparazzi_ = new SimSet();
    car_ = "";
  };

  DNCServer.loadOutListeners_.add(RoyalEscortGMServerSO);
  MissionCleanup.add(RoyalEscortGMServerSO);
}
