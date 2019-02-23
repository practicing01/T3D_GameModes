function serverCmdLevelVoteDNC(%client, %level)
{
  if (isObject(DNCServer))
  {
    %data = new ArrayObject();
    %data.add("client", %client);
    %data.add("level", %level);
    DNCServer.EventManager_.postEvent("LevelVoteCast", %data);

    %data.delete();
  }
}
