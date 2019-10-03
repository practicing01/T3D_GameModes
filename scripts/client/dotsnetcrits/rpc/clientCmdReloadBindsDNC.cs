function clientCmdReloadBindsDNC()
{
  if (isObject(DNCClient))
  {
    DNCClient.ReloadBinds();
  }
}
