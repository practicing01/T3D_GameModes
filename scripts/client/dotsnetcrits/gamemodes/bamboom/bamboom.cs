function BamboomGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BamboomGMClientQueue";

}

function BamboomGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("BamboomGMClient go bye bye");
}

if (isObject(BamboomGMClientSO))
{
  BamboomGMClientSO.delete();
}
else
{
  new ScriptObject(BamboomGMClientSO)
  {
    class = "BamboomGMClient";
    EventManager_ = "";
  };
}
