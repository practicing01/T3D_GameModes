function GhostsGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "GhostsGMClientQueue";

}

function GhostsGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("GhostsGMClient go bye bye");
}

if (isObject(GhostsGMClientSO))
{
  GhostsGMClientSO.delete();
}
else
{
  new ScriptObject(GhostsGMClientSO)
  {
    class = "GhostsGMClient";
    EventManager_ = "";
  };
}
