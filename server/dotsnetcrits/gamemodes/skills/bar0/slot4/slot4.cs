function AOESkillsGM::Action(%this, %client, %guiSlot)
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

  %mask = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;

  %rayResult = %player.doRaycast(1000.0, %mask);

  %obj = firstWord(%rayResult);

  %pos = getWord(%rayResult, 1) SPC getWord(%rayResult, 2) SPC getWord(%rayResult, 3);

  if (!isObject(%obj))
  {
    return;
  }

  //if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  //{
    if (!%obj.isField("aoeSet_"))
    {
      %obj.aoeSet_ = new SimSet();
      %this.aoeSets_.add(%obj.aoeSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = AOEEmitterNodeData;
      emitter = AOEEmitter;
      active = true;
      velocity = 0.0;
      position = %pos;
    };

    %aoe = new ScriptObject()
    {
      class = "AOEInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
      pulseInterval_ = 1.0;
      pulseIntervalCount_ = 0;
      pulseDuration_ = %this.duration_;
      target_ = %obj;
      power_ = 10.0;
      position_ = %pos;
      sphereCastRadius_ = %this.sphereCastRadius_;
    };

    //%obj.mountObject(%targetEmitterNode, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %obj.aoeSet_.add(%aoe);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = AOEEmitterNodeData;
      emitter = AOEEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, 0, MatrixCreate("0 0 1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %aoe.schedule(0, "Pulse");
    %aoe.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);
  //}

}

function AOEInstanceSkillsGM::Pulse(%this)
{
  if (%this.pulseIntervalCount_ >= %this.pulseDuration_)
  {
    return;
  }

  %this.pulseIntervalCount_++;

  initContainerRadiusSearch(%this.position_, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

  while ( (%obj = containerSearchNext()) != 0 )
  {
    if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
    {
      if (%obj.isField("shieldSet_"))
      {
        for (%x = 0; %x < %obj.shieldSet_.getCount(); %x++)
        {
          %objArmor += %obj.shieldSet_.getObject(%x).power_;
        }
      }

      %obj.damage(%this, %this.position_, getMax(0, %this.power_ - %objArmor), "aoe");
    }
  }

  %this.schedule(%this.pulseInterval_ * 1000, "Pulse");
}

function AOEInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function AOESkillsGM::onAdd(%this)
{
  %this.aoeSets_ = new SimSet();
}

function AOESkillsGM::onRemove(%this)
{
  if (isObject(%this.aoeSets_))
  {
    %this.aoeSets_.callOnChildren("deleteAllObjects");
    %this.aoeSets_.deleteAllObjects();
    %this.aoeSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function AOESkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function AOESkillsGM::CoolDown(%this)
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
    class = "AOESkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    aoeSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
    sphereCastRadius_ = 5.0;
  };
}
