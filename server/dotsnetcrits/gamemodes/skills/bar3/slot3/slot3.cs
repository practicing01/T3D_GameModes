function SnareSkillsGM::Action(%this, %client, %guiSlot)
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
    if (!%obj.isField("snareSet_"))
    {
      %obj.snareSet_ = new SimSet();
      %this.snareSets_.add(%obj.snareSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = SnareEmitterNodeData;
      emitter = SnareEmitter;
      active = true;
      velocity = 0.0;
    };

    %snare = new ScriptObject()
    {
      class = "SnareInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
      model_ = "";
    };

    %snare.model_ = new StaticShape()
    {
      dataBlock = "ketchupbottleSkillsGM";
      position = %obj.position;
      rotation = "1 0 0 0";
      scale = "0.3 0.3 0.3";
    };

    %obj.mountObject(%targetEmitterNode, GetMountIndexDNC(%obj, 0));

    %obj.snareSet_.add(%snare);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = SnareEmitterNodeData;
      emitter = SnareEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, GetMountIndexDNC(%player, 0));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %snare.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);

    %velocity = VectorScale(%obj.getVelocity(), -1);
    %obj.setVelocity(%velocity);
  }

}

function SnareInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }

  if (isObject(%this.model_))
  {
    %this.model_.delete();
  }
}

function SnareSkillsGM::onAdd(%this)
{
  %this.snareSets_ = new SimSet();
}

function SnareSkillsGM::onRemove(%this)
{
  if (isObject(%this.snareSets_))
  {
    %this.snareSets_.callOnChildren("deleteAllObjects");
    %this.snareSets_.deleteAllObjects();
    %this.snareSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function SnareSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function SnareSkillsGM::CoolDown(%this)
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
    class = "SnareSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    snareSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
