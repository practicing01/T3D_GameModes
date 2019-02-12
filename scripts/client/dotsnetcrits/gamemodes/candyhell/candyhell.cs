function CandyHellGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "CandyHellGMClientQueue";

}

function CandyHellGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("CandyHellGMClient go bye bye");
}

if (isObject(CandyHellGMClientSO))
{
  CandyHellGMClientSO.delete();
}
else
{
  new ScriptObject(CandyHellGMClientSO)
  {
    class = "CandyHellGMClient";
    EventManager_ = "";
  };
}
