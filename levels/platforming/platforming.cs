//With global variables, come great responsibility.
//Only using them here cus the map is being released outside of the mod.
//Otherwise the mod would have dealt with all of the following differently.
//Mounted object isn't synchronizing with the PathCamera.
//Not sure how to make PathCamera loop, can't test visually because mounted object isn't synching position.
//Keys maintain old ActionMap functions until depressed.
$PlatformingLadderSchedules = new ArrayObject();

datablock TriggerData(PlatformingLadderTrigger)
{
  tickPeriodMS = 500;
  ladderMoveInterval_ = 500;
};

datablock PathCameraData(PlatformingDummyCam)
{
};

function PathCamSpawner::onRemove(%this)
{
  if (isObject(%this.pathCam_))
  {
    %this.pathCam_.delete();
  }
}

function PathCamSpawner::onAdd(%this)
{
  %crate = "";
  %camPath = "";

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);

    if (%obj.name $= "crate")
    {
      %crate = %obj;
    }
    else if (%obj.name $= "camPath")
    {
      %camPath = %obj;
    }
  }

  %pathCam = new PathCamera()
  {
    dataBlock = PlatformingDummyCam;
    class = "PlatformingPathCamClass";
    position = %crate.position;
    path = %camPath;
  };

  %this.pathCam_ = %pathCam;

  %pathCam.mountObject(%crate, 0);

  for (%x = 0; %x < %camPath.getCount(); %x++)
  {
    %marker = %camPath.getObject(%x);
    %pathCam.pushBack(%marker.getTransform(), 5.0, %marker.type, %marker.smoothingType);
  }

  %pathCam.setPosition(0);
}

function PlatformingPathCamClass::onNode(%this, %index)
{
  if (%index >= %this.path.getCount())
  {
    %this.setPosition(0);
    %this.setTarget(1.0);
    %this.setState("forward");
  }
}

function PlatformingLadderTriggerClass::MoveUp(%this, %val)
{
  commandToServer('PlatformingLadderState', 1, %val);
}

function PlatformingLadderTriggerClass::MoveDown(%this, %val)
{
  commandToServer('PlatformingLadderState', 2, %val);
}

function serverCmdPlatformingLadderState(%client, %dir, %state)
{
  %index = $PlatformingLadderSchedules.getIndexFromKey(%client);
  if (%index == -1)
  {
    $PlatformingLadderSchedules.add(%client, -1);
    %index = $PlatformingLadderSchedules.getIndexFromKey(%client);
  }

  if (%state == 0)
  {
    %value = $PlatformingLadderSchedules.getValue(%index);
    if (%value != -1)
    {
      cancel(%value);
    }
    return;
  }

  %schedule = PlatformingLadderTrigger.schedule(PlatformingLadderTrigger.ladderMoveInterval_, "MoveAlongLadder", %client, %dir);

  $PlatformingLadderSchedules.setValue(%schedule, %index);
}

function PlatformingLadderTriggerClass::onAdd(%this)
{
  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "w", %this @ ".MoveUp(1);", %this @ ".MoveUp(0);");
  %this.actionMap_.bindCmd(keyboard, "s", %this @ ".MoveDown(1);", %this @ ".MoveDown(0);");
}

function PlatformingLadderTriggerClass::onRemove(%this)
{
  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  if (isObject($PlatformingLadderSchedules))
  {
    $PlatformingLadderSchedules.delete();
  }
}

function PlatformingLadderTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if ( %obj.getType() & $TypeMasks::PlayerObjectType )
  {
    commandToClient(%obj.client, 'LadderIOToggle', true);
  }
}

function PlatformingLadderTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if ( %obj.getType() & $TypeMasks::PlayerObjectType )
  {
    commandToClient(%obj.client, 'LadderIOToggle', false);
  }
}

function PlatformingLadderTrigger::MoveAlongLadder(%this, %client, %dir)
{
  %index = $PlatformingLadderSchedules.getIndexFromKey(%client);

  %upVector = %client.player.getUpVector();
  %forwardVector = %client.player.getForwardVector();
  %forwardVector = VectorScale(%forwardVector, 1000.0);

  if (%dir == 1)
  {
    %upVector = VectorScale(%upVector, 300.0);
    %client.player.applyImpulse("0 0 0", VectorAdd(%upVector, %forwardVector));
  }
  else if (%dir == 2)
  {
    %upVector = VectorScale(%upVector, -300.0);
    %client.player.applyImpulse("0 0 0", %upVector);
  }

  %schedule = PlatformingLadderTrigger.schedule(PlatformingLadderTrigger.ladderMoveInterval_, "MoveAlongLadder", %client, %dir);

  $PlatformingLadderSchedules.setValue(%schedule, %index);
}

function clientCmdLadderIOToggle(%state)
{
  %ladder = "";

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);

    if (%obj.class $= "PlatformingLadderTriggerClass")
    {
      %ladder = %obj;
      break;
    }
  }

  if (!isObject(%ladder))
  {
    return;
  }

  if (%state == true)
  {
    %ladder.actionMap_.push();
  }
  else
  {
    %ladder.actionMap_.pop();
  }
}
