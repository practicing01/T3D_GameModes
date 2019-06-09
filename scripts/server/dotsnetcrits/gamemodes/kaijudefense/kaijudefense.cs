function kaijudefenseGMServer::Fire(%this, %client, %unprojection)
{
  %startPos = %client.camera.position;
  %dir = VectorSub(%unprojection, %startPos);
  %dir = VectorNormalize(%dir);
  %dir = VectorScale(%dir, 1000);
  %endPos = VectorAdd(%startPos, %dir);

  %rayResult = containerRayCast(%startPos, %endPos, $TypeMasks::DynamicShapeObjectType, %client.camera);

  %objTarget = firstWord(%rayResult);

  if (!isObject(%objTarget))
  {
    return;
  }

  if (%objTarget.class !$= "KaijuAsteroidClass")
  {
    return;
  }

  %objPos = getWords(%rayResult, 1, 3);

  %slot = ClientGroup.getObjectIndex(%client);
  %slot = %slot % 16;
  %slotTrans = %this.kaiju_.getSlotTransform(%slot);

  %this.dummyCam_.setTransform(%slotTrans);
  %this.dummyCam_.lookAt(%objPos);

  %crayonBeam = new StaticShape()
  {
    dataBlock = "crayonBeamStaticShapeData";
  };

  %crayonBeam.setTransform(%this.dummyCam_.getTransform());
  %distance = VectorDist(%objPos, %crayonBeam.position);
  %halfDist = %distance * 0.5;
  %dir = VectorSub(%objPos, %crayonBeam.position);
  %dir = VectorNormalize(%dir);
  %crayonBeam.scale = "1.0" SPC (%distance * 5.0) SPC "1.0";
  %crayonBeam.position = VectorAdd(%crayonBeam.position, VectorScale(%dir, %halfDist));

  %client.player.playAudio(0, crayonFireSound);

  %crayonBeam.schedule(1000, "delete");

  %objTarget.schedule(0, "delete");

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    Game.incScore(%client, 1, false);
  }
}

function serverCmdFirekaijudefenseGM(%client, %unprojection)
{
  if (isObject(kaijudefenseGMServerSO))
  {
    kaijudefenseGMServerSO.Fire(%client, %unprojection);
  }
}

function kaijudefenseAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %score = %client.score * -1;
    Game.incScore(%client, %score, false);
  }
}

function kaijudefenseAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function kaijudefenseAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function KaijuClass::setDest(%this)
{
  if (!isObject(PlayerDropPoints))
  {
    return;
  }

  %spawnPoint = PlayerDropPoints.getRandom();
  %result = %this.setPathDestination(%spawnPoint.position);

  if (!%result)
  {
    %this.schedule(1000, "setDest");
    return;
  }

  %this.setAimLocation(%spawnPoint.position);
  %this.clearAim();
}

function KaijuAsteroidRB::onRemove(%this, %asteroid)
{
  %asteroid.parent_.asteroidCount_--;
}

function KaijuAsteroidRB::onCollision(%this, %asteroid, %collObj, %vec, %len)
{
  %collObj.damage(%asteroid, %vec, 1, "asteroid");
}

function kaijudefenseGMServer::SpawnAsteroid(%this)
{
  if (%this.asteroidCount_ >= %this.maxAsteroids_)
  {
    %this.asteroidSchedule_ = %this.schedule(%this.asteroidInterval_, "SpawnAsteroid");
    return;
  }

  for (%x = 0; %x < 4; %x++)
  {
    %pos = %this.kaiju_.position;
    %pos.z += 100;

    %angleY = mDegToRad(getRandom(0, 100) * m2Pi());
    %angleXZ = mDegToRad(getRandom(0, 100) * m2Pi());

    %pos.x = %pos.x + (mCos(%angleY) * mSin(%angleXZ) * getRandom(-%this.fieldRadius_, %this.fieldRadius_));
    %pos.y = %pos.y + (mCos(%angleXZ) * getRandom(-%this.fieldRadius_, %this.fieldRadius_));

    %asteroid = new RigidShape()
    {
      datablock = "KaijuAsteroidRB";
      class = "KaijuAsteroidClass";
      position = %pos;
      parent_ = %this;
    };

    MissionCleanup.add(%asteroid);

    %dir = VectorSub(%this.kaiju_.position, %pos);
    %dir = VectorNormalize(%dir);
    %dir = VectorScale(%dir, 100);

    %asteroid.applyImpulse(%pos, %dir);

    %this.asteroidCount_++;

    %asteroid.schedule(10000, "delete");
  }

  %this.asteroidSchedule_ = %this.schedule(%this.asteroidInterval_, "SpawnAsteroid");
}

