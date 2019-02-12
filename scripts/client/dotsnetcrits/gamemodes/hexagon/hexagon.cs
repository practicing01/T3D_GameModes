function HexagonGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HexagonGMClientQueue";

}

function HexagonGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("HexagonGMClient go bye bye");
}

if (isObject(HexagonGMClientSO))
{
  HexagonGMClientSO.delete();
}
else
{
  new ScriptObject(HexagonGMClientSO)
  {
    class = "HexagonGMClient";
    EventManager_ = "";
  };
}
