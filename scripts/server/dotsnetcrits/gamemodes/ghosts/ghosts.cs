function GhostsGMServer::Ghostify(%this, %player)
{
  %player.setCloaked(true);
}

function GhostsGMServer::UnGhostify(%this, %player)
{
  %player.setCloaked(false);
}

function GhostsGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "GhostsGMServerQueue";

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %obj = ClientGroup.getObject(%x).getControlObject();

    %this.Ghostify(%obj);
  }
}

function GhostsGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %obj = ClientGroup.getObject(%x).getControlObject();

    %this.UnGhostify(%obj);
  }
}

function GhostsGMServer::loadOut(%this, %player)
{
  %this.Ghostify(%player);
}

function GhostsGMServer::incKills(%this, %client, %kill)
{
  %this.UnGhostify(%client.player);

  %this.schedule(2000, "Ghostify", %client.player);
}

if (isObject(GhostsGMServerSO))
{
  GhostsGMServerSO.delete();
}
else
{
  new ScriptObject(GhostsGMServerSO)
  {
    class = "GhostsGMServer";
    EventManager_ = "";
  };

  DNCServer.loadOutListeners_.add(GhostsGMServerSO);
  DNCServer.incKillsListeners_.add(GhostsGMServerSO);
  MissionCleanup.add(GhostsGMServerSO);
}
