function MinaryCo2AI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  if (%sourceObject.getClassName() !$= "Player")
  {
    return;
  }

  Game.incScore(%sourceObject.client, 1, false);

  %spawnpoint = PlayerDropPoints.getRandom();
  %shape.setTransform(%spawnpoint.getTransform());
  %shape.setDest();
}

function MinaryClass::UseObject(%this, %player)
{
  %this.playThread(0, "mine");
}

function MinaryCo2AI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  if (!isObject(%collObj))
  {
    return;
  }

  if (%collObj.dataBlock !$= "MinaryStaticShapeData")
  {
    return;
  }

  %collObj.playThread(0, "dead");

  %obj.setDest();
}

function MinaryGMServer::TransformObject(%this, %originObject, %offsetObject, %offsetScale)
{
  %teleDir = %originObject.getForwardVector();

  %size = %originObject.getObjectBox();
  %scale = %originObject.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %offsetObject.rotation = %originObject.rotation;

  %sizeTarget = %offsetObject.getObjectBox();
  %scaleTarget = %offsetObject.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %offsetObject.position = VectorAdd( %originObject.position, VectorScale(%teleDir, (%sizex + %sizexTarget) * %offsetScale) );
}

function MinaryCo2AI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function MinaryCo2AI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function MinaryCo2AIClass::setDest(%this)
{
  if (!isObject(%this))
  {
    return;
  }

  %minary = %this.parent_.minaries_.getRandom();
  %dest = %minary.position;

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

function MinaryGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "MinaryGMServerQueue";

  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.minaries_ = new SimSet();
  %this.crystals_ = new SimSet();

  %this.co2_ = new AiPlayer()
  {
    dataBlock = MinaryCo2AI;
    class = "MinaryCo2AIClass";
    parent_ = %this;
  };

  %spawnpoint = PlayerDropPoints.getRandom();

  %this.co2_.setTransform(%spawnpoint.getTransform());
  %this.co2_.setCloaked(true);
  %this.co2_.setDest();

  for (%x = 0; %x < PlayerDropPoints.getCount(); %x++)
  {
    %spawnpoint = PlayerDropPoints.getObject(%x);

    %minary = new StaticShape()
    {
      dataBlock = MinaryStaticShapeData;
      class = "MinaryClass";
      parent_ = %this;
    };

    %minary.setTransform(%spawnpoint.getTransform());

    %minary.playThread(0, "mine");

    %this.minaries_.add(%minary);

    %startPos = %minary.position;
    %endPos = VectorAdd(%startPos, "0 0 -50");
    %rayResult = containerRayCast(%startPos, %endPos, %this.rayMask_, %minary);
    %objTarget = firstWord(%rayResult);

    if (isObject(%objTarget))
    {
      %minary.position = getWords(%rayResult, 1, 3);
    }

    %crystal = new StaticShape()
    {
      dataBlock = MinaryCrystalStaticShapeData;
      parent_ = %this;
    };

    %this.TransformObject(%minary, %crystal, 0.25);

    %this.crystals_.add(%crystal);

  }
}

function MinaryGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.co2_))
  {
    %this.co2_.delete();
  }

  if (isObject(%this.minaries_))
  {
    %this.minaries_.deleteAllObjects();
    %this.minaries_.delete();
  }

  if (isObject(%this.crystals_))
  {
    %this.crystals_.deleteAllObjects();
    %this.crystals_.delete();
  }
}

if (isObject(MinaryGMServerSO))
{
  MinaryGMServerSO.delete();
}
else
{
  new ScriptObject(MinaryGMServerSO)
  {
    class = "MinaryGMServer";
    EventManager_ = "";
    minaries_ = "";
    crystals_ = "";
    co2_ = "";
    rayMask_ = "";
  };

  MissionCleanup.add(MinaryGMServerSO);
}
