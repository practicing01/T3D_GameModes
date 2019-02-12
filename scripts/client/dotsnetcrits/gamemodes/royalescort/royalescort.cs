function RoyalEscortGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "RoyalEscortGMClientQueue";

}

function RoyalEscortGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("RoyalEscortGMClient go bye bye");
}

if (isObject(RoyalEscortGMClientSO))
{
  RoyalEscortGMClientSO.delete();
}
else
{
  new ScriptObject(RoyalEscortGMClientSO)
  {
    class = "RoyalEscortGMClient";
    EventManager_ = "";
  };
}
