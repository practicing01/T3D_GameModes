function MommaBirdGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "MommaBirdGMClientQueue";

}

function MommaBirdGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("MommaBirdGMClient go bye bye");
}

if (isObject(MommaBirdGMClientSO))
{
  MommaBirdGMClientSO.delete();
}
else
{
  new ScriptObject(MommaBirdGMClientSO)
  {
    class = "MommaBirdGMClient";
    EventManager_ = "";
  };
}
