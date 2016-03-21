function ALLVONEGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ALLVONEGMClientQueue";

}

function ALLVONEGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("ALLVONEGMClient go bye bye");
}

if (isObject(ALLVONEGMClientSO))
{
  ALLVONEGMClientSO.delete();
}
else
{
  new ScriptObject(ALLVONEGMClientSO)
  {
    class = "ALLVONEGMClient";
    EventManager_ = "";
  };
}
