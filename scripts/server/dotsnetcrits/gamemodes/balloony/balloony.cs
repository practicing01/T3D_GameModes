function GeorgieBalloonyAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  if (%sourceObject.getClassName() !$= "Player")
  {
    return;
  }

  %sourceObject.damage(%this, %this.position, 1000, "georgie");
}

function BalloonyStaticShapeData::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  if (%sourceObject.getClassName() !$= "Player")
  {
    return;
  }

  Game.incScore(%sourceObject.client, 1, false);
}

function GeorgieBalloonyAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function GeorgieBalloonyAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function GeorgieBalloonyAIClass::setDest(%this)
{
  if (!isObject(%this))
  {
    return;
  }

  %dest = %this.parent_.GetRandomVector(%this.parent_.clownt_.position, %this.parent_.fieldRadius_);

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

function BalloonyGMServer::GetRandomVector(%this, %origin, %radius)
{
  %pos = %origin;

  %angleY = mDegToRad(getRandom(0, 100) * m2Pi());
  %angleXZ = mDegToRad(getRandom(0, 100) * m2Pi());

  %pos.x = %pos.x + (mCos(%angleY) * mSin(%angleXZ) * getRandom(-%radius, %radius));
  %pos.y = %pos.y + (mCos(%angleXZ) * getRandom(-%radius, %radius));

  return %pos;
}

function BalloonyGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BalloonyGMServerQueue";

  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.georgies_ = new SimSet();
  %this.balloons_ = new SimSet();

  %this.clownt_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/gamemodes/balloony/clownt/clownt.dae";
    scale = "0.05 0.05 0.05";
  };

  %spawnpoint = PlayerDropPoints.getRandom();

  %this.clownt_.setTransform(%spawnpoint.getTransform());

  %startPos = %spawnpoint.position;
  %endPos = VectorAdd(%startPos, "0 0 -50");
  %rayResult = containerRayCast(%startPos, %endPos, %this.rayMask_, %this.clownt_);
  %objTarget = firstWord(%rayResult);

  if (isObject(%objTarget))
  {
    %this.clownt_.position = getWords(%rayResult, 1, 3);
  }

  for (%x = 0; %x < %this.maxGeorgies_; %x++)
  {
    %pos = %this.GetRandomVector(%spawnpoint.position, %this.fieldRadius_);

    %georgie = new AIPlayer()
    {
      dataBlock = GeorgieBalloonyAI;
      class = "GeorgieBalloonyAIClass";
      position = %pos;
      parent_ = %this;
      scale = "0.05 0.05 0.05";
    };

    %georgie.playThread(0, "run");

    %this.georgies_.add(%georgie);

    %georgie.setDest();

    %balloon = new StaticShape()
    {
      dataBlock = BalloonyStaticShapeData;
      class = "BalloonBalloonyClass";
      parent_ = %this;
      scale = "0.05 0.05 0.05";
    };

    %this.balloons_.add(%balloon);

    %georgie.mountObject(%balloon, 0, MatrixCreate("0 0 2", "1 0 0 0"));
  }
}

function BalloonyGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.clownt_))
  {
    %this.clownt_.delete();
  }

  if (isObject(%this.georgies_))
  {
    %this.georgies_.deleteAllObjects();
    %this.georgies_.delete();
  }

  if (isObject(%this.balloons_))
  {
    %this.balloons_.deleteAllObjects();
    %this.balloons_.delete();
  }
}

if (isObject(BalloonyGMServerSO))
{
  BalloonyGMServerSO.delete();
}
else
{
  new ScriptObject(BalloonyGMServerSO)
  {
    class = "BalloonyGMServer";
    EventManager_ = "";
    georgies_ = "";
    balloons_ = "";
    clownt_ = "";
    rayMask_ = "";
    maxGeorgies_ = 10;
    fieldRadius_ = 5;
  };

  MissionCleanup.add(BalloonyGMServerSO);
}
