function PumptreeStaticShapeData::onDisabled(%this, %shape, %state)
{
  %shape.setDamageLevel(0.5);
  %shape.setDamageState("Enabled");
}

function PumptreeGMServer::ClampScale(%this, %originalScale, %scalar, %minClamp, %maxClamp)
{
  %scale = VectorScale(%originalScale, %scalar);
  %x = getWord(%scale, 0);
  %y = getWord(%scale, 1);
  %z = getWord(%scale, 2);
  %x = mClamp(%x, %minClamp, %maxClamp);
  %y = mClamp(%y, %minClamp, %maxClamp);
  %z = mClamp(%z, %minClamp, %maxClamp);
  %scale = %x SPC %y SPC %z;
  return %scale;
}

function PumptreeGMTrigger::onTickTrigger(%this, %trigger)
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

    if (%obj.client == %trigger.playerClient_)
    {
      continue;
    }

    %obj.damage(%trigger, %obj.position, 1, "pumptree");
  }
}

function PumptreeStaticShapeData::onDamage(%this, %shape, %delta)
{
  if (%shape.getDamageState() $= "Disabled")
  {
    return;
  }

  parent::onDamage(%this, %shape);

  if (%delta < 0)
  {
    %shape.scale = %shape.parent_.ClampScale(%shape.scale, 1.1, 0.1, 5.0);
    %shape.stopRepair();

    %treeFog = %shape.treeFog_;
    %treeFog.scale = %treeFog.parent_.ClampScale(%treeFog.scale, 1.05, 20.0, 200.0);

    %trigger = %shape.trigger_;
    %trigger.scale = %trigger.parent_.ClampScale(%trigger.scale, 1.05, 1.0, 10.0);
  }
  else
  {
    %shape.scale = %shape.parent_.ClampScale(%shape.scale, 0.95, 0.1, 5.0);

    %treeFog = %shape.treeFog_;
    %treeFog.scale = %treeFog.parent_.ClampScale(%treeFog.scale, 0.95, 20.0, 200.0);

    %trigger = %shape.trigger_;
    %trigger.scale = %trigger.parent_.ClampScale(%trigger.scale, 0.95, 1.0, 10.0);
  }
}

function PumptreeGMServer::TransformNPC(%this, %player, %npc)
{
  %teleDir = %player.getForwardVector();

  %size = %player.getObjectBox();
  %scale = %player.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %npc.rotation = %player.rotation;

  %sizeTarget = %npc.getObjectBox();
  %scaleTarget = %npc.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %npc.position = VectorAdd( %player.position, VectorScale(%teleDir, %sizex + %sizexTarget) );
}

function PumptreeGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PumptreeGMServerQueue";

  %this.pumptrees_ = new SimSet();
  %this.treeFogs_ = new SimSet();
  %this.triggers_ = new SimSet();

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %player = %client.getControlObject();

    %treeFog = new VolumetricFog()
    {
      shapeName = "art/environment/Fog_Cube.DAE";
      fogColor = "161 154 48 255";
      fogDensity = "0.2";
      ignoreWater = "0";
      MinSize = "250";
      FadeSize = "750";
      texture = "art/environment/FogMod_heavy.dds";
      tiles = "1.5";
      modStrength = "0.2";
      PrimSpeed = "-0.01 0.04";
      SecSpeed = "0.02 -0.02";
      scale = "1 1 1";
      useGlow = "true";
      parent_ = %this;
    };

    %this.treeFogs_.add(%treeFog);

    %pumptree = new StaticShape()
    {
      dataBlock = "PumptreeStaticShapeData";
      class = "PumptreeClass";
      parent_ = %this;
      playerClient_ = %client;
      treeFog_ = %treeFog;
      trigger_ = "";
    };

    %this.pumptrees_.add(%pumptree);

    %pumptree.setDamageLevel(0.5);

    %pumptree.scale = "0.1 0.1 0.1";

    %this.TransformNPC(%player, %pumptree);

    %treeFog.position = %pumptree.position;
    %treeFog.rotation = %pumptree.rotation;
    %treeFog.scale = "20 20 20";

    %trigger = new Trigger()
    {
      dataBlock = "PumptreeGMTrigger";
      //polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
      polyhedron = "-20 20 0.0 40.0 0.0 0.0 0.0 -40.0 0.0 0.0 0.0 40.0";
      parent_ = %this;
      playerClient_ = %client;
    };

    %this.triggers_.add(%trigger);

    %trigger.setTransform(%treeFog.getTransform());

    %pumptree.trigger_ = %trigger;
  }
}

function PumptreeGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.pumptrees_))
  {
    %this.pumptrees_.deleteAllObjects();
    %this.pumptrees_.delete();
  }

  if (isObject(%this.treeFogs_))
  {
    %this.treeFogs_.deleteAllObjects();
    %this.treeFogs_.delete();
  }

  if (isObject(%this.triggers_))
  {
    %this.triggers_.deleteAllObjects();
    %this.triggers_.delete();
  }
}

if (isObject(PumptreeGMServerSO))
{
  PumptreeGMServerSO.delete();
}
else
{
  new ScriptObject(PumptreeGMServerSO)
  {
    class = "PumptreeGMServer";
    EventManager_ = "";
    pumptrees_ = "";
    treeFogs_ = "";
    triggers_ = "";
  };

  MissionCleanup.add(PumptreeGMServerSO);
}
