function SilenceSkillsGM::Action(%this, %client, %guiSlot)
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
    if (!%obj.isField("silenceSet_"))
    {
      %obj.silenceSet_ = new SimSet();
      %this.silenceSets_.add(%obj.silenceSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = SilenceEmitterNodeData;
      emitter = SilenceEmitter;
      active = true;
      velocity = 0.0;
    };

    %silence = new ScriptObject()
    {
      class = "SilenceInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
    };

    %obj.mountObject(%targetEmitterNode, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));

    %obj.silenceSet_.add(%silence);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = SummonCircle2EmitterNodeData;
      emitter = SummonCircle2Emitter;
      active = true;
    };

    %player.mountObject(%this.emitterNode_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %silence.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("shoot", false);
    %player.playAudio(0, silenceSpellSound);
  }

}

function SilenceInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function SilenceSkillsGM::onAdd(%this)
{
  %this.silenceSets_ = new SimSet();
}

function SilenceSkillsGM::onRemove(%this)
{
  if (isObject(%this.silenceSets_))
  {
    %this.silenceSets_.callOnChildren("deleteAllObjects");
    %this.silenceSets_.deleteAllObjects();
    %this.silenceSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function SilenceSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function SilenceSkillsGM::CoolDown(%this)
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
    class = "SilenceSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    silenceSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
