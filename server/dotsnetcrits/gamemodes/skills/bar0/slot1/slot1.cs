function MeleeSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %player = %client.getControlObject();

  if (%player.isField("blindSet_"))
  {
    if (%player.blindSet_.getCount() > 0)
    {
      return;
    }
  }

  %pos = %player.getPosition();

  %this.guiSlot_ = %guiSlot;

  initContainerRadiusSearch(%pos, %this.sphereCastRadius_, $TypeMasks::ShapeBaseObjectType);

  while ( (%obj = containerSearchNext()) != 0 )
  {
    if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
    {
      if (%obj == %player)
      {
        continue;
      }

      if (!%obj.isField("meleeSet_"))
      {
        %obj.meleeSet_ = new SimSet();
        %this.meleeSets_.add(%obj.meleeSet_);
      }

      %targetEmitterNode = new ParticleEmitterNode()
      {
        datablock = MeleeEmitterNodeData;
        emitter = MeleeEmitter;
        active = true;
        velocity = 0.0;
      };

      %melee = new ScriptObject()
      {
        class = "MeleeInstanceSkillsGM";
        emitterNode_ = %targetEmitterNode;
      };

      %obj.mountObject(%targetEmitterNode, 1, MatrixCreate("0 0 1", "1 0 0 0"));

      %obj.meleeSet_.add(%melee);

      %melee.schedule(%this.duration_ * 1000, "delete");

      %objArmor = 0;

      if (%obj.isField("shieldSet_"))
      {
        for (%x = 0; %x < %obj.shieldSet_.getCount(); %x++)
        {
          %objArmor += %obj.shieldSet_.getObject(%x).power_;
        }
      }

      %obj.damage(%player, %pos, getMax(0, %this.power_ - %objArmor), "melee");
    }
  }

  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }

  %this.emitterNode_ = new ParticleEmitterNode()
  {
    datablock = MeleeEmitterNodeData;
    emitter = MeleeEmitter;
    active = true;
    velocity = 0.0;
  };

  %player.mountObject(%this.emitterNode_, 1, MatrixCreate("0 0 1", "1 0 0 0"));

  %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

  %this.cooling_ = true;
  //%this.guiSlot_.setText(%this.coolDownTime_);
  %this.coolDownElapsedTime_ = 0.0;
  %this.schedule(1000, "CoolDown");

  %player.setActionThread("shoot", false);
}

function MeleeInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function MeleeSkillsGM::onAdd(%this)
{
  %this.meleeSets_ = new SimSet();
}

function MeleeSkillsGM::onRemove(%this)
{
  if (isObject(%this.meleeSets_))
  {
    %this.meleeSets_.callOnChildren("deleteAllObjects");
    %this.meleeSets_.deleteAllObjects();
    %this.meleeSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function MeleeSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function MeleeSkillsGM::CoolDown(%this)
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
    class = "MeleeSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 1.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 1.0;
    meleeSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
    sphereCastRadius_ = 2.0;
    power_ = 10.0;
  };
}
