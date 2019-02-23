function clientCmdNPCLoadDNC(%npc, %state)
{
  if (isObject(DNCClient))
  {
    %data = new ArrayObject();
    %data.add("npc", %npc);
    %data.add("state", %state);
    DNCClient.EventManager_.postEvent("NPCLoadRequest", %data);
  }
}
