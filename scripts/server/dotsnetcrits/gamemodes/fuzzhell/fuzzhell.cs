function FuzzHellGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "FuzzHellGMServerQueue";

  %spawnPoint = PlayerDropPoints.getRandom();

  %trans = %spawnPoint.getTransform();

  %this.npc_ = new AiPlayer()
  {
    dataBlock = FuzzHellAI;
    class = "FuzzHellAIClass";
    fireSchedule_ = "";
  };

  %this.npc_.setTransform(%trans);

  %this.npc_.position = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, DNCServer.envRayMask_, %this.npc_);

  %this.npc_.FireFuzz();

  MissionCleanup.add(%this.npc_);

  %this.trigger_ = new Trigger()
  {
    dataBlock = "FuzzHellGMTrigger";
    polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
    position = %this.npc_.position;
    scale = "20 20 5";
  };
}

function FuzzHellGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if(isObject(%this.npc_))
  {
    cancel(%this.npc_.fireSchedule_);
    %this.npc_.delete();
  }

  if (isObject(%this.trigger_))
  {
    %this.trigger_.delete();
  }
}

function FuzzHellGMTrigger::onTickTrigger(%this, %trigger)
{
  for (%x = 0; %x < %trigger.getNumObjects(); %x++)
  {
    %obj = %trigger.getObject(%x);

    if (%obj.getClassName() !$= "Player")
    {
      continue;
    }

    %damageState = %obj.getDamageState();
    if (%damageState $= "Disabled" || %damageState $= "Destroyed")
    {
      continue;
    }

    Game.incScore(%obj.client, 1, false);
  }
}

function FuzzHellGMProjectile::damage(%this, %sourceObject, %position, %damage, %damageType)
{
  //
}

function FuzzHellGMProjectile::onCollision(%this, %proj, %col, %fade, %pos, %normal)
{
  if (%col.getClassName() !$= "Player")
  {
    return;
  }

  %col.setWhiteOut(1.0);
  Game.incScore(%col.client, -1, false);
}

function FuzzHellAIClass::FireFuzz(%this)
{
  %startDeg = getRandom(1, 20);
  %endDeg = 360;

  for (%x = %startDeg; %x < %endDeg; %x += getRandom(1, 20))
  {
    %projectile = new Projectile()
    {
      datablock = FuzzHellGMProjectile;
      initialPosition = %this.getEyePoint();
      initialVelocity = "0 0 0";
      sourceObject = %this;
      sourceSlot = 0;
      client = %this.client;
    };

    %rotMat = MatrixCreateFromEuler("0 0" SPC mDegToRad(%x));//mDegToRad((%x % 360)));
    %trans = %projectile.getTransform();
    %projectile.setTransform(MatrixMultiply(%trans, %rotMat));

    %projectileDir = VectorNormalize(%projectile.getForwardVector());
    %projectileVelocity = VectorScale(%projectileDir, 5.0);
    %projectile.initialVelocity = %projectileVelocity;
    //%projectile.initialPosition = VectorAdd(%projectile.initialPosition, VectorScale(%projectileDir, 2.0));
  }

  %this.fireSchedule_ = %this.schedule(1 * 500, "FireFuzz");
}

function FuzzHellAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
}

if (isObject(FuzzHellGMServerSO))
{
  FuzzHellGMServerSO.delete();
}
else
{
  new ScriptObject(FuzzHellGMServerSO)
  {
    class = "FuzzHellGMServer";
    EventManager_ = "";
    npc_ = "";
    trigger_ = "";
  };

  MissionCleanup.add(FuzzHellGMServerSO);
}
