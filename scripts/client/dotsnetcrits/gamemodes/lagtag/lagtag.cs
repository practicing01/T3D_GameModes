function LagTagGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "LagTagGMClientQueue";

}

function LagTagGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("LagTagGMClient go bye bye");
}

if (isObject(LagTagGMClientSO))
{
  LagTagGMClientSO.delete();
}
else
{
  new ScriptObject(LagTagGMClientSO)
  {
    class = "LagTagGMClient";
    EventManager_ = "";
  };
}
