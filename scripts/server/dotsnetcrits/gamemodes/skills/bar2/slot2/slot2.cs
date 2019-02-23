function HOTSkillsGM::Action(%this, %client, %guiSlot)
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
    if (!%obj.isField("hotSet_"))
    {
      %obj.hotSet_ = new SimSet();
      %this.hotSets_.add(%obj.hotSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = HOTEmitterNodeData;
      emitter = HOTEmitter;
      active = true;
      velocity = 0.0;
    };

    %hot = new ScriptObject()
    {
      class = "HOTInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
      pulseInterval_ = 1.0;
      pulseIntervalCount_ = 0;
      pulseDuration_ = %this.duration_;
      target_ = %obj;
      power_ = 10.0;
    };

    %obj.mountObject(%targetEmitterNode, 0, MatrixCreate("0 0 0.1", "1 0 0 0"));

    %obj.hotSet_.add(%hot);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = SummonCircle3EmitterNodeData;
      emitter = SummonCircle3Emitter;
      active = true;
    };

    %player.mountObject(%this.emitterNode_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %hot.schedule(0, "Pulse");
    %hot.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("shoot", false);
    %player.playAudio(0, hotSpellSound);
  }

}

function HOTInstanceSkillsGM::Pulse(%this)
{
  if (%this.pulseIntervalCount_ >= %this.pulseDuration_)
  {
    return;
  }

  %this.pulseIntervalCount_++;

  if (isObject(%this.target_))
  {
    /*%objArmor = 0;

    if (%this.target_.isField("shieldSet_"))
    {
      for (%x = 0; %x < %this.target_.shieldSet_.getCount(); %x++)
      {
        %objArmor += %this.target_.shieldSet_.getObject(%x).power_;
      }
    }

    %this.target_.damage(%this, %this.target_.position, getMax(0, %this.power_ - %objArmor), "hot");*/

    %this.target_.applyRepair(%this.power_);
  }

  %this.schedule(%this.pulseInterval_ * 1000, "Pulse");
}

function HOTInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function HOTSkillsGM::onAdd(%this)
{
  %this.hotSets_ = new SimSet();
}

function HOTSkillsGM::onRemove(%this)
{
  if (isObject(%this.hotSets_))
  {
    %this.hotSets_.callOnChildren("deleteAllObjects");
    %this.hotSets_.deleteAllObjects();
    %this.hotSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function HOTSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function HOTSkillsGM::CoolDown(%this)
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
    class = "HOTSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    hotSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
