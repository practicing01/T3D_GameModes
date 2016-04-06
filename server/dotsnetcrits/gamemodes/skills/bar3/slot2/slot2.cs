function PoisonSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %this.guiSlot_ = %guiSlot;

  %player = %client.getControlObject();

  if (%player.isField("poisonSet_"))
  {
    if (%player.poisonSet_.getCount() > 0)
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
    if (!%obj.isField("poisonSet_"))
    {
      %obj.poisonSet_ = new SimSet();
      %this.poisonSets_.add(%obj.poisonSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = PoisonEmitterNodeData;
      emitter = PoisonEmitter;
      active = true;
      velocity = 0.0;
    };

    %poison = new ScriptObject()
    {
      class = "PoisonInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
      pulseInterval_ = 1.0;
      pulseIntervalCount_ = 0;
      pulseDuration_ = %this.duration_;
      target_ = %obj;
      power_ = 10.0;
    };

    %obj.mountObject(%targetEmitterNode, GetMountIndexDNC(%obj, 0));

    %obj.poisonSet_.add(%poison);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = PoisonEmitterNodeData;
      emitter = PoisonEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, GetMountIndexDNC(%player, 0));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %poison.schedule(0, "Pulse");
    %poison.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);
  }

}

function PoisonInstanceSkillsGM::Pulse(%this)
{
  if (%this.pulseIntervalCount_ >= %this.pulseDuration_)
  {
    return;
  }

  %this.pulseIntervalCount_++;

  if (isObject(%this.target_))
  {
    %objArmor = 0;

    if (%this.target_.isField("shieldSet_"))
    {
      for (%x = 0; %x < %this.target_.shieldSet_.getCount(); %x++)
      {
        %objArmor += %this.target_.shieldSet_.getObject(%x).power_;
      }
    }

    %this.target_.damage(%this, %this.target_.position, getMax(0, %this.power_ - %objArmor), "poison");
  }

  %this.schedule(%this.pulseInterval_ * 1000, "Pulse");
}

function PoisonInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function PoisonSkillsGM::onAdd(%this)
{
  %this.poisonSets_ = new SimSet();
}

function PoisonSkillsGM::onRemove(%this)
{
  if (isObject(%this.poisonSets_))
  {
    %this.poisonSets_.callOnChildren("deleteAllObjects");
    %this.poisonSets_.deleteAllObjects();
    %this.poisonSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function PoisonSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function PoisonSkillsGM::CoolDown(%this)
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
    class = "PoisonSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    poisonSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
