function ExcavatorAIClass::CoolDown(%this)
{
  %this.canAttack_ = true;
}

function ExcavatorAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (%collObj.isMemberOfClass("Player"))
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

  if (%this.targets_.getCount() == 0)
  {
    %this.FindTarget();
    return;
  }

  %target = %this.targets_.getObject(0);

  if (VectorDist(%pos, %target.position) > 2)
  {
    %this.followObject(%target, 0);
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

  %this.attackSchedule_ = %this.schedule(1000, "Attack");
}

function ExcavatorAIClass::AddTarget(%this, %target)
{
  if (!%this.targets_.isMember(%target))
  {
    %this.targets_.add(%target);
  }

  %this.followObject(%this.targets_.getObject(0), 0);

  %this.attackSchedule_ = %this.schedule(1000, "Attack");
}

function ExcavatorAIClass::FindTarget(%this)
{
  %this.targets_.clear();
  %client = ClientGroup.getRandom();
  %player = %client.player;

  if (!isObject(%player))
  {
    %this.searchSchedule_ = %this.schedule(1000, "FindTarget");
    return;
  }

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
    targets_ = "";
    searchSchedule_ = "";
    attackSchedule_ = "";
  };

  %this.excavator_.targets_ = new SimSet();

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
    %this.excavator_.targets_.delete();
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
