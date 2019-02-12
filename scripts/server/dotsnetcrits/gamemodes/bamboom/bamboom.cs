function BamboomAI::onTargetEnterLOS(%this, %ai)
{
  %ai.fire(true);
}

function BamboomAI::onTargetExitLOS(%this, %ai)
{
  %ai.fire(false);
}

function BamboomAIClass::AcquireTarget(%this)
{
  %closestPlayer = -1;
  %minDist = 10000;

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %obj = ClientGroup.getObject(%x).getControlObject();

    %dist = VectorDist(%this.position, %obj.position);

    if (%dist < %minDist)
    {
      %minDist = %dist;
      %closestPlayer = %obj;
    }
  }

  if (%closestPlayer != -1)
  {
    %this.setAimObject(%closestPlayer, "0 0 1");
  }

  %this.targetSchedule_ = %this.schedule(5000, "AcquireTarget");
}

function BamboomClass::Detonate(%this)
{
  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %player = %client.getControlObject();
    %player.damage(%this, "0 0 0", 1000, "Bamboom");
  }

  BamboomAI.onDisabled(%this.planter_, "disabled");
  %this.schedule(500, "delete");
}

function BamboomClass::UseObject(%this, %player)
{
  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    Game.incScore(%client, 1, false);
  }

  centerPrintAll("Bamboom diffused!", 2, 1);
  cancel(%his.detonateSchedule_);
  BamboomAI.onDisabled(%this.planter_, "disabled");
  %this.schedule(500, "delete");
}

function BamboomGMServer::SpawnBamboom(%this, %planter)
{
  %this.bamboom_ = new StaticShape()
  {
     dataBlock = "BamboomStaticShapeData";
     class = "BamboomClass";
     planter_ = %planter;
  };

  %this.bamboom_.setTransform(%planter.getTransform());

  centerPrintAll("Bamboom Planted! 30 seconds remaining!", 3, 1);
  %this.bamboom_.detonateSchedule_ = %this.bamboom_.schedule(30000, "Detonate");
}

function BamboomAI::onReachDestination(%this, %npc)
{
  if (%npc.isPlanter_ == true)
  {
    %npc.parent_.SpawnBamboom(%npc);
    return;
  }

  %npc.setDest();
}

function BamboomAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function BamboomAIClass::setDest(%this)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  %this.setPathDestination(%spawnPoint.position);
  %this.setAimLocation(%spawnPoint);
  %this.clearAim();
}

function BamboomGMServer::SpawnNPC(%this)
{
  %npc = new AiPlayer()
  {
     dataBlock = "BamboomAI";
     class = "BamboomAIClass";
     state_ = "idle";
     parent_ = %this;
     isPlanter_ = false;
     targetSchedule_ = "";
  };

  %npc.mountImage(phaser0Image, 0);
  %npc.incInventory(phaser0Ammo, 100);

  %this.npcSet_.add(%npc);

  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%npc, %spawnPoint);

  %npc.setTransform(%transform);

  %npc.setDest();

  if (!isObject(%this.planter_))
  {
    %this.planter_ = %npc;
    %npc.isPlanter_ = true;
  }

  %npc.targetSchedule_ = %npc.schedule(5000, "AcquireTarget");
}

function BamboomGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BamboomGMServerQueue";

  for (%x = 0; %x < %this.npcMax_; %x++)
  {
    %this.SpawnNPC();
  }
}

function BamboomGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.npcSet_))
  {
    for (%x = 0;%x < %this.npcSet_.getCount(); %x++)
    {
      %ai = %this.npcSet_.getObject(%x);
      cancel(%ai.targetSchedule_);
    }
    %this.npcSet_.deleteAllObjects();
    %this.npcSet_.delete();
  }

  if (isObject(%this.bamboom_))
  {
    cancel(%this.bamboom_.detonateSchedule_);
    %this.bamboom_.delete();
  }
}

function BamboomAI::onDisabled(%this, %ai, %state)
{
  if (!isObject(%ai.parent_))
  {
    cancel(%ai.targetSchedule_);
    %ai.schedule(500, "delete");
    return;
  }

  if (%ai == %ai.parent_.planter_)
  {
    %ai.parent_.planter_ = "";
  }

  %ai.parent_.npcSet_.remove(%ai);
  %ai.parent_.SpawnNPC();
  //parent::onDisabled(%this, %ai, %state);
  cancel(%ai.targetSchedule_);
  %ai.schedule(500, "delete");
}

if (isObject(BamboomGMServerSO))
{
  BamboomGMServerSO.delete();
}
else
{
  new ScriptObject(BamboomGMServerSO)
  {
    class = "BamboomGMServer";
    EventManager_ = "";
    npcMax_ = 4;
    npcSet_ = new SimSet();
    planter_ = "";
    bamboom_ = "";
  };
}
