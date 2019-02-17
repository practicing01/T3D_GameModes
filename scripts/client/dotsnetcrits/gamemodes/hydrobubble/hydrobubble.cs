function HydrobubbleGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HydrobubbleGMClientQueue";

}

function HydrobubbleGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("HydrobubbleGMClient go bye bye");
}

if (isObject(HydrobubbleGMClientSO))
{
  HydrobubbleGMClientSO.delete();
}
else
{
  new ScriptObject(HydrobubbleGMClientSO)
  {
    class = "HydrobubbleGMClient";
    EventManager_ = "";
  };
}
