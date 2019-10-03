function HellwashGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HellwashGMClientQueue";

}

function HellwashGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("HellwashGMClient go bye bye");
}

if (isObject(HellwashGMClientSO))
{
  HellwashGMClientSO.delete();
}
else
{
  new ScriptObject(HellwashGMClientSO)
  {
    class = "HellwashGMClient";
    EventManager_ = "";
  };
}
