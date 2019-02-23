if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "UndeadSoulshooterDungeonLevel" SPC "UndeadSoulshooterClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function UndeadSoulshooterDungeonLevel::onAdd(%this, %obj)
{
  %obj.projectileSchedule_ = 0;

  %obj.setCloaked(true);

  %obj.sprite_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/levels/dungeonunits/undead-soulshooter/undeadsoulshooter.cached.dts";
    collisionType = "none";
  };

  %obj.mountObject(%obj.sprite_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));
}

function UndeadSoulshooterDungeonLevel::onRemove(%this, %obj)
{
  cancelAll(%obj);
  cancelAll(%this);
  %obj.sprite_.delete();
}

function UndeadSoulshooterDungeonLevel::onReachDestination(%this, %ai)
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

function UndeadSoulshooterDungeonLevel::onMoveStuck(%this, %ai)
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

function UndeadSoulshooterDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function UndeadSoulshooterClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function UndeadSoulshooterDungeonLevel::onTargetEnterLOS(%this, %obj)
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

  %obj.projectileSchedule_ = %this.schedule(1000, "onTargetEnterLOS", %obj);
}

function UndeadSoulshooterDungeonLevel::onTargetExitLOS(%this, %obj)
{
  cancelAll(%this);
}

function UndeadSoulshooterDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
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
