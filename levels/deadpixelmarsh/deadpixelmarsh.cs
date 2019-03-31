//return;
datablock PlayerData(DPMAI : DemoPlayer)
{
  //
};

function DPMAI::onRemove(%this, %npc)
{
  cancel(%npc.moveSchedule_);

  if (isObject(%npc.pathSet_))
  {
    %npc.pathSet_.delete();
  }

  if (isObject(%npc.emitter_))
  {
    %npc.emitter_.delete();
  }
}

function DPMAI::onDisabled(%this, %npc, %state)
{
  %npc.setDamageLevel(0);
  %npc.setDamageState("Enabled");
  %npc.setScale("1 1 1");
  %npc.moveSchedule_ = %npc.schedule(1000, "setDest");
}

function DPMAI::onReachDestination(%this, %npc)
{
  if (%npc.pathSet_.getCount())
  {
    %prevTile = %npc.pathSet_.getObject(0);
    %npc.pathSet_.remove(%prevTile);
  }

  if (%npc.pathSet_.getCount())
  {
    %nextTile = %npc.pathSet_.getObject(0);
    %npc.setMoveDestination(%nextTile.position_);
  }
  else
  {
    %npc.moveSchedule_ = %npc.schedule(1000, "setDest");
  }
}

function DPMAI::onMoveStuck(%this, %npc)
{
  %npc.moveSchedule_ = %npc.schedule(1000, "setDest");
}

function DPMAIClass::setDest(%this)
{
  %startTile = %this.parent_.groundTiles_.getRandom();
  %endTile = %this.parent_.groundTiles_.getRandom();
  %pathSet = %this.parent_.gimpStar_.GetPath(%this.parent_.groundTiles_, %startTile, %endTile);

  if (%pathSet == -1)
  {
    %this.moveSchedule_ = %this.schedule(1000, "setDest");
    return;
  }
  else
  {
    if (isObject(%this.pathSet_))
    {
      %this.pathSet_.delete();
    }

    if (isObject(%this.startEmitter_))
    {
      %this.startEmitter_.delete();
    }

    if (isObject(%this.endEmitter_))
    {
      %this.endEmitter_.delete();
    }

    if (%pathSet.getCount() == 1)
    {
      %pathSet.delete();
      %this.moveSchedule_ = %this.schedule(1000, "setDest");
      return;
    }

    %this.pathSet_ = %pathSet;
    %this.position = %startTile.position_.x SPC %startTile.position_.y SPC %this.position.z;
    %nextTile = %this.pathSet_.getObject(0);
    %this.setMoveDestination(%nextTile.position_);

    %this.startEmitter_ = new ParticleEmitterNode()
    {
      datablock = TorchFireEmitterNode;
      emitter = TorchFireEmitter;
      active = true;
      position = %startTile.position_.x SPC %startTile.position_.y SPC %this.position.z;
    };

    MissionCleanup.add(%this.startEmitter_);

    %this.endEmitter_ = new ParticleEmitterNode()
    {
      datablock = TorchFireEmitterNode;
      emitter = TorchFireEmitter;
      active = true;
      position = %endTile.position_.x SPC %endTile.position_.y SPC %this.position.z;
    };

    MissionCleanup.add(%this.endEmitter_);
  }
}

function DPMSOClass::SpawnGhost(%this, %parent)
{
  %npc = new AiPlayer()
  {
    dataBlock = DPMAI;
    class = "DPMAIClass";
    moveSchedule_ = "";
    pathSet_ = "";
    emitter_ = "";
    startEmitter_ = "";
    endEmitter_ = "";
    parent_ = %parent;
  };

  MissionCleanup.add(%npc);

  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%npc, %spawnPoint);
  %npc.setTransform(%transform);

  %npc.setCloaked(true);

  %emitter = new ParticleEmitterNode()
  {
    datablock = TorchFireEmitterNode;
    emitter = TorchFireEmitter;
    active = true;
  };

  MissionCleanup.add(%emitter);
  %npc.emitter_ = %emitter;

  %npc.mountObject(%emitter, 0, MatrixCreate("0 0 0", "1 0 0 0"));

  %npc.moveSchedule_ = %npc.schedule(1000, "setDest");
}

