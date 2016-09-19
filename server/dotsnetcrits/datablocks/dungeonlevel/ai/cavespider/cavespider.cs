if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "CaveSpiderDungeonLevel" SPC "CaveSpiderClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function CaveSpiderDungeonLevel::onReachDestination(%this, %ai)
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

function CaveSpiderDungeonLevel::onMoveStuck(%this, %ai)
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

function CaveSpiderDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function CaveSpiderClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function CaveSpiderDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
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

  %obj.canAttack_ = false;
  %obj.schedule(1000, "AttackCD");

  %obj.setActionThread("melee");
}
