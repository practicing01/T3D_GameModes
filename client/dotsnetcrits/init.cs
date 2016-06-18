function DotsNetCritsClient::execDirScripts(%this, %dir, %exclusions)
{
  %dirList = getDirectoryList("scripts/client/dotsnetcrits/" @ %dir @ "/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    %file = getField(%dirList, %x);
    %file = strlwr(%file);

    if (strIsMatchMultipleExpr(%exclusions, %file))
    {
      continue;
    }

    exec("scripts/client/dotsnetcrits/" @ %dir @ "/" @ %file @ "/" @ %file @ ".cs");
  }
}

function DotsNetCritsClient::onAdd(%this)
{
  %this.loadedNPCs_ = new SimSet();

  exec("scripts/client/dotsnetcrits/rpc/clientCmdLoadGamemodeDNC.cs");
  exec("scripts/client/dotsnetcrits/rpc/clientCmdNPCLoadDNC.cs");

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "DotsNetCritsQueue";

  %this.EventManager_.registerEvent("MainMenuToggle");

  %this.EventManager_.subscribe(%this, "MainMenuToggle");

  %this.EventManager_.registerEvent("LoadGamemode");

  %this.EventManager_.subscribe(%this, "LoadGamemode");

  %this.EventManager_.registerEvent("NPCLoadRequest");

  exec("art/gui/DNCMain.gui");
  exec("scripts/client/dotsnetcrits/DNCButt.cs");
  exec("scripts/client/dotsnetcrits/GamemodeTab.cs");
  exec("scripts/client/dotsnetcrits/TeamTab.cs");
  exec("scripts/client/dotsnetcrits/WeaponTab.cs");
  exec("scripts/client/dotsnetcrits/LevelsTab.cs");
  exec("scripts/client/dotsnetcrits/NPCTab.cs");
  exec("scripts/client/dotsnetcrits/NodesTab.cs");

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "escape", "", "escapeFromGameDNC();");

  %this.actionMap_.push();

  %this.execDirScripts("npcs", "");
}

function DotsNetCritsClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.pop();
    %this.actionMap_.delete();
  }

  if (isObject(%this.loadedNPCs_))
  {
    %this.loadedNPCs_.deleteAllObjects();
    %this.loadedNPCs_.delete();
  }
  echo("dnc client go bye bye");
}

function DotsNetCritsClient::onMainMenuToggle(%this)
{
  if (isObject(DNCMain))
  {
    if (!Canvas.isMember(DNCMain))
    {
      Canvas.pushDialog(DNCMain);
    }
    else
    {
      Canvas.popDialog(DNCMain);
    }
  }
}

function DotsNetCritsClient::onLoadGamemode(%this, %gamemode)
{
  %dirList = getDirectoryList("scripts/client/dotsnetcrits/gamemodes/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    if (getField(%dirList, %x) $= %gamemode)//Make sure the gamemode exists.
    {
      exec("scripts/client/dotsnetcrits/gamemodes/"@%gamemode@"/"@%gamemode@".cs");
      break;
    }
  }
}

new ScriptObject(DNCClient)
{
  class = "DotsNetCritsClient";
  EventManager_ = "";
  actionMap_ = "";
  loadedNPCs_ = "";
};

function escapeFromGameDNC()
{
   if (isObject(DNCClient))
   {
     DNCClient.EventManager_.postEvent("MainMenuToggle");
   }
}
