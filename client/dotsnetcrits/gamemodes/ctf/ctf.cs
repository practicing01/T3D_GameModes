function CTFGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.sphereCastRadius_ = 3.0;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "CTFGMClientQueue";

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "e", "", %this @ ".CTFAction();");

  %this.actionMap_.push();

}

function CTFGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.pop();
    %this.actionMap_.delete();
  }
  echo("CTFGMClient go bye bye");
}

function CTFGMClient::CTFAction(%this)
{
  commandToServer('CTFActionCTFGM');
  return;
  //todo figure out how to get datablock name of an object on the ClientGroup

  %obj = ServerConnection.getControlObject();

  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType, true);

  while ( (%targetObject = containerSearchNext(true)) != 0 )
  {
    if(%targetObject.getName() $= "CTF")
    {
      commandToServer('CTFActionCTFGM');
      break;
    }
  }
}

if (isObject(CTFGMClientSO))
{
  CTFGMClientSO.delete();
}
else
{
  new ScriptObject(CTFGMClientSO)
  {
    class = "CTFGMClient";
    EventManager_ = "";
    sphereCastRadius_ = "";
    actionMap_ = "";
  };
}