function kaijudefenseGMServer::SetupCamera(%this, %client, %npc)
{
  if (isObject(%client.camera) && isObject(%client.player))
  {
    %client.player.delete();
    %client.player = %npc;
    %client.camera.setTransform(%npc.getEyeTransform());
    %client.camera.setOrbitObject(%npc, %client.camera.getRotation(), 4, 8, 4, true, "0 0 0", false);
    %client.camera.controlMode = "OrbitObject";
    %client.setControlObject(%client.camera);
  }
}

function kaijudefenseGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "kaijudefenseGMServerQueue";

  if (!isObject(%this.dummyCam_))
  {
    %this.dummyCam_ = new Camera()
    {
      datablock = "Observer";
    };

    MissionCleanup.add(%this.dummyCam_);
  }

  %this.kaiju_ = new AiPlayer()
  {
     dataBlock = "kaijudefenseAI";
     class = "KaijuClass";
     state_ = "idle";
     parent_ = %this;
     scale = "0.5 0.5 0.5";
  };

  %spawnPoint = PlayerDropPoints.getRandom();
  %this.kaiju_.setTransform(%spawnPoint.getTransform());

  MissionCleanup.add(%this.kaiju_);

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);

    %this.SetupCamera(%client, %this.kaiju_);
  }

  //%this.kaiju_.setDest();

  %this.gravZone_ = new PhysicalZone() {
    velocityMod = "1";
    gravityMod = "0";
    //appliedForce = "0 0 20000";
    polyhedron = "-0.5000000 0.5000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
    position = %this.kaiju_.position;
    rotation = "1 0 0 0";
    scale = %this.fieldRadius_ SPC %this.fieldRadius_ SPC %this.fieldRadius_;
    isRenderEnabled = "true";
    canSaveDynamicFields = "1";
    enabled = "1";
    renderZones = true;
  };

  %this.SpawnAsteroid();
}

function kaijudefenseGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.kaiju_))
  {
    %this.kaiju_.delete();
  }

  if (isObject(%this.gravZone_))
  {
    %this.gravZone_.delete();
  }

  if (isObject(%this.dummyCam_))
  {
    %this.dummyCam_.delete();
  }

  cancel(%this.asteroidSchedule_);

  if (isObject(PlayerDropPoints))
  {
    %spawnPoint = PlayerDropPoints.getRandom();

    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      %client.spawnPlayer(%spawnPoint);
    }
  }
}

function kaijudefenseGMServer::loadOut(%this, %player)
{
  %client = %player.client;

  %this.SetupCamera(%client, %this.kaiju_);
}

if (isObject(kaijudefenseGMServerSO))
{
  kaijudefenseGMServerSO.delete();
}
else
{
  new ScriptObject(kaijudefenseGMServerSO)
  {
    class = "kaijudefenseGMServer";
    EventManager_ = "";
    kaiju_ = "";
    asteroidSchedule_ = "";
    asteroidInterval_ = 1000;
    maxAsteroids_ = 40;
    asteroidCount_ = 0;
    fieldRadius_ = 500;
    gravZone_ = "";
    dummyCam_ = "";
  };

  DNCServer.loadOutListeners_.add(kaijudefenseGMServerSO);
  MissionCleanup.add(kaijudefenseGMServerSO);
}
