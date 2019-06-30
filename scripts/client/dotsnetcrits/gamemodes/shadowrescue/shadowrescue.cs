function ShadowRescueGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ShadowRescueGMClientQueue";
}

function ShadowRescueGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("ShadowRescueGMClient go bye bye");
}

if (isObject(ShadowRescueGMClientSO))
{
  ShadowRescueGMClientSO.delete();
}
else
{
  new ScriptObject(ShadowRescueGMClientSO)
  {
    class = "ShadowRescueGMClient";
    EventManager_ = "";
  };
}
