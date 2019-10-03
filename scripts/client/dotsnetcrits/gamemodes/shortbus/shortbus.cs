function ShortbusGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ShortbusGMClientQueue";

}

function ShortbusGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("ShortbusGMClient go bye bye");
}

if (isObject(ShortbusGMClientSO))
{
  ShortbusGMClientSO.delete();
}
else
{
  new ScriptObject(ShortbusGMClientSO)
  {
    class = "ShortbusGMClient";
    EventManager_ = "";
  };
}
