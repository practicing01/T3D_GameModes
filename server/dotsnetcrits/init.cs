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

  DNCServer.execDirScripts("datablocks", "players");

  exec("scripts/server/dotsnetcrits/datablocks/players/players.cs");
  return;
}

function DotsNetCritsServer::onAdd(%this)
{
  %this.ClientLeaveCleanup_ = new ArrayObject();
  %this.ClientLeaveListeners_ = new SimSet();
  %this.loadOutListeners_ = new SimSet();
  %this.loadedGamemodes_ = new SimSet();
  %this.loadedWeapons_ = new SimSet();
  %this.loadedNPCs_ = new SimSet();

  %this.execDirScripts("datablocks", "players");

  exec("scripts/server/dotsnetcrits/datablocks/players/players.cs");

  exec("scripts/server/dotsnetcrits/rpc/serverCmdGamemodeVoteDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdJoinTeamDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdWeaponLoadDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdLevelVoteDNC.cs");
  exec("scripts/server/dotsnetcrits/rpc/serverCmdNPCLoadDNC.cs");
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

  %this.EventManager_.registerEvent("NPCLoadRequest");

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

  %this.execDirScripts("npcs", "");
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

  if (isObject(%this.ClientLeaveListeners_))
  {
    %this.ClientLeaveListeners_.deleteAllObjects();
  }

  if (isObject(%this.loadOutListeners_))
  {
    %this.loadOutListeners_.deleteAllObjects();
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

  if (isObject(%this.loadedNPCs_))
  {
    %this.loadedNPCs_.deleteAllObjects();
    %this.loadedNPCs_.delete();
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

  if (%this.isField("clip_"))
  {
    %player.setInventory(%this.clip_, %player.maxInventory(%this.clip_));
  }

  if (%this.isField("ammo_"))
  {
    %player.setInventory(%this.ammo_, %player.maxInventory(%this.ammo_));
  }

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

function DeathMatchGame::spawnPlayer(%game, %client, %spawnPoint, %noControl)
{
  $Game::DefaultPlayerDataBlock = "QueChan";
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::spawnPlayer");

   if (isObject(%client.player))
   {
      // The client should not already have a player. Assigning
      // a new one could result in an uncontrolled player object.
      error("Attempting to create a player for a client that already has one!");
   }

   // Attempt to treat %spawnPoint as an object
   if (getWordCount(%spawnPoint) == 1 && isObject(%spawnPoint))
   {
      // Defaults
      %spawnClass      = $Game::DefaultPlayerClass;
      %spawnDataBlock  = $Game::DefaultPlayerDataBlock;

      // Overrides by the %spawnPoint
      if (isDefined("%spawnPoint.spawnClass"))
      {
         %spawnClass = %spawnPoint.spawnClass;
         %spawnDataBlock = %spawnPoint.spawnDatablock;
      }
      else if (isDefined("%spawnPoint.spawnDatablock"))
      {
         // This may seem redundant given the above but it allows
         // the SpawnSphere to override the datablock without
         // overriding the default player class
         %spawnDataBlock = %spawnPoint.spawnDatablock;
      }

      %spawnProperties = %spawnPoint.spawnProperties;
      %spawnScript     = %spawnPoint.spawnScript;

      // Spawn with the engine's Sim::spawnObject() function
      %player = spawnObject(%spawnClass, %spawnDatablock, "",
                            %spawnProperties, %spawnScript);

      // If we have an object do some initial setup
      if (isObject(%player))
      {
         // Pick a location within the spawn sphere.
         %spawnLocation = GameCore::pickPointInSpawnSphere(%player, %spawnPoint);
         %player.setTransform(%spawnLocation);

      }
      else
      {
         // If we weren't able to create the player object then warn the user
         // When the player clicks OK in one of these message boxes, we will fall through
         // to the "if (!isObject(%player))" check below.
         if (isDefined("%spawnDatablock"))
         {
               MessageBoxOK("Spawn Player Failed",
                             "Unable to create a player with class " @ %spawnClass @
                             " and datablock " @ %spawnDatablock @ ".\n\nStarting as an Observer instead.",
                             "");
         }
         else
         {
               MessageBoxOK("Spawn Player Failed",
                              "Unable to create a player with class " @ %spawnClass @
                              ".\n\nStarting as an Observer instead.",
                              "");
         }
      }
   }
   else
   {

      // Create a default player
      %player = spawnObject($Game::DefaultPlayerClass, $Game::DefaultPlayerDataBlock);

      if (!%player.isMemberOfClass("Player"))
         warn("Trying to spawn a class that does not derive from Player.");

      // Treat %spawnPoint as a transform
      %player.setTransform(%spawnPoint);
   }

   // If we didn't actually create a player object then bail
   if (!isObject(%player))
   {
      // Make sure we at least have a camera
      %client.spawnCamera(%spawnPoint);

      return;
   }

   // Update the default camera to start with the player
   if (isObject(%client.camera) && !isDefined("%noControl"))
   {
      if (%player.getClassname() $= "Player")
         %client.camera.setTransform(%player.getEyeTransform());
      else
         %client.camera.setTransform(%player.getTransform());
   }

   // Add the player object to MissionCleanup so that it
   // won't get saved into the level files and will get
   // cleaned up properly
   MissionCleanup.add(%player);

   // Store the client object on the player object for
   // future reference
   %player.client = %client;

   // If the player's client has some owned turrets, make sure we let them
   // know that we're a friend too.
   if (%client.ownedTurrets)
   {
      for (%i=0; %i<%client.ownedTurrets.getCount(); %i++)
      {
         %turret = %client.ownedTurrets.getObject(%i);
         %turret.addToIgnoreList(%player);
      }
   }

   // Player setup...
   if (%player.isMethod("setShapeName"))
      %player.setShapeName(%client.playerName);

   if (%player.isMethod("setEnergyLevel"))
      %player.setEnergyLevel(%player.getDataBlock().maxEnergy);

   if (!isDefined("%client.skin"))
   {
      // Determine which character skins are not already in use
      %availableSkins = %player.getDatablock().availableSkins;             // TAB delimited list of skin names
      %count = ClientGroup.getCount();
      for (%cl = 0; %cl < %count; %cl++)
      {
         %other = ClientGroup.getObject(%cl);
         if (%other != %client)
         {
            %availableSkins = strreplace(%availableSkins, %other.skin, "");
            %availableSkins = strreplace(%availableSkins, "\t\t", "");     // remove empty fields
         }
      }

      // Choose a random, unique skin for this client
      %count = getFieldCount(%availableSkins);
      %client.skin = addTaggedString( getField(%availableSkins, getRandom(%count)) );
   }

   %player.setSkinName(%client.skin);

   // Give the client control of the player
   %client.player = %player;

   // Give the client control of the camera if in the editor
   if( $startWorldEditor )
   {
      %control = %client.camera;
      %control.mode = "Fly";
      EditorGui.syncCameraGui();
   }
   else
      %control = %player;

   // Allow the player/camera to receive move data from the GameConnection.  Without this
   // the user is unable to control the player/camera.
   if (!isDefined("%noControl"))
      %client.setControlObject(%control);
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
  loadedNPCs_ = "";
};
