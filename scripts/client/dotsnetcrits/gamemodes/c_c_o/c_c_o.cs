function C_C_OGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "C_C_OGMClientQueue";

}

function C_C_OGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("C_C_OGMClient go bye bye");
}

if (isObject(C_C_OGMClientSO))
{
  C_C_OGMClientSO.delete();
}
else
{
  new ScriptObject(C_C_OGMClientSO)
  {
    class = "C_C_OGMClient";
    EventManager_ = "";
  };
}
