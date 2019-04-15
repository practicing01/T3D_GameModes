function clientCmdLadderAMToggle(%state)
{
  if (!isObject(DNCClient))
  {
    return;
  }

  DNCClient.ToggleLadderActionMap(%state);
}

function DotsNetCritsClient::LadderMoveUp(%this, %val)
{
  commandToServer('LadderState', 1, %val);
}

function DotsNetCritsClient::LadderMoveDown(%this, %val)
{
  commandToServer('LadderState', 2, %val);
}

function DotsNetCritsClient::ToggleLadderActionMap(%this, %state)
{
  if (%state == true)
  {
    %this.ladderActionMap_.push();
  }
  else
  {
    %this.ladderActionMap_.pop();
  }
}
