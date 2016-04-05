function CloakSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %this.guiSlot_ = %guiSlot;

  %player = %client.getControlObject();

  if (%player.isField("cloakSet_"))
  {
    if (%player.cloakSet_.getCount() > 0)
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
    if (!%obj.isField("cloakSet_"))
    {
      %obj.cloakSet_ = new SimSet();
      %this.cloakSets_.add(%obj.cloakSet_);
    }

    /*%targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = CloakEmitterNodeData;
      emitter = CloakEmitter;
      active = true;
      velocity = 0.0;
    };*/

    %cloak = new ScriptObject()
    {
      class = "CloakInstanceSkillsGM";
      //emitterNode_ = %targetEmitterNode;
      target_ = %obj;
    };

    %obj.setCloaked(true);

    //%obj.mountObject(%targetEmitterNode, GetMountIndexDNC(%obj, 0));

    %obj.cloakSet_.add(%cloak);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = CloakEmitterNodeData;
      emitter = CloakEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, GetMountIndexDNC(%player, 0));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %cloak.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);
  }

}

function CloakInstanceSkillsGM::onRemove(%this)
{
  /*if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }*/

  if (isObject(%this.target_))
  {
    if (%this.target_.cloakSet_.getCount() == 1)
    {
      %this.target_.setCloaked(false);
    }
  }
}

function CloakSkillsGM::onAdd(%this)
{
  %this.cloakSets_ = new SimSet();
}

function CloakSkillsGM::onRemove(%this)
{
  if (isObject(%this.cloakSets_))
  {
    %this.cloakSets_.callOnChildren("deleteAllObjects");
    %this.cloakSets_.deleteAllObjects();
    %this.cloakSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function CloakSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function CloakSkillsGM::CoolDown(%this)
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
    class = "CloakSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    cloakSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
