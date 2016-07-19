function DiffusalGMServer::onAdd(%this)
{
  %this.sphereCastRadius_ = 3.0;

  %this.castingTime_ = 5.0;

  %this.detonationDelay_ = 10.0;

  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "DiffusalGMServerQueue";

  %obj = ClientGroup.getObject(0).getControlObject();

  %pos = %obj.getPosition();

  %this.trigger_ = new Trigger()
  {
    dataBlock = "DiffusalGMTrigger";
    polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
    position = %pos;
    scale = "4 4 4";
    enabled_ = false;
  };

  %this.bomb_ = new StaticShape(bomb)
  {
    dataBlock = "bombDiffusalGM";
    position = %pos;
    rotation = "1 0 0 0";
    scale = "0.1 0.1 0.1";
    team_ = -1;
  };

  %obj.mountObject(%this.bomb_, 0, MatrixCreate("0 0 1", "1 0 0 0"));

  %this.diffusalCandidates_ = new SimSet();

  DNCServer.loadOutListeners_.add(%this);
}

function DiffusalGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.trigger_))
  {
    %this.trigger_.delete();
  }

  if (isObject(%this.bomb_))
  {
    %this.bomb_.delete();
  }

  if (isObject(%this.diffusalCandidates_))
  {
    %this.diffusalCandidates_.delete();
  }

  cancel(%this.diffuseSchedule_);
  cancel(%this.detonateSchedule_);
}

function DiffusalGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  /*if (%trigger.enabled_ == false)
  {
    return;
  }*/

//the following for debug until i can test multiplayer
  if (!DiffusalGMServerSO.diffusalCandidates_.isMember(%obj))
  {
    DiffusalGMServerSO.diffusalCandidates_.add(%obj);
  }

  return;

  if (isObject(DiffusalGMServerSO))
  {
    if (DiffusalGMServerSO.bomb_.team_ == 0)//team A
    {
      if (isObject(DNCServer.TeamChooser_))
      {
        for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
        {
          %playerObj = DNCServer.TeamChooser_.teamB_.getObject(%x).getControlObject();

          if (%obj == %playerObj)
          {
            if (!DiffusalGMServerSO.diffusalCandidates_.isMember(%obj))
            {
              DiffusalGMServerSO.diffusalCandidates_.add(%obj);
            }
            break;
          }
        }
      }
    }
    else if (DiffusalGMServerSO.bomb_.team_ == 1)//team B
    {
      if (isObject(DNCServer.TeamChooser_))
      {
        for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
        {
          %playerObj = DNCServer.TeamChooser_.teamA_.getObject(%x).getControlObject();

          if (%obj == %playerObj)
          {
            if (!DiffusalGMServerSO.diffusalCandidates_.isMember(%obj))
            {
              DiffusalGMServerSO.diffusalCandidates_.add(%obj);
            }
            break;
          }
        }
      }
    }

  }
}

function DiffusalGMTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  /*if (%trigger.enabled_ == false)
  {
    return;
  }*/

  if (isObject(DiffusalGMServerSO))
  {
    DiffusalGMServerSO.diffusalCandidates_.remove(%obj);

    if (DiffusalGMServerSO.diffuser_ == %obj)
    {
      DiffusalGMServerSO.diffuser_ = -1;
      cancel(DiffusalGMServerSO.diffuseSchedule_);
    }
  }
}

/*function DiffusalGMTrigger::onTickTrigger(%this, %trigger)
{
  //
}*/

function DiffusalGMServer::Diffuse(%this)
{
  %diffuserTeam = -1;

  if (isObject(DNCServer.TeamChooser_))
  {
    for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
    {
      %client = DNCServer.TeamChooser_.teamA_.getObject(%x);

      if (%client == %this.diffuser_.client)
      {
        %diffuserTeam = 0;//team A
        break;
      }
    }

    for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
    {
      %client = DNCServer.TeamChooser_.teamB_.getObject(%x);

      if (%client == %this.diffuser_.client)
      {
        %diffuserTeam = 1;//team B
        break;
      }
    }

  }

  if (%this.bomb_.team_ == 0 && %diffuserTeam != %this.bomb_.team_)//team A
  {
    if (isObject(DNCServer.TeamChooser_))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
        Game.incScore(%client, 1, false);
        centerPrintAll("Team B Diffused!", 3, 1);
      }
    }
  }
  else if (%this.bomb_.team_ == 1 && %diffuserTeam != %this.bomb_.team_)//team B
  {
    if (isObject(DNCServer.TeamChooser_))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
        Game.incScore(%client, 1, false);
        centerPrintAll("Team A Diffused!", 3, 1);
      }
    }
  }
  else if (%diffuserTeam == %this.bomb_.team_)
  {
    centerPrintAll("Detonation Cancelled!", 3, 1);
  }
  else
  {
    centerPrintAll("Anon Diffused!", 3, 1);
  }

  ServerPlay2D(diffuseBombSound);

  %this.trigger_.enabled_ = false;
  %this.diffuser_.mountObject(%this.bomb_, 0, MatrixCreate("0 0 1", "1 0 0 0"));
  %this.diffuser_ = -1;

  cancel(%this.detonateSchedule_);

}

