function AllvOneGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "AllvOneGMClientQueue";

}

function AllvOneGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("AllvOneGMClient go bye bye");
}

if (isObject(AllvOneGMClientSO))
{
  AllvOneGMClientSO.delete();
}
else
{
  new ScriptObject(AllvOneGMClientSO)
  {
    class = "AllvOneGMClient";
    EventManager_ = "";
  };
}
