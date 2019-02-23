function AmmolessGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "AmmolessGMClientQueue";

}

function AmmolessGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("AmmolessGMClient go bye bye");
}

if (isObject(AmmolessGMClientSO))
{
  AmmolessGMClientSO.delete();
}
else
{
  new ScriptObject(AmmolessGMClientSO)
  {
    class = "AmmolessGMClient";
    EventManager_ = "";
  };
}
