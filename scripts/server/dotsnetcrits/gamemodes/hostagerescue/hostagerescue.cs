function HostageRescueGMServer::SpawnHostie(%this)
{
  %pos = ClientGroup.getObject(0).getControlObject().getPosition();

  %rot = ClientGroup.getObject(0).getControlObject().rotation;

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "HostageSpawnHostageRescueGM")
    {
      %pos = %obj.position;
      %rot = %obj.rotation;

      break;
    }
  }

  if (isObject(Hostage))
  {
    Hostage.delete();
  }

  %this.hostage_ = new AiPlayer(Hostage)
  {
    dataBlock = HostieHostageRescueGM;
    mMoveTolerance = 3.0;
    position = %pos;
    rotation = %rot;
    following_ = false;
    rescuer_ = "";
  };
}

function HostageRescueGMServer::onAdd(%this)
{
  %this.sphereCastRadius_ = 3.0;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HostageRescueGMServerQueue";

  %pos = ClientGroup.getObject(0).getControlObject().getPosition();

  %rot = ClientGroup.getObject(0).getControlObject().rotation;

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "HostageSpawnHostageRescueGM")
    {
      %pos = %obj.position;
      %rot = %obj.rotation;

      %this.hostageSpawn_ = new Marker()
      {
        position = %pos;
        rotation = %rot;
      };
    }
    else if (%obj.getName() $= "HostageRescueHostageRescueGM")
    {
      %this.rescueTrigger_ = new Trigger()
      {
        dataBlock = "HostageRescueGMTrigger";
        polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
        position = %obj.position;
        scale = "6 6 6";
      };
    }
  }

  %this.hostage_ = new AiPlayer(Hostage)
  {
    dataBlock = HostieHostageRescueGM;
    mMoveTolerance = 3.0;
    position = %pos;
    rotation = %rot;
    following_ = false;
    rescuer_ = "";
  };

  if (!isObject(%this.hostageSpawn_))
  {
    %this.hostageSpawn_ = new Marker()
    {
      position = %pos;
      rotation = %rot;
    };
  }

  if (!isObject(%this.rescueTrigger_))
  {
    %this.rescueTrigger_ = new Trigger()
    {
      dataBlock = "HostageRescueGMTrigger";
      polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
      position = VectorAdd(%pos, ClientGroup.getObject(0).getControlObject().getForwardVector());
      scale = "6 6 6";
    };
  }

  DNCServer.loadOutListeners_.add(%this);
}

function HostageRescueGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if(isObject(%this.hostage_))
  {
    %this.hostage_.delete();
  }

  if(isObject(%this.hostageSpawn_))
  {
    %this.hostageSpawn_.delete();
  }

  if(isObject(%this.rescueTrigger_))
  {
    %this.rescueTrigger_.delete();
  }

}

function HostageRescueGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (isObject(HostageRescueGMServerSO))
  {
    if (%obj == HostageRescueGMServerSO.hostage_)
    {
      if (isObject(DNCServer.TeamChooser_))
      {
        if (DNCServer.TeamChooser_.teamA_.isMember(%obj.rescuer_.client))
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
          {
            %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
            Game.incScore(%client, 1, false);
          }
        }
        else if (DNCServer.TeamChooser_.teamB_.isMember(%obj.rescuer_.client))
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
          {
            %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
            Game.incScore(%client, 1, false);
          }
        }
      }

      ServerPlay2D(hostageRescuedSound);

      %obj.stop();
      %obj.following_ = false;
      %obj.rescuer_ = "";
      %obj.setPosition(HostageRescueGMServerSO.hostageSpawn_.getPosition());
      %obj.rotation = HostageRescueGMServerSO.hostageSpawn_.rotation;
    }
  }
}

function HostageRescueGMServer::HostageAction(%this, %client)
{
  %obj = %client.getControlObject();
  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    if(%targetObject.getName() $= "Hostage")
    {
      if (%targetObject.following_ == false)
      {
        %targetObject.following_ = true;

        %targetObject.rescuer_ = %obj;

        %forwardVector = %obj.getForwardVector();
        %forwardVector = VectorScale(%forwardVector, -2.0);//todo change to obj box size, same for mMoveTolerance

        %targetObject.setMoveDestination(VectorAdd(%pos, %forwardVector));
      }
      else
      {
        %targetObject.stop();
        %targetObject.following_ = false;
        %targetObject.rescuer_ = "";
      }

      break;
    }
  }

}

function serverCmdHostageActionHostageRescueGM(%client)
{
  if (isObject(HostageRescueGMServerSO))
  {
    HostageRescueGMServerSO.HostageAction(%client);
  }
}

function HostieHostageRescueGM::onReachDestination(%this, %ai)
{
  %forwardVector = %ai.rescuer_.getForwardVector();
  %forwardVector = VectorScale(%forwardVector, -2.0);//todo change to obj box size, same for mMoveTolerance

  %ai.setMoveDestination(VectorAdd(%ai.rescuer_.position, %forwardVector));

}

function HostieHostageRescueGM::onDisabled(%this, %obj, %state)
{
  parent::onDisabled(%this, %obj, %state);

  if (isObject(HostageRescueGMServerSO))
  {
    HostageRescueGMServerSO.SpawnHostie();
  }
}

function HostageRescueGMServer::loadOut(%this, %player)
{
  commandToClient(%player.client, 'ReloadActionMapHostageRescueGM', false);
}

if (isObject(HostageRescueGMServerSO))
{
  HostageRescueGMServerSO.delete();
}
else
{
  new ScriptObject(HostageRescueGMServerSO)
  {
    class = "HostageRescueGMServer";
    EventManager_ = "";
    actionMap_ = "";
    sphereCastRadius_ = "";
    hostage_ = "";
    rescueTrigger_ = "";
    hostageSpawn_ = "";
  };

  MissionCleanup.add(HostageRescueGMServerSO);
}
