function PointAtoBGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PointAtoBGMClientQueue";

}

function PointAtoBGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("PointAtoBGMClient go bye bye");
}

if (isObject(PointAtoBGMClientSO))
{
  PointAtoBGMClientSO.delete();
}
else
{
  new ScriptObject(PointAtoBGMClientSO)
  {
    class = "PointAtoBGMClient";
    EventManager_ = "";
  };
}
