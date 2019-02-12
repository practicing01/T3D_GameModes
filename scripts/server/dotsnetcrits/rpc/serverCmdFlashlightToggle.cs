function serverCmdFlashlightToggle(%client, %state)
{
  if (isObject(DNCServer))
  {
    %player = %client.player;
    %player.FlashlightToggle(%state);
  }
}
