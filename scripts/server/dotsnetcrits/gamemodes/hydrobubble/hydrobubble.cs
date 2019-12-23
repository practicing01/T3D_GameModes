function HydrobubbleGMServer::RemoveBall(%this, %player)
{
  %image = bubbleball.getFieldValue("image");

  if (%image.isField("ammo"))
  {
    %player.setInventory(%image.getFieldValue("ammo"), 0);
  }

  if (%image.isField("clip"))
  {
    %player.setInventory(%image.getFieldValue("clip"), 0);
  }
}

function HydrobubbleGMServer::SpawnBubbleball(%this, %pos)
{
  %bubbleball = new Item() {
     static = "1";
     rotate = "1";
     dataBlock = "bubbleballAmmo";
     position = %pos;
     scale = "1 1 1";
  };

  MissionCleanup.add(%bubbleball);
}

function HydrobubbleGoalClass::onCollision(%this, %collObj, %vec, %len)
{
  if (%collObj.dataBlock $= "bubbleballBulletProjectile")
  {
    %player = %collObj.sourceObject;

    if (DNCServer.TeamChooser_.teamA_.isMember(%player.client))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
        Game.incScore(%client, 10, false);
      }
    }
    else if (DNCServer.TeamChooser_.teamB_.isMember(%player.client))
    {
      for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
      {
        %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
        Game.incScore(%client, 10, false);
      }
    }
    else
    {
      Game.incScore(%player.client, 10, false);
    }

    %spawnPoint = PlayerDropPoints.getRandom();
    %transform = GameCore::pickPointInSpawnSphere(%this, %spawnPoint);
    %this.setTransform(%transform);
  }
}

function HydrobubbleGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HydrobubbleGMServerQueue";

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %obj = ClientGroup.getObject(%x).getControlObject();

    %this.RemoveBall(%obj);
  }

  %this.goal_ = new StaticShape()
  {
     dataBlock = "HydrobubbleGoalStaticShapeData";
     class = "HydrobubbleGoalClass";
     parent_ = %this;
  };

  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%this.goal_, %spawnPoint);
  %this.goal_.setTransform(%transform);

  %spawnPoint = PlayerDropPoints.getRandom();
  %this.SpawnBubbleball(%spawnPoint.position);
}

function HydrobubbleGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.goal_))
  {
    %this.goal_.delete();
  }
}

function HydrobubbleGMServer::loadOut(%this, %player)
{
  %this.RemoveBall(%player);
}

function HydrobubbleGMServer::onDeath(%this, %game, %client, %sourceObject, %sourceClient, %damageType, %damLoc)
{
  %player = %client.getControlObject();

  if (!%player.hasInventory(bubbleballAmmo))
  {
    return;
  }

  %this.SpawnBubbleball(%player.position);
}

if (isObject(HydrobubbleGMServerSO))
{
  HydrobubbleGMServerSO.delete();
}
else
{
  new ScriptObject(HydrobubbleGMServerSO)
  {
    class = "HydrobubbleGMServer";
    EventManager_ = "";
    goal_ = "";
  };

  DNCServer.deathListeners_.add(HydrobubbleGMServerSO);
  DNCServer.loadOutListeners_.add(HydrobubbleGMServerSO);
  MissionCleanup.add(HydrobubbleGMServerSO);
}
