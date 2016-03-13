function HostageRescueGMServer::onAdd(%this)
{
  %this.sphereCastRadius_ = 3.0;

  MissionCleanup.add(%this);

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

}

function HostageRescueGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (isObject(HostageRescueGMServerSO))
  {
    if (%obj == HostageRescueGMServerSO.ball_)
    {
      HostageRescueGMServerSO.ball_.reset();
      HostageRescueGMServerSO.ball_.setPosition(HostageRescueGMServerSO.ballSpawn_.getPosition());

      if (%trigger == HostageRescueGMServerSO.teamATrigger_)
      {
        if (isObject(DNCServer.TeamChooser_))
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
          {
            %playerObj = DNCServer.TeamChooser_.teamA_.getObject(%x);
            Game.incScore(%playerObj.client, 1, false);
          }
        }
      }
      else if (%trigger == HostageRescueGMServerSO.teamBTrigger_)
      {
        if (isObject(DNCServer.TeamChooser_))
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
          {
            %playerObj = DNCServer.TeamChooser_.teamB_.getObject(%x);
            Game.incScore(%playerObj.client, 1, false);
          }
        }
      }
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
    hostageSpawns_ = "";
  };
}
