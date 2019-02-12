function ScorpionHatGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ScorpionHatGMClientQueue";

}

function ScorpionHatGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("ScorpionHatGMClient go bye bye");
}

if (isObject(ScorpionHatGMClientSO))
{
  ScorpionHatGMClientSO.delete();
}
else
{
  new ScriptObject(ScorpionHatGMClientSO)
  {
    class = "ScorpionHatGMClient";
    EventManager_ = "";
  };
}
