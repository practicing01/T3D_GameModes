function RastamonSnapGMClient::Fire(%this)
{
  commandToServer('FireRastamonSnapGM');
}

function RastamonSnapGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "RastamonSnapGMClientQueue";

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(mouse0, "button0", "", %this @ ".Fire();");
  %this.actionMap_.push();
}

function RastamonSnapGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.pop();
    %this.actionMap_.delete();

    if (isObject(DNCClient))
    {
      DNCClient.actionMap_.push();
    }
  }

  //hideCursor();
  echo("RastamonSnapGMClient go bye bye");
}

if (isObject(RastamonSnapGMClientSO))
{
  RastamonSnapGMClientSO.delete();
}
else
{
  new ScriptObject(RastamonSnapGMClientSO)
  {
    class = "RastamonSnapGMClient";
    EventManager_ = "";
    actionMap_ = "";
  };
}
