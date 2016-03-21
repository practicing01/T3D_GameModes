function HydroballGMServer::onAdd(%this)
{
  %this.sphereCastRadius_ = 3.0;

  %this.throwMagnitude_ = 20.0;

  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HydroballGMServerQueue";

  %pos = ClientGroup.getObject(0).getControlObject().getPosition();

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "fieldSpawnHydroballGM")
    {
      %pos = %obj.position;
    }
  }

  %this.prefab_ = new Prefab()
  {
    class = "HydroballGMPrefab";
    fileName = "art/shapes/dotsnetcrits/gamemodes/hydroball/hydroball.prefab";
  };

  %this.prefab_.setPosition(%pos);

  for (%x = 0; %x < %this.prefabGroup_.getCount(); %x++)
  {
    %obj = %this.prefabGroup_.getObject(%x);
    if (%obj.getName() $= "hydroball")
    {
      %this.ball_ = %obj;
    }
    else if (%obj.getName() $= "teamATrigger")
    {
      %this.teamATrigger_ = %obj;
    }
    else if (%obj.getName() $= "teamBTrigger")
    {
      %this.teamBTrigger_ = %obj;
    }
    else if (%obj.getName() $= "ballSpawnHydroballGM")
    {
      %this.ballSpawn_ = %obj;
    }
  }

  //%this.ball_ = %this.prefabGroup_.findObjectByInternalName("hydroball");

  //Temporary fix cus RigidShape mounting is gimped.
  %this.ballDummy_ = new StaticShape(hydroballDummy)
  {
    dataBlock = "balldummyHydroballGM";
    position = "0 0 0";
    rotation = "1 0 0 0";
    scale = "0.1 0.1 0.1";
    collisionType = "None";
  };

  %this.ballDummy_.setHidden(true);
}

function HydroballGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if(isObject(%this.prefabGroup_))
  {
    %this.prefabGroup_.deleteAllObjects();
    %this.prefabGroup_.delete();
  }

  if(isObject(%this.prefab_))
  {
    %this.prefab_.delete();
  }

  if (isObject(%this.ballDummy_))
  {
    %this.ballDummy_.delete();
  }
}

function HydroballGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (isObject(HydroballGMServerSO))
  {
    if (%obj == HydroballGMServerSO.ball_)
    {
      HydroballGMServerSO.ball_.reset();
      HydroballGMServerSO.ball_.setPosition(HydroballGMServerSO.ballSpawn_.getPosition());

      if (%trigger == HydroballGMServerSO.teamATrigger_)
      {
        if (isObject(DNCServer.TeamChooser_))
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
          {
            %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
            Game.incScore(%client, 1, false);
          }
        }
      }
      else if (%trigger == HydroballGMServerSO.teamBTrigger_)
      {
        if (isObject(DNCServer.TeamChooser_))
        {
          for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
          {
            %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
            Game.incScore(%client, 1, false);
          }
        }
      }
    }
  }
}

/*function HydroballGMTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (isObject(HydroballGMServerSO))
  {
    //
  }
}*/

/*function HydroballGMTrigger::onTickTrigger(%this, %trigger)
{
  //
}*/

function HydroballGMPrefab::onLoad(%this, %group)
{
  if (isObject(HydroballGMServerSO))
  {
    HydroballGMServerSO.prefabGroup_ = %group;
  }
}

function HydroballGMServer::BallAction(%this, %client)
{
  %obj = %client.getControlObject();

  %parentObj = %this.ballDummy_.getObjectMount();

  if (%parentObj == %obj)//throw
  {
    %throwDir = VectorNormalize(%obj.getEyeVector());

    %eyePos = %obj.getEyePoint();
    %this.ball_.freezeSim(false);
    %this.ball_.setHidden(false);

    %pos = %eyePos;
    %size = %obj.getObjectBox();
    %scale = %obj.getScale();
    %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
    %sizex *= 1.5;

    %this.ball_.setPosition( VectorAdd( %pos, VectorScale(%throwDir, %sizex) ) );

    %this.ball_.reset();
    %this.ball_.applyImpulse("0 0 0", VectorScale(%throwDir, %this.throwMagnitude_));

    %this.ballDummy_.unmount();
    %this.ballDummy_.setHidden(true);
  }
  else if (%parentObj == 0)//grab
  {
    %pos = %obj.getPosition();

    initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

    while ( (%targetObject = containerSearchNext()) != 0 )
    {
      if(%targetObject.getName() $= "hydroball")
      {
        //%obj.mountObject(%targetObject, GetMountIndexDNC(%obj, 0));

        %this.ball_.reset();
        %this.ball_.freezeSim(true);
        %this.ball_.setHidden(true);

        %obj.mountObject(%this.ballDummy_, GetMountIndexDNC(%obj, 0));
        %this.ballDummy_.setHidden(false);
        break;
      }
    }
  }

}

function serverCmdBallActionHydroballGM(%client)
{
  if (isObject(HydroballGMServerSO))
  {
    HydroballGMServerSO.BallAction(%client);
  }
}

if (isObject(HydroballGMServerSO))
{
  HydroballGMServerSO.delete();
}
else
{
  new ScriptObject(HydroballGMServerSO)
  {
    class = "HydroballGMServer";
    EventManager_ = "";
    prefab_ = "";
    prefabGroup_ = "";
    actionMap_ = "";
    sphereCastRadius_ = "";
    throwMagnitude_ = "";
    ballDummy_ = "";
    ball_ = "";
    teamATrigger_ = "";
    teamBTrigger_ = "";
    ballSpawn_ = "";
  };
}
