datablock TriggerData(SpringInvasionTrigger)
{
  tickPeriodMS = 1000;
};

function SpringInvasionTrigger::onAdd(%this, %trigger)
{
  %trigger.npcs_ = new SimSet();
  %trigger.players_ = new SimSet();
}

function SpringInvasionTrigger::onRemove(%this, %trigger)
{
  if (isObject(%trigger.npcs_))
  {
    %trigger.npcs_.delete();
  }

  if (isObject(%trigger.players_))
  {
    %trigger.players_.delete();
  }
}

function SpringInvasionTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (!isObject(%obj))
  {
    return;
  }

  if (!%obj.isMethod("getClassName"))
  {
    return;
  }

  if (%obj.getClassName() $= "AIPlayer")
  {
    if (!%trigger.npcs_.isMember(%obj))
    {
      %trigger.npcs_.add(%obj);
      if (%trigger.players_.getCount() == 0)
      {
        return;
      }

      %obj.setAimObject(%trigger.players_.getRandom(), "0 0 1");
    }
  }

  if (%obj.getClassName() $= "Player")
  {
    if (!%trigger.players_.isMember(%obj))
    {
      %trigger.players_.add(%obj);
    }
  }
}

function SpringInvasionTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (!isObject(%obj))
  {
    return;
  }

  if (!%obj.isMethod("getClassName"))
  {
    return;
  }

  if (%obj.getClassName() $= "AIPlayer")
  {
    if (%trigger.npcs_.isMember(%obj))
    {
      %trigger.npcs_.remove(%obj);
    }
  }

  if (%obj.getClassName() $= "Player")
  {
    if (%trigger.players_.isMember(%obj))
    {
      %trigger.players_.remove(%obj);
    }
  }
}

datablock PlayerData(SpringInvasionBunnyAI : DemoPlayer)
{
  shapeFile = "art/shapes/dotsnetcrits/levels/springinvasion/springinvasionbunny/springinvasionbunny.dae";
  moveStuckTestDelay = 1;
  moveStuckTolerance = 1;
};

function SpringInvasionBunnyAI::onTargetEnterLOS(%this, %ai)
{
  if (!isObject(%ai))
  {
    return;
  }

  %ai.fire(true);
  %ai.incInventory(staplerAmmo, 1);
}

function SpringInvasionBunnyAI::onTargetExitLOS(%this, %ai)
{
  %ai.fire(false);
}

function SpringInvasionBunnyAIClass::setDest(%this)
{
  if (!isObject(AIDropPoints))
  {
    %this.schedule(500, "delete");
    return;
  }

  %spawnPoint = AIDropPoints.getRandom();
  %this.setPathDestination(%spawnPoint.position);
  %this.setAimLocation(%spawnPoint);
  %this.clearAim();
}

function SpringInvasionBunnyAI::onReachDestination(%this, %ai)
{
  if (!isObject(%ai))
  {
    return;
  }

  %ai.setDest();
}

function SpringInvasionBunnyAI::onMoveStuck(%this, %ai)//not being called, idk why
{
  if (!isObject(%ai))
  {
    return;
  }

  %ai.setDest();
  %ai.applyImpulse("0 0 0", VectorScale(%ai.getUpVector(), 5000.0));
}

function SpringInvasionBunnyAI::onAdd(%this, %obj)
{
  %obj.class = SpringInvasionBunnyAIClass;
  %obj.mountImage(staplerImage, 0);
  %obj.incInventory(staplerAmmo, 100);
  %obj.moveStuckTestDelay = 1;
  %obj.moveStuckTolerance = 1;

  %obj.schedule(10000, "setDest");
}

function SpringInvasionBunnyAI::onDisabled(%this, %obj, %state)
{
  parent::onDisabled(%this, %obj, %state);

  %obj.stop();
  %obj.state_ = "dead";
  %obj.clearAim();
  %obj.fire(false);

  %obj.schedule(500, "delete");
}

function SpringInvasionBunnyAIClass::onRemove(%this)
{
  %this.stop();
  cancelAll(%this);
}

function SISOClass::onAdd(%this)
{
  DNCServer.missionLoadedListeners_.add(%this);
}

function SISOClass::onRemove(%this)
{
  DNCServer.missionLoadedListeners_.remove(%this);
}

function SISOClass::onMissionLoaded(%this, %game)
{
  for (%x = 0; %x < AIDropPoints.getCount(); %x++)
  {
    %spawnSphere = AIDropPoints.getObject(%x);
    %bunny = %spawnSphere.spawnObject("");
    MissionCleanup.add(%bunny);
  }
}

singleton Material(silver32grass4)
{
   mapTo = "silver32grass4";
   terrainMaterials = "1";
   ShowDust = "1";
   showFootprints = "1";
   materialTag0 = "Terrain";
};

new TerrainMaterial()
{
   internalName = "silver32grass4";
   diffuseMap = "art/terrains/dotsnetcrits/silver32terrain/grass4.png";
   detailMap = "art/terrains/dotsnetcrits/silver32terrain/grass4_d.png";
   normalMap = "art/terrains/dotsnetcrits/silver32terrain/grass4_n.png";
   detailSize = "1";
   isManaged = "1";
   detailBrightness = "1";
   Enabled = "1";
   diffuseSize = "1";
};

singleton Material(silver32sand)
{
   mapTo = "silver32sand";
   terrainMaterials = "1";
   ShowDust = "1";
   showFootprints = "1";
   materialTag0 = "Terrain";
};

new TerrainMaterial()
{
   internalName = "silver32sand";
   diffuseMap = "art/terrains/dotsnetcrits/silver32terrain/sand.png";
   detailMap = "art/terrains/dotsnetcrits/silver32terrain/sand_d.png";
   normalMap = "art/terrains/dotsnetcrits/silver32terrain/sand_n.png";
   detailSize = "1";
   isManaged = "1";
   detailBrightness = "1";
   Enabled = "1";
   diffuseSize = "1";
};

singleton Material(silver32flower)
{
   diffuseMap[0] = "art/terrains/dotsnetcrits/silver32terrain/flowers.png";
   //detailMap[0] = "art/terrains/dotsnetcrits/silver32terrain/flowers_d.png";
   //normalMap[0] = "art/terrains/dotsnetcrits/silver32terrain/flowers_n.png";
   translucent = false;
   materialTag0 = "Foliage";
   mapTo = "flowers.png";
   castShadows = "0";
   alphaTest = "1";
   alphaRef = "255";
};
