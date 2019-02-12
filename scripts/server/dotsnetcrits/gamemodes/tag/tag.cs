function TagGMServer::SetTheOne(%this)
{
  if (isObject(DNCServer.TeamChooser_))
  {
    for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
    {
      %client = DNCServer.TeamChooser_.teamB_.getObject(%x);

      %data = new ArrayObject();
      %data.add("client", %client);
      %data.add("team", 0);
      DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);

      %data.delete();
    }
  }

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);

    if (!DNCServer.TeamChooser_.teamA_.isMember(%client))
    {
      %data = new ArrayObject();
      %data.add("client", %client);
      %data.add("team", 0);
      DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);

      %data.delete();
    }
  }

  %theOne = ClientGroup.getRandom();

  %data = new ArrayObject();
  %data.add("client", %theOne);
  %data.add("team", 1);
  DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);

  %data.delete();

  %theOne.getControlObject().playAudio(0, tagOneChosenSound);
}

function TagGMServer::onAdd(%this)
{
  %this.sphereCastRadius_ = 3.0;

  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "TagGMServerQueue";

  %this.SetTheOne();

  DNCServer.loadOutListeners_.add(%this);
}

function TagGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

}

function TagGMServer::TagAction(%this, %client)
{
  if (DNCServer.TeamChooser_.teamA_.isMember(%client))
  {
    return;
  }

  %obj = %client.getControlObject();
  %pos = %obj.getPosition();

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    if(%targetObject.getClassName() $= "Player")
    {
      if (%targetObject != %obj)
      {
        if (DNCServer.TeamChooser_.teamA_.isMember(%targetObject.client))
        {
          %obj.playAudio(0, tagTouchSound);

          if (DNCServer.TeamChooser_.teamA_.getCount() == 1)
          {
            %this.SetTheOne();
            return;
          }

          %data = new ArrayObject();
          %data.add("client", %targetObject.client);
          %data.add("team", 1);
          DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);

          %data.delete();
        }
      }
    }
  }

}

function serverCmdTagActionTagGM(%client)
{
  if (isObject(TagGMServerSO))
  {
    TagGMServerSO.TagAction(%client);
  }
}

function TagGMServer::loadOut(%this, %player)
{
  commandToClient(%player.client, 'ReloadActionMapTagGM', false);
}

if (isObject(TagGMServerSO))
{
  TagGMServerSO.delete();
}
else
{
  new ScriptObject(TagGMServerSO)
  {
    class = "TagGMServer";
    EventManager_ = "";
    actionMap_ = "";
    sphereCastRadius_ = "";
  };
}
