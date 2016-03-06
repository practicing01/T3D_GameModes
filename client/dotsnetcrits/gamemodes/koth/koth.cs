function KOTHGMClient::onAdd(%this)
{
  MissionCleanup.add(%this);
  
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "KOTHGMClientQueue";

}

function KOTHGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("KOTHGMClient go bye bye");
}

if (isObject(KOTHGMClientSO))
{
  KOTHGMClientSO.delete();
}
else
{
  new ScriptObject(KOTHGMClientSO)
  {
    class = "KOTHGMClient";
    EventManager_ = "";
  };
}
