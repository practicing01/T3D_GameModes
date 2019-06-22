function Skit3dGMServer::ResetTrees(%this, %player)
{
  %key = %this.dictionary_.getIndexFromKey(%player.client);
  %playerSO = %this.dictionary_.getValue(%key);

  %playerSO.perimiterTrigger_.position = %player.position;

  %playerSO.trees_.callOnChildren("setHidden", "true");

  %index = 0;

  for (%y = -1; %y <= 1; %y++)
  {
    for (%x = -1; %x <= 1; %x++)
    {
      %trigger = %playerSO.triggers_.getObject(%index);
      %tree = %playerSO.trees_.getObject(%index);

      %offset = VectorScale(%x SPC %y SPC "0", 50);
      %offset = VectorAdd(%player.position, %offset);

      %startPos = VectorAdd(%offset, "0 0 50");
      %endPos = VectorAdd(%offset, "0 0 -50");
      %rayResult = containerRayCast(%startPos, %endPos, %this.rayMask_, %tree);
      %objTarget = firstWord(%rayResult);

      if (isObject(%objTarget))
      {
        %offset = getWords(%rayResult, 1, 3);
      }

      %tree.position = %offset;

      %offset = VectorAdd(%offset, "0 0 -25");
      %trigger.position = %offset;

      %index++;
    }
  }

  %playerSO.trees_.callOnChildren("setHidden", "false");
}

function Skit3dGMServer::MoveTrees(%this, %trigger, %player)
{
  %key = %this.dictionary_.getIndexFromKey(%player.client);
  %playerSO = %this.dictionary_.getValue(%key);
  %playerSO.perimiterTrigger_.position = %trigger.position;

  %playerSO.trees_.callOnChildren("setHidden", "true");

  %index = 0;

  for (%y = -1; %y <= 1; %y++)
  {
    for (%x = -1; %x <= 1; %x++)
    {
      if (%x == 0 && %y == 0)
      {
        continue;
      }

      %triggerNeighbor = %playerSO.triggers_.getObject(%index);

      if (%triggerNeighbor == %trigger)
      {
        %index++;
        %triggerNeighbor = %playerSO.triggers_.getObject(%index);
      }

      %treeNeighbor = %playerSO.trees_.getObject(%index);

      %offset = VectorScale(%x SPC %y SPC "0", 50);
      %offset = VectorAdd(%trigger.position, %offset);

      %startPos = VectorAdd(%offset, "0 0 50");
      %endPos = VectorAdd(%offset, "0 0 -50");
      %rayResult = containerRayCast(%startPos, %endPos, %this.rayMask_, %treeNeighbor);
      %objTarget = firstWord(%rayResult);

      if (isObject(%objTarget))
      {
        %offset = getWords(%rayResult, 1, 3);
      }
      else
      {
        %offset = VectorAdd(%offset, "0 0 25");
      }

      %treeNeighbor.position = %offset;

      %offset = VectorAdd(%offset, "0 0 -25");
      %triggerNeighbor.position = %offset;

      %index++;
    }
  }

  %playerSO.trees_.callOnChildren("setHidden", "false");
}

function Skit3dGMTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (%obj.client == %trigger.client_)
  {
    if (!%trigger.isPerimiter_)
    {
      return;
    }

    %trigger.parent_.ResetTrees(%obj);
  }
}

function Skit3dGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj.client == %trigger.client_)
  {
    if (%trigger.isPerimiter_)
    {
      return;
    }

    %trigger.parent_.MoveTrees(%trigger, %obj);
  }
}

function YetiAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  if (!isObject(%collObj))
  {
    return;
  }

  if (%collObj.getClassName() !$= "AIPlayer")
  {
    return;
  }

  %score = %collObj.client.score * -1;

  if (%score == 0)
  {
    return;
  }

  Game.incScore(%client, %score, false);
}

function YetiAI::onReachDestination(%this, %npc)
{
  %npc.setDest();
}

function YetiAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function YetiClass::setDest(%this)
{
  if (!isObject(%this))
  {
    return;
  }
  
  %client = ClientGroup.getRandom();
  %player = %client.player;

  %result = %this.setPathDestination(%player.position);

  if (!%result)
  {
    %this.setMoveDestination(%player.position);
    %this.schedule(1000, "setDest");
    return;
  }

  %this.setAimLocation(%player.position);
  %this.clearAim();
  %this.schedule(10000, "setDest");
}

function SkierAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  Game.incScore(%shape.client, -1, false);
}

function Skit3dGMServer::Move(%this, %client, %dir, %state)
{
  %moveDir = %client.player.moveDir_;

  if (%dir == 0)
  {
    %moveDir.y = 1 * %state;
  }
  else if (%dir == 1)
  {
    %moveDir.x = -1 * %state;
  }
  else if (%dir == 2)
  {
    %moveDir.y = -1 * %state;
  }
  else if (%dir == 3)
  {
    %moveDir.x = 1 * %state;
  }

  %newDir = VectorScale(%moveDir, 1000);
  %dest = VectorAdd(%client.player.position, %newDir);

  %client.player.moveDir_ = %moveDir;

  if (%moveDir $= "0 0 0")
  {
    %client.player.stop();
    %client.player.clearAim();
    return;
  }

  %client.player.setMoveDestination(%dest);
  //%client.player.setPathDestination(%dest);

  %client.player.setAimLocation(%dest);
  %client.player.clearAim();
}

