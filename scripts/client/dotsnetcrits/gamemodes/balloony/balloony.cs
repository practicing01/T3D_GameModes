function BalloonyGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BalloonyGMClientQueue";

}

function BalloonyGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("BalloonyGMClient go bye bye");
}

if (isObject(BalloonyGMClientSO))
{
  BalloonyGMClientSO.delete();
}
else
{
  new ScriptObject(BalloonyGMClientSO)
  {
    class = "BalloonyGMClient";
    EventManager_ = "";
  };
}
