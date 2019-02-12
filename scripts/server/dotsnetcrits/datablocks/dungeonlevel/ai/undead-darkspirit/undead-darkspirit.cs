if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "UndeadDarkspiritDungeonLevel" SPC "UndeadDarkspiritClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function UndeadDarkspiritDungeonLevel::onAdd(%this, %obj)
{
  %obj.setCloaked(true);

  /*%obj.aura_ = new Trigger() {
    polyhedron = "-0.5000000 0.5000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
    dataBlock = "UndeadDarkspiritTrigger";
    scale = "6 6 6";
  };

  %obj.mountObject(%obj.aura_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));*/

  %obj.sprite_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/levels/dungeonunits/undead-darkspirit/undead-darkspirit.cached.dts";
    collisionType = "none";
  };

  %obj.mountObject(%obj.sprite_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));
}

function UndeadDarkspiritDungeonLevel::onRemove(%this, %obj)
{
  cancelAll(%obj);
  cancelAll(%this);
  
  if (isObject(%obj.aura_))
  {
    %obj.aura_.delete();
  }

  %obj.sprite_.delete();
}

function UndeadDarkspiritDungeonLevel::onReachDestination(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    %ai.target_ = ClientGroup.getRandom().getControlObject();
  }

  %targPos = %ai.target_.getPosition();

  //setField(%targPos, 2, getField(%ai.getPosition(), 2));

  %ai.setMoveDestination(%targPos);

  %ai.setAimObject(%ai.target_);

}

function UndeadDarkspiritDungeonLevel::onMoveStuck(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    %ai.target_ = ClientGroup.getRandom().getControlObject();
  }

  %targPos = %ai.target_.getPosition();

  //setField(%targPos, 2, getField(%ai.getPosition(), 2));

  %ai.setMoveDestination(%targPos);

  %ai.setAimObject(%ai.target_);

}

function UndeadDarkspiritDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function UndeadDarkspiritClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function UndeadDarkspiritDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (!isObject(%collObj))
  {
    return;
  }

  if (%collObj.getClassName() !$= "Player" && %collObj.getClassName() !$= "AIPlayer")
  //if (!(%collObj.getType() & ($TypeMasks::ShapeBaseObjectType)))
  {
    return;
  }

  %damageState = %obj.getDamageState();
  if (%damageState $= "Disabled" || %damageState $= "Destroyed")
  {
    return;
  }

  if (%collObj.getClassName() $= "AIPlayer")
  {
    %obj.applyRepair(100);
    %collObj.applyRepair(100);
    return;
  }

  if (%obj.canAttack_ == false)
  {
    return;
  }

  if (%obj.getClassName() $= %collObj.getClassName())
  {
    return;
  }

  %collObj.damage(%obj, %vec, 10, "melee");
  %obj.canAttack_ = false;
  %obj.schedule(1000, "AttackCD");

  //%obj.setActionThread("melee");
}

function UndeadDarkspiritTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "AIPlayer")
  {
    %obj.applyRepair(100);
  }
}

function UndeadDarkspiritTrigger::onTickTrigger(%this, %trigger)
{
  for (%x = 0; %x < %trigger.getNumObjects(); %x++)
  {
    %obj = %trigger.getObject(%x);

    if (%obj.getClassName() $= "AIPlayer")
    {
      %obj.applyRepair(10);
    }
  }
}
