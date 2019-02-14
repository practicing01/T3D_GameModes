function ScorpionHatGMServer::RandySpawnTrans(%this, %obj)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%obj, %spawnPoint);
  %obj.setTransform(%transform);
}

function ScorpionHatGMServer::SetTheOne(%this)
{
  %this.theOne_ = ClientGroup.getRandom();
  %player = %this.theOne_.getControlObject();
  %player.mountObject(%this.hat_, 1, MatrixCreate("0 0 2", "1 0 0 0"));
  %this.RandySpawnTrans(%this.pyramid_);
}

function ScorpionHatPyramidClass::UseObject(%this, %player)
{
  if (!%this.parent_.hat_.isMounted())
  {
    return;
  }

  %scorpionMount = %this.parent_.hat_.getObjectMount();

  if (%player == %scorpionMount)
  {
    Game.incScore(%player.client, 10, false);

    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);

      %score = Game.getScore(%client);
      %score = %score * -1;

      Game.incScore(%client, %score, false);
    }

    %this.parent_.hat_.unmount();

    %this.parent_.SetTheOne();
  }
}

function ScorpionHatGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ScorpionHatGMServerQueue";

  %this.hat_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/gamemodes/scorpionhat/scorpion.dae";
    collisionType = "none";
    parent_ = %this;
  };

  %this.pyramid_ = new StaticShape()
  {
     dataBlock = "ScorpionHatPyramidStaticShapeData";
     class = "ScorpionHatPyramidClass";
     parent_ = %this;
  };

  %this.SetTheOne();
}

function ScorpionHatGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.hat_))
  {
    %this.hat_.unmount();
    %this.hat_.delete();
  }

  if (isObject(%this.pyramid_))
  {
    %this.pyramid_.delete();
  }

}

function ScorpionHatGMServer::loadOut(%this, %player)
{
  if (%player.client == %this.theOne_)
  {
    %score = Game.getScore(%player.client);
    %score = %score * -1;
    Game.incScore(%player.client, %score, false);

    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      Game.incScore(%client, 10, false);
    }

    %this.SetTheOne();
  }
}

if (isObject(ScorpionHatGMServerSO))
{
  ScorpionHatGMServerSO.delete();
}
else
{
  new ScriptObject(ScorpionHatGMServerSO)
  {
    class = "ScorpionHatGMServer";
    EventManager_ = "";
    theOne_ = "";
    hat_ = "";
    pyramid_ = "";
  };

  DNCServer.loadOutListeners_.add(ScorpionHatGMServerSO);
  MissionCleanup.add(ScorpionHatGMServerSO);
}
