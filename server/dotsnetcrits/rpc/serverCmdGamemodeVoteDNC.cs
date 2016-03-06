function serverCmdGamemodeVoteDNC(%client, %gamemode)
{
  if (isObject(DNCServer))
  {
    %data = new ArrayObject();
    %data.add("client", %client);
    %data.add("gamemode", %gamemode);
    DNCServer.EventManager_.postEvent("GamemodeVoteCast", %data);

    %data.delete();
  }
}
