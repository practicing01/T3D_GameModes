function PredatorGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PredatorGMClientQueue";

}

function PredatorGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("PredatorGMClient go bye bye");
}

if (isObject(PredatorGMClientSO))
{
  PredatorGMClientSO.delete();
}
else
{
  new ScriptObject(PredatorGMClientSO)
  {
    class = "PredatorGMClient";
    EventManager_ = "";
  };
}
