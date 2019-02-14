function BloodFlowersGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BloodFlowersGMClientQueue";

}

function BloodFlowersGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("BloodFlowersGMClient go bye bye");
}

if (isObject(BloodFlowersGMClientSO))
{
  BloodFlowersGMClientSO.delete();
}
else
{
  new ScriptObject(BloodFlowersGMClientSO)
  {
    class = "BloodFlowersGMClient";
    EventManager_ = "";
  };
}
