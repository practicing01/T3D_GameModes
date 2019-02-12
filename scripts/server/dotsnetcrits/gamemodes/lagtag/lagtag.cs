function LagTagAIClass::CheckPlayers(%this)
{
  %pos = %this.getPosition();

  initContainerRadiusSearch(%pos, 30, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    if(%targetObject != %this)
    {
     %velocity = %targetObject.getVelocity();

     if (%velocity !$= "0 0 0" && %targetObject.isMethod("damage"))
     {
      %targetObject.damage(%this, %pos, 1000, "LagTag");
     }
    }
  }

  %this.setDest();
  %this.schedule_ = %this.schedule(1000 * getRandom(1, 10), "CountDown");
}

function LagTagAIClass::CountDown(%this)
{
  %this.stop();
  %this.sfxEmitter_.setTransform(%this.getTransform());
  %this.sfxEmitter_.play();
  %this.schedule_ = %this.schedule(1 * 3000, "CheckPlayers");
}

function LagTagAIClass::setDest(%this)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  %this.setPathDestination(%spawnPoint.position);
  %this.setAimLocation(%spawnPoint);
  %this.clearAim();
}

function LagTagAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
}

function LagTagAI::onReachDestination(%this, %ai)
{
  %ai.setDest();
}

function LagTagAI::onMoveStuck(%this, %ai)
{
  %ai.setDest();
}

function LagTagGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "LagTagGMServerQueue";

  %spawnPoint = PlayerDropPoints.getRandom();

  %this.npc_ = new AiPlayer()
  {
    dataBlock = LagTagAI;
    class = LagTagAIClass;
    schedule_ = "";
    sfxEmitter_ = "";
  };

  %this.npc_.setTransform(%spawnPoint.getTransform());

  %this.sfxEmitter_ = new SFXEmitter()
  {
    track = lagtaggm_CountdownSFX;
    referenceDistance = 1.0;
    maxDistance       = 30.0;
  };

  %this.npc_.sfxEmitter_ = %this.sfxEmitter_;

  %this.sfxEmitter_.setTransform(%spawnPoint.getTransform());

  %this.npc_.mountObject(%this.sfxEmitter_, 0);

  %this.npc_.setDest();

  %this.npc_.schedule_ = %this.npc_.schedule(1000 * getRandom(1, 10), "CountDown");
}

function LagTagGMServer::onRemove(%this)
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

  if (isObject(%this.sfxEmitter_))
  {
    %this.sfxEmitter_.delete();
  }
}

if (isObject(LagTagGMServerSO))
{
  LagTagGMServerSO.delete();
}
else
{
  new ScriptObject(LagTagGMServerSO)
  {
    class = "LagTagGMServer";
    EventManager_ = "";
    npc_ = "";
    sfxEmitter_ = "";
  };

  MissionCleanup.add(LagTagGMServerSO);
}
