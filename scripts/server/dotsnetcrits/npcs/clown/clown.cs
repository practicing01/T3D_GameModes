function clownScriptMsgListener::onClientLeaveGame(%this, %client)
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

function clownScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();
  %this.rayMask_ = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;
}

function clownScriptMsgListener::onRemove(%this)
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

function clownAI::onDisabled(%this, %obj, %state)
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

function clownAI::onTargetEnterLOS(%this, %ai)
{
  %ai.fire(true);
}

function clownAI::onTargetExitLOS(%this, %ai)
{
  %ai.fire(false);
}

function ClownAIClass::AcquireTarget(%this)
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

  %this.targetSchedule_ = %this.schedule(5 * 1000, "AcquireTarget");
}

function clownScriptMsgListener::SpawnNPC(%this, %client)
{
  %npc = new AiPlayer()
  {
    dataBlock = clownAI;
    class = "ClownAIClass";
    //scale = "0.25 0.25 0.25";
    parentClient_ = %client;
    parentNPCScript_ = %this;
    state_ = "idle";
    targetSchedule_ = "";
  };

  MissionCleanup.add(%npc);

  //%npc.mountImage(crayonImage, 0);
  %npc.mountImage(lipoRifleImage, 0);
  %npc.incInventory(lipoRifleAmmo, 100);

  return %npc;
}

function clownScriptMsgListener::NPCSetDestination(%this, %npc)
{
  %npc.state_ = strreplace(%npc.state_, "idle", "");
  %npc.state_ = %npc.state_ @ "moving";
  %spawnPoint = PlayerDropPoints.getRandom();
  %npc.setPathDestination(%spawnPoint.position);
}

function clownScriptMsgListener::onNPCLoadRequest(%this, %data)
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

    %clown = %this.SpawnNPC(%client);
    %clown.rotation = %player.rotation;

    %sizeTarget = %clown.getObjectBox();
    %scaleTarget = %clown.getScale();
    %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
    %sizexTarget *= 1.5;

    %clown.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

    %this.npcArray_.setValue(%clown, %index);

    %this.NPCSetDestination(%clown);

    %clown.targetSchedule_ = %clown.schedule(5 * 1000, "AcquireTarget");

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

  %clown = %this.SpawnNPC(%client);
  %clown.rotation = %player.rotation;

  %sizeTarget = %clown.getObjectBox();
  %scaleTarget = %clown.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %clown.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

  %this.npcArray_.add(%client, %clown);

  %this.NPCSetDestination(%clown);

  %clown.targetSchedule_ = %clown.schedule(5 * 1000, "AcquireTarget");

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);
}

function clownScriptMsgListener::MoveNPC(%this, %npc, %player)
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

function clownScriptMsgListener::NPCRangedAttack(%this, %npc, %player)
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
  %npc.mountImage(staplerImage, 0);
  %npc.incInventory(staplerAmmo, 1);
  %npc.setAimLocation(%objTargetPos);
  %npc.fire(true);
  %npc.incInventory(staplerAmmo, 1);
  %npc.fire(false);
}

function clownScriptMsgListener::NPCMeleeAttack(%this, %npc, %player)
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

function clownScriptMsgListener::NPCExecuteMeleeAttack(%this, %npc)
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

function clownScriptMsgListener::CommandNPC(%this, %key, %npc, %player)
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

function clownScriptMsgListener::onReachDestination(%this, %npc)
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

function clownAI::onReachDestination(%this, %npc)
{
  %npc.parentNPCScript_.onReachDestination(%npc);
}

function clownScriptMsgListener::onNPCActionclown(%this, %data)
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

function serverCmdNPCActionclown(%client, %key)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("key", %key);
  DNCServer.EventManager_.postEvent("NPCActionclown", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "clownScriptMsgListener";
  npc_ = "clown";
  npcArray_ = "";
  rayMask_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActionclown");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActionclown");
DNCServer.ClientLeaveListeners_.add(%NPC);
