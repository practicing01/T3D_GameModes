function AOEHealSkillsGM::Action(%this, %client, %guiSlot)
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
    if (!%obj.isField("aoeHealSet_"))
    {
      %obj.aoeHealSet_ = new SimSet();
      %this.aoeHealSets_.add(%obj.aoeHealSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = AOEHealEmitterNodeData;
      emitter = AOEHealEmitter;
      active = true;
      velocity = 0.0;
      position = %pos;
    };

    %aoeHeal = new ScriptObject()
    {
      class = "AOEHealInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
      pulseInterval_ = 1.0;
      pulseIntervalCount_ = 0;
      pulseDuration_ = %this.duration_;
      target_ = %obj;
      power_ = 10.0;
      position_ = %pos;
      sphereCastRadius_ = %this.sphereCastRadius_;
    };

    //%obj.mountObject(%targetEmitterNode, GetMountIndexDNC(%obj, 0));

    %obj.aoeHealSet_.add(%aoeHeal);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = AOEHealEmitterNodeData;
      emitter = AOEHealEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, GetMountIndexDNC(%player, 0));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %aoeHeal.schedule(0, "Pulse");
    %aoeHeal.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);
  //}

}

function AOEHealInstanceSkillsGM::Pulse(%this)
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
      %obj.applyRepair(%this.power_);
    }
  }

  %this.schedule(%this.pulseInterval_ * 1000, "Pulse");
}

function AOEHealInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function AOEHealSkillsGM::onAdd(%this)
{
  %this.aoeHealSets_ = new SimSet();
}

function AOEHealSkillsGM::onRemove(%this)
{
  if (isObject(%this.aoeHealSets_))
  {
    %this.aoeHealSets_.callOnChildren("deleteAllObjects");
    %this.aoeHealSets_.deleteAllObjects();
    %this.aoeHealSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function AOEHealSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function AOEHealSkillsGM::CoolDown(%this)
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
    class = "AOEHealSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    aoeHealSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
    sphereCastRadius_ = 5.0;
  };
}
