function SilenceSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %this.guiSlot_ = %guiSlot;

  %player = %client.getControlObject();

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

    %obj.mountObject(%targetEmitterNode, GetMountIndexDNC(%obj, 0));

    %obj.silenceSet_.add(%silence);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = SilenceEmitterNodeData;
      emitter = SilenceEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, GetMountIndexDNC(%player, 0));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %silence.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;

    %this.schedule(%this.coolDownTime_ * 1000, "CoolDown");

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
  %this.silenceSets_.callOnChildren("deleteAllObjects");
  %this.silenceSets_.deleteAllObjects();
  %this.silenceSets_.delete();

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
  %this.cooling_ = false;
}

$skill = "";

if (isObject(SkillsGMServerSO))
{
  $skill = new ScriptObject()
  {
    class = "SilenceSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    cooling_ = false;
    duration_ = 5.0;
    silenceSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
