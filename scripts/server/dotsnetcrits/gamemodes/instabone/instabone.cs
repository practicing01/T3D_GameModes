function InstaboneAIClass::SetTarget(%this)
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

function InstaboneGMServer::SpawnNPC(%this)
{
  %npc = new AiPlayer()
  {
     dataBlock = "InstaboneAI";
     class = "InstaboneAIClass";
     state_ = "idle";
     followTarget_ = "";
     parent_ = %this;
  };

  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%npc, %spawnPoint);

  %npc.setTransform(%transform);

  %npc.SetTarget();
}

function InstaboneGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "InstaboneGMServerQueue";

  for (%x = 0; %x < %this.npcMax_; %x++)
  {
    %this.SpawnNPC();
  }
}

function InstaboneGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
}

function InstaboneAI::onDisabled(%this, %ai, %state)
{
  if (!isObject(%ai.parent_))
  {
    %ai.schedule(500, "delete");
    return;
  }

  %ai.parent_.SpawnNPC();
  //parent::onDisabled(%this, %ai, %state);
  %ai.schedule(500, "delete");
}

function InstaboneAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (!isObject(%collObj))
  {
    %obj.SetTarget();
    return;
  }

  if (%collObj.getClassName() $= "Player")
  {
    %collObj.damage(%obj, %vec, 1000, "instabone");
    %obj.SetTarget();
  }
}

if (isObject(InstaboneGMServerSO))
{
  InstaboneGMServerSO.delete();
}
else
{
  new ScriptObject(InstaboneGMServerSO)
  {
    class = "InstaboneGMServer";
    EventManager_ = "";
    npcMax_ = 16;
  };
}
