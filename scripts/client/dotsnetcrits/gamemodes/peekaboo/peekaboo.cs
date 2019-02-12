function PeekabooGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PeekabooGMClientQueue";

}

function PeekabooGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("PeekabooGMClient go bye bye");
}

if (isObject(PeekabooGMClientSO))
{
  PeekabooGMClientSO.delete();
}
else
{
  new ScriptObject(PeekabooGMClientSO)
  {
    class = "PeekabooGMClient";
    EventManager_ = "";
  };
}
