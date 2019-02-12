function serverCmdJoinTeamDNC(%client, %team)
{
  if (isObject(DNCServer))
  {
    %data = new ArrayObject();
    %data.add("client", %client);
    %data.add("team", %team);
    DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);

    %data.delete();
  }
}
