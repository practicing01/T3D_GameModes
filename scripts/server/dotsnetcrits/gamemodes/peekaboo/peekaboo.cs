function PeekabooAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  if (!isObject(%collObj))
  {
    return;
  }

  if (%collObj.getClassName() !$= "Player")
  {
    return;
  }

  %damageState = %obj.getDamageState();
  if (%damageState $= "Disabled" || %damageState $= "Destroyed")
  {
    return;
  }

  %collObj.damage(%obj, %vec, 1000, "death");
}

function PeekabooAIClass::Disappear(%this)
{
  %this.stop();
  %this.setHidden(true);
  %this.stopAudio(0);
  %this.schedule_ = %this.schedule(1000 * getRandom(1, 10), "ShowDeath");
}

function PeekabooAIClass::Attack(%this)
{
  %pos = %this.getPosition();

  initContainerRadiusSearch(%pos, 40, $TypeMasks::PlayerObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    if(%targetObject != %this)
    {
      %this.playAudio(0, Peekaboogm_SongSFX);
      %this.followObject(%targetObject, 0);
      break;
    }
  }

  %this.schedule_ = %this.schedule(10000, "Disappear");
}

function PeekabooAIClass::ShowDeath(%this)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  %this.setTransform(%spawnPoint.getTransform());
  %this.setHidden(false);
  %this.Attack();
}

function PeekabooAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
}

function PeekabooGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PeekabooGMServerQueue";

  %spawnPoint = PlayerDropPoints.getRandom();

  %this.npc_ = new AiPlayer()
  {
    dataBlock = PeekabooAI;
    class = PeekabooAIClass;
    schedule_ = "";
  };

  %this.npc_.setHidden(true);
  %this.npc_.schedule_ = %this.npc_.schedule(1000 * getRandom(1, 10), "ShowDeath");
}

function PeekabooGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.npc_))
  {
    cancel(%this.npc_.schedule_);
    %this.npc_.delete();
  }
}

if (isObject(PeekabooGMServerSO))
{
  PeekabooGMServerSO.delete();
}
else
{
  new ScriptObject(PeekabooGMServerSO)
  {
    class = "PeekabooGMServer";
    EventManager_ = "";
    npc_ = "";
  };

  MissionCleanup.add(PeekabooGMServerSO);
}
