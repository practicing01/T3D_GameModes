function BloodbeakGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BloodbeakGMClientQueue";
}

function BloodbeakGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("BloodbeakGMClient go bye bye");
}

if (isObject(BloodbeakGMClientSO))
{
  BloodbeakGMClientSO.delete();
}
else
{
  new ScriptObject(BloodbeakGMClientSO)
  {
    class = "BloodbeakGMClient";
    EventManager_ = "";
  };
}
