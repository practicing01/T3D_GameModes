function DotsNetCritsServer::execDirScripts(%this, %dir, %exclusions)
{
  %dirList = getDirectoryList("scripts/server/dotsnetcrits/" @ %dir @ "/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    %file = getField(%dirList, %x);
    %file = strlwr(%file);

    if (strIsMatchMultipleExpr(%exclusions, %file))
    {
      continue;
    }

    exec("scripts/server/dotsnetcrits/" @ %dir @ "/" @ %file @ "/" @ %file @ ".cs");
  }
}

if (isObject(DNCServer))
{
  DNCServer.SoftOnRemove();

  exec("scripts/server/dotsnetcrits/gameDM.cs");
  exec("scripts/server/dotsnetcrits/player.cs");
  DNCServer.execDirScripts("datablocks", "players misc");
  exec("scripts/server/dotsnetcrits/datablocks/players/players.cs");
  exec("scripts/server/dotsnetcrits/datablocks/misc/misc.cs");
  DNCServer.execDirScripts("npcs", "");
  DNCServer.execDirScripts("utilities", "");
  return;
}

if (!isObject(GuiModelessControlProfile))
{
  new GuiControlProfile( GuiModelessControlProfile : GuiDefaultProfile )
  {
    modal = false;
  };
}

function DotsNetCritsServer::onAdd(%this)
{
  $Pref::Server::AdminPassword = "";
  $Pref::Server::BanTime = 1800;
  $Pref::Server::ConnectionError = "You do not have the correct version of Full or the related art needed to play on this server, please contact the server administrator.";
  $Pref::Server::FloodProtectionEnabled = 1;
  $Pref::Server::Info = "This is a Dots Net Crits server.";
  $Pref::Server::KickBanTime = 300;
  $Pref::Server::MaxChatLen = 120;
  $Pref::Server::MaxPlayers = 16;
  $Pref::Server::Name = "Dots Net Crits Server";
  $Pref::Server::Password = "";
  $Pref::Server::Port = 28000;
  $Pref::Server::RegionMask = 2;
  $Pref::Server::TimeLimit = 0;
  $Game::Duration = 0;
  $Game::EndGameScore = 0;
  $Game::Cycling = false;
  $pref::Net::DisplayOnMaster = "Never";//"Never";Always
  //$pref::Master[0] = "2:185.185.42.17:28028";
  //schedule(0,0,startHeartbeat);
  stopHeartbeat();

  %this.ClientLeaveCleanup_ = new ArrayObject();
  %this.ClientLeaveListeners_ = new SimSet();
  %this.loadOutListeners_ = new SimSet();
  %this.incKillsListeners_ = new SimSet();
  %this.missionLoadedListeners_ = new SimSet();
  %this.deathListeners_ = new SimSet();
  %this.loadedGamemodes_ = new SimSet();
  %this.loadedWeapons_ = new SimSet();
  %this.loadedNPCs_ = new SimSet();
  %this.clientModels_ = new ArrayObject();

  exec("scripts/server/dotsnetcrits/rpc/serverCmdGamemodeVoteDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdJoinTeamDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdWeaponLoadDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdLevelVoteDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdNPCLoadDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdModelLoadDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdFlashlightToggle.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdUseObject.cs");
  exec("scripts/server/dotsnetcrits/GamemodeVoteMachine.cs");
  exec("scripts/server/dotsnetcrits/TeamChooser.cs");
  exec("scripts/server/dotsnetcrits/LevelVoteMachine.cs");
  exec("scripts/server/dotsnetcrits/gameDM.cs");
  exec("scripts/server/dotsnetcrits/player.cs");

  %this.execDirScripts("datablocks", "players misc");
  exec("scripts/server/dotsnetcrits/datablocks/players/players.cs");
  exec("scripts/server/dotsnetcrits/datablocks/misc/misc.cs");
  %this.execDirScripts("utilities", "");

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "DotsNetCritsClientQueue";

  %this.EventManager_.registerEvent("GamemodeVoteCast");

  %this.EventManager_.registerEvent("GamemodeVoteTallied");

  %this.EventManager_.registerEvent("TeamJoinRequest");

  %this.EventManager_.registerEvent("WeaponLoadRequest");

  %this.EventManager_.registerEvent("LevelVoteCast");

  %this.EventManager_.registerEvent("LevelVoteTallied");

  %this.EventManager_.registerEvent("NPCLoadRequest");

  %this.EventManager_.registerEvent("ModelLoadRequest");

  %this.EventManager_.subscribe(%this, "GamemodeVoteTallied");

  %this.EventManager_.subscribe(%this, "WeaponLoadRequest");

  %this.EventManager_.subscribe(%this, "LevelVoteTallied");

  %this.EventManager_.subscribe(%this, "ModelLoadRequest");

  %this.GameModeVoteMachine_ = new ScriptObject()
  {
    class = "GameModeVoteMachine";
  };

  %this.ladderManager_ = new ScriptObject()
  {
    class = "LadderManager";
  };

  %this.ClientLeaveListeners_.add(%this.ladderManager_);

  %this.EventManager_.subscribe(%this.GameModeVoteMachine_, "GamemodeVoteCast");

  %this.LevelVoteMachine_ = new ScriptObject()
  {
    class = "LevelVoteMachine";
  };

  %this.EventManager_.subscribe(%this.LevelVoteMachine_, "LevelVoteCast");

  %this.TeamChooser_ = new ScriptObject()
  {
    class = "TeamChooser";
  };

  %this.EventManager_.subscribe(%this.TeamChooser_, "TeamJoinRequest");
  %this.loadOutListeners_.add(%this.TeamChooser_);

  %this.execDirScripts("npcs", "");

  if (!isObject(DNCCrosshair))
  {
    exec("art/gui/dotsnetcrits/crosshair.gui");
  }

  PlayGui.add(DNCCrosshair);
}

