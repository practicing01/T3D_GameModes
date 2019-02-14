function C_C_OGMServer::Promote(%this)
{
  %this.ceoClient_ = ClientGroup.getRandom();

  %player = %this.ceoClient_.getControlObject();

  %player.scale = VectorScale(%player.scale, 0.1);

  %player.setRepairRate(0.5);
}

function C_C_OGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "C_C_OGMServerQueue";

  %this.Promote();
}

function C_C_OGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  %player = %this.ceoClient_.getControlObject();

  if (isObject(%player))
  {
    %player.scale = "1 1 1";
    %player.setRepairRate(0.0033);
  }
}

function C_C_OGMServer::loadOut(%this, %player)
{
  if (%player.client == %this.ceoClient_)
  {
    //%player.scale = "1 1 1";
    //%player.setRepairRate(0.0033);
    %this.Promote();
  }
}

if (isObject(C_C_OGMServerSO))
{
  C_C_OGMServerSO.delete();
}
else
{
  new ScriptObject(C_C_OGMServerSO)
  {
    class = "C_C_OGMServer";
    EventManager_ = "";
    ceoClient_ = "";
  };

  DNCServer.loadOutListeners_.add(C_C_OGMServerSO);
  MissionCleanup.add(C_C_OGMServerSO);
}
