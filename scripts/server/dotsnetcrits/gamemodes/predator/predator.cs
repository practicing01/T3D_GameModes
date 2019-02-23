function PredatorGMServer::SetTheOne(%this)
{
  if (isObject(%this.theOne_))
  {
    %this.theOne_.getControlObject().setShapeName(%this.theOnesName_);
  }

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %client.getControlObject().setCloaked(false);
  }

  %this.theOne_ = ClientGroup.getRandom();
  %this.theOne_.getControlObject().setCloaked(true);
  %this.theOnesName_ = %this.theOne_.getControlObject().getShapeName();
  %this.theOne_.getControlObject().setShapeName("");
}

function PredatorGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PredatorGMServerQueue";

  %this.SetTheOne();

}

function PredatorGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.theOne_))
  {
    %this.theOne_.getControlObject().setShapeName(%this.theOnesName_);
    %this.theOne_.getControlObject().setCloaked(false);
  }
}

function PredatorGMServer::loadOut(%this, %player)
{
  if (%player.client == %this.theOne_)
  {
    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);

      if (%client != %this.theOne_)
      {
        Game.incScore(%client, 1, false);
      }
    }

    %this.SetTheOne();
  }
  else
  {
    Game.incScore(%this.theOne_, 1, false);
  }

}

if (isObject(PredatorGMServerSO))
{
  PredatorGMServerSO.delete();
}
else
{
  new ScriptObject(PredatorGMServerSO)
  {
    class = "PredatorGMServer";
    EventManager_ = "";
    theOne_ = "";
    theOnesName_ = "";
  };

  DNCServer.loadOutListeners_.add(PredatorGMServerSO);
  MissionCleanup.add(PredatorGMServerSO);
}
