function ExcavatorGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ExcavatorGMClientQueue";
}

function ExcavatorGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("ExcavatorGMClient go bye bye");
}

if (isObject(ExcavatorGMClientSO))
{
  ExcavatorGMClientSO.delete();
}
else
{
  new ScriptObject(ExcavatorGMClientSO)
  {
    class = "ExcavatorGMClient";
    EventManager_ = "";
  };
}