function DotsNetCritsServer::SoftOnRemove(%this)
{
  if (isObject(%this.ClientLeaveCleanup_))
  {
    for (%x = 0; %x < DNCServer.ClientLeaveCleanup_.count(); %x++)
    {
      DNCServer.ClientLeaveCleanup_.getValue(%x).delete();
    }
  }

  if (isObject(%this.clientModels_))
  {
    %this.clientModels_.empty();
  }

  if (isObject(%this.ClientLeaveListeners_))
  {
    %this.ClientLeaveListeners_.deleteAllObjects();
  }

  if (isObject(%this.loadOutListeners_))
  {
    %this.loadOutListeners_.deleteAllObjects();
  }

  if (isObject(%this.incKillsListeners_))
  {
    %this.incKillsListeners_.deleteAllObjects();
  }

  if (isObject(%this.missionLoadedListeners_))
  {
    %this.missionLoadedListeners_.deleteAllObjects();
  }

  if (isObject(%this.deathListeners_))
  {
    %this.deathListeners_.deleteAllObjects();
  }

  if (isObject(%this.loadedGamemodes_))
  {
    %this.loadedGamemodes_.deleteAllObjects();
  }

  if (isObject(%this.loadedWeapons_))
  {
    %this.loadedWeapons_.deleteAllObjects();
  }

  if (isObject(%this.loadedNPCs_))
  {
    %this.loadedNPCs_.deleteAllObjects();
  }

  %this.ladderManager_ = new ScriptObject()
  {
    class = "LadderManager";
  };

  %this.ClientLeaveListeners_.add(%this.ladderManager_);

  %this.TeamChooser_ = new ScriptObject()
  {
    class = "TeamChooser";
  };

  %this.EventManager_.subscribe(%this.TeamChooser_, "TeamJoinRequest");
  %this.loadOutListeners_.add(%this.TeamChooser_);
}

function DotsNetCritsServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.ladderManager_))
  {
    %this.ladderManager_.delete();
  }

  if (isObject(%this.GameModeVoteMachine_))
  {
    %this.GameModeVoteMachine_.delete();
  }

  if (isObject(%this.LevelVoteMachine_))
  {
    %this.LevelVoteMachine_.delete();
  }

  if (isObject(%this.TeamChooser_))
  {
    %this.TeamChooser_.delete();
  }

  if (isObject(%this.ClientLeaveCleanup_))
  {
    for (%x = 0; %x < DNCServer.ClientLeaveCleanup_.count(); %x++)
    {
      DNCServer.ClientLeaveCleanup_.getValue(%x).delete();
    }

    %this.ClientLeaveCleanup_.delete();
  }

  if (isObject(%this.clientModels_))
  {
    %this.clientModels_.delete();
  }

  if (isObject(%this.ClientLeaveListeners_))
  {
    %this.ClientLeaveListeners_.delete();
  }

  if (isObject(%this.loadOutListeners_))
  {
    %this.loadOutListeners_.delete();
  }

  if (isObject(%this.incKillsListeners_))
  {
    %this.incKillsListeners_.delete();
  }

  if (isObject(%this.missionLoadedListeners_))
  {
    %this.missionLoadedListeners_.delete();
  }

  if (isObject(%this.deathListeners_))
  {
    %this.deathListeners_.delete();
  }

  if (isObject(%this.loadedGamemodes_))
  {
    %this.loadedGamemodes_.deleteAllObjects();
    %this.loadedGamemodes_.delete();
  }

  if (isObject(%this.loadedWeapons_))
  {
    %this.loadedWeapons_.deleteAllObjects();
    %this.loadedWeapons_.delete();
  }

  if (isObject(%this.loadedNPCs_))
  {
    %this.loadedNPCs_.deleteAllObjects();
    %this.loadedNPCs_.delete();
  }

  if (isObject(DNCCrosshair))
  {
    PlayGui.remove(DNCCrosshair);
    DNCCrosshair.delete();
  }

  echo("dnc server go bye bye");
}

