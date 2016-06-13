function CleanseSkillsGM::Action(%this, %client, %guiSlot)
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
    if (!%obj.isField("cleanseSet_"))
    {
      %obj.cleanseSet_ = new SimSet();
      %this.cleanseSets_.add(%obj.cleanseSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = CleanseEmitterNodeData;
      emitter = CleanseEmitter;
      active = true;
      velocity = 0.0;
    };

    %cleanse = new ScriptObject()
    {
      class = "CleanseInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
    };

    %obj.mountObject(%targetEmitterNode, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %obj.cleanseSet_.add(%cleanse);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = CleanseEmitterNodeData;
      emitter = CleanseEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %cleanse.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);

    if (%obj.isField("silenceSet_"))
    {
      %obj.silenceSet_.deleteAllObjects();
    }

    if (%obj.isField("blindSet_"))
    {
      %obj.blindSet_.deleteAllObjects();
    }

    if (%obj.isField("poisonSet_"))
    {
      %obj.poisonSet_.deleteAllObjects();
    }

    if (%obj.isField("snareSet_"))
    {
      %obj.snareSet_.deleteAllObjects();
    }
  }

}

function CleanseInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function CleanseSkillsGM::onAdd(%this)
{
  %this.cleanseSets_ = new SimSet();
}

function CleanseSkillsGM::onRemove(%this)
{
  if (isObject(%this.cleanseSets_))
  {
    %this.cleanseSets_.callOnChildren("deleteAllObjects");
    %this.cleanseSets_.deleteAllObjects();
    %this.cleanseSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function CleanseSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function CleanseSkillsGM::CoolDown(%this)
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
    class = "CleanseSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    cleanseSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
