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

function altTrigger(%val)
{
   $mvTriggerCount1++;
}

function DotsNetCritsClient::ReloadBinds(%this)
{
  moveMap.push();
  %this.actionMap_.push();
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

  //Quickly thrown-in flashlight.  Credits to the resource.
  %this.flashlightState_ = false;

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();

  moveMap.unbind(keyboard, "escape");
  %this.actionMap_.bindCmd(keyboard, "escape", "", "escapeFromGameDNC();");
  %this.actionMap_.bindCmd(keyboard, "g", %this @ ".FlashlightToggle();");
  moveMap.unbind(keyboard, "e");
  %this.actionMap_.bindCmd(keyboard, "e", "", %this @ ".UseObject();");

  %this.actionMap_.bind( mouse, button2, altTrigger );

  moveMap.unbind(keyboard, "1");
  %this.actionMap_.bindCmd(keyboard, "1", "commandToServer('use',\"crayon\");", "");
  moveMap.unbind(keyboard, "2");
  %this.actionMap_.bindCmd(keyboard, "2", "commandToServer('use',\"scissors\");", "");
  moveMap.unbind(keyboard, "3");
  %this.actionMap_.bindCmd(keyboard, "3", "commandToServer('use',\"pogoblade\");", "");
  moveMap.unbind(keyboard, "4");
  %this.actionMap_.bindCmd(keyboard, "4", "commandToServer('use',\"dollarstack\");", "");
  moveMap.unbind(keyboard, "5");
  %this.actionMap_.bindCmd(keyboard, "5", "commandToServer('use',\"glassbottle\");", "");
  moveMap.unbind(keyboard, "6");
  %this.actionMap_.bindCmd(keyboard, "6", "commandToServer('use',\"makeupkit\");", "");
  moveMap.unbind(keyboard, "7");
  %this.actionMap_.bindCmd(keyboard, "7", "commandToServer('use',\"mushtrap\");", "");
  moveMap.unbind(keyboard, "8");
  %this.actionMap_.bindCmd(keyboard, "8", "commandToServer('use',\"healerrifle\");", "");
  moveMap.unbind(keyboard, "9");
  %this.actionMap_.bindCmd(keyboard, "9", "commandToServer('use',\"troutscepter\");", "");
  moveMap.unbind(keyboard, "0");
  %this.actionMap_.bindCmd(keyboard, "0", "commandToServer('use',\"phoneshield\");", "");

  %this.actionMap_.push();

  vehicleMap.bindCmd(keyboard, "escape", "", "escapeFromGameDNC();");

  if (isObject(%this.ladderActionMap_))
  {
    %this.ladderActionMap_.delete();
  }
  %this.ladderActionMap_ = new ActionMap();
  %this.ladderActionMap_.bindCmd(keyboard, "w", %this @ ".LadderMoveUp(1);", %this @ ".LadderMoveUp(0);");
  %this.ladderActionMap_.bindCmd(keyboard, "s", %this @ ".LadderMoveDown(1);", %this @ ".LadderMoveDown(0);");

  %this.execDirScripts("datablocks", "");
  %this.execDirScripts("npcs", "");
  %this.execDirScripts("weapons", "");
  %this.execDirScripts("utilities", "");

  if (isObject(playGui))
  {
    playGui.delete();
  }

  exec("art/gui/dotsnetcrits/playGui.gui");

  if (!isObject(DNCCrosshair))
  {
    exec("art/gui/dotsnetcrits/crosshair.gui");
  }

  PlayGui.add(DNCCrosshair);

  /*for (%x = 0; %x < playGui.getCount(); %x++)
  {
    %gui = playGui.getObject(%x);

    if (%gui.getClassName() $= "GuiShapeNameHud" || %gui.getClassName() $= "GuiCrossHairHud")
    {
      %gui.delete();//there's a client cmd that enables the reticle, so get rid of it altogether.
      //%gui.setVisible(false);
      //%gui.setActive(false);
    }
  }*/
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

  if (isObject(%this.ladderActionMap_))
  {
    %this.ladderActionMap_.pop();
    %this.ladderActionMap_.delete();
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
      ObjectPreviewDNC.setBitmap("art/gui/dotsnetcrits/background.png");
      ObjectDescriptionDNC.setText("");
      ObjectScrlDNC.visible = false;
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

function DotsNetCritsClient::UseObject(%this)
{
  commandToServer('UseObject');
}

function DotsNetCritsClient::FlashlightToggle(%this)
{
  %this.flashlightState_ = !%this.flashlightState_;
  commandToServer('FlashlightToggle', %this.flashlightState_);
}

new ScriptObject(DNCClient)
{
  class = "DotsNetCritsClient";
  EventManager_ = "";
  actionMap_ = "";
  ladderActionMap_ = "";
  loadedNPCs_ = "";
};

function escapeFromGameDNC()
{
   if (isObject(DNCClient))
   {
     DNCClient.EventManager_.postEvent("MainMenuToggle");
   }
}
