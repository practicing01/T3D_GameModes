function HostageRescueGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.sphereCastRadius_ = 3.0;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HostageRescueGMClientQueue";

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "e", "", %this @ ".HostageAction();");

  %this.actionMap_.push();

}

function HostageRescueGMClient::onRemove(%this)
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
  echo("HostageRescueGMClient go bye bye");
}

function HostageRescueGMClient::HostageAction(%this)
{
  commandToServer('HostageActionHostageRescueGM');
  return;
  //todo figure out how to get datablock name of an object on the ClientGroup

  %obj = ServerConnection.getControlObject();

  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType, true);

  while ( (%targetObject = containerSearchNext(true)) != 0 )
  {
    if(%targetObject.getName() $= "Hostage")
    {
      commandToServer('HostageActionHostageRescueGM');
      break;
    }
  }
}

function clientCmdReloadActionMapHostageRescueGM()
{
  if (isObject(HostageRescueGMClientSO.actionMap_))
  {
    HostageRescueGMClientSO.actionMap_.push();
  }
}

if (isObject(HostageRescueGMClientSO))
{
  HostageRescueGMClientSO.delete();
}
else
{
  new ScriptObject(HostageRescueGMClientSO)
  {
    class = "HostageRescueGMClient";
    EventManager_ = "";
    sphereCastRadius_ = "";
    actionMap_ = "";
  };
}
