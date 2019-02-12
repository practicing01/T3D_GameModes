function ZombieWavesGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ZombieWavesGMClientQueue";

}

function ZombieWavesGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  echo("ZombieWavesGMClient go bye bye");
}

if (isObject(ZombieWavesGMClientSO))
{
  ZombieWavesGMClientSO.delete();
}
else
{
  new ScriptObject(ZombieWavesGMClientSO)
  {
    class = "ZombieWavesGMClient";
    EventManager_ = "";
  };
}
