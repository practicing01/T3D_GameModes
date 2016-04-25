function ZombieWavesGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ZombieWavesGMServerQueue";

  %pos = ClientGroup.getObject(0).getControlObject().getPosition();

  %rot = ClientGroup.getObject(0).getControlObject().rotation;

  %this.ZombieSpawns_ = new SimSet();
  %this.Zombies_ = new SimSet();

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
    %zombieSpawn = new Marker()
    {
      position = %pos;
      rotation = %rot;
    };

    %this.ZombieSpawns_.add(%zombieSpawn);
    MissionCleanup.add(%zombieSpawn);
  }

  %this.SpawnZombie();
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
    position = %zombieSpawn.getPosition();
    rotation = %zombieSpawn.rotation;
    target_ = "";
    canAttack_ = true;
  };

  %this.Zombies_.add(%zombie);

  %randyTarget = ClientGroup.getRandom();

  %zombie.target_ = %randyTarget.getControlObject();

  %pos = %zombie.target_.getPosition();

  %zombie.setMoveDestination(%pos);

  %this.zombieCount_++;

  %this.zombieSpawnSchedule_ = %this.schedule(%this.zombieSpawnInterval_ * 1000, "SpawnZombie");
}

function ZombieZombieWavesGM::onReachDestination(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    %ai.target_ = ClientGroup.getRandom().getControlObject();
  }

  %ai.setMoveDestination(VectorAdd(%ai.target_.getPosition(), %forwardVector));

}

function ZombieZombieWavesGM::onMoveStuck(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    %ai.target_ = ClientGroup.getRandom().getControlObject();
  }

  %ai.setMoveDestination(VectorAdd(%ai.target_.getPosition(), %forwardVector));

}

function ZombieZombieWavesGM::onDisabled(%this, %obj, %state)
{
  parent::onDisabled(%this, %obj, %state);

  if (isObject(ZombieWavesGMServerSO))
  {
    ZombieWavesGMServerSO.Zombies_.remove(%obj);
    ZombieWavesGMServerSO.zombieCount_--;
  }
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

  %obj.setActionThread("Celebrate_01", false);
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
    maxZombies_ = 20;
    zombieCount_ = 0;
    zombieSpawnSchedule_ = 0;
    zombieSpawnInterval_ = 5;
  };
}
