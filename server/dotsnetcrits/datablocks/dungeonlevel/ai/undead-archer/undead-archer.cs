if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "UndeadArcherDungeonLevel" SPC "UndeadArcherClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function UndeadArcherDungeonLevel::onReachDestination(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    %ai.target_ = ClientGroup.getRandom().getControlObject();
  }

  %targPos = %ai.target_.getPosition();

  //setField(%targPos, 2, getField(%ai.getPosition(), 2));

  //%ai.setMoveDestination(%targPos);

  %ai.setAimObject(%ai.target_);

}

function UndeadArcherDungeonLevel::onMoveStuck(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    %ai.target_ = ClientGroup.getRandom().getControlObject();
  }

  %targPos = %ai.target_.getPosition();

  //setField(%targPos, 2, getField(%ai.getPosition(), 2));

  //%ai.setMoveDestination(%targPos);

  %ai.setAimObject(%ai.target_);

}

function UndeadArcherDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function UndeadArcherClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function UndeadArcherDungeonLevel::onAdd(%this, %obj)
{
  %obj.projectileSchedule_ = 0;
}

function UndeadArcherDungeonLevel::onTargetEnterLOS(%this, %obj)
{
  if (isEventPending(%obj.projectileSchedule_))
  {
    return;
  }

  if (!isObject(%obj))
  {
    return;
  }

  %projectileVelocity = VectorScale(%obj.getEyeVector(), 10.0);

  %projectile = new Projectile()
  {
    datablock = RangedSkillsGMProjectile;
    initialPosition = %obj.getEyePoint();
    initialVelocity = %projectileVelocity;
    sourceObject = %obj;
    sourceSlot = 0;
    client = %obj.client;
  };

  %obj.projectileSchedule_ = %this.schedule(4000, "onTargetEnterLOS", %obj);
}

function UndeadArcherDungeonLevel::onTargetExitLOS(%this, %obj)
{
  //cancelAll(%this);
}

function UndeadArcherDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
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
