function scarecrowScriptMsgListener::onClientLeaveGame(%this, %client)
{
  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %npc = %this.npcArray_.getValue(%index);

    if (isObject(%npc))
    {
      cancel(%npc.targetSchedule_);
      %npc.delete();
    }

    %this.npcArray_.erase(%index);
    //can choose to delete npc here.
  }
}

function scarecrowScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();
  %this.rayMask_ = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;
}

function scarecrowScriptMsgListener::onRemove(%this)
{
  if (isObject(%this.npcArray_))
  {
    for (%x = 0; %x < %this.npcArray_.count(); %x++)
    {
      %npc = %this.npcArray_.getValue(%x);

      if (isObject(%npc))
      {
        cancel(%npc.targetSchedule_);
        %npc.delete();
      }
    }

    %this.npcArray_.delete();
  }
}

function scarecrowAI::onDisabled(%this, %obj, %state)
{
  %obj.setDamageLevel(0);
  %obj.setDamageState("Enabled");
  %obj.setScale("1 1 1");
  %spawnPoint = PlayerDropPoints.getRandom();
  %obj.setTransform(%spawnPoint.getTransform());
  %obj.parentNPCScript_.NPCSetDestination(%obj);
  return;
  parent::onDisabled(%this, %obj, %state);

  %obj.state_ = "dead";
  %obj.fire(false);
  cancel(%obj.targetSchedule_);
}

function scarecrowAI::onTargetEnterLOS(%this, %ai)
{
  %ai.fire(true);
}

function scarecrowAI::onTargetExitLOS(%this, %ai)
{
  %ai.fire(false);
}

function scarecrowAIClass::AcquireTarget(%this)
{
  %pos = %this.getPosition();
  %teleDir = %this.getForwardVector();

  %size = %this.getObjectBox();
  %scale = %this.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %pos = VectorAdd( %pos, VectorScale(%teleDir, %sizex + 2) );

  %targetObject = containerFindFirst($TypeMasks::PlayerObjectType, %pos, "4 4 4");

  while ( %targetObject != 0 )
  {
    if (%targetObject != %this)
    {
      %targetObject.applyImpulse("0 0 0", VectorScale(%this.getUpVector(), 5000.0));
    }

    %targetObject = containerFindNext();
  }

  %this.targetSchedule_ = %this.schedule(2 * 1000, "AcquireTarget");
  return;

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

  %this.targetSchedule_ = %this.schedule(5 * 1000, "AcquireTarget");
}

function scarecrowScriptMsgListener::SpawnNPC(%this, %client)
{
  %npc = new AiPlayer()
  {
    dataBlock = scarecrowAI;
    class = "scarecrowAIClass";
    //scale = "0.25 0.25 0.25";
    parentClient_ = %client;
    parentNPCScript_ = %this;
    state_ = "idle";
    targetSchedule_ = "";
  };

  MissionCleanup.add(%npc);

  //%npc.mountImage(crayonImage, 0);
  //%npc.mountImage(lipoRifleImage, 0);
  //%npc.incInventory(lipoRifleAmmo, 100);

  return %npc;
}

function scarecrowScriptMsgListener::NPCSetDestination(%this, %npc)
{
  %npc.state_ = strreplace(%npc.state_, "idle", "");
  %npc.state_ = %npc.state_ @ "moving";
  %spawnPoint = PlayerDropPoints.getRandom();
  %npc.setPathDestination(%spawnPoint.position);
}

function scarecrowScriptMsgListener::onNPCLoadRequest(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %npcName = %data.getValue(%data.getIndexFromKey("npc"));
  %player = %client.getControlObject();

  if (%npcName !$= %this.npc_)
  {
    return;
  }

  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %npc = %this.npcArray_.getValue(%index);

    //delete npc
    if (isObject(%npc))
    {
      %npc.delete();
      commandToClient(%client, 'NPCLoadDNC', %npcName, false);
      return;
    }

    //create npc.
    %pos = %player.getPosition();
    %teleDir = %player.getForwardVector();

    %size = %player.getObjectBox();
    %scale = %player.getScale();
    %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
    %sizex *= 1.5;

    %scarecrow = %this.SpawnNPC(%client);
    %scarecrow.rotation = %player.rotation;

    %sizeTarget = %scarecrow.getObjectBox();
    %scaleTarget = %scarecrow.getScale();
    %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
    %sizexTarget *= 1.5;

    %scarecrow.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

    %this.npcArray_.setValue(%scarecrow, %index);

    %this.NPCSetDestination(%scarecrow);

    %scarecrow.targetSchedule_ = %scarecrow.schedule(5 * 1000, "AcquireTarget");

    commandToClient(%client, 'NPCLoadDNC', %npcName, true);

    return;
  }

  //add index.
  %pos = %player.getPosition();
  %teleDir = %player.getForwardVector();

  %size = %player.getObjectBox();
  %scale = %player.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %scarecrow = %this.SpawnNPC(%client);
  %scarecrow.rotation = %player.rotation;

  %sizeTarget = %scarecrow.getObjectBox();
  %scaleTarget = %scarecrow.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %scarecrow.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

  %this.npcArray_.add(%client, %scarecrow);

  %this.NPCSetDestination(%scarecrow);

  %scarecrow.targetSchedule_ = %scarecrow.schedule(5 * 1000, "AcquireTarget");

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);
}

