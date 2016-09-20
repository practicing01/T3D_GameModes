if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "UndeadSoullessDungeonLevel" SPC "UndeadSoullessClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function UndeadSoullessDungeonLevel::onAdd(%this, %obj)
{
  %obj.setCloaked(true);

  %sprite = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/levels/dungeonunits/undead-soulless/undeadsoulless.cached.dts";
  };

  %obj.mountObject(%sprite, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));
}

function UndeadSoullessDungeonLevel::onReachDestination(%this, %ai)
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

function UndeadSoullessDungeonLevel::onMoveStuck(%this, %ai)
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

function UndeadSoullessDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function UndeadSoullessClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function UndeadSoullessDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (!isObject(%collObj))
  {
    return;
  }

  if (%collObj.getClassName() !$= "Player")// && %collObj.getClassName() !$= "AIPlayer")
  //if (!(%collObj.getType() & ($TypeMasks::ShapeBaseObjectType)))
  {
    return;
  }

  %damageState = %obj.getDamageState();
  if (%damageState $= "Disabled" || %damageState $= "Destroyed")
  {
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

  %obj.setActionThread("melee");
}
