function catEarsScriptMsgListenerClient::NPCAction(%this, %key)
{
  commandToServer('NPCActioncatEars', %key);
}

function catEarsScriptMsgListenerClient::onAdd(%this)
{
  //
}

function catEarsScriptMsgListenerClient::onRemove(%this)
{
  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.pop();
    %this.actionMap_.delete();
  }
}

function catEarsScriptMsgListenerClient::onNPCLoadRequest(%this, %data)
{
  %npcName = %data.getValue(%data.getIndexFromKey("npc"));
  %state = %data.getValue(%data.getIndexFromKey("state"));

  if (%npcName !$= %this.npc_)
  {
    return;
  }

  if (%state)
  {
    if (isObject(%this.actionMap_))
    {
      %this.actionMap_.delete();
    }

    %this.actionMap_ = new ActionMap();
    %this.actionMap_.bindCmd(keyboard, "numpad0", "", %this @ ".NPCAction(" @ "numpad0" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad1", "", %this @ ".NPCAction(" @ "numpad1" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad2", "", %this @ ".NPCAction(" @ "numpad2" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad3", "", %this @ ".NPCAction(" @ "numpad3" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad4", "", %this @ ".NPCAction(" @ "numpad4" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad5", "", %this @ ".NPCAction(" @ "numpad5" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad6", "", %this @ ".NPCAction(" @ "numpad6" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad7", "", %this @ ".NPCAction(" @ "numpad7" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad8", "", %this @ ".NPCAction(" @ "numpad8" @ ");");
    %this.actionMap_.bindCmd(keyboard, "numpad9", "", %this @ ".NPCAction(" @ "numpad9" @ ");");

    //%this.actionMap_.push();
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

%NPC = new ScriptMsgListener()
{
  class = "catEarsScriptMsgListenerClient";
  npc_ = "catEars";
  actionMap_ = "";
};

DNCClient.loadedNPCs_.add(%NPC);
DNCClient.EventManager_.subscribe(%NPC, "NPCLoadRequest");
