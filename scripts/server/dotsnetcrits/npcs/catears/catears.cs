function catEarsScriptMsgListener::onClientLeaveGame(%this, %client)
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

function catEarsScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();
}

function catEarsScriptMsgListener::onRemove(%this)
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

function catEarsScriptMsgListener::SpawnNPC(%this, %client)
{
  %npc = new StaticShape()
  {
    dataBlock = "catEarsStaticShapeData";
    collisionType = "None";
  };

  %player = %client.getControlObject();

  //%player.mountObject(%npc, 2, MatrixCreate("0 0 1", "1 0 0 0"));
  %player.mountObject(%npc, 2);

  return %npc;
}

function catEarsScriptMsgListener::onNPCLoadRequest(%this, %data)
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
    %catEars = %this.SpawnNPC(%client);

    %this.npcArray_.setValue(%catEars, %index);

    commandToClient(%client, 'NPCLoadDNC', %npcName, true);

    return;
  }

  //add index.
  %catEars = %this.SpawnNPC(%client);

  %this.npcArray_.add(%client, %catEars);

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);
}

function catEarsScriptMsgListener::CommandNPC(%this, %key, %npc, %player)
{
  //
}

function catEarsScriptMsgListener::onNPCActioncatEars(%this, %data)
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

function serverCmdNPCActioncatEars(%client, %key)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("key", %key);
  DNCServer.EventManager_.postEvent("NPCActioncatEars", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "catEarsScriptMsgListener";
  npc_ = "catEars";
  npcArray_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActioncatEars");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActioncatEars");
DNCServer.ClientLeaveListeners_.add(%NPC);
