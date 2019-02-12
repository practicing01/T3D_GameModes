if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "UndeadNightgauntDungeonLevel" SPC "UndeadNightgauntClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function UndeadNightgauntDungeonLevel::onAdd(%this, %obj)
{
  %obj.setCloaked(true);

  %obj.sprite_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/levels/dungeonunits/undead-nightgaunt/undeadnightgaunt.cached.dts";
    collisionType = "none";
  };

  %obj.mountObject(%obj.sprite_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));
}

function UndeadNightgauntDungeonLevel::onRemove(%this, %obj)
{
  cancelAll(%obj);
  cancelAll(%this);
  %obj.sprite_.delete();
}

function UndeadNightgauntDungeonLevel::onReachDestination(%this, %ai)
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

function UndeadNightgauntDungeonLevel::onMoveStuck(%this, %ai)
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

function UndeadNightgauntDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function UndeadNightgauntClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function UndeadNightgauntDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
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
    datablock = DefaultEmitterNodeData;
    emitter = GrenadeExpDustEmitter;
    active = true;
    velocity = 0.0;
    position = %obj.position;
  };

  %targetEmitterNode.schedule(5000, "delete");

  %obj.canAttack_ = false;
  %obj.schedule(1000, "AttackCD");

  %obj.setActionThread("melee");
}
