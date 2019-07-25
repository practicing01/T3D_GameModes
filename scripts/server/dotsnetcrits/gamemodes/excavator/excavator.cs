function ExcavatorAIClass::CoolDown(%this)
{
  %this.canAttack_ = true;
}

function ExcavatorAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (%collObj.isMemberOfClass("ShapeBase"))
  {
    if (!%obj.canAttack_)
    {
      return;
    }
    
    %collObj.damage(%obj, %vec, 100, "Excavator");
    %obj.playThread(0, "attack");
    %obj.canAttack_ = false;
    %obj.cooldownSchedule_ = %obj.schedule(1000, "CoolDown");
  }
}

function ExcavatorAIClass::Attack(%this)
{
  %pos = %this.getPosition();

  if (VectorDist(%pos, %this.target_.position) > 2)
  {
    %this.followObject(%this.target_, 0);
    %this.attackSchedule_ = %this.schedule(1000, "Attack");
    return;
  }

  if (%this.canAttack_)
  {
    %this.playThread(0, "attack");
    %this.canAttack_ = false;
    %this.cooldownSchedule_ = %this.schedule(1000, "CoolDown");

    initContainerRadiusSearch(%pos, 2, $TypeMasks::ShapeBaseObjectType);

    while ( (%targetObject = containerSearchNext()) != 0 )
    {
      if(%targetObject != %this)
      {
       %targetObject.damage(%this, %pos, 100, "excavator");
      }
    }
  }

  if (!isObject(%this.target_))
  {
    %this.FindTarget();
    return;
  }

  %this.attackSchedule_ = %this.schedule(1000, "Attack");
}

function ExcavatorAIClass::SetTarget(%this, %target)
{
  %this.target_ = %target;
  %this.followObject(%target, 0);
  %this.attackSchedule_ = %this.schedule(1000, "Attack");
}

function ExcavatorAIClass::FindTarget(%this)
{
  %client = ClientGroup.getRandom();
  %player = %client.player;

  if (!isObject(%player))
  {
    %this.searchSchedule_ = %this.schedule(1000, "FindTarget");
    return;
  }

  %this.target_ = %player;
  %this.followObject(%player, 0);
}

function ExcavatorAI::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  %shape.damage_ ++;

  if (%shape.damage_ >= 100)
  {
    for (%x = 0; %x < ClientGroup.getCount(); %x++)
    {
      %client = ClientGroup.getObject(%x);
      Game.incScore(%client, 1, false);
    }

    %shape.damage_ = 0;

    %spawnpoint = PlayerDropPoints.getRandom();
    %shape.setTransform(%spawnpoint.getTransform());
    %shape.FindTarget();
  }
  return;
}

function ExcavatorGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ExcavatorGMServerQueue";

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %this.EquipExcavaber(%client.player);
  }

  %this.excavator_ = new AiPlayer()
  {
    dataBlock = ExcavatorAI;
    class = "ExcavatorAIClass";
    state_ = "idle";
    parent_ = %this;
    canAttack_ = true;
    cooldownSchedule_ = "";
    damage_ = 0;
    target_ = "";
    searchSchedule_ = "";
    attackSchedule_ = "";
  };

  %spawnpoint = PlayerDropPoints.getRandom();
  %this.excavator_.setTransform(%spawnpoint.getTransform());
  %this.excavator_.FindTarget();
}

function ExcavatorGMServer::EquipExcavaber(%this, %player)
{
  %player.use(excavaber);
}

function ExcavatorGMServer::loadOut(%this, %player)
{
  if (%player.client == %this.excavator_.target_.client)
  {
    %this.excavator_.FindTarget();
  }

  %this.EquipExcavaber(%player);
}

function ExcavatorGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.excavator_))
  {
    cancel(%this.excavator_.cooldownSchedule_);
    cancel(%this.excavator_.searchSchedule_);
    cancel(%this.excavator_.attackSchedule_);
    %this.excavator_.delete();
  }
}

if (isObject(ExcavatorGMServerSO))
{
  ExcavatorGMServerSO.delete();
}
else
{
  new ScriptObject(ExcavatorGMServerSO)
  {
    class = "ExcavatorGMServer";
    EventManager_ = "";
    excavator_ = "";
  };

  DNCServer.loadOutListeners_.add(ExcavatorGMServerSO);
  MissionCleanup.add(ExcavatorGMServerSO);
}
