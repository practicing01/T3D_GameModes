function MerbillyGMServer::GetRandomVector(%this, %origin, %radius)
{
  %pos = %origin;

  %angleY = mDegToRad(getRandom(0, 100) * m2Pi());
  %angleXZ = mDegToRad(getRandom(0, 100) * m2Pi());

  %pos.x = %pos.x + (mCos(%angleY) * mSin(%angleXZ) * getRandom(-%radius, %radius));
  %pos.y = %pos.y + (mCos(%angleXZ) * getRandom(-%radius, %radius));

  return %pos;
}

function MerbillyBaitAIClass::UseObject(%this, %player)
{
  if (%this.isBait_ == true)
  {
    Game.incScore(%player.client, 1, false);

    %pos = %this.parent_.GetRandomVector(%this.parent_.merbilly_.position, %this.parent_.fieldRadius_);
    %this.position = %pos;
  }
  else
  {
    %player.damage(%this, %this.position, 1000, "seallager");
  }
}

function MerbillyBaitAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function MerbillyBaitAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function MerbillyBaitAIClass::setDest(%this)
{
  if (!isObject(%this))
  {
    return;
  }

  %merbilly = %this.parent_.merbilly_;
  %dest = %this.parent_.GetRandomVector(%merbilly.position, %this.parent_.fieldRadius_);

  %result = %this.setPathDestination(%dest);

  if (!%result)
  {
    %this.setMoveDestination(%dest);
    %this.schedule(1000, "setDest");
    return;
  }

  %this.setAimLocation(%dest);
  %this.clearAim();
  %this.schedule(10000, "setDest");
}

function MerbillyGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "MerbillyGMServerQueue";

  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.bait_ = new SimSet();

  %spawnpoint = PlayerDropPoints.getRandom();

  %this.merbilly_ = new AiPlayer()
  {
    dataBlock = MerbillyAI;
    class = "MerbillyAIClass";
    parent_ = %this;
  };

  %this.merbilly_.setTransform(%spawnpoint.getTransform());

  for (%x = 0; %x < 10; %x++)
  {
    %npc = new AiPlayer()
    {
      dataBlock = MerbillyBaitAI;
      class = "MerbillyBaitAIClass";
      position = %this.GetRandomVector(%this.merbilly_.position, %this.fieldRadius_);
      parent_ = %this;
    };

    %this.bait_.add(%npc);

    %npc.setDest();
  }

  %bait = %this.bait_.getRandom();
  %bait.isBait_ = true;

  %this.merbilly_.setAimObject(%bait);
}

function MerbillyGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.merbilly_))
  {
    %this.merbilly_.delete();
  }

  if (isObject(%this.bait_))
  {
    %this.bait_.deleteAllObjects();
    %this.bait_.delete();
  }
}

if (isObject(MerbillyGMServerSO))
{
  MerbillyGMServerSO.delete();
}
else
{
  new ScriptObject(MerbillyGMServerSO)
  {
    class = "MerbillyGMServer";
    EventManager_ = "";
    merbilly_ = "";
    bait_ = "";
    rayMask_ = "";
    fieldRadius_ = 40;
  };

  MissionCleanup.add(MerbillyGMServerSO);
}
