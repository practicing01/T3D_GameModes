function BlindSkillsGM::Action(%this, %client, %guiSlot)
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
    if (!%obj.isField("blindSet_"))
    {
      %obj.blindSet_ = new SimSet();
      %this.blindSets_.add(%obj.blindSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = BlindEmitterNodeData;
      emitter = BlindEmitter;
      active = true;
      velocity = 0.0;
    };

    %blind = new ScriptObject()
    {
      class = "BlindInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
    };

    %obj.mountObject(%targetEmitterNode, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %obj.blindSet_.add(%blind);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = BlindEmitterNodeData;
      emitter = BlindEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %blind.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);
  }

}

function BlindInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function BlindSkillsGM::onAdd(%this)
{
  %this.blindSets_ = new SimSet();
}

function BlindSkillsGM::onRemove(%this)
{
  if (isObject(%this.blindSets_))
  {
    %this.blindSets_.callOnChildren("deleteAllObjects");
    %this.blindSets_.deleteAllObjects();
    %this.blindSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function BlindSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function BlindSkillsGM::CoolDown(%this)
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
    class = "BlindSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    blindSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