function scarecrowScriptMsgListener::MoveNPC(%this, %npc, %player)
{
  if (strstr(%npc.state_, "dead") != -1)
  {
    return;
  }

  %rayResult = %player.doRaycast(10000.0, %this.rayMask_);

  //%objTarget = firstWord(%rayResult);
  %objTargetPos = getWords(%rayResult, 1, 3);
  //%objTargetDir = getWords(%rayResult, 4, 6);

  %npc.state_ = strreplace(%npc.state_, "idle", "");
  %npc.state_ = %npc.state_ @ "moving";
  %npc.setMoveDestination(%objTargetPos);
}

function scarecrowScriptMsgListener::NPCRangedAttack(%this, %npc, %player)
{
  if (%npc.state_ $= "dead")
  {
    return;
  }

  %rayResult = %player.doRaycast(10000.0, %this.rayMask_);

  //%objTarget = firstWord(%rayResult);
  %objTargetPos = getWords(%rayResult, 1, 3);
  //%objTargetDir = getWords(%rayResult, 4, 6);

  /*%size = %objTarget.getObjectBox();
  %scale = %objTarget.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);*/

  //%npc.state_ = strreplace(%npc.state_, "idle", "");
  //%npc.state_ = %npc.state_ @ "rangedAttacking";//todo find finishedFiring callback so i can remove this state
  //%npc.setAimObject(%objTarget, "0 0" SPC %sizex);
  %npc.mountImage(LurkerGrenadeLauncherImage, 0);
  %npc.incInventory(LurkerGrenadeAmmo, 1);
  %npc.setAimLocation(%objTargetPos);
  %npc.fire(true);
  %npc.incInventory(LurkerGrenadeAmmo, 1);
  %npc.fire(false);
}

function scarecrowScriptMsgListener::NPCMeleeAttack(%this, %npc, %player)
{
  if (strstr(%npc.state_, "dead") != -1)
  {
    return;
  }

  %rayResult = %player.doRaycast(10000.0, %this.rayMask_);//todo: range doesn't seem right

  //%objTarget = firstWord(%rayResult);
  %objTargetPos = getWords(%rayResult, 1, 3);
  //%objTargetDir = getWords(%rayResult, 4, 6);

  %npc.state_ = strreplace(%npc.state_, "idle", "");
  %npc.state_ = %npc.state_ @ "moving";
  %npc.state_ = %npc.state_ @ "melee";
  %npc.setMoveDestination(%objTargetPos);
}

function scarecrowScriptMsgListener::NPCExecuteMeleeAttack(%this, %npc)
{
  if (strstr(%npc.state_, "dead") != -1)
  {
    return;
  }

  %npc.mountImage(fistClubImage, 0);
  %npc.setAimLocation(%objTargetPos);
  %npc.fire(true);
  %npc.fire(false);
}

function scarecrowScriptMsgListener::CommandNPC(%this, %key, %npc, %player)
{
  if (%key $= "numpad0")
  {
    %this.MoveNPC(%npc, %player);
  }
  else if (%key $= "numpad5")
  {
    %this.NPCRangedAttack(%npc, %player);
  }
  else if (%key $= "numpad1")
  {
    %this.NPCMeleeAttack(%npc, %player);
  }
}

function scarecrowScriptMsgListener::onReachDestination(%this, %npc)
{
  %this.NPCSetDestination(%npc);
  return;

  if (strstr(%npc.state_, "moving") != -1)
  {
    %npc.state_ = %npc.state_ @ "idle";
    %npc.state_ = strreplace(%npc.state_, "moving", "");
  }
  if (strstr(%npc.state_, "melee") != -1)
  {
    %npc.state_ = strreplace(%npc.state_, "melee", "");
    %npc.parentNPCScript_.NPCExecuteMeleeAttack(%npc);
  }
}

function scarecrowAI::onReachDestination(%this, %npc)
{
  %npc.parentNPCScript_.onReachDestination(%npc);
}

function scarecrowScriptMsgListener::onNPCActionscarecrow(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %key = %data.getValue(%data.getIndexFromKey("key"));
  %player = %client.getControlObject();

  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %npc = %this.npcArray_.getValue(%index);

    if (isObject(%npc))
    {
      %this.CommandNPC(%key, %npc, %player);
    }
  }
}

function serverCmdNPCActionscarecrow(%client, %key)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("key", %key);
  DNCServer.EventManager_.postEvent("NPCActionscarecrow", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "scarecrowScriptMsgListener";
  npc_ = "scarecrow";
  npcArray_ = "";
  rayMask_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActionscarecrow");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActionscarecrow");
DNCServer.ClientLeaveListeners_.add(%NPC);
