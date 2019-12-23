function GooshballItemData::onCollision(%this, %obj, %collObj, %vec, %len)
{
  if (%collObj.getClassName() !$= "Player")
  {
    return;
  }

  %collObj.scale = vectorScale(%collObj.scale, 1.1);
  %collObj.client.gooshball_.scale = vectorScale(%collObj.client.gooshball_.scale, 1.1);

  %obj.schedule(0, "delete");
}

function GooshballAI::onDisabled(%this, %obj, %state)
{
  %obj.setDamageLevel(0);
  %obj.setDamageState("Enabled");
}

function GooshballAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  if (!%collObj.isMethod("damage"))
  {
    return;
  }

  %damageState = %collObj.getDamageState();
  if (%damageState $= "Disabled" || %damageState $= "Destroyed")
  {
    return;
  }

  if (%collObj.client == %obj.client_)
  {
    return;
  }

  if (VectorLen(%obj.scale) > VectorLen(%collObj.scale))
  {
    %collObj.damage(%obj, %vec, 1000, "gooshball");
  }
}

function GooshballGMServer::TransformNPC(%this, %player, %npc)
{
  %teleDir = %player.getForwardVector();

  %teleDir = VectorScale(%teleDir, -1);

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

function GooshballGMServer::MountGooshball(%this, %player)
{
  if (isObject(%player.client.gooshball_))
  {
    %player.client.gooshball_.delete();
  }

  %gooshball = new AiPlayer()
  {
    dataBlock = "GooshballAI";
    client_ = %player.client;
  };

  %this.TransformNPC(%player, %gooshball);

  %gooshball.followObject(%player, 4);

  %player.client.gooshball_ = %gooshball;
}

function GooshballGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "GooshballGMServerQueue";

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %player = ClientGroup.getObject(%x).getControlObject();

    %this.MountGooshball(%player);
  }
}

function GooshballGMServer::onRemove(%this)
{
  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);

    if (isObject(%client.gooshball_))
    {
      %client.gooshball_.delete();
    }
  }

  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
}

function GooshballGMServer::loadOut(%this, %player)
{
  %this.MountGooshball(%player);
}

function GooshballGMServerSO::onDeath(%this, %game, %client, %sourceObject, %sourceClient, %damageType, %damLoc)
{
  if (isObject(%client.gooshball_))
  {
    %client.gooshball_.delete();
  }

  %player = %client.player;
  %item = ItemData::createItem(GooshballItemData);
  %item.sourceObject = %player;
  %item.static = false;
  MissionCleanup.add(%item);

  %vec = (-1.0 + getRandom() * 2.0) SPC (-1.0 + getRandom() * 2.0) SPC getRandom();
  %vec = vectorScale(%vec, 10);
  %eye = %player.getEyeVector();
  %dot = vectorDot("0 0 1", %eye);
  if (%dot < 0)
     %dot = -%dot;
  %vec = vectorAdd(%vec, vectorScale("0 0 8", 1 - %dot));
  %vec = vectorAdd(%vec, %player.getVelocity());
  %pos = getBoxCenter(%player.getWorldBox());

  %item.setTransform(%pos);
  %item.applyImpulse(%pos, %vec);
  %item.setCollisionTimeout(%player);
}

if (isObject(GooshballGMServerSO))
{
  GooshballGMServerSO.delete();
}
else
{
  new ScriptObject(GooshballGMServerSO)
  {
    class = "GooshballGMServer";
    EventManager_ = "";
  };

  DNCServer.loadOutListeners_.add(GooshballGMServerSO);
  DNCServer.deathListeners_.add(GooshballGMServerSO);
  MissionCleanup.add(GooshballGMServerSO);
}
