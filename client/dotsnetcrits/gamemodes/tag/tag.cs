function TagGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.sphereCastRadius_ = 3.0;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "TagGMClientQueue";

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "e", "", %this @ ".TagAction();");

  %this.actionMap_.push();

}

function TagGMClient::onRemove(%this)
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
  echo("TagGMClient go bye bye");
}

function TagGMClient::TagAction(%this)
{
  commandToServer('TagActionTagGM');
  return;
  //todo figure out how to get datablock name of an object on the ClientGroup

  %obj = ServerConnection.getControlObject();

  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType, true);

  while ( (%targetObject = containerSearchNext(true)) != 0 )
  {
    if(%targetObject.getName() $= "Tag")
    {
      commandToServer('TagActionTagGM');
      break;
    }
  }
}

if (isObject(TagGMClientSO))
{
  TagGMClientSO.delete();
}
else
{
  new ScriptObject(TagGMClientSO)
  {
    class = "TagGMClient";
    EventManager_ = "";
    sphereCastRadius_ = "";
  };
}
