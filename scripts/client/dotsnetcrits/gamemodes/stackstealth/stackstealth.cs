function StackStealthGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "StackStealthGMClientQueue";
}

function StackStealthGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("StackStealthGMClient go bye bye");
}

if (isObject(StackStealthGMClientSO))
{
  StackStealthGMClientSO.delete();
}
else
{
  new ScriptObject(StackStealthGMClientSO)
  {
    class = "StackStealthGMClient";
    EventManager_ = "";
  };
}