function DiffusalGMServer::Detonate(%this)
{
  if (%this.bomb_.team_ == 0)//team A
  {
    if (isObject(DNCServer.TeamChooser_))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
        Game.incScore(%client, 1, false);
        centerPrintAll("Team A Detonated!", 3, 1);
      }
    }
  }
  else if (%this.bomb_.team_ == 1)//team B
  {
    if (isObject(DNCServer.TeamChooser_))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
        Game.incScore(%client, 1, false);
        centerPrintAll("Team B Detonated!", 3, 1);
      }
    }
  }
  else
  {
    centerPrintAll("Anon Detonated!", 3, 1);
  }

  ServerPlay2D(detonateBombSound);

  %this.trigger_.enabled_ = false;
  //%obj = ClientGroup.getObject(getRandom(0, ClientGroup.getCount() - 1)).getControlObject();
  //%obj.mountObject(%this.bomb_, 0, MatrixCreate("0 0 1", "1 0 0 0"));
  %this.diffuser_ = -1;

  cancel(%this.diffuseSchedule_);

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %player = %client.getControlObject();
    %player.damage(%this, "0 0 0", 1000, "bomb");
  }
}

function DiffusalGMServer::BombAction(%this, %client)
{
  %obj = %client.getControlObject();

  %parentObj = %this.bomb_.getObjectMount();

  if (%parentObj == %obj)//plant
  {
    %this.bomb_.unmount();
    %this.bomb_.setPosition( %obj.position );

    %this.trigger_.setPosition( %obj.position );
    %this.trigger_.enabled_ = true;

    if (isObject(DNCServer.TeamChooser_))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
      {
        %playerObj = DNCServer.TeamChooser_.teamA_.getObject(%x).getControlObject();

        if (%playerObj == %obj)
        {
          %this.bomb_.team_ = 0;//team A
          %this.detonateSchedule_ = %this.schedule(%this.detonationDelay_ * 1000, "Detonate");

          centerPrintAll("Team A Planted!", 3, 1);
          ServerPlay2D(plantBombSound);
          return;
        }
      }

      for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
      {
        %playerObj = DNCServer.TeamChooser_.teamB_.getObject(%x).getControlObject();

        if (%playerObj == %obj)
        {
          %this.bomb_.team_ = 1;//team B
          %this.detonateSchedule_ = %this.schedule(%this.detonationDelay_ * 1000, "Detonate");

          centerPrintAll("Team B Planted!", 3, 1);
          ServerPlay2D(plantBombSound);
          return;
        }
      }

      %this.bomb_.team_ = -1;
      %this.detonateSchedule_ = %this.schedule(%this.detonationDelay_ * 1000, "Detonate");

      centerPrintAll("Anon Planted!", 3, 1);

      ServerPlay2D(plantBombSound);
    }
  }
  else if (%parentObj == 0)//diffuse or pickup
  {
    %pos = %obj.getPosition();

    initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

    while ( (%targetObject = containerSearchNext()) != 0 )
    {
      if(%targetObject.getName() $= "bomb")
      {
        if (isEventPending(%this.detonateSchedule_))//diffuse
        {
          if (%this.diffusalCandidates_.isMember(%obj))
          {
            %this.diffuser_ = %obj;

            %this.diffuseSchedule_ = %this.schedule(%this.castingTime_ * 1000, "Diffuse");
          }
        }
        else
        {
          %obj.mountObject(%this.bomb_, 0, MatrixCreate("0 0 1", "1 0 0 0"));
        }

        break;
      }
    }
  }

}

function serverCmdBombActionDiffusalGM(%client)
{
  if (isObject(DiffusalGMServerSO))
  {
    DiffusalGMServerSO.BombAction(%client);
  }
}

function DiffusalGMServer::loadOut(%this, %player)
{
  commandToClient(%player.client, 'ReloadActionMapDiffusalGM', false);
}

if (isObject(DiffusalGMServerSO))
{
  DiffusalGMServerSO.delete();
}
else
{
  new ScriptObject(DiffusalGMServerSO)
  {
    class = "DiffusalGMServer";
    EventManager_ = "";
    actionMap_ = "";
    sphereCastRadius_ = "";
    bomb_ = "";
    trigger_ = "";
    diffusalCandidates_ = "";
    diffuser_ = -1;
    castingTime_ = "";
    diffuseSchedule_ = "";
    planter_ = "";
    detonationDelay_ = "";
    detonateSchedule_ = "";
  };
}
