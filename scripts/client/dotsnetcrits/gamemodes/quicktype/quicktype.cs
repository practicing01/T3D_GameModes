function QuickTypeGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "QuickTypeGMClientQueue";

}

function QuickTypeGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("QuickTypeGMClient go bye bye");
}

if (isObject(QuickTypeGMClientSO))
{
  QuickTypeGMClientSO.delete();
}
else
{
  new ScriptObject(QuickTypeGMClientSO)
  {
    class = "QuickTypeGMClient";
    EventManager_ = "";
  };
}
