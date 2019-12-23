function NoirBirdAIClass::Poop(%this)
{
  if (%this.parent_.decalSet_.getCount() >= %this.parent_.decalMax_)
  {
    return;
  }

  %pos = %this.position;//VectorAdd(%this.position, "0.0 0.0 1.0");
  %norm = "0.0 0.0 1.0";
  %rot = 0.5;
  %scale = 1.0;

  %id = decalManagerAddDecal(%pos, %norm, %rot, %scale, "poopDecalnoirbird", true);

  %decalSO = new ScriptObject()
  {
    id_ = %id;
  };

  %this.parent_.decalSet_.add(%decalSO);

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);

    commandToClient(%client, 'NoirBirdPoop', %pos);
  }

  %this.poopSchedule_ = %this.schedule(10000, "Poop");
}

function NoirBirdAIClass::RandyTely(%this)
{
  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%this, %spawnPoint);

  %this.setTransform(%transform);

  %this.setDest();

  %this.telySchedule_ = %this.schedule(30000, "RandyTely");
}

function NoirBirdAI::onRemove(%this, %npc)
{
  cancel(%npc.poopSchedule_);
  cancel(%npc.telySchedule_);
}

function NoirBirdAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function NoirBirdAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function NoirBirdAIClass::setDest(%this)
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

function NoirBirdGMServer::SpawnNPC(%this)
{
  %npc = new AiPlayer()
  {
     dataBlock = "NoirBirdAI";
     class = "NoirBirdAIClass";
     state_ = "idle";
     followTarget_ = "";
     parent_ = %this;
     poopSchedule_ = "";
     telySchedule_ = "";
  };

  %this.npc_ = %npc;

  MissionCleanup.add(%npc);

  %npc.RandyTely();

  %npc.poopSchedule_ = %npc.schedule(10000, "Poop");
  %npc.telySchedule_ = %npc.schedule(30000, "RandyTely");
}

function NoirBirdGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "NoirBirdGMServerQueue";

  %this.decalSet_ = new SimSet();

  %this.SpawnNPC();
}

function NoirBirdGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.npc_))
  {
    %this.npc_.delete();
  }

  if (isObject(%this.decalSet_))
  {
    for (%x = 0; %x < %this.decalSet_.getCount(); %x++)
    {
      %decalSO = %this.decalSet_.getObject(%x);
      decalManagerRemoveDecal(%decalSO.id_);
    }

    %this.decalSet_.delete();
  }
}

function NoirBirdAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
}

function NoirBirdGMServer::EquipSqueegee(%this, %player)
{
  %db = %player.getDataBlock();
  %db.maxInv[squeegee] = 1;

  %player.setInventory(squeegee, 1);
  %player.addToWeaponCycle(squeegee);
  %player.use(squeegee);
}

function NoirBirdGMServer::loadOut(%this, %player)
{
  %this.EquipSqueegee(%player);
}

if (isObject(NoirBirdGMServerSO))
{
  NoirBirdGMServerSO.delete();
}
else
{
  new ScriptObject(NoirBirdGMServerSO)
  {
    class = "NoirBirdGMServer";
    EventManager_ = "";
    npc_ = "";
    decalSet_ = "";
    decalMax_ = 50;
  };

  DNCServer.loadOutListeners_.add(NoirBirdGMServerSO);
  MissionCleanup.add(NoirBirdGMServerSO);
}
