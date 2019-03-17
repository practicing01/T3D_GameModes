function GooshballGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "GooshballGMClientQueue";

}

function GooshballGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("GooshballGMClient go bye bye");
}

if (isObject(GooshballGMClientSO))
{
  GooshballGMClientSO.delete();
}
else
{
  new ScriptObject(GooshballGMClientSO)
  {
    class = "GooshballGMClient";
    EventManager_ = "";
  };
}
