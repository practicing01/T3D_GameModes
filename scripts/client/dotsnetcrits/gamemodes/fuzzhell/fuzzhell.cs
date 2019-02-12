function FuzzHellGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "FuzzHellGMClientQueue";

}

function FuzzHellGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("FuzzHellGMClient go bye bye");
}

if (isObject(FuzzHellGMClientSO))
{
  FuzzHellGMClientSO.delete();
}
else
{
  new ScriptObject(FuzzHellGMClientSO)
  {
    class = "FuzzHellGMClient";
    EventManager_ = "";
  };
}
