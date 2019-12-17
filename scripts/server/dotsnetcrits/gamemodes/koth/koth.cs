function KOTHGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "KOTHGMServerQueue";

  %spawnPoint = PlayerDropPoints.getRandom();
  %throneSpawnPos = DNCServer.GetRayHitPos("0 0 -1", 1000, 0, DNCServer.envRayMask_, %spawnPoint);

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "throneSpawnKOTHGM")
    {
      %throneSpawnPos = %obj.position;
    }
  }

  %this.userpers_ = new SimSet();

  %this.trigger_ = new Trigger()
  {
    dataBlock = "KOTHGMTrigger";
    polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
    position = %throneSpawnPos;
    scale = "20 20 5";
  };

  %this.throne_ = new StaticShape()
  {
    dataBlock = "throneKOTHGM";
    position = %throneSpawnPos;
    rotation = "1 0 0 0";
    scale = "0.2 0.2 0.2";
  };

  %throneSpawnPos.z += 0.5;

  %this.plunger_ = new StaticShape()
  {
    dataBlock = "plungerKOTHGM";
    position = %throneSpawnPos;
    rotation = "1 0 0 0";
    scale = "4.0 4.0 4.0";
  };

}

function KOTHGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.throne_))
  {
    %this.throne_.delete();
  }

  if (isObject(%this.plunger_))
  {
    %this.plunger_.delete();
  }

  if (isObject(%this.trigger_))
  {
    %this.trigger_.delete();
  }

  if (isObject(%this.userpers_))
  {
    %this.userpers_.delete();
  }

}

function KOTHGMServer::Coronate(%this)
{
  if (%this.trigger_.getNumObjects())
  {
    %king = %this.plunger_.getObjectMount();

    %userper = %this.trigger_.getObject(0);

    if (%king == %userper)
    {
      return;
    }

    %userper.mountObject(%this.plunger_, 2, MatrixCreate("0 0 1", "1 0 0 0"));

    ServerPlay2D(kothCoronateSound);
  }
}

function KOTHGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (isObject(KOTHGMServerSO))
  {
    KOTHGMServerSO.Coronate();
  }
}

function KOTHGMTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (isObject(KOTHGMServerSO))
  {
    KOTHGMServerSO.Coronate();
  }
}

function KOTHGMTrigger::onTickTrigger(%this, %trigger)
{
  %obj = %trigger.getObject(0);

  %damageState = %obj.getDamageState();
  if (%damageState $= "Disabled" || %damageState $= "Destroyed")
  {
    return;
  }

  Game.incScore(%obj.client, 1, false);
}

if (isObject(KOTHGMServerSO))
{
  KOTHGMServerSO.delete();
}
else
{
  new ScriptObject(KOTHGMServerSO)
  {
    class = "KOTHGMServer";
    EventManager_ = "";
    throne_ = "";
    plunger_ = "";
    trigger_ = "";
    userpers_ = "";
  };

  MissionCleanup.add(KOTHGMServerSO);
}
