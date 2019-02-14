function SupplyRunPaperClass::UseObject(%this, %player)
{
  %player.SupplyRunPaperAcquired_ = true;
  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%this, %spawnPoint);
  %this.setTransform(%transform);
}

function SupplyRunPrinterAIClass::UseObject(%this, %player)
{
  if (%player.SupplyRunPaperAcquired_ == true)
  {
    %this.creepsToSpawn_ += 10;
    %player.SupplyRunPaperAcquired_ = false;
    return;
  }

  if (%this.followTarget_ == %player)
  {
    %this.followTarget_ = "";
    %this.stop();
    return;
  }

  %this.followTarget_ = %player;
  %this.followObject(%player, 1);
}

function SupplyRunPrinterAIClass::Hit(%this)
{
  if (%this == SupplyRunGMServerSO.SupplyRunPrinterB_)
  {
    for (%x = 0; %x < DNCServer.TeamChooser_.teamA_.getCount(); %x++)
    {
      %client = DNCServer.TeamChooser_.teamA_.getObject(%x);
      Game.incScore(%client, 5, false);
    }
  }
  else
  {
    for (%x = 0; %x < DNCServer.TeamChooser_.teamB_.getCount(); %x++)
    {
      %client = DNCServer.TeamChooser_.teamB_.getObject(%x);
      Game.incScore(%client, 5, false);
    }
  }
}

function SupplyRunAirplaneAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (%collObj.class $= "SupplyRunPrinterAIClass")
  {
    %collObj.Hit();
    %obj.schedule(0, "delete");
  }
  else if (%collObj.class $= "SupplyRunAirplaneAIClass")
  {
    %obj.schedule(0, "delete");
  }
}

function SupplyRunPrinterAIClass::SpawnAirplane(%this)
{
  if (%this.creepsToSpawn_ > 0)
  {
    %this.creepsToSpawn_--;

    %airplane = new AiPlayer()
    {
       dataBlock = "SupplyRunAirplaneAI";
       class = "SupplyRunAirplaneAIClass";
       state_ = "attack";
       parentPrinter_ = %this;
    };

    MissionCleanup.add(%airplane);

    %pos = %this.getPosition();
    %teleDir = %this.getForwardVector();

    %size = %this.getObjectBox();
    %scale = %this.getScale();
    %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
    %sizex *= 1.5;

    %sizeTarget = %airplane.getObjectBox();
    %scaleTarget = %airplane.getScale();
    %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
    %sizexTarget *= 1.5;

    %airplane.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

    if (%this == SupplyRunGMServerSO.SupplyRunPrinterA_)
    {
      %airplane.followObject(SupplyRunGMServerSO.SupplyRunPrinterB_, 0);
    }
    else
    {
      %airplane.followObject(SupplyRunGMServerSO.SupplyRunPrinterA_, 0);
    }
  }

  %this.spawnSchedule_ = %this.schedule(1000, "SpawnAirplane");
}

function SupplyRunGMServer::SpawnPaper(%this)
{
  %this.paper_ = new StaticShape()
  {
     dataBlock = "SupplyRunPaperStaticShapeData";
     class = "SupplyRunPaperClass";
  };

  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%this.paper_, %spawnPoint);
  %this.paper_.setTransform(%transform);
}

function SupplyRunGMServer::SpawnPrinters(%this)
{
  %this.SupplyRunPrinterA_ = new AiPlayer()
  {
     dataBlock = "SupplyRunPrinterAI";
     class = "SupplyRunPrinterAIClass";
     state_ = "idle";
     followTarget_ = "";
     creepsToSpawn_ = 0;
     spawnSchedule_ = "";
     emitter_ = "";
  };

  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%this.SupplyRunPrinterA_, %spawnPoint);

  %this.SupplyRunPrinterA_.setTransform(%transform);

  %this.SupplyRunPrinterA_.spawnSchedule_ = %this.SupplyRunPrinterA_.schedule(1000, "SpawnAirplane");

  %emitter = new ParticleEmitterNode()
  {
    datablock = teamEmitterNodeData;
    emitter = teamAOutlinerEmitter;
    active = true;
  };

  %this.SupplyRunPrinterA_.emitter_ = %emitter;

  %this.SupplyRunPrinterA_.mountObject(%emitter, 0, MatrixCreate("0 0 1", "1 0 0 0"));

  %this.SupplyRunPrinterB_ = new AiPlayer()
  {
    dataBlock = "SupplyRunPrinterAI";
    class = "SupplyRunPrinterAIClass";
    state_ = "idle";
    followTarget_ = "";
    creepsToSpawn_ = 0;
    spawnSchedule_ = "";
    emitter_ = "";
  };

  %spawnPoint = PlayerDropPoints.getRandom();

  %transform = GameCore::pickPointInSpawnSphere(%this.SupplyRunPrinterB_, %spawnPoint);

  %this.SupplyRunPrinterB_.setTransform(%transform);

  %this.SupplyRunPrinterB_.spawnSchedule_ = %this.SupplyRunPrinterB_.schedule(1000, "SpawnAirplane");

  %emitter = new ParticleEmitterNode()
  {
    datablock = teamEmitterNodeData;
    emitter = teamBOutlinerEmitter;
    active = true;
  };

  %this.SupplyRunPrinterB_.emitter_ = %emitter;

  %this.SupplyRunPrinterB_.mountObject(%emitter, 0, MatrixCreate("0 0 1", "1 0 0 0"));

}

function SupplyRunGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "SupplyRunGMServerQueue";

  %this.SpawnPrinters();
  %this.SpawnPaper();
}

function SupplyRunGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  if (isObject(%this.SupplyRunPrinterA_))
  {
    cancel(%this.SupplyRunPrinterA_.spawnSchedule_);
    %this.SupplyRunPrinterA_.emitter_.delete();
    %this.SupplyRunPrinterA_.delete();
  }
  if (isObject(%this.SupplyRunPrinterB_))
  {
    cancel(%this.SupplyRunPrinterB_.spawnSchedule_);
    %this.SupplyRunPrinterB_.emitter_.delete();
    %this.SupplyRunPrinterB_.delete();
  }
  if (isObject(%this.paper_))
  {
    %this.paper_.delete();
  }
}

function SupplyRunGMServer::loadOut(%this, %player)
{
  //
}

if (isObject(SupplyRunGMServerSO))
{
  SupplyRunGMServerSO.delete();
}
else
{
  new ScriptObject(SupplyRunGMServerSO)
  {
    class = "SupplyRunGMServer";
    EventManager_ = "";
    SupplyRunPrinterA_ = "";
    SupplyRunPrinterB_ = "";
    paper_ = "";
  };

  DNCServer.loadOutListeners_.add(SupplyRunGMServerSO);
  MissionCleanup.add(SupplyRunGMServerSO);
}