function DPMSOClass::ConnectTile(%this, %tile, %disconnect)
{
  %x = %tile.position_.x / %this.tileOffset_;
  %y = %tile.position_.y / %this.tileOffset_;
  %gridPos = %x SPC %y;

  for (%y = -1; %y < 2; %y++)
  {
    for (%x = -1; %x < 2; %x++)
    {
      if (%x == 0 && %y == 0)
      {
        continue;
      }

      if (%gridPos.x + %x < 0 || %gridPos.y + %y < 0)
      {
        continue;
      }

      if (%gridPos.x + %x >= %this.gridSize_.x || %gridPos.y + %y >= %this.gridSize_.y)
      {
        continue;
      }

      %connectedX = %gridPos.x + %x;
      %connectedY = %gridPos.y + %y;

      %setIndex = (%this.gridSize_.x * %connectedY) + %connectedX;
      %connectedTile = %this.tileGrid_.getObject(%setIndex);

      if (%disconnect == false)
      {
        %tile.connectedTiles_.add(%connectedTile);
      }
      else
      {
        if (%tile.connectedTiles_.isMember(%connectedTile))
        {
          %tile.connectedTiles_.remove(%connectedTile);
        }
        if (%connectedTile.connectedTiles_.isMember(%tile))
        {
          %connectedTile.connectedTiles_.remove(%tile);
        }
      }
    }
  }
}

function DPMSOClass::onAdd(%this)
{
  %this.gridSize_ = "20 20";
  %this.tileOffset_ = 10;

  DNCServer.missionLoadedListeners_.add(%this);
}

function DPMSOClass::onRemove(%this)
{
  DNCServer.missionLoadedListeners_.remove(%this);

  for (%x = 0; %x < %this.tileGrid_.getCount(); %x++)
  {
    %tile = %this.tileGrid_.getObject(%x);
    %tile.connectedTiles_.delete();
  }

  %this.tileGrid_.deleteAllObjects();
  %this.tileGrid_.delete();
  %this.groundTiles_.delete();

  %this.gimpStar_.delete();
}

function DPMSOClass::onMissionLoaded(%this, %game)
{
  %this.gimpStar_ = new ScriptObject()
  {
    class = "GimpStar";
  };
  MissionCleanup.add(%this.gimpStar_);

  %this.tileGrid_ = new SimSet();

  for (%y = 0; %y < %this.gridSize_.y; %y++)
  {
    for (%x = 0; %x < %this.gridSize_.x; %x++)
    {
      %tile = new ScriptObject()
      {
        position_ = (%x * %this.tileOffset_) SPC (%y * %this.tileOffset_) SPC "1";
        distanceToGoal_ = 0;
        connectedTiles_ = new SimSet();
        deadend_ = false;
        unused_ = true;
      };

      %this.tileGrid_.add(%tile);
    }
  }

  for (%x = 0; %x < %this.tileGrid_.getCount(); %x++)
  {
    %this.ConnectTile(%this.tileGrid_.getObject(%x), false);
  }

  %this.groundTiles_ = new SimSet();

  %count = %this.tileGrid_.getCount();
  %count *= 0.5;

  for (%x = 0; %x < %count; %x++)
  {
    %randyTile = %this.tileGrid_.getRandom();
    %this.groundTiles_.add(%randyTile);
    %randyTile.unused_ = false;

    %groundMesh = new TSStatic()
    {
      shapeName = "art/shapes/dotsnetcrits/levels/deadpixelmarsh/ground.dae";
      collisionType = "Visible Mesh";
      decalType = "Visible Mesh";
      allowPlayerStep = "1";
      parent_ = %this;
      position = %randyTile.position_;
    };

    MissionCleanup.add(%groundMesh);

    %spawnSphere = new SpawnSphere() {
       autoSpawn = "0";
       spawnTransform = "0";
       radius = "5";
       sphereWeight = "1";
       indoorWeight = "1";
       outdoorWeight = "1";
       isAIControlled = "0";
       dataBlock = "SpawnSphereMarker";
       position = %randyTile.position_.x SPC %randyTile.position_.y SPC "2";
       rotation = "1 0 0 0";
       scale = "1 1 1";
       canSave = "1";
       canSaveDynamicFields = "1";
          enabled = "1";
          homingCount = "0";
          lockCount = "0";
    };

    MissionCleanup.add(%spawnSphere);
    PlayerDropPoints.add(%spawnSphere);
  }

  %count = %this.groundTiles_.getCount();
  %count *= 0.05;

  for (%x = 0; %x < %count; %x++)
  {
    %randyTile = %this.groundTiles_.getRandom();
    %randyTile.unused_ = false;

    %treeMesh = new TSStatic()
    {
      shapeName = "art/shapes/dotsnetcrits/levels/deadpixelmarsh/tree.dae";
      collisionType = "None";//"Collision Mesh";
      decalType = "None";//"Collision Mesh";
      //allowPlayerStep = "1";
      parent_ = %this;
      position = %randyTile.position_;
    };

    MissionCleanup.add(%treeMesh);
  }

  for (%x = 0; %x < %this.tileGrid_.getCount(); %x++)
  {
    %tile = %this.tileGrid_.getObject(%x);
    if (%tile.unused_ == true)
    {
      %this.ConnectTile(%tile, true);
    }
  }

  %this.SpawnGhost(%this);
}
