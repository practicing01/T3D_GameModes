function DiffusalGMClient::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.sphereCastRadius_ = 3.0;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "DiffusalGMClientQueue";

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "e", "", %this @ ".BombAction();");

  %this.actionMap_.push();

}

function DiffusalGMClient::onRemove(%this)
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
  echo("DiffusalGMClient go bye bye");
}

function DiffusalGMClient::BombAction(%this)
{
  %obj = ServerConnection.getControlObject();

  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    if(%targetObject.getName() $= "bomb")
    {
      commandToServer('BombActionDiffusalGM');
      break;
    }
  }
}

if (isObject(DiffusalGMClientSO))
{
  DiffusalGMClientSO.delete();
}
else
{
  new ScriptObject(DiffusalGMClientSO)
  {
    class = "DiffusalGMClient";
    EventManager_ = "";
    sphereCastRadius_ = "";
  };
}
