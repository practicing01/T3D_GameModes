function GoslowstopGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "GoslowstopGMClientQueue";
}

function GoslowstopGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("GoslowstopGMClient go bye bye");
}

if (isObject(GoslowstopGMClientSO))
{
  GoslowstopGMClientSO.delete();
}
else
{
  new ScriptObject(GoslowstopGMClientSO)
  {
    class = "GoslowstopGMClient";
    EventManager_ = "";
  };
}
