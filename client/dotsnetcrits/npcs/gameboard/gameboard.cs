/*function gameboardScriptMsgListenerClient::NPCAction(%this, %key)
{
  //commandToServer('NPCActiongameboard', %key);
}*/

function gameboardScriptMsgListenerClient::PopulateDeckList(%this)
{
  if (isObject(tcgdeck))
  {
    tcgdeck.clear();
    tcgdeckimage.setBitmap("");

    if (tcgdeck.getSelectedRow())
    {
      %text = tcgdeck.getRowText(tcgdeck.getSelectedRow());
      tcgdeckimage.setBitmap("art/shapes/dotsnetcrits/tcg/" @ %text @ "/" @ %text @ ".png");
    }

    //todo store deck list once
    %dirList = getDirectoryList("art/shapes/dotsnetcrits/tcg/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      tcgdeck.addRow(%x, getField(%dirList, %x));
    }
  }

}

function gameboardScriptMsgListenerClient::PopulateFieldList(%this)
{
  if (isObject(tcgfield))
  {
    tcgfield.clear();
    tcgfieldimage.setBitmap("");

    if (tcgfield.getSelectedRow())
    {
      %text = tcgfield.getRowText(tcgfield.getSelectedRow());
      tcgfieldimage.setBitmap("art/shapes/dotsnetcrits/tcg/" @ %text @ "/" @ %text @ ".png");
    }

    for (%x = 0; %x < %this.fieldList_.count(); %x++)
    {
      tcgfield.addRow(%x, %this.fieldList_.getValue(%x));
    }

    tcgfieldScroller.computeSizes();
  }

}

function tcgcontroller::onClose(%this)
{
  Canvas.popDialog(tcggameboard);
}

function tcgspawn::onClick(%this)
{
  %action = "spawn";
  %card = tcgdeck.getRowText(tcgdeck.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, -1);
}

function tcgremove::onClick(%this)
{
  %action = "remove";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgmove::onClick(%this)
{
  %action = "move";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgzcw::onClick(%this)
{
  %action = "zcw";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgzccw::onClick(%this)
{
  %action = "zccw";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgxcw::onClick(%this)
{
  %action = "xcw";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgxccw::onClick(%this)
{
  %action = "xccw";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgycw::onClick(%this)
{
  %action = "ycw";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgyccw::onClick(%this)
{
  %action = "yccw";
  %card = tcgfield.getRowText(tcgfield.getSelectedRow());

  if (%card $= "")
  {
    return;
  }

  commandToServer('NPCActiongameboard', %action, %card, tcgfield.getSelectedRow());
}

function tcgdeck::onSelect(%this, %cellID, %text)
{
  tcgdeckimage.setBitmap("art/shapes/dotsnetcrits/tcg/" @ %text @ "/" @ %text @ "_preview.png");
}

function tcgfield::onSelect(%this, %cellID, %text)
{
  tcgfieldimage.setBitmap("art/shapes/dotsnetcrits/tcg/" @ %text @ "/" @ %text @ "_preview.png");
}

function gameboardScriptMsgListenerClient::onAdd(%this)
{
  if (!isObject(tcggameboard))
  {
    exec("art/gui/dotsnetcrits/tcgcontroller.gui");
  }

  if (!isObject(%this.fieldList_))
  {
    %this.fieldList_ = new ArrayObject();
  }
  else
  {
    %this.fieldList_.empty();
  }
}

function gameboardScriptMsgListenerClient::onRemove(%this)
{
  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  if (isObject(%this.fieldList_))
  {
    %this.fieldList_.delete();
  }
}

function gameboardScriptMsgListenerClient::ToggleMenu(%this)
{
  if (isObject(tcggameboard))
  {
    if (!Canvas.isMember(tcggameboard))
    {
      Canvas.pushDialog(tcggameboard);

      %data = new ArrayObject();
      %data.add("action", "populatelists");
      %data.add("fieldCount", -1);
      %data.add("card", -1);
      %data.add("index", -1);
      DNCClient.EventManager_.postEvent("NPCActiongameboard", %data);

      %data.delete();
    }
    else
    {
      Canvas.popDialog(tcggameboard);
    }
  }
}

function gameboardScriptMsgListenerClient::onNPCLoadRequest(%this, %data)
{
  %npcName = %data.getValue(%data.getIndexFromKey("npc"));
  %state = %data.getValue(%data.getIndexFromKey("state"));

  if (%npcName !$= %this.npc_)
  {
    return;
  }

  if (isObject(tcggameboard))
  {
    if (!Canvas.isMember(tcggameboard))
    {
      Canvas.pushDialog(tcggameboard);

      %data = new ArrayObject();
      %data.add("action", "populatelists");
      %data.add("fieldCount", -1);
      %data.add("card", -1);
      %data.add("index", -1);
      DNCClient.EventManager_.postEvent("NPCActiongameboard", %data);

      %data.delete();
    }
    else
    {
      Canvas.popDialog(tcggameboard);
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
      %this.actionMap_.delete();
    }

  }
}

function clientCmdNPCActiongameboard(%action, %fieldCount, %card, %index)
{
  if (isObject(DNCClient))
  {
    %data = new ArrayObject();
    %data.add("action", %action);
    %data.add("fieldCount", %fieldCount);
    %data.add("card", %card);
    %data.add("index", %index);
    DNCClient.EventManager_.postEvent("NPCActiongameboard", %data);

    %data.delete();
  }
}

function gameboardScriptMsgListenerClient::onNPCActiongameboard(%this, %data)
{
  %action = %data.getValue(%data.getIndexFromKey("action"));
  %fieldCount = %data.getValue(%data.getIndexFromKey("fieldCount"));
  %card = %data.getValue(%data.getIndexFromKey("card"));
  %index = %data.getValue(%data.getIndexFromKey("index"));

  if (%action $= "populatelists")
  {
    %this.PopulateDeckList();
    %this.PopulateFieldList();
  }
  else if (%action $= "spawn")
  {
    %this.fieldList_.insert(%fieldCount, %card, %fieldCount);
    %this.PopulateDeckList();
    %this.PopulateFieldList();
  }
  else if (%action $= "remove")
  {
    %this.fieldList_.erase(%index);
    %this.PopulateDeckList();
    %this.PopulateFieldList();
  }
}

%NPC = new ScriptMsgListener()
{
  class = "gameboardScriptMsgListenerClient";
  npc_ = "gameboard";
  actionMap_ = "";
  fieldList_ = "";
};

DNCClient.loadedNPCs_.add(%NPC);
DNCClient.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCClient.EventManager_.registerEvent("NPCActiongameboard");
DNCClient.EventManager_.subscribe(%NPC, "NPCActiongameboard");
