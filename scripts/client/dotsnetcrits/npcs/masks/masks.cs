/*function masksScriptMsgListenerClient::NPCAction(%this, %key)
{
  //commandToServer('NPCActionmasks', %key);
}*/

function masksScriptMsgListenerClient::PopulateMaskList(%this)
{
  if (isObject(masksList))
  {
    masksList.clear();
    masksBitmap.setBitmap("");

    %dirList = getDirectoryList("art/shapes/dotsnetcrits/masks/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      masksList.addRow(%x, getField(%dirList, %x));
    }

    masksScroll.computeSizes();
  }

}

function masksWindow::onClose(%this)
{
  Canvas.popDialog(masksdnc);
}

function masksButt::onClick(%this)
{
  %action = "wear";
  %mask = masksList.getRowText(masksList.getSelectedRow());

  if (%mask $= "")
  {
    return;
  }

  commandToServer('NPCActionmasks', %action, %mask);
}

function masksList::onSelect(%this, %cellID, %text)
{
  masksBitmap.setBitmap("art/shapes/dotsnetcrits/masks/" @ %text @ "/" @ %text @ "_preview.png");
}

function masksScriptMsgListenerClient::onAdd(%this)
{
  if (!isObject(masksdnc))
  {
    exec("art/gui/dotsnetcrits/masks.gui");
  }
}

function masksScriptMsgListenerClient::onRemove(%this)
{
  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.pop();
    %this.actionMap_.delete();
  }
}

function masksScriptMsgListenerClient::ToggleMenu(%this)
{
  if (isObject(masksdnc))
  {
    if (!Canvas.isMember(masksdnc))
    {
      Canvas.pushDialog(masksdnc);

      %data = new ArrayObject();
      %data.add("action", "populatelists");
      %data.add("mask", -1);
      DNCClient.EventManager_.postEvent("NPCActionmasks", %data);

      %data.delete();
    }
    else
    {
      Canvas.popDialog(masksdnc);
    }
  }
}

function masksScriptMsgListenerClient::onNPCLoadRequest(%this, %data)
{
  %npcName = %data.getValue(%data.getIndexFromKey("npc"));
  %state = %data.getValue(%data.getIndexFromKey("state"));

  if (%npcName !$= %this.npc_)
  {
    return;
  }

  if (isObject(masksdnc))
  {
    if (!Canvas.isMember(masksdnc))
    {
      Canvas.pushDialog(masksdnc);

      %data = new ArrayObject();
      %data.add("action", "populatelists");
      %data.add("mask", -1);
      DNCClient.EventManager_.postEvent("NPCActionmasks", %data);

      %data.delete();
    }
    else
    {
      Canvas.popDialog(masksdnc);
    }
  }

  if (%state)
  {

    if (isObject(%this.actionMap_))
    {
      %this.actionMap_.delete();
    }

    %this.actionMap_ = new ActionMap();
    %this.actionMap_.bindCmd(keyboard, "x", "", %this @ ".ToggleMenu();");
    /*%this.actionMap_.bindCmd(keyboard, "numpad0", "", %this @ ".NPCAction(" @ "numpad0" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad1", "", %this @ ".NPCAction(" @ "numpad1" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad2", "", %this @ ".NPCAction(" @ "numpad2" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad3", "", %this @ ".NPCAction(" @ "numpad3" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad4", "", %this @ ".NPCAction(" @ "numpad4" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad5", "", %this @ ".NPCAction(" @ "numpad5" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad6", "", %this @ ".NPCAction(" @ "numpad6" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad7", "", %this @ ".NPCAction(" @ "numpad7" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad8", "", %this @ ".NPCAction(" @ "numpad8" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad9", "", %this @ ".NPCAction(" @ "numpad9" @ ");");*/

    %this.actionMap_.push();
  }
  else
  {

    if (isObject(%this.actionMap_))
    {
      %this.actionMap_.pop();
      %this.actionMap_.delete();
    }

  }
}

function clientCmdNPCActionmasks(%action, %mask)
{
  if (isObject(DNCClient))
  {
    %data = new ArrayObject();
    %data.add("action", %action);
    %data.add("mask", %mask);
    DNCClient.EventManager_.postEvent("NPCActionmasks", %data);

    %data.delete();
  }
}

function masksScriptMsgListenerClient::onNPCActionmasks(%this, %data)
{
  %action = %data.getValue(%data.getIndexFromKey("action"));
  %mask = %data.getValue(%data.getIndexFromKey("mask"));

  if (%action $= "populatelists")
  {
    %this.PopulateMaskList();
  }
}

%NPC = new ScriptMsgListener()
{
  class = "masksScriptMsgListenerClient";
  npc_ = "masks";
  actionMap_ = "";
};

DNCClient.loadedNPCs_.add(%NPC);
DNCClient.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCClient.EventManager_.registerEvent("NPCActionmasks");
DNCClient.EventManager_.subscribe(%NPC, "NPCActionmasks");
