function masksScriptMsgListener::onClientLeaveGame(%this, %client)
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

function masksScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();
}

function masksScriptMsgListener::onRemove(%this)
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

function masksScriptMsgListener::SpawnNPC(%this, %client)
{
  return -1;
}

function masksScriptMsgListener::onNPCLoadRequest(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %npcName = %data.getValue(%data.getIndexFromKey("npc"));
  %player = %client.getControlObject();

  if (%npcName !$= %this.npc_)
  {
    return;
  }

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);

  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %npc = %this.npcArray_.getValue(%index);

    if (isObject(%npc))
    {
      return;
    }

    %npc = %this.SpawnNPC(%client);

    %this.npcArray_.setValue(%npc, %index);

    return;
  }

  //add index.
  %npc = %this.SpawnNPC(%client);

  %this.npcArray_.add(%client, %npc);

}

function masksScriptMsgListener::CommandNPC(%this, %action, %mask, %player)
{
  if (%action $= "wear")
  {
    %dirList = getDirectoryList("art/shapes/dotsnetcrits/masks/", 1);

    %mask = strlwr(%mask);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      if (getField(%dirList, %x) $= %mask)
      {
        %index = %this.npcArray_.getIndexFromKey(%player.client);

        %npc = %this.npcArray_.getValue(%index);

        if (isObject(%npc))
        {
          if (%npc.mask_ $= %mask)//toggle off
          {
            %npc.delete();
            %this.npcArray_.setValue(-1, %index);
            return;
          }
          %npc.delete();
        }

        %npc = new TSStatic()
        {
          shapeName = "art/shapes/dotsnetcrits/masks/" @ %mask @ "/" @ %mask @ ".dae";
          mask_ = %mask;
        };

        %player.mountObject(%npc, 2);

        %this.npcArray_.setValue(%npc, %index);

        break;
      }
    }
  }

}

function masksScriptMsgListener::onNPCActionmasks(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %action = %data.getValue(%data.getIndexFromKey("action"));
  %mask = %data.getValue(%data.getIndexFromKey("mask"));
  %player = %client.getControlObject();

  %this.CommandNPC(%action, %mask, %player);
}

function serverCmdNPCActionmasks(%client, %action, %mask)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("action", %action);
  %data.add("mask", %mask);
  DNCServer.EventManager_.postEvent("NPCActionmasks", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "masksScriptMsgListener";
  npc_ = "masks";
  npcArray_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActionmasks");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActionmasks");
DNCServer.ClientLeaveListeners_.add(%NPC);
