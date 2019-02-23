function toySoldierScriptMsgListener::onClientLeaveGame(%this, %client)
{
  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %this.npcArray_.erase(%index);
    //can choose to delete npc here.
  }
}

function toySoldierScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();
  %this.rayMask_ = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;
}

function toySoldierScriptMsgListener::onRemove(%this)
{
  if (isObject(%this.npcArray_))
  {
    for (%x = 0; %x < %this.npcArray_.count(); %x++)
    {
      %npc = %this.npcArray_.getValue(%x);

      if (isObject(%npc))
      {
        %npc.delete();
      }
    }

    %this.npcArray_.delete();
  }
}

function ToySoldierAI::onDisabled(%this, %obj, %state)
{
  parent::onDisabled(%this, %obj, %state);

  %obj.state_ = "dead";
  %obj.clearAim();
  %obj.fire(false);
}

function toySoldierScriptMsgListener::SpawnNPC(%this, %client)
{
  %npc = new AiPlayer()
  {
    dataBlock = ToySoldierAI;
    scale = "0.25 0.25 0.25";
    parentClient_ = %client;
    parentNPCScript_ = %this;
    state_ = "idle";
  };

  MissionCleanup.add(%npc);

  %npc.mountImage(staplerImage, 0);
  %npc.incInventory(staplerAmmo, 2);

  return %npc;
}

function toySoldierScriptMsgListener::onNPCLoadRequest(%this, %data)
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

    %toySoldier = %this.SpawnNPC(%client);
    %toySoldier.rotation = %player.rotation;

    %sizeTarget = %toySoldier.getObjectBox();
    %scaleTarget = %toySoldier.getScale();
    %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
    %sizexTarget *= 1.5;

    %toySoldier.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

    %this.npcArray_.setValue(%toySoldier, %index);

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

  %toySoldier = %this.SpawnNPC(%client);
  %toySoldier.rotation = %player.rotation;

  %sizeTarget = %toySoldier.getObjectBox();
  %scaleTarget = %toySoldier.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %toySoldier.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

  %this.npcArray_.add(%client, %toySoldier);

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);
}

function toySoldierScriptMsgListener::MoveNPC(%this, %npc, %player)
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

function toySoldierScriptMsgListener::NPCRangedAttack(%this, %npc, %player)
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

function toySoldierScriptMsgListener::NPCMeleeAttack(%this, %npc, %player)
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

function toySoldierScriptMsgListener::NPCExecuteMeleeAttack(%this, %npc)
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

function toySoldierScriptMsgListener::CommandNPC(%this, %key, %npc, %player)
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

function toySoldierScriptMsgListener::onReachDestination(%this, %npc)
{
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

function ToySoldierAI::onReachDestination(%this, %npc)
{
  %npc.parentNPCScript_.onReachDestination(%npc);
}

function toySoldierScriptMsgListener::onNPCActionToySoldier(%this, %data)
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

function serverCmdNPCActionToySoldier(%client, %key)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("key", %key);
  DNCServer.EventManager_.postEvent("NPCActionToySoldier", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "toySoldierScriptMsgListener";
  npc_ = "toysoldier";
  npcArray_ = "";
  rayMask_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActionToySoldier");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActionToySoldier");
DNCServer.ClientLeaveListeners_.add(%NPC);
