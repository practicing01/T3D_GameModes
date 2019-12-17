function ZombieWavesGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ZombieWavesGMServerQueue";

  %this.ZombieSpawns_ = new SimSet();
  %this.Zombies_ = new SimSet();

  %this.zombieAISchedule_ = "";

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "ZombieSpawnsZombieWavesGM")
    {
      for (%y = 0; %y < %obj.getCount(); %y++)
      {
        %this.ZombieSpawns_.add(%obj.getObject(%y));
      }
      break;
    }
  }

  if (%this.ZombieSpawns_.getCount() == 0)
  {
    %spawnPoint = PlayerDropPoints.getRandom();
    %spawnPos = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, DNCServer.envRayMask_, %spawnPoint);
    %spawnRot = %spawnPoint.rotation;

    %zombieSpawn = new Marker()
    {
      position = %spawnPos;
      rotation = %spawnRot;
    };

    %this.ZombieSpawns_.add(%zombieSpawn);
    MissionCleanup.add(%zombieSpawn);
  }

  %this.SpawnZombie();
  %this.zombieAISchedule_ = %this.schedule(1 * 1000, "ProcessAI");
}

function ZombieWavesGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if(isObject(%this.Zombies_))
  {
    %this.Zombies_.deleteAllObjects();
    %this.Zombies_.delete();
  }

  if(isObject(%this.ZombieSpawns_))
  {
    %this.ZombieSpawns_.delete();
  }

  cancel(%this.zombieSpawnSchedule_);
  cancel(%this.ProcessAI);
}

function ZombieWavesGMServer::ProcessAI(%this)
{
  for (%x = 0; %x < %this.Zombies_.getCount(); %x++)
  {
    %ai = %this.Zombies_.getObject(%x);
    ZombieZombieWavesGM.onReachDestination(%ai);
  }

  %this.zombieAISchedule_ = %this.schedule(1 * 1000, "ProcessAI");
}

function ZombieWavesGMServer::SpawnZombie(%this)
{
  if (%this.zombieCount_ >= %this.maxZombies_)
  {
    %this.zombieSpawnSchedule_ = %this.schedule(%this.zombieSpawnInterval_ * 1000, "SpawnZombie");
    return;
  }

  %zombieSpawn = %this.ZombieSpawns_.getRandom();

  %zombie = new AiPlayer()
  {
    dataBlock = ZombieZombieWavesGM;
    class = ZombieClassZombieWavesGM;
    //mMoveTolerance = 1.0;
    //moveStuckTolerance = 1.0;
    position = %zombieSpawn.getPosition();
    rotation = %zombieSpawn.rotation;
    target_ = "";
    canAttack_ = true;
  };

  MissionCleanup.add(%zombie);

  %this.Zombies_.add(%zombie);

  %randyTarget = ClientGroup.getRandom();

  %zombie.target_ = %randyTarget.getControlObject();

  %targPos = %zombie.target_.getPosition();

  //setField(%targPos, 2, getField(%zombie.getPosition(), 2));

  %zombie.setMoveDestination(%targPos);

  %zombie.setAimObject(%zombie.target_);

  %this.zombieCount_++;

  %this.zombieSpawnSchedule_ = %this.schedule(%this.zombieSpawnInterval_ * 1000, "SpawnZombie");
}

function ZombieZombieWavesGM::onReachDestination(%this, %ai)
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

function ZombieZombieWavesGM::onMoveStuck(%this, %ai)
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

function ZombieZombieWavesGM::onDisabled(%this, %obj, %state)
{
  if (isObject(ZombieWavesGMServerSO))
  {
    ZombieWavesGMServerSO.Zombies_.remove(%obj);
    ZombieWavesGMServerSO.zombieCount_--;
  }

  %obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  %obj.schedule(500, "delete");
}

function ZombieClassZombieWavesGM::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function ZombieZombieWavesGM::onCollision(%this, %obj, %collObj, %vec, %len)
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

if (isObject(ZombieWavesGMServerSO))
{
  ZombieWavesGMServerSO.delete();
}
else
{
  new ScriptObject(ZombieWavesGMServerSO)
  {
    class = "ZombieWavesGMServer";
    EventManager_ = "";
    Zombies_ = "";
    ZombieSpawns_ = "";
    maxZombies_ = 10;
    zombieCount_ = 0;
    zombieSpawnSchedule_ = 0;
    zombieSpawnInterval_ = 5;
  };

  MissionCleanup.add(ZombieWavesGMServerSO);
}
