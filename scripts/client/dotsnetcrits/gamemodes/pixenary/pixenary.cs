function PixenaryGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PixenaryGMClientQueue";
}

function PixenaryGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("PixenaryGMClient go bye bye");
}

if (isObject(PixenaryGMClientSO))
{
  PixenaryGMClientSO.delete();
}
else
{
  new ScriptObject(PixenaryGMClientSO)
  {
    class = "PixenaryGMClient";
    EventManager_ = "";
  };
}
