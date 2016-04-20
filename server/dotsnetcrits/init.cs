if (isObject(DNCServer))
{
  %dirList = getDirectoryList("scripts/server/dotsnetcrits/datablocks/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    %file = getField(%dirList, %x);
    %file = strlwr(%file);
    exec("scripts/server/dotsnetcrits/datablocks/" @ %file @ "/" @ %file @ ".cs");
  }
  return;
}

function DotsNetCritsServer::onAdd(%this)
{
  %this.ClientLeaveCleanup_ = new ArrayObject();
  %this.ClientLeaveListeners_ = new SimSet();
  %this.loadOutListeners_ = new SimSet();
  %this.loadedGamemodes_ = new SimSet();

  %dirList = getDirectoryList("scripts/server/dotsnetcrits/datablocks/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    %file = getField(%dirList, %x);
    %file = strlwr(%file);
    exec("scripts/server/dotsnetcrits/datablocks/" @ %file @ "/" @ %file @ ".cs");
  }

  exec("scripts/server/dotsnetcrits/rpc/serverCmdGamemodeVoteDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdJoinTeamDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdWeaponLoadDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdLevelVoteDNC.cs");
  exec("scripts/server/dotsnetcrits/GamemodeVoteMachine.cs");
  exec("scripts/server/dotsnetcrits/TeamChooser.cs");
  exec("scripts/server/dotsnetcrits/LevelVoteMachine.cs");

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "DotsNetCritsClientQueue";

  %this.EventManager_.registerEvent("GamemodeVoteCast");

  %this.EventManager_.registerEvent("GamemodeVoteTallied");

  %this.EventManager_.registerEvent("TeamJoinRequest");

  %this.EventManager_.registerEvent("WeaponLoadRequest");

  %this.EventManager_.registerEvent("LevelVoteCast");

  %this.EventManager_.registerEvent("LevelVoteTallied");

  %this.EventManager_.subscribe(%this, "GamemodeVoteTallied");

  %this.EventManager_.subscribe(%this, "WeaponLoadRequest");

  %this.EventManager_.subscribe(%this, "LevelVoteTallied");

  %this.GameModeVoteMachine_ = new ScriptObject()
  {
    class = "GameModeVoteMachine";
  };

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

}

function DotsNetCritsServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
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

  if (isObject(%this.ClientLeaveListeners_))
  {
    %this.ClientLeaveListeners_.delete();
  }

  if (isObject(%this.loadOutListeners_))
  {
    %this.loadOutListeners_.delete();
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
      %player.setInventory(%weapon, 1);
      %player.addToWeaponCycle(%weapon);

      //%player.mountImage(%weapon, 0);
      %player.use(%weapon);

      break;
    }
  }
}

function GetMountIndexDNC(%obj, %slot)
{
  %modelFile = %obj.getModelFile();

  %shapeConstructor = "";

  for (%x = 0; %x <  TSShapeConstructorGroup.getCount(); %x++)
  {
    %TSShapeConstructor = TSShapeConstructorGroup.getObject(%x);

    if (%TSShapeConstructor.baseShape $= %modelFile)
    {
      %shapeConstructor = %TSShapeConstructor;
      break;
    }
  }

  %index = -1;

  for (%x = 0; %x <  %shapeConstructor.	getNodeCount(); %x++)
  {
    if (strstr(%shapeConstructor.getNodeName(%x), "mount") != -1)
    {
      %index++;
    }
  }

  return %index - %slot;
}

function DeathMatchGame::onClientLeaveGame(%game, %client)
{
  if (!isObject(DNCServer))
  {
    return;
  }

  %index = DNCServer.ClientLeaveCleanup_.getIndexFromKey(%client);
  %simset = DNCServer.ClientLeaveCleanup_.getValue(%index);

  %simset.deleteAllObjects();
  %simset.delete();

  DNCServer.ClientLeaveCleanup_.erase(%index);

  for (%x = 0; %x < DNCServer.ClientLeaveListeners_.count(); %x++)
  {
    DNCServer.ClientLeaveListeners_.getValue(%x).onClientLeaveGame(%client);
  }

  parent::onClientLeaveGame(%game, %client);
}

function DeathMatchGame::loadOut(%game, %player)
{
  parent::loadOut(%game, %player);

  for (%x = 0; %x < DNCServer.loadOutListeners_.getCount(); %x++)
  {
    DNCServer.loadOutListeners_.getObject(%x).loadOut(%player);
    /*%obj = DNCServer.loadOutListeners_.getObject(%x);
    %obj.call("loadOut", %player);*/
  }
}

function WeaponLoader::loadOut(%this, %player)
{
  %player.setInventory(%this.weapon_, 1);
  %player.addToWeaponCycle(%this.weapon_);
}

function DeathMatchGame::onClientEnterGame(%game, %client)
{
   parent::onClientEnterGame(%game, %client);

  if (isObject(DNCServer))
  {
    for (%y = 0; %y < DNCServer.loadedGamemodes_.getCount(); %y++)
    {
      commandToClient(%client, 'LoadGamemodeDNC', DNCServer.loadedGamemodes_.getObject(%y).name_);
    }
  }
}

new ScriptObject(DNCServer)
{
  class = "DotsNetCritsServer";
  EventManager_ = "";
  GameModeVoteMachine_ = "";
  TeamChooser_ = "";
  ClientLeaveCleanup_ = "";
  ClientLeaveListeners_ = "";
  loadOutListeners_ = "";
  loadedGamemodes_ = "";
  loadedWeapons_ = "";
  LevelVoteMachine_ = "";
};
