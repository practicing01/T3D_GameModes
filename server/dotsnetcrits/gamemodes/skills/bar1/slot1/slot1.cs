function TeleportSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %this.guiSlot_ = %guiSlot;

  %player = %client.getControlObject();

  if (%player.isField("teleportSet_"))
  {
    if (%player.teleportSet_.getCount() > 0)
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

  if (!isObject(%obj))
  {
    return;
  }

  //if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  //{
    if (!%obj.isField("teleportSet_"))
    {
      %obj.teleportSet_ = new SimSet();
      %this.teleportSets_.add(%obj.teleportSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = TeleportEmitterNodeData;
      emitter = TeleportEmitter;
      active = true;
      velocity = 0.0;
    };

    %teleport = new ScriptObject()
    {
      class = "TeleportInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
    };

    %obj.mountObject(%targetEmitterNode, GetMountIndexDNC(%obj, 0));

    %obj.teleportSet_.add(%teleport);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = TeleportEmitterNodeData;
      emitter = TeleportEmitter;
      active = true;
      velocity = 0.0;
    };

    %player.mountObject(%this.emitterNode_, GetMountIndexDNC(%player, 0));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %teleport.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("Celebrate_01", false);

    %player.position = %obj.position;
  //}

}

function TeleportInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function TeleportSkillsGM::onAdd(%this)
{
  %this.teleportSets_ = new SimSet();
}

function TeleportSkillsGM::onRemove(%this)
{
  if (isObject(%this.teleportSets_))
  {
    %this.teleportSets_.callOnChildren("deleteAllObjects");
    %this.teleportSets_.deleteAllObjects();
    %this.teleportSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function TeleportSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function TeleportSkillsGM::CoolDown(%this)
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
    class = "TeleportSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 5.0;
    teleportSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
  };
}