function serverCmdMoveSkit3dGM(%client, %dir, %state)
{
  if (isObject(Skit3dGMServerSO))
  {
    Skit3dGMServerSO.Move(%client, %dir, %state);
  }
}

function Skit3dGMServer::SetupCamera(%this, %client)
{
  %client.camera.position = VectorAdd(%client.player.position, "0 -10 10");
  %client.camera.lookAt(%client.player.position);
  %client.camera.setOrbitObject(%client.player, %client.camera.getRotation(), 5, 10, 10, true, "0 -10 10", true);
  %client.camera.controlMode = "OrbitObject";
  %client.setControlObject(%client.camera);
}

function Skit3dGMServer::SpawnNPC(%this, %client)
{
  %player = %client.getControlObject();

  %playerSO = new ScriptObject()
  {
    npc_ = "";
    trees_ = "";
    triggers_ = "";
    perimiterTrigger_ = "";
  };

  %playerSO.trees_ = new SimSet();
  %playerSO.triggers_ = new SimSet();

  %playerSO.npc_ = new AiPlayer()
  {
    dataBlock = SkierAI;
    class = "SkierAIClass";
    client = %client;
    moveDir_ = "0 0 0";
  };

  %playerSO.npc_.setTransform(%player.getTransform());
  %this.dictionary_.add(%client, %playerSO);

  %client.player.delete();
  %client.player = %playerSO.npc_;

  %this.SetupCamera(%client);

  for (%y = -1; %y <= 1; %y++)
  {
    for (%x = -1; %x <= 1; %x++)
    {
      %offset = VectorScale(%x SPC %y SPC "0", 50);
      %offset = VectorAdd(%client.player.position, %offset);

      %tree = new StaticShape()
      {
         dataBlock = "SkiTreeStaticShapeData";
         position = %offset;
      };

      %playerSO.trees_.add(%tree);

      %offset = VectorAdd(%offset, "0 0 -25");

      %trigger = new Trigger()
      {
        dataBlock = "Skit3dGMTrigger";
        polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
        position = %offset;
        scale = "45 45 45";
        parent_ = %this;
        client_ = %client;
        isPerimiter_ = false;
      };

      %playerSO.triggers_.add(%trigger);
    }
  }

  %offset = VectorAdd(%client.player.position, "0 0 -25");

  %playerSO.perimiterTrigger_ = new Trigger()
  {
    dataBlock = "Skit3dGMTrigger";
    polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
    position = %offset;
    scale = "150 150 45";
    parent_ = %this;
    client_ = %client;
    isPerimiter_ = true;
  };
}

function Skit3dGMServer::Score(%this)
{
  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    Game.incScore(%client, 1, false);
  }

  %this.scoreSchedule_ = %this.schedule(10000, "Score");
}

function Skit3dGMServer::onAdd(%this)
{
  %this.rayMask_ = $TypeMasks::EnvironmentObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType;

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "Skit3dGMServerQueue";

  %this.yeti_ = new AiPlayer()
  {
     dataBlock = "YetiAI";
     class = "YetiClass";
     state_ = "idle";
     parent_ = %this;
  };

  %spawnPoint = PlayerDropPoints.getRandom();
  %this.yeti_.setTransform(%spawnPoint.getTransform());

  %this.dictionary_ = new ArrayObject();

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);

    %this.SpawnNPC(%client);
  }

  %this.yeti_.setDest();

  %this.scoreSchedule_ = %this.schedule(10000, "Score");
}

function Skit3dGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.yeti_))
  {
    %this.yeti_.delete();
  }

  if (isObject(%this.dictionary_))
  {
    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      %key = %this.dictionary_.getIndexFromKey(%client);
      %playerSO = %this.dictionary_.getValue(%key);

      %playerSO.npc_.delete();
      %playerSO.trees_.deleteAllObjects();
      %playerSO.trees_.delete();
      %playerSO.triggers_.deleteAllObjects();
      %playerSO.triggers_.delete();
      %playerSO.perimiterTrigger_.delete();
    }
    %this.dictionary_.delete();
  }

  if (isObject(PlayerDropPoints))
  {
    %spawnPoint = PlayerDropPoints.getRandom();

    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      %client.spawnPlayer(%spawnPoint);
    }
  }

  cancel(%this.scoreSchedule_);
}

function Skit3dGMServer::loadOut(%this, %player)
{
  %client = %player.client;
  %key = %this.dictionary_.getIndexFromKey(%client);
  %npc = "";

  if (%key == -1)
  {
    %npc = %this.SpawnNPC(%client, %this);
  }
}

if (isObject(Skit3dGMServerSO))
{
  Skit3dGMServerSO.delete();
}
else
{
  new ScriptObject(Skit3dGMServerSO)
  {
    class = "Skit3dGMServer";
    EventManager_ = "";
    dictionary_ = "";
    yeti_ = "";
    rayMask_ = "";
    scoreSchedule_ = "";
  };

  DNCServer.loadOutListeners_.add(Skit3dGMServerSO);
  MissionCleanup.add(Skit3dGMServerSO);
}
