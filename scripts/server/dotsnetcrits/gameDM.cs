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

  %index = DNCServer.clientModels_.getIndexFromKey(%client);
  DNCServer.clientModels_.erase(%index);

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

function DeathMatchGame::onMissionLoaded(%game)
{
   $Server::MissionType = "DeathMatch";
   parent::onMissionLoaded(%game);

   for (%x = 0; %x < DNCServer.missionLoadedListeners_.getCount(); %x++)
   {
     DNCServer.missionLoadedListeners_.getObject(%x).onMissionLoaded(%game);
   }
}

function DeathMatchGame::incKills(%game, %client, %kill, %dontMessageAll)
{
  parent::incKills(%game, %client, %kill, %dontMessageAll);

  for (%x = 0; %x < DNCServer.incKillsListeners_.getCount(); %x++)
  {
    DNCServer.incKillsListeners_.getObject(%x).incKills(%client, %kill);
    /*%obj = DNCServer.incKillsListeners_.getObject(%x);
    %obj.call("incKills", %client, %kill);*/
  }
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
  %model = "raven2d";

  %index = DNCServer.clientModels_.getIndexFromKey(%client);

  if (%index == -1)
  {
    DNCServer.clientModels_.add(%client, "raven2d");
  }
  else
  {
    %model = DNCServer.clientModels_.getValue(%index);
  }

  $Game::DefaultPlayerDataBlock = %model;
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
