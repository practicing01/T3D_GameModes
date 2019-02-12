function serverCmdNPCLoadDNC(%client, %npc)
{
  if (isObject(DNCServer))
  {
    %data = new ArrayObject();
    %data.add("client", %client);
    %data.add("npc", %npc);
    DNCServer.EventManager_.postEvent("NPCLoadRequest", %data);

    %data.delete();
  }
}
