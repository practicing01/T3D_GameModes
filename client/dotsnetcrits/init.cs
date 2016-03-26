function DotsNetCritsClient::onAdd(%this)
{
  exec("scripts/client/dotsnetcrits/rpc/clientCmdLoadGamemodeDNC.cs");

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "DotsNetCritsQueue";

  %this.EventManager_.registerEvent("MainMenuToggle");

  %this.EventManager_.subscribe(%this, "MainMenuToggle");

  %this.EventManager_.registerEvent("LoadGamemode");

  %this.EventManager_.subscribe(%this, "LoadGamemode");

  exec("art/gui/DNCMain.gui");
  exec("scripts/client/dotsnetcrits/DNCButt.cs");
  exec("scripts/client/dotsnetcrits/GamemodeTab.cs");
  exec("scripts/client/dotsnetcrits/TeamTab.cs");
  exec("scripts/client/dotsnetcrits/WeaponTab.cs");

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "escape", "", "escapeFromGameDNC();");

  %this.actionMap_.push();
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
};

function escapeFromGameDNC()
{
   if (isObject(DNCClient))
   {
     DNCClient.EventManager_.postEvent("MainMenuToggle");
   }
}
