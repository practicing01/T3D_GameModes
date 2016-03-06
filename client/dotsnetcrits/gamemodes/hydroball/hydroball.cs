function HydroballGMClient::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.sphereCastRadius_ = 3.0;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HydroballGMClientQueue";

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "e", "", %this @ ".BallAction();");

  %this.actionMap_.push();

}

function HydroballGMClient::onRemove(%this)
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
  echo("HydroballGMClient go bye bye");
}

function HydroballGMClient::BallAction(%this)
{
  %obj = ServerConnection.getControlObject();

  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    if(%targetObject.getName() $= "hydroball" || %targetObject.getName() $= "hydroballDummy")
    {
      commandToServer('BallActionHydroballGM');
      break;
    }
  }
}

if (isObject(HydroballGMClientSO))
{
  HydroballGMClientSO.delete();
}
else
{
  new ScriptObject(HydroballGMClientSO)
  {
    class = "HydroballGMClient";
    EventManager_ = "";
    sphereCastRadius_ = "";
  };
}
