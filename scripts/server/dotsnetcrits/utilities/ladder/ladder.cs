function serverCmdLadderState(%client, %dir, %state)
{
  if (!isObject(DNCServer))
  {
    return;
  }

  %index = DNCServer.ladderManager_.ladderSchedules_.getIndexFromKey(%client);
  if (%index == -1)
  {
    DNCServer.ladderManager_.ladderSchedules_.add(%client, -1);
    %index = DNCServer.ladderManager_.ladderSchedules_.getIndexFromKey(%client);
  }

  if (%state == 0)
  {
    %value = DNCServer.ladderManager_.ladderSchedules_.getValue(%index);
    if (%value != -1)
    {
      cancel(%value);
    }
    return;
  }

  %schedule = DNCServer.ladderManager_.schedule(LadderTrigger.ladderMoveInterval_, "MoveAlongLadder", %client, %dir);

  DNCServer.ladderManager_.ladderSchedules_.setValue(%schedule, %index);
}

function LadderManager::MoveAlongLadder(%this, %client, %dir)
{
  %index = %this.ladderSchedules_.getIndexFromKey(%client);

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

  %schedule = %this.schedule(LadderTrigger.ladderMoveInterval_, "MoveAlongLadder", %client, %dir);

  %this.ladderSchedules_.setValue(%schedule, %index);
}

function LadderManager::onAdd(%this)
{
  %this.ladderSchedules_ = new ArrayObject();
}

function LadderManager::onRemove(%this)
{
  %this.ladderSchedules_.delete();
}

function LadderManager::onClientLeaveGame(%this, %client)
{
  %index = %this.ladderSchedules_.getIndexFromKey(%client);
  if (%index != -1)
  {
    %this.ladderSchedules_.erase(%index);
  }
}

function LadderTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if ( %obj.getType() & $TypeMasks::PlayerObjectType )
  {
    commandToClient(%obj.client, 'LadderAMToggle', true);
  }
}

function LadderTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if ( %obj.getType() & $TypeMasks::PlayerObjectType )
  {
    commandToClient(%obj.client, 'LadderAMToggle', false);
  }
}
