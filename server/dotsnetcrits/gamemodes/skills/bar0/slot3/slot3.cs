function RangedSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %this.guiSlot_ = %guiSlot;

  %player = %client.getControlObject();

  if (%player.isField("blindSet_"))
  {
    if (%player.blindSet_.getCount() > 0)
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
    if (!%obj.isField("rangedSet_"))
    {
      %obj.rangedSet_ = new SimSet();
      %this.rangedSets_.add(%obj.rangedSet_);
    }

    %targetEmitterNode = new ParticleEmitterNode()
    {
      datablock = RangedEmitterNodeData;
      emitter = RangedEmitter;
      active = true;
      velocity = 0.0;
    };

    %ranged = new ScriptObject()
    {
      class = "RangedInstanceSkillsGM";
      emitterNode_ = %targetEmitterNode;
    };

    %obj.mountObject(%targetEmitterNode, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));

    %obj.rangedSet_.add(%ranged);

    if (isObject(%this.emitterNode_))
    {
      %this.emitterNode_.delete();
    }

    %this.emitterNode_ = new ParticleEmitterNode()
    {
      datablock = SummonCircle1EmitterNodeData;
      emitter = SummonCircle1Emitter;
      active = true;
    };

    %player.mountObject(%this.emitterNode_, 1, MatrixCreate("0 0 0.1", "1 0 0 0"));

    %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

    %ranged.schedule(%this.duration_ * 1000, "delete");

    %this.cooling_ = true;
    //%this.guiSlot_.setText(%this.coolDownTime_);
    %this.coolDownElapsedTime_ = 0.0;
    %this.schedule(1000, "CoolDown");

    %player.setActionThread("shoot", false);

    %projectileVelocity = VectorScale(%player.getEyeVector(), %this.velocityMagnitude_);

    %projectile = new Projectile()
    {
      datablock = RangedSkillsGMProjectile;
      initialPosition = %player.getEyePoint();
      initialVelocity = %projectileVelocity;
      sourceObject = %player;
      sourceSlot = 0;
      client = %player.client;
    };
  }

}

function RangedInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function RangedSkillsGM::onAdd(%this)
{
  %this.rangedSets_ = new SimSet();
}

function RangedSkillsGM::onRemove(%this)
{
  if (isObject(%this.rangedSets_))
  {
    %this.rangedSets_.callOnChildren("deleteAllObjects");
    %this.rangedSets_.deleteAllObjects();
    %this.rangedSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function RangedSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function RangedSkillsGM::CoolDown(%this)
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
    class = "RangedSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 5.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 1.0;
    rangedSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
    velocityMagnitude_ = 2.0;
  };
}
