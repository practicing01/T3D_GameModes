if (isObject(DungeonLevelHandle))
{
  %count = DungeonLevelHandle.shapeAIStrings_.count();
  %string = "UndeadDreadlichDungeonLevel" SPC "UndeadDreadlichClassDungeonLevel";
  DungeonLevelHandle.shapeAIStrings_.add(%count, %string);
}

function UndeadDreadlichDungeonLevel::onAdd(%this, %obj)
{
  %obj.setCloaked(true);

  %sprite = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/levels/dungeonunits/undead-dreadlich/undeaddreadlich.cached.dts";
  };

  %obj.mountObject(%sprite, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));
}

function UndeadDreadlichDungeonLevel::onReachDestination(%this, %ai)
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

function UndeadDreadlichDungeonLevel::onMoveStuck(%this, %ai)
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

function UndeadDreadlichDungeonLevel::onDisabled(%this, %obj, %state)
{
  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function UndeadDreadlichClassDungeonLevel::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function UndeadDreadlichDungeonLevel::onAdd(%this, %obj)
{
  %obj.schedule(5000, "SpawnNPC");
}

function UndeadDreadlichClassDungeonLevel::SpawnNPC(%this)
{
  %string = "UndeadZombieDungeonLevel" SPC "UndeadZombieClassDungeonLevel";

  %npc = new AiPlayer()
  {
    dataBlock = getWord(%string, 0);
    class = getWord(%string, 1);
    //mMoveTolerance = 1.0;
    //moveStuckTolerance = 1.0;
    //moveStuckTestDelay = 1.0;
    position = %this.position;
    //rotation = %zombieSpawn.rotation;
    target_ = "";
    canAttack_ = true;
  };

  %npc.target_ = %this.target_;

  %targPos = %npc.target_.getPosition();

  //setField(%targPos, 2, getField(%zombie.getPosition(), 2));

  %npc.setMoveDestination(%targPos);

  %npc.setAimObject(%npc.target_);

  %this.schedule(5000, "SpawnNPC");
}

function UndeadDreadlichDungeonLevel::onCollision(%this, %obj, %collObj, %vec, %len)
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
