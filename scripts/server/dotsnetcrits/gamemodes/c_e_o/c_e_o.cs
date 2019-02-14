function C_E_OGMServer::Promote(%this)
{
  %this.ceoClient_ = ClientGroup.getRandom();

  %player = %this.ceoClient_.getControlObject();

  %player.scale = VectorScale(%player.scale, 5);

  %player.setRepairRate(0.5);
}

function C_E_OGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "C_E_OGMServerQueue";

  %this.Promote();
}

function C_E_OGMServer::onRemove(%this)
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

function C_E_OGMServer::loadOut(%this, %player)
{
  if (%player.client == %this.ceoClient_)
  {
    //%player.scale = "1 1 1";
    //%player.setRepairRate(0.0033);
    %this.Promote();
  }
}

if (isObject(C_E_OGMServerSO))
{
  C_E_OGMServerSO.delete();
}
else
{
  new ScriptObject(C_E_OGMServerSO)
  {
    class = "C_E_OGMServer";
    EventManager_ = "";
    ceoClient_ = "";
  };

  DNCServer.loadOutListeners_.add(C_E_OGMServerSO);
  MissionCleanup.add(C_E_OGMServerSO);
}
