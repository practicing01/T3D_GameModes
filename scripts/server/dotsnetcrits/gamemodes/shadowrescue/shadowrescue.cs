function VoidBallClass::UseObject(%this, %player)
{
  Game.incScore(%player.client, 1, false);

  %spawnpoint = PlayerDropPoints.getRandom();

  %this.setTransform(%spawnpoint.getTransform());
}

function ShadowRescueGMServer::CheckLOS(%this, %originObject, %destObject)
{
  if (!isObject(%originObject) || !isObject(%destObject))
  {
    return false;
  }

  %startPos = VectorAdd(%originObject.position, "0 0 1");
  %endPos = VectorAdd(%destObject.position, "0 0 1");
  %rayResult = containerRayCast(%startPos, %endPos, %this.rayMask_, %originObject);
  %objTarget = firstWord(%rayResult);

  if (%objTarget == %destObject)
  {
    return true;
  }
  else
  {
    return false;
  }
}

function ShadowRescueGMServer::ScanTargets(%this)
{
  for (%x = 0; %x < %this.lamps_.getCount(); %x++)
  {
    %lamp = %this.lamps_.getObject(%x);

    %index = %lamp.getNearestPlayerTarget();

    if (%index == -1)
    {
      continue;
    }

    %player = ClientGroup.getObject(%index).player;

    %dist = VectorDist(%player.position, %lamp.position);

    if (%dist < 20)
    {
      if (%lamp.checkInFoV(%player) && %this.CheckLOS(%lamp, %player))
      {
        %player.damage(%this, %lamp.position, 1000, "lamp");
      }
      else
      {
        %client = %player.client;
        %key = %this.dictionary_.getIndexFromKey(%client);

        if (%key == -1)
        {
          continue;
        }
        else
        {
          %npc = %this.dictionary_.getValue(%key);

          if (%lamp.checkInFoV(%npc) && %this.CheckLOS(%lamp, %npc))
          {
            %player.damage(%this, %lamp.position, 1000, "lamp");
          }
        }
      }
    }
  }

  %this.scanSchedule_ = %this.schedule(1000, "ScanTargets");
}

function LampSRAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function LampSRAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function LampSRClass::setDest(%this)
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

function LampSRAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  return;
}

function ShadowSRAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  return;
}

function ShadowRescueGMServer::TransformObject(%this, %originObject, %offsetObject, %offsetScale)
{
  %teleDir = %originObject.getForwardVector();

  %size = %originObject.getObjectBox();
  %scale = %originObject.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %offsetObject.rotation = %originObject.rotation;

  %sizeTarget = %offsetObject.getObjectBox();
  %scaleTarget = %offsetObject.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %offsetObject.position = VectorAdd( %originObject.position, VectorScale(%teleDir, (%sizex + %sizexTarget) * %offsetScale) );
}

function ShadowRescueGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ShadowRescueGMServerQueue";

  %this.rayMask_ = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;

  %this.lamps_ = new SimSet();
  %this.shadows_ = new SimSet();
  %this.dictionary_ = new ArrayObject();

  for (%x = 0; %x < %this.maxLamps_; %x++)
  {
    %lamp = new AiPlayer()
    {
       dataBlock = "LampSRAI";
       class = "LampSRClass";
       state_ = "idle";
       parent_ = %this;
    };

    %spawnpoint = PlayerDropPoints.getRandom();

    %lamp.setTransform(%spawnpoint.getTransform());

    %lamp.playThread(0, "walk");

    %lamp.setDest();

    %this.lamps_.add(%lamp);
  }

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %player = %client.getControlObject();

    %npc = new AiPlayer()
    {
      dataBlock = ShadowSRAI;
      class = "ShadowSRAIClass";
      client_ = %client;
    };

    %this.shadows_.add(%npc);

    %this.TransformObject(%player, %npc, -1);

    %npc.followObject(%player, 1);

    %this.dictionary_.add(%client, %npc);
  }

  %this.voidball_ = new StaticShape()
  {
    dataBlock = VoidBallStaticShapeData;
    class = "VoidBallClass";
    parent_ = %this;
  };

  %spawnpoint = PlayerDropPoints.getRandom();

  %this.voidball_.setTransform(%spawnpoint.getTransform());

  %this.scanSchedule_ = %this.schedule(5000, "ScanTargets");
}

function ShadowRescueGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.lamps_))
  {
    %this.lamps_.deleteAllObjects();
    %this.lamps_.delete();
  }

  if (isObject(%this.shadows_))
  {
    %this.shadows_.deleteAllObjects();
    %this.shadows_.delete();
  }

  if (isObject(%this.dictionary_))
  {
    %this.dictionary_.delete();
  }

  if (isObject(%this.voidball_))
  {
    %this.voidball_.delete();
  }

  cancel(%this.scanSchedule_);
}

function ShadowRescueGMServer::loadOut(%this, %player)
{
  %client = %player.client;
  %key = %this.dictionary_.getIndexFromKey(%client);
  %npc = "";

  if (%key == -1)
  {
    %npc = new AiPlayer()
    {
      dataBlock = ShadowSRAI;
      class = "ShadowSRAIClass";
      client_ = %client;
    };

    %this.shadows_.add(%npc);

    %this.dictionary_.add(%client, %npc);
  }
  else
  {
    %npc = %this.dictionary_.getValue(%key);
  }

  %this.TransformObject(%player, %npc, -1);
  %npc.followObject(%player, 1);
}

if (isObject(ShadowRescueGMServerSO))
{
  ShadowRescueGMServerSO.delete();
}
else
{
  new ScriptObject(ShadowRescueGMServerSO)
  {
    class = "ShadowRescueGMServer";
    EventManager_ = "";
    lamps_ = "";
    maxLamps_ = 10;
    shadows_ = "";
    dictionary_ = "";
    scanSchedule_ = "";
    rayMask_ = "";
    voidball_ = "";
  };

  DNCServer.loadOutListeners_.add(ShadowRescueGMServerSO);
  MissionCleanup.add(ShadowRescueGMServerSO);
}
