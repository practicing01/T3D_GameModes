function DotsNetCritsServer::onAdd(%this)
{
  %this.ClientLeaveCleanup_ = new ArrayObject();
  %this.ClientLeaveListeners_ = new ArrayObject();

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
  exec("scripts/server/dotsnetcrits/GamemodeVoteMachine.cs");
  exec("scripts/server/dotsnetcrits/TeamChooser.cs");

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "DotsNetCritsClientQueue";

  %this.EventManager_.registerEvent("GamemodeVoteCast");

  %this.EventManager_.registerEvent("GamemodeVoteTallied");

  %this.EventManager_.registerEvent("TeamJoinRequest");

  %this.EventManager_.registerEvent("WeaponLoadRequest");

  %this.EventManager_.subscribe(%this, "GamemodeVoteTallied");

  %this.EventManager_.subscribe(%this, "WeaponLoadRequest");

  %this.GameModeVoteMachine_ = new ScriptObject()
  {
    class = "GameModeVoteMachine";
  };

  %this.EventManager_.subscribe(%this.GameModeVoteMachine_, "GamemodeVoteCast");

  %this.TeamChooser_ = new ScriptObject()
  {
    class = "TeamChooser";
  };

  %this.EventManager_.subscribe(%this.TeamChooser_, "TeamJoinRequest");

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

  if (isObject(%this.TeamChooser_))
  {
    %this.TeamChooser_.delete();
  }

  if (isObject(%this.ClientLeaveCleanup_))
  {
    %this.ClientLeaveCleanup_.delete();
  }

  if (isObject(%this.ClientLeaveListeners_))
  {
    %this.ClientLeaveListeners_.delete();
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
    if (getField(%dirList, %x) $= %weapon)//Make sure the weapon exists.
    {
      exec("scripts/server/dotsnetcrits/weapons/"@%weapon@"/"@%weapon@".cs");

      %player = %client.getControlObject();
      %player.setInventory(%weapon, 1);
      %player.addToWeaponCycle(%weapon);

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

function DeathMatchGame::loadOut(%game, %player)
{
  parent::loadOut(%game, %player);
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

new ScriptObject(DNCServer)
{
  class = "DotsNetCritsServer";
  EventManager_ = "";
  GameModeVoteMachine_ = "";
  TeamChooser_ = "";
  ClientLeaveCleanup_ = "";
  ClientLeaveListeners_ = "";
};
