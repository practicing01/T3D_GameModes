function LechosaClass::onNode(%this, %node)
{
  if (%node >= %this.nodeCount_)
  {
    %this.setPosition(0);
    %this.setTarget(1);
    %this.setState();
  }
  else
  {
    %this.setState();
  }
}

function lechosaScriptMsgListener::onClientLeaveGame(%this, %client)
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

function lechosaScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();
}

function lechosaScriptMsgListener::onRemove(%this)
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

function lechosaScriptMsgListener::SpawnNPC(%this, %client)
{
  %spawnpoint = PlayerDropPoints.getObject(0);
  %origin = %spawnpoint.getTransform();

  %npc = new PathCamera()
  {
    dataBlock = "LechosaCamDB";
    class = "LechosaClass";
    position = %spawnpoint.position;
    nodeCount_ = 0;
  };

  %npc.reset(20);

  for (%x = 1; %x < PlayerDropPoints.getCount(); %x++)
  {
    %spawnpoint = PlayerDropPoints.getObject(%x);
    %transform = %spawnpoint.getTransform();

    %npc.pushBack(%transform, 20);
    %npc.nodeCount_++;
  }

  %npc.pushBack(%origin, 20);
  %npc.nodeCount_++;

  %npc.setState();

  return %npc;
}

function lechosaScriptMsgListener::onNPCLoadRequest(%this, %data)
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
    %lechosa = %this.SpawnNPC(%client);

    %this.npcArray_.setValue(%lechosa, %index);

    commandToClient(%client, 'NPCLoadDNC', %npcName, true);

    return;
  }

  //add index.
  %lechosa = %this.SpawnNPC(%client);

  %this.npcArray_.add(%client, %lechosa);

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);
}

function lechosaScriptMsgListener::CommandNPC(%this, %key, %npc, %player)
{
  //
}

function lechosaScriptMsgListener::onNPCActionlechosa(%this, %data)
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

function serverCmdNPCActionlechosa(%client, %key)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("key", %key);
  DNCServer.EventManager_.postEvent("NPCActionlechosa", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "lechosaScriptMsgListener";
  npc_ = "lechosa";
  npcArray_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActionlechosa");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActionlechosa");
DNCServer.ClientLeaveListeners_.add(%NPC);
