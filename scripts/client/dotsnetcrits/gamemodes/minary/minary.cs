function MinaryGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "MinaryGMClientQueue";

}

function MinaryGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("MinaryGMClient go bye bye");
}

if (isObject(MinaryGMClientSO))
{
  MinaryGMClientSO.delete();
}
else
{
  new ScriptObject(MinaryGMClientSO)
  {
    class = "MinaryGMClient";
    EventManager_ = "";
  };
}
