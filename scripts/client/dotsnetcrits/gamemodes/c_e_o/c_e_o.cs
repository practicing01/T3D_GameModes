function C_E_OGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "C_E_OGMClientQueue";

}

function C_E_OGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("C_E_OGMClient go bye bye");
}

if (isObject(C_E_OGMClientSO))
{
  C_E_OGMClientSO.delete();
}
else
{
  new ScriptObject(C_E_OGMClientSO)
  {
    class = "C_E_OGMClient";
    EventManager_ = "";
  };
}
