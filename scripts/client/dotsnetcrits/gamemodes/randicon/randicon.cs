function RandiconGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "RandiconGMClientQueue";

}

function RandiconGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("RandiconGMClient go bye bye");
}

if (isObject(RandiconGMClientSO))
{
  RandiconGMClientSO.delete();
}
else
{
  new ScriptObject(RandiconGMClientSO)
  {
    class = "RandiconGMClient";
    EventManager_ = "";
  };
}
