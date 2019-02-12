function HoverWhaleScriptMsgListener::onClientLeaveGame(%this, %client)
{
  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %npc = %this.npcArray_.getValue(%index);

    if (isObject(%npc))
    {
      %npc.delete();
    }

    %this.npcArray_.erase(%index);
  }
}

function HoverWhaleScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();
}

function HoverWhaleScriptMsgListener::onRemove(%this)
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

function HoverWhaleScriptMsgListener::SpawnNPC(%this, %client)
{
  %npc = new HoverVehicle()
  {
    dataBlock = "HoverWhaleDB";
    class = "HoverWhaleClass";
  };

  %player = %client.getControlObject();

  //%npc.setTransform(%player.getTransform());

  %pos = %player.getPosition();

  %teleDir = %player.getForwardVector();

  %size = %player.getObjectBox();
  %scale = %player.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %npc.rotation = %player.rotation;

  %sizeTarget = %npc.getObjectBox();
  %scaleTarget = %npc.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %npc.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

  return %npc;
}

function HoverWhaleScriptMsgListener::onNPCLoadRequest(%this, %data)
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
    %HoverWhale = %this.SpawnNPC(%client);

    %this.npcArray_.setValue(%HoverWhale, %index);

    commandToClient(%client, 'NPCLoadDNC', %npcName, true);

    return;
  }

  //add index.
  %HoverWhale = %this.SpawnNPC(%client);

  %this.npcArray_.add(%client, %HoverWhale);

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);
}

function HoverWhaleScriptMsgListener::CommandNPC(%this, %key, %npc, %player)
{
  //
}

function HoverWhaleScriptMsgListener::onNPCActionHoverWhale(%this, %data)
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

function serverCmdNPCActionHoverWhale(%client, %key)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("key", %key);
  DNCServer.EventManager_.postEvent("NPCActionHoverWhale", %data);

  %data.delete();
}

function HoverWhaleClass::UseObject(%this, %player)
{
  /*%vehicleDB = %this.getDataBlock();
  %vehicleDB.mountPlayer(%this, %player);*/

  %this.mountObject(%player, 0);
  %player.mVehicle = %this;
}

%NPC = new ScriptMsgListener()
{
  class = "HoverWhaleScriptMsgListener";
  npc_ = "HoverWhale";
  npcArray_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActionHoverWhale");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActionHoverWhale");
DNCServer.ClientLeaveListeners_.add(%NPC);
