function serverCmdModelLoadDNC(%client, %model)
{
  if (isObject(DNCServer))
  {
    %data = new ArrayObject();
    %data.add("client", %client);
    %data.add("model", %model);
    DNCServer.EventManager_.postEvent("ModelLoadRequest", %data);

    %data.delete();
  }
}
