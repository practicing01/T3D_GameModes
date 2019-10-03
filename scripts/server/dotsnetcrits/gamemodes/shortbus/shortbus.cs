function ShortbusTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj == %trigger.parent_.shortbus_)
  {
    %player = %obj.getMountedObject(0);
    Game.incScore(%player.client, 10, false);

    %trigger.parent_.SpawnTrigger();
  }
}

function ShortbusClass::UseObject(%this, %player)
{
  %driver = %this.getMountedObject(0);

  if (isObject(%driver))
  {
    Game.incScore(%driver.client, -10, false);
    %this.unmountObject(%driver);
    %driver.mVehicle = "";
    commandToClient(%driver.client, 'toggleVehicleMap', false);
    commandToClient(%driver.client, 'ReloadBinds');

    Game.incScore(%player.client, 1, false);
  }

  %this.mountObject(%player, 0);
  %player.mVehicle = %this;
  commandToClient(%player.client, 'toggleVehicleMap', true);
}

function ShortbusGMServer::SpawnShortbus(%this)
{
  if (isObject(%this.shortbus_))
  {
    %this.shortbus_.delete();
  }

  %this.shortbus_ = new HoverVehicle()
  {
     dataBlock = "ShortbusHVD";
     class = "ShortbusClass";
     parent_ = %this;
  };

  %player = ClientGroup.getRandom().getControlObject();

  %forward = %player.getForwardVector();
  %projection = VectorNormalize(VectorAdd(%forward, "0 0 1"));

  %this.shortbus_.position = DNCServer.ProjectPos(%projection, 5, 0, %this.rayMask_, %player);
}

function ShortbusGMServer::SpawnTrigger(%this)
{
  if (!isObject(%this.goalTrigger_))
  {
    %this.goalTrigger_ = new Trigger()
    {
      dataBlock = "ShortbusTrigger";
      polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
      scale = "10 10 10";
      parent_ = %this;
    };
  }

  %spawnPoint = PlayerDropPoints.getRandom();

  %this.goalTrigger_.position = %spawnPoint.position;
  %this.goalTrigger_.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, %this.rayMask_, %this.goalTrigger_);
  %this.busStop_.position = %this.goalTrigger_.position;
}

function ShortbusGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ShortbusGMServerQueue";

  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.busStop_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/gamemodes/shortbus/busStop.dae";
  };

  %this.SpawnShortbus();
  %this.SpawnTrigger();
}

function ShortbusGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  %driver = %this.shortbus_.getMountedObject(0);

  if (isObject(%driver))
  {
    %this.shortbus_.unmountObject(%driver);
    %driver.mVehicle = "";    
    commandToClient(%driver.client, 'toggleVehicleMap', false);
    commandToClient(%driver.client, 'ReloadBinds');
  }

  if (isObject(%this.shortbus_))
  {
    %this.shortbus_.delete();
  }

  if (isObject(%this.goalTrigger_))
  {
    %this.goalTrigger_.delete();
  }

  if (isObject(%this.busStop_))
  {
    %this.busStop_.delete();
  }
}

function ShortbusClass::onDisabled(%this, %obj, %state)
{
  if (isObject(ShortbusGMServerSO))
  {
    %obj.parent_.schedule(1000, "SpawnShortbus");
  }
}

function ShortbusGMServer::loadOut(%this, %player)
{
  //
}

if (isObject(ShortbusGMServerSO))
{
  ShortbusGMServerSO.delete();
}
else
{
  new ScriptObject(ShortbusGMServerSO)
  {
    class = "ShortbusGMServer";
    EventManager_ = "";
    shortbus_ = "";
    rayMask_ = "";
    goalTrigger_ = "";
    busStop_ = "";
  };

  DNCServer.loadOutListeners_.add(ShortbusGMServerSO);
  MissionCleanup.add(ShortbusGMServerSO);
}
