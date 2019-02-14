function CTFGMServer::onAdd(%this)
{
  %this.sphereCastRadius_ = 3.0;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "CTFGMServerQueue";

  %pos = ClientGroup.getObject(0).getControlObject().getPosition();

  %rot = ClientGroup.getObject(0).getControlObject().rotation;

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "FlagSpawnCTFGM")
    {
      %pos = %obj.position;
      %rot = %obj.rotation;

      %this.FlagSpawn_ = new Marker()
      {
        position = %pos;
        rotation = %rot;
      };
    }
    else if (%obj.getName() $= "CaptureACTFGM")
    {
      %this.captureATrigger_ = new Trigger()
      {
        dataBlock = "CTFGMTrigger";
        polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
        position = %obj.position;
        scale = "6 6 6";
      };
    }
    else if (%obj.getName() $= "CaptureBCTFGM")
    {
      %this.captureBTrigger_ = new Trigger()
      {
        dataBlock = "CTFGMTrigger";
        polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
        position = %obj.position;
        scale = "6 6 6";
      };
    }
  }

  %this.flag_ = new StaticShape(Flag)
  {
    dataBlock = flagCTFGM;
    mMoveTolerance = 3.0;
    position = %pos;
    rotation = %rot;
    following_ = false;
    capturer_ = "";
    scale = "0.2 0.2 0.2";
  };

  if (!isObject(%this.FlagSpawn_))
  {
    %this.FlagSpawn_ = new Marker()
    {
      position = %pos;
      rotation = %rot;
    };
  }

  if (!isObject(%this.captureATrigger_))
  {
    %finalPos = VectorScale(ClientGroup.getObject(0).getControlObject().getForwardVector(), 12.0);
    %finalPos = VectorAdd(%pos, %finalPos);

    %this.captureATrigger_ = new Trigger()
    {
      dataBlock = "CTFGMTrigger";
      polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
      position = %finalPos;
      scale = "6 6 6";
    };
  }

  if (!isObject(%this.captureBTrigger_))
  {
    %finalPos = VectorScale(ClientGroup.getObject(0).getControlObject().getForwardVector(), 12.0);
    %finalPos = VectorSub(%pos, %finalPos);

    %this.captureBTrigger_ = new Trigger()
    {
      dataBlock = "CTFGMTrigger";
      polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
      position = %finalPos;
      scale = "6 6 6";
    };
  }

  DNCServer.loadOutListeners_.add(%this);
}

function CTFGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if(isObject(%this.flag_))
  {
    %this.flag_.delete();
  }

  if(isObject(%this.FlagSpawn_))
  {
    %this.FlagSpawn_.delete();
  }

  if(isObject(%this.captureATrigger_))
  {
    %this.captureATrigger_.delete();
  }

  if(isObject(%this.captureBTrigger_))
  {
    %this.captureBTrigger_.delete();
  }

}

function CTFGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (isObject(CTFGMServerSO))
  {
    //if (%obj == CTFGMServerSO.flag_)
    if (%obj == CTFGMServerSO.flag_.getObjectMount())
    {
      if (isObject(DNCServer.TeamChooser_))
      {

        if (%trigger == CTFGMServerSO.captureATrigger_)
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
          {
            %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
            Game.incScore(%client, 1, false);
          }
        }
        else if (%trigger == CTFGMServerSO.captureBTrigger_)
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
          {
            %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
            Game.incScore(%client, 1, false);
          }
        }

        ServerPlay2D(ctfScoreSound);

      }

      /*%obj.unmount();
      %obj.following_ = false;
      %obj.capturer_ = "";
      %obj.setPosition(CTFGMServerSO.FlagSpawn_.getPosition());
      %obj.rotation = CTFGMServerSO.FlagSpawn_.rotation;*/

      CTFGMServerSO.flag_.unmount();
      CTFGMServerSO.flag_.following_ = false;
      CTFGMServerSO.flag_.capturer_ = "";
      CTFGMServerSO.flag_.setPosition(CTFGMServerSO.FlagSpawn_.getPosition());
      CTFGMServerSO.flag_.rotation = CTFGMServerSO.FlagSpawn_.rotation;
    }
  }
}

function CTFGMServer::CTFAction(%this, %client)
{
  %obj = %client.getControlObject();
  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    if(%targetObject.getName() $= "Flag")
    {
      if (%targetObject.following_ == false)
      {
        %targetObject.following_ = true;

        %targetObject.capturer_ = %obj;

        %obj.mountObject(%targetObject, 0, MatrixCreate("0 0 1", "1 0 0 0"));
      }
      else
      {
        %targetObject.unmount();
        %targetObject.following_ = false;
        %targetObject.capturer_ = "";
      }

      break;
    }
  }

}

function serverCmdCTFActionCTFGM(%client)
{
  if (isObject(CTFGMServerSO))
  {
    CTFGMServerSO.CTFAction(%client);
  }
}

function CTFGMServer::loadOut(%this, %player)
{
  commandToClient(%player.client, 'ReloadActionMapCTFGM', false);
}

if (isObject(CTFGMServerSO))
{
  CTFGMServerSO.delete();
}
else
{
  new ScriptObject(CTFGMServerSO)
  {
    class = "CTFGMServer";
    EventManager_ = "";
    actionMap_ = "";
    sphereCastRadius_ = "";
    flag_ = "";
    captureATrigger_ = "";
    captureBTrigger_ = "";
    FlagSpawn_ = "";
  };

  MissionCleanup.add(CTFGMServerSO);
}