function DotsNetCritsServer::onGamemodeVoteTallied(%this, %gamemode)
{
  %dirList = getDirectoryList("scripts/server/dotsnetcrits/gamemodes/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    if (getField(%dirList, %x) $= %gamemode)//Make sure the gamemode exists.
    {
      exec("scripts/server/dotsnetcrits/gamemodes/"@%gamemode@"/"@%gamemode@".cs");

      if (isObject(ClientGroup))
      {
        for (%y = 0; %y < ClientGroup.getCount(); %y++)
        {
          commandToClient(ClientGroup.getObject(%y), 'LoadGamemodeDNC', %gamemode);
        }
      }

      %newGM = true;
      for (%y = 0; %y < %this.loadedGamemodes_.getCount(); %y++)
      {
        if (%this.loadedGamemodes_.getObject(%y).name $= %gamemode)
        {
          %gamemode = %this.loadedGamemodes_.getObject(%y);
          %this.loadedGamemodes_.remove(%gamemode);
          %gamemode.delete();
          %newGM = false;
          break;
        }
      }

      if (%newGM == true)
      {
        %loadedGM = new ScriptObject()
        {
          name_ = %gamemode;
        };

        %this.loadedGamemodes_.add(%loadedGM);
      }

      break;
    }
  }
}

function DotsNetCritsServer::onLevelVoteTallied(%this, %level)
{
  %dirList = getDirectoryList("scripts/server/dotsnetcrits/levels/", 1);

  %level = strlwr(%level);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    if (getField(%dirList, %x) $= %level)//Make sure the level exists.
    {
      schedule(0, 0, loadMission, "levels/"@ %level @ ".mis", false);
      break;
    }
  }
}

function DotsNetCritsServer::onWeaponLoadRequest(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %weapon = %data.getValue(%data.getIndexFromKey("weapon"));

  %dirList = getDirectoryList("scripts/server/dotsnetcrits/weapons/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    if (strlwr(getField(%dirList, %x)) $= strlwr(%weapon))//Make sure the weapon exists.
    {
      //exec("scripts/server/dotsnetcrits/weapons/"@%weapon@"/"@%weapon@".cs");

      %player = %client.getControlObject();

      %db = %player.getDataBlock();
      %db.maxInv[%weapon] = 1;

      %player.setInventory(%weapon, 1);
      %player.addToWeaponCycle(%weapon);

      //%player.mountImage(%weapon, 0);
      %player.use(%weapon);

      break;
    }
  }
}

function DotsNetCritsServer::onModelLoadRequest(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %model = %data.getValue(%data.getIndexFromKey("model"));

  %dirList = getDirectoryList("art/shapes/dotsnetcrits/actors/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    if (strlwr(getField(%dirList, %x)) $= strlwr(%model))//Make sure the model exists.
    {
      %player = %client.getControlObject();

      %index = %this.clientModels_.getIndexFromKey(%client);
      %this.clientModels_.setValue(%model, %index);

      %player.setDataBlock(%model);

      break;
    }
  }
}

function WeaponLoader::loadOut(%this, %player)
{
  %db = %player.getDataBlock();
  %db.maxInv[%this.weapon_] = 1;

  if (%this.weapon_.isMethod("loadOut"))
  {
    %this.weapon_.loadOut(%player);
  }

  %player.setInventory(%this.weapon_, 1);

  if (%this.isField("clip_"))
  {
    %player.setInventory(%this.clip_, %player.maxInventory(%this.clip_));
    %db.maxInv[%this.clip_] = %player.maxInventory(%this.clip_);
  }

  if (%this.isField("ammo_"))
  {
    %player.setInventory(%this.ammo_, %player.maxInventory(%this.ammo_));
    %db.maxInv[%this.ammo_] = %player.maxInventory(%this.ammo_);
  }

  %player.addToWeaponCycle(%this.weapon_);
}

new ScriptObject(DNCServer)
{
  class = "DotsNetCritsServer";
  EventManager_ = "";
  ladderManager_ = "";
  GameModeVoteMachine_ = "";
  TeamChooser_ = "";
  ClientLeaveCleanup_ = "";
  ClientLeaveListeners_ = "";
  loadOutListeners_ = "";
  incKillsListeners_ = "";
  missionLoadedListeners_ = "";
  deathListeners_ = "";
  loadedGamemodes_ = "";
  loadedWeapons_ = "";
  LevelVoteMachine_ = "";
  loadedNPCs_ = "";
  clientModels_ = "";
};
