function NoirBirdGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "NoirBirdGMClientQueue";

}

function NoirBirdGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("NoirBirdGMClient go bye bye");
}

if (isObject(NoirBirdGMClientSO))
{
  NoirBirdGMClientSO.delete();
}
else
{
  new ScriptObject(NoirBirdGMClientSO)
  {
    class = "NoirBirdGMClient";
    EventManager_ = "";
  };
}
