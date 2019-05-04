function BloodFlowersAIClass::SetTarget(%this)
{
  if (!isObject(%this))
  {
    return;
  }

  %randyTarget = ClientGroup.getRandom();

  %this.followTarget_ = %randyTarget.getControlObject();

  if (!isObject(%this.followTarget_))
  {
    %this.schedule(1000, "SetTarget");
    return;
  }

  %this.followObject(%this.followTarget_, 0);
  %this.schedule(10000, "SetTarget");
}

function BloodFlowersAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function BloodFlowersAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function BloodFlowersAIClass::setDest(%this)
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
  
  %this.setAimLocation(%spawnPoint);
  %this.clearAim();
}

function BloodFlowersGMServer::SpawnNPC(%this, %client)
{
  %npc = new AiPlayer()
  {
     dataBlock = "BloodFlowersAI";
     class = "BloodFlowersAIClass";
     state_ = "idle";
     followTarget_ = "";
     parent_ = %this;
  };

  MissionCleanup.add(%npc);

  %npc.setTransform(%client.player.getTransform());

  %npc.setDest();
}

function BloodFlowersGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BloodFlowersGMServerQueue";
}

function BloodFlowersGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
}

function BloodFlowersAI::onDisabled(%this, %ai, %state)
{
  //parent::onDisabled(%this, %ai, %state);
  %ai.schedule(500, "delete");
}

function BloodFlowersAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (!isObject(%collObj))
  {
    //%obj.SetTarget();
    return;
  }

  if (%collObj.getClassName() $= "Player")
  {
    %collObj.damage(%obj, %vec, 30, "BloodFlowers");
    //%obj.SetTarget();
  }
}

function BloodFlowersGMServerSO::onDeath(%this, %game, %client, %sourceObject, %sourceClient, %damageType, %damLoc)
{
  %this.SpawnNPC(%client);
}

if (isObject(BloodFlowersGMServerSO))
{
  BloodFlowersGMServerSO.delete();
}
else
{
  new ScriptObject(BloodFlowersGMServerSO)
  {
    class = "BloodFlowersGMServer";
    EventManager_ = "";
  };

  DNCServer.deathListeners_.add(BloodFlowersGMServerSO);
  MissionCleanup.add(BloodFlowersGMServerSO);
}
