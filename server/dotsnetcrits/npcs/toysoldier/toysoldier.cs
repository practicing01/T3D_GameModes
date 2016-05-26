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
    %npc = %this.npcArray_.getValue(%x);

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

    /*%sizeTarget = %objTarget.getObjectBox();
    %scaleTarget = %objTarget.getScale();
    %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
    %sizexTarget *= 1.5;*/
    %sizexTarget = %sizex * 0.25;

    %finalPosition = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

    %toySoldier = new AiPlayer()
    {
      dataBlock = DemoPlayer;
      class = toySoldierDNCNPC;
      position = %finalPosition;
      rotation = %player.rotation;
      scale = "0.25 0.25 0.25";
    };

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

  /*%sizeTarget = %objTarget.getObjectBox();
  %scaleTarget = %objTarget.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;*/
  %sizexTarget = %sizex * 0.25;

  %finalPosition = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

  %toySoldier = new AiPlayer()
  {
    dataBlock = DemoPlayer;
    class = toySoldierDNCNPC;
    position = %finalPosition;
    rotation = %player.rotation;
    scale = "0.25 0.25 0.25";
  };

  %this.npcArray_.add(%client, %toySoldier);

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);
}

function toySoldierScriptMsgListener::MoveNPC(%this, %npc, %player)
{
  %rayResult = %player.doRaycast(10000.0, %this.rayMask_);

  //%objTarget = firstWord(%rayResult);
  %objTargetPos = getWords(%rayResult, 1, 3);
  //%objTargetDir = getWords(%rayResult, 4, 6);

  %npc.setMoveDestination(%objTargetPos);
}

function toySoldierScriptMsgListener::CommandNPC(%this, %key, %npc, %player)
{
  if (%key $= "numpad0")
  {
    %this.MoveNPC(%npc, %player);
  }
}

function toySoldierScriptMsgListener::onNPCActionToySoldier(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %key = %data.getValue(%data.getIndexFromKey("key"));
  %player = %client.getControlObject();

  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %npc = %this.npcArray_.getValue(%x);

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
