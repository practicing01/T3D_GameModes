function SprintSkillsGM::Action(%this, %client, %guiSlot)
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

  //%rayResult = %player.doRaycast(1000.0, $TypeMasks::ShapeBaseObjectType);

  //%obj = firstWord(%rayResult);

  /*if (!isObject(%obj))
  {
    return;
  }*/

  %obj = %player;

  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    if (!%obj.isField("sprintSet_"))
    {
      %obj.sprintSet_ = new SimSet();
      %this.sprintSets_.add(%obj.sprintSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = SprintEmitterNodeData;
      emitter = SprintEmitter;
      active = true;
      velocity = 0.0;
    };

    %sprint = new ScriptObject()
    {
      class = "SprintInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
    };

    %obj.mountObject(%targetEmitterNode, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %obj.sprintSet_.add(%sprint);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = SprintEmitterNodeData;
      emitter = SprintEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %sprint.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);

    %obj.setVelocity(VectorScale(%obj.getForwardVector(), %this.velocityMagnitude_));
  }

}

function SprintInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function SprintSkillsGM::onAdd(%this)
{
  %this.sprintSets_ = new SimSet();
}

function SprintSkillsGM::onRemove(%this)
{
  if (isObject(%this.sprintSets_))
  {
    %this.sprintSets_.callOnChildren("deleteAllObjects");
    %this.sprintSets_.deleteAllObjects();
    %this.sprintSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function SprintSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function SprintSkillsGM::CoolDown(%this)
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
    class = "SprintSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 1.0;
    sprintSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
    velocityMagnitude_ = 1000.0;
  };
}
