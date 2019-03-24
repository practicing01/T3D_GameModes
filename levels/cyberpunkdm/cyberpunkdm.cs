return;
datablock PlayerData(CyberpunkDMAI : DemoPlayer)
{
  //
};

datablock TriggerData(CyberpunkDMAITrigger)
{
  tickPeriodMS = 1000;
};

function CyberpunkDMLevel::onAdd(%this)//should be using onMissionLoaded, need to redo.
{
  CyberpunkDMAI.Spawn();
}

function CyberpunkDMAI::onRemove(%this, %ai)
{
  %ai.stop();
}

function CyberpunkDMAI::onReachDestination(%this, %ai)
{
  if (!isObject(%ai.path_))
  {
    %ai.stop();
    return;
  }

  %ai.marker_++;

  if (%ai.marker_ >= %ai.path_.getCount())
  {
    %ai.marker_ = 0;
  }

  %marker = %ai.path_.getObject(%ai.marker_);

  %ai.setMoveDestination(%marker.position);
}

function CyberpunkDMAI::Spawn(%this)
{
  %path = "";

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);

    if (%obj.name $= "aipath")
    {
      %path = %obj;
      break;
    }
  }

  %npc = new AiPlayer()
  {
    dataBlock = CyberpunkDMAI;
    class = CyberpunkDMAIClass;
    position = %position;
    path_ = %path;
    marker_ = 1;
    targets_ = new SimSet();
    aiTrigger_ = "";
  };

  MissionCleanup.add(%npc);

  %npc.mountImage(staplerImage, 0);
  %npc.incInventory(staplerAmmo, 100);

  %marker = %npc.path_.getObject(0);

  %npc.position = %marker.position;

  %marker = %npc.path_.getObject(1);

  %npc.setMoveDestination(%marker.position);

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);

    if (%obj.name $= "aiTrigger")
    {
      %npc.aiTrigger_ = %obj;
      %obj.aiPlayer_ = %npc;
      break;
    }
  }

  return %npc;
}

function CyberpunkDMAI::onDisabled(%this, %obj, %state)
{
  parent::onDisabled(%this, %obj, %state);

  %obj.clearAim();
  %obj.fire(false);

  %npc = %this.Spawn();

  for (%x = 0;%x < %obj.targets_.getCount(); %x++)
  {
    %npc.targets_.add(%obj.targets_.getObject(%x));
  }

  %npc.setAimObject(%obj.getAimObject(), "0 0 1");
}

function CyberpunkDMAI::onTargetEnterLOS(%this, %ai)
{
  %ai.fire(true);
  %ai.incInventory(staplerAmmo, 1);
}

function CyberpunkDMAI::onTargetExitLOS(%this, %ai)
{
  %ai.fire(false);
}

function CyberpunkDMAITrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj.class $= "CyberpunkDMAIClass")
  {
    return;
  }

  if (!%trigger.aiPlayer_.targets_.isMember(%obj))
  {
    %trigger.aiPlayer_.targets_.add(%obj);
    %trigger.aiPlayer_.setAimObject(%obj, "0 0 1");
  }
}

function CyberpunkDMAITrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (%obj.class $= "CyberpunkDMAIClass")
  {
    return;
  }

  if (%trigger.aiPlayer_.targets_.isMember(%obj))
  {
    %trigger.aiPlayer_.targets_.remove(%obj);

    if (%trigger.aiPlayer_.targets_.getCount())
    {
      %trigger.aiPlayer_.setAimObject(%trigger.aiPlayer_.targets_.getObject(0), "0 0 1");
    }
    else
    {
      %trigger.aiPlayer_.fire(false);
    }
  }
}
