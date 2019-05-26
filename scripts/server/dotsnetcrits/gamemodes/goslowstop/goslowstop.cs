function TrafficLightAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  %player = "";

  if (%sourceObject.getClassName() !$= "Player" && %sourceObject.getClassName() !$= "AIPlayer")
  {
    %player = %sourceObject.sourceObject;
  }
  else
  {
    %player = %sourceObject;
  }

  %client = %player.client;

  if (%shape.color_ == 0)
  {
    Game.incScore(%client, 1, false);
  }
  else if (%shape.color_ == 1)
  {
    Game.incScore(%client, -1, false);
  }
  else
  {
    %score = %client.score * -1;
    Game.incScore(%client, %score, false);

    %player.damage(%shape, %shape.position, 1000, "trafficlight");
  }
}

function TrafficLightAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function TrafficLightAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function TrafficlightClass::setDest(%this)
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

function GoslowstopGMServer::ChangeState(%this)
{
  %this.trafficlight_.color_ = getRandom(0, 2);

  if (%this.trafficlight_.color_ == 0)
  {
    %this.trafficlight_.setSkinName("base=base");
  }
  else if (%this.trafficlight_.color_ == 1)
  {
    %this.trafficlight_.setSkinName("base=slow");
  }
  else
  {
    %this.trafficlight_.setSkinName("base=stop");
  }

  %this.changeSchedule_ = %this.schedule(%this.changeInterval_, "ChangeState");
}

function GoslowstopGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "GoslowstopGMServerQueue";

  %player = ClientGroup.getRandom().getControlObject();

  %teleDir = %player.getForwardVector();

  %scaledDir = VectorScale(%teleDir, 10);

  %projection = VectorAdd( %player.position, %scaledDir );

  %this.trafficlight_ = new AiPlayer()
  {
     dataBlock = "TrafficLightAI";
     class = "TrafficlightClass";
     state_ = "idle";
     parent_ = %this;
     position = %projection;
     color_ = 0;
  };

  MissionCleanup.add(%this.trafficlight_);

  %this.trafficlight_.setDest();

  %this.ChangeState();
}

function GoslowstopGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.trafficlight_))
  {
    %this.trafficlight_.delete();
  }

  cancel(%this.changeSchedule_);
}

if (isObject(GoslowstopGMServerSO))
{
  GoslowstopGMServerSO.delete();
}
else
{
  new ScriptObject(GoslowstopGMServerSO)
  {
    class = "GoslowstopGMServer";
    EventManager_ = "";
    trafficlight_ = "";
    changeSchedule_ = "";
    changeInterval_ = 1000;
  };

  MissionCleanup.add(GoslowstopGMServerSO);
}
