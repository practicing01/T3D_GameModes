function MerbillyGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "MerbillyGMClientQueue";

}

function MerbillyGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("MerbillyGMClient go bye bye");
}

if (isObject(MerbillyGMClientSO))
{
  MerbillyGMClientSO.delete();
}
else
{
  new ScriptObject(MerbillyGMClientSO)
  {
    class = "MerbillyGMClient";
    EventManager_ = "";
  };
}
