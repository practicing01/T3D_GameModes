function BloodbeakAIClass::CoolDown(%this)
{
  %this.canAttack_ = true;
}

function BloodbeakAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (!isObject(%collObj))
  {
    return;
  }

  if (%collObj.class $= "BenedictClass")
  {
    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      %player = %client.getControlObject();
      %player.damage(%obj, %vec, 20, "Bloodbeak");
    }

    %spawnpoint = PlayerDropPoints.getRandom();
    %collObj.setTransform(%spawnpoint.getTransform());
    %spawnpoint = PlayerDropPoints.getRandom();
    %obj.setTransform(%spawnpoint.getTransform());
  }
  else if (%collObj.isMemberOfClass("ShapeBase"))
  {
    if (!%obj.canAttack_)
    {
      return;
    }

    %collObj.damage(%obj, %vec, 30, "Bloodbeak");
    %obj.playThread(0, "attack");
    %obj.canAttack_ = false;
    %obj.cooldownSchedule_ = %obj.schedule(1000, "CoolDown");
  }
}

function BenedictAI::onReachDestination(%this, %npc)
{
  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    Game.incScore(%client, 1, false);
  }

  %npc.setDest();
}

function BenedictAI::onMoveStuck(%this, %npc)
{
  %npc.setDest();
}

function BenedictClass::setDest(%this)
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

function BenedictAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  return;
}

function BloodbeakAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  return;
}

function BloodbeakGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "BloodbeakGMServerQueue";

  %this.benedict_ = new AiPlayer()
  {
     dataBlock = "BenedictAI";
     class = "BenedictClass";
     state_ = "idle";
     parent_ = %this;
  };

  %spawnpoint = PlayerDropPoints.getRandom();
  %this.benedict_.setTransform(%spawnpoint.getTransform());
  %this.benedict_.playThread(0, "run");
  %this.benedict_.setDest();

  %this.bloodbeak_ = new AiPlayer()
  {
    dataBlock = BloodbeakAI;
    class = "BloodbeakAIClass";
    state_ = "idle";
    parent_ = %this;
    canAttack_ = true;
    cooldownSchedule_ = "";
  };

  %spawnpoint = PlayerDropPoints.getRandom();
  %this.bloodbeak_.setTransform(%spawnpoint.getTransform());
  %this.bloodbeak_.followObject(%this.benedict_, 0);
}

function BloodbeakGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.benedict_))
  {
    %this.benedict_.delete();
  }

  if (isObject(%this.bloodbeak_))
  {
    cancel(%this.bloodbeak_.cooldownSchedule_);
    %this.bloodbeak_.delete();
  }
}

if (isObject(BloodbeakGMServerSO))
{
  BloodbeakGMServerSO.delete();
}
else
{
  new ScriptObject(BloodbeakGMServerSO)
  {
    class = "BloodbeakGMServer";
    EventManager_ = "";
    benedict_ = "";
    bloodbeak_ = "";
  };

  MissionCleanup.add(BloodbeakGMServerSO);
}
