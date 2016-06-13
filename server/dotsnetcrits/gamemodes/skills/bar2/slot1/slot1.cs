function ShieldSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %this.guiSlot_ = %guiSlot;

  %player = %client.getControlObject();

  if (%player.isField("silenceSet_"))
  {
    if (%player.silenceSet_.getCount() > 0)
    {
      return;
    }
  }

  %rayResult = %player.doRaycast(1000.0, $TypeMasks::ShapeBaseObjectType);

  %obj = firstWord(%rayResult);

  if (!isObject(%obj))
  {
    return;
  }

  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    if (!%obj.isField("shieldSet_"))
    {
      %obj.shieldSet_ = new SimSet();
      %this.shieldSets_.add(%obj.shieldSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = ShieldEmitterNodeData;
      emitter = ShieldEmitter;
      active = true;
      velocity = 0.0;
    };

    %shield = new ScriptObject()
    {
      class = "ShieldInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
      power_ = %this.power_;
    };

    %obj.mountObject(%targetEmitterNode, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %obj.shieldSet_.add(%shield);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = ShieldEmitterNodeData;
      emitter = ShieldEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %shield.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);
  }

}

function ShieldInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function ShieldSkillsGM::onAdd(%this)
{
  %this.shieldSets_ = new SimSet();
}

function ShieldSkillsGM::onRemove(%this)
{
  if (isObject(%this.shieldSets_))
  {
    %this.shieldSets_.callOnChildren("deleteAllObjects");
    %this.shieldSets_.deleteAllObjects();
    %this.shieldSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function ShieldSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function ShieldSkillsGM::CoolDown(%this)
{
  %this.coolDownElapsedTime_ += 1.0;

  //%this.guiSlot_.setText(%this.coolDownTime_ - %this.coolDownElapsedTime_);

  if (%this.coolDownElapsedTime_ >= %this.coolDownTime_)
  {
    %this.cooling_ = false;
    return;
  }

  %this.schedule(1000, "CoolDown");
}

$skill = "";

if (isObject(SkillsGMServerSO))
{
  $skill = new ScriptObject()
  {
    class = "ShieldSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    shieldSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
    power_ = 10.0;
  };
}
