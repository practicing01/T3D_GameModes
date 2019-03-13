function PumptreeGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PumptreeGMClientQueue";

}

function PumptreeGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("PumptreeGMClient go bye bye");
}

if (isObject(PumptreeGMClientSO))
{
  PumptreeGMClientSO.delete();
}
else
{
  new ScriptObject(PumptreeGMClientSO)
  {
    class = "PumptreeGMClient";
    EventManager_ = "";
  };
}
