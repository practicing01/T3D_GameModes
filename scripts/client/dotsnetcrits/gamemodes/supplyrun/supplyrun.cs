function SupplyRunGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "SupplyRunGMClientQueue";

}

function SupplyRunGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("SupplyRunGMClient go bye bye");
}

if (isObject(SupplyRunGMClientSO))
{
  SupplyRunGMClientSO.delete();
}
else
{
  new ScriptObject(SupplyRunGMClientSO)
  {
    class = "SupplyRunGMClient";
    EventManager_ = "";
  };
}
