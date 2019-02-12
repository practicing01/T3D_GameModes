function clientCmdLoadGamemodeDNC(%gamemode)
{
  if (isObject(DNCClient))
  {
    DNCClient.EventManager_.postEvent("LoadGamemode", %gamemode);
  }
}
