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

    if (isObject(%npc))
    {
      %npc.delete();
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
}

%NPC = new ScriptMsgListener()
{
  class = "toySoldierScriptMsgListener";
  npc_ = "toysoldier";
  npcArray_ = "";
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.ClientLeaveListeners_.add(%NPC);
