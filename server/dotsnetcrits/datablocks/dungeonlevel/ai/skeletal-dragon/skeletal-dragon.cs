if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "SkeletalDragonDungeonLevel" SPC "SkeletalDragonClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function SkeletalDragonDungeonLevel::onAdd(%this, %obj)
{
  %obj.projectileSchedule_ = 0;

  %obj.setCloaked(true);
  %obj.scale = "5 5 5";

  %obj.sprite_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/levels/dungeonunits/skeletal-dragon/skeletal-dragon.cached.dts";
    collisionType = "none";
  };

  %obj.mountObject(%obj.sprite_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));
}

function SkeletalDragonDungeonLevel::onRemove(%this, %obj)
{
  %obj.sprite_.delete();
}

function SkeletalDragonDungeonLevel::onTargetEnterLOS(%this, %obj)
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
    datablock = SkeletalDragonDungeonLevelProjectile;
    initialPosition = %obj.getEyePoint();
    initialVelocity = %projectileVelocity;
    sourceObject = %obj;
    sourceSlot = 0;
    client = %obj.client;
  };

  %obj.projectileSchedule_ = %this.schedule(4000, "onTargetEnterLOS", %obj);
}

function SkeletalDragonDungeonLevel::onTargetExitLOS(%this, %obj)
{
  cancelAll(%this);
}

function SkeletalDragonDungeonLevel::onReachDestination(%this, %ai)
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

function SkeletalDragonDungeonLevel::onMoveStuck(%this, %ai)
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

function SkeletalDragonDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function SkeletalDragonClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function SkeletalDragonDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
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

  %collObj.damage(%obj, %vec, 50, "melee");

  %targetEmitterNode = new ParticleEmitterNode()
  {
    datablock = PoisonEmitterNodeData;
    emitter = PoisonEmitter;
    active = true;
    velocity = 0.0;
  };

  %poison = new ScriptObject()
  {
    class = "PoisonInstanceSkillsGM";
    emitterNode_ = %targetEmitterNode;
    pulseInterval_ = 1.0;
    pulseIntervalCount_ = 0;
    pulseDuration_ = 10000;
    target_ = %collObj;
    power_ = 10.0;
  };

  %collObj.mountObject(%targetEmitterNode, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));

  %targetEmitterNode.schedule(10000, "delete");

  %poison.schedule(0, "Pulse");
  %poison.schedule(10000, "delete");

  %obj.applyRepair(100);

  %obj.canAttack_ = false;
  %obj.schedule(1000, "AttackCD");

  %obj.setActionThread("melee");
}
