function InstaboneGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "InstaboneGMClientQueue";

}

function InstaboneGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("InstaboneGMClient go bye bye");
}

if (isObject(InstaboneGMClientSO))
{
  InstaboneGMClientSO.delete();
}
else
{
  new ScriptObject(InstaboneGMClientSO)
  {
    class = "InstaboneGMClient";
    EventManager_ = "";
  };
}
