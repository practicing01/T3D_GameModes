function serverCmdWeaponLoadDNC(%client, %weapon)
{
  if (isObject(DNCServer))
  {
    %data = new ArrayObject();
    %data.add("client", %client);
    %data.add("weapon", %weapon);
    DNCServer.EventManager_.postEvent("WeaponLoadRequest", %data);

    %data.delete();
  }
}
