function AllvOneGMServer::SetTheOne(%this)
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

  %this.theOne_ = ClientGroup.getRandom();

  %data = new ArrayObject();
  %data.add("client", %this.theOne_);
  %data.add("team", 1);
  DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);

  %data.delete();
}

function AllvOneGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "AllvOneGMServerQueue";

  %this.SetTheOne();

}

function AllvOneGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

}

function AllvOneGMServer::loadOut(%this, %player)
{
  if (isObject(DNCServer.TeamChooser_))
  {
    if (DNCServer.TeamChooser_.teamA_.isMember(%player.client))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
        Game.incScore(%client, 1, false);
      }
    }
    else if (DNCServer.TeamChooser_.teamB_.isMember(%player.client))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
        Game.incScore(%client, 1, false);
      }

      %this.SetTheOne();
    }
  }

}

if (isObject(AllvOneGMServerSO))
{
  AllvOneGMServerSO.delete();
}
else
{
  %gmSO = new ScriptObject()
  {
    class = "AllvOneGMServer";
  };

  DNCServer.loadOutListeners_.add(%gmSO);
  MissionCleanup.add(%gmSO);

  new ScriptObject(AllvOneGMServerSO)
  {
    class = "AllvOneGMServer";
    EventManager_ = "";
    theOne_ = "";
  };
}
