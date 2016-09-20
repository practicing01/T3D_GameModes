if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "UndeadDeathbladeDungeonLevel" SPC "UndeadDeathbladeClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function UndeadDeathbladeDungeonLevel::onAdd(%this, %obj)
{
  %obj.setCloaked(true);

  %sprite = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/levels/dungeonunits/undead-deathblade/undeaddeathblade.cached.dts";
  };

  %obj.mountObject(%sprite, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));
}

function UndeadDeathbladeDungeonLevel::onReachDestination(%this, %ai)
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

function UndeadDeathbladeDungeonLevel::onMoveStuck(%this, %ai)
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

function UndeadDeathbladeDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function UndeadDeathbladeClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function UndeadDeathbladeDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
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

  %collObj.damage(%obj, %vec, 20, "melee");
  %obj.canAttack_ = false;
  %obj.schedule(1000, "AttackCD");

  %obj.setActionThread("melee");
}
