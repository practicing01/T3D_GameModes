function serverCmdUseObject(%client, %state)
{
  if (isObject(DNCServer))
  {
    %player = %client.player;
    %player.UseObject();
  }
}
