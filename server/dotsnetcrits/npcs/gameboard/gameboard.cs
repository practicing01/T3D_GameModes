function gameboardScriptMsgListener::onClientLeaveGame(%this, %client)
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

function gameboardScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();

  %dirList = getDirectoryList("art/shapes/dotsnetcrits/tcg/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    %card = getField(%dirList, %x);
    exec("art/shapes/dotsnetcrits/tcg/" @ %card @ "/materials.cs");
  }
}

function gameboardScriptMsgListener::onRemove(%this)
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

function gameboardScriptMsgListener::SpawnNPC(%this, %client)
{
  %fieldList = new ArrayObject();
  return %fieldList;
}

function gameboardScriptMsgListener::onNPCLoadRequest(%this, %data)
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
    %fieldList = %this.npcArray_.getValue(%index);

    if (isObject(%fieldList))
    {
      return;
    }

    //create fieldList.
    %fieldList = %this.SpawnNPC(%client);

    %this.npcArray_.setValue(%fieldList, %index);

    return;
  }

  //add index.
  %fieldList = %this.SpawnNPC(%client);

  %this.npcArray_.add(%client, %fieldList);

}

function gameboardScriptMsgListener::CommandNPC(%this, %action, %card, %player)
{
  if (%action $= "spawn")
  {
    //check if card exists
    %dirList = getDirectoryList("art/shapes/dotsnetcrits/tcg/", 1);

    %card = strlwr(%card);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      if (getField(%dirList, %x) $= %card)//Make sure the card exists.
      {
        %cardModel = new TSStatic()
        {
          shapeName = "art/shapes/dotsnetcrits/tcg/card.cached.dts";
          position = %player.position;
          skin = %card;
        };

        %index = %this.npcArray_.getIndexFromKey(%player.client);

        if (%index != -1)
        {
          %fieldList = %this.npcArray_.getValue(%index);

          %fieldCount = %fieldList.count();

          %fieldList.add(%fieldList.count(), %cardModel);

          commandToClient(%player.client, 'NPCActiongameboard', "spawn", %fieldCount, %card);
        }

        break;
      }
    }
  }

}

function gameboardScriptMsgListener::onNPCActiongameboard(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %action = %data.getValue(%data.getIndexFromKey("action"));
  %card = %data.getValue(%data.getIndexFromKey("card"));
  %player = %client.getControlObject();

  %this.CommandNPC(%action, %card, %player);
}

function serverCmdNPCActiongameboard(%client, %action, %card)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("action", %action);
  %data.add("card", %card);
  DNCServer.EventManager_.postEvent("NPCActiongameboard", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "gameboardScriptMsgListener";
  npc_ = "gameboard";
  npcArray_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActiongameboard");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActiongameboard");
DNCServer.ClientLeaveListeners_.add(%NPC);
