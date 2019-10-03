function HellwashAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function HellwashAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function HellwashAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  return;
}

function HellwashAIClass::UseObject(%this, %player)
{
  if (%player.clothes_ > 0)
  {
    Game.incScore(%player.client, %player.clothes_, false);
    %player.clothes_ = 0;
  }
}

function HellwashAIClass::setDest(%this)
{
  if (!isObject(PlayerDropPoints))
  {
    return;
  }

  %spawnPoint = PlayerDropPoints.getRandom();
  %result = %this.setPathDestination(%spawnPoint.position);

  if (!%result)
  {
    %this.schedule(1000, "setDest");
    return;
  }

  %this.setAimLocation(%spawnPoint.position);
  %this.clearAim();
}

function HellwashClothes::onCollision(%this, %obj, %collObj, %vec, %len)
{
  if (%collObj.getClassName() !$= "Player")
  {
    return;
  }

  %collObj.clothes_ = %collObj.clothes_ + 1;
  %obj.delete();
}

function HellwashGMServer::SpawnClothes(%this, %client)
{
  %position = %client.player.position;
  %forward = VectorScale(%client.player.getForwardVector(), 4);
  %forward = VectorAdd(%forward, "0 0 0.1");
  %position = VectorAdd(%position, %forward);

  %clothes = new Item()
  {
    datablock = "HellwashClothes";
    class = "HellwashClothesClass";
    position = %position;
    static = true;
  };

  MissionGroup.add(%clothes);
  MissionCleanup.add(%clothes);
}

function HellwashGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HellwashGMServerQueue";

  %this.washingMachine_ = new AiPlayer()
  {
     dataBlock = "HellwashAI";
     class = "HellwashAIClass";
     state_ = "idle";
     parent_ = %this;
  };

  %spawnPoint = PlayerDropPoints.getRandom();
  %this.washingMachine_.position = %spawnPoint.position;

  %this.washingMachine_.setDest();
}

function HellwashGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.washingMachine_))
  {
    %this.washingMachine_.delete();
  }
}

function HellwashGMServerSO::onDeath(%this, %game, %client, %sourceObject, %sourceClient, %damageType, %damLoc)
{
  %this.SpawnClothes(%client);
}

if (isObject(HellwashGMServerSO))
{
  HellwashGMServerSO.delete();
}
else
{
  new ScriptObject(HellwashGMServerSO)
  {
    class = "HellwashGMServer";
    EventManager_ = "";
    washingMachine_ = "";
  };

  DNCServer.deathListeners_.add(HellwashGMServerSO);
  MissionCleanup.add(HellwashGMServerSO);
}
