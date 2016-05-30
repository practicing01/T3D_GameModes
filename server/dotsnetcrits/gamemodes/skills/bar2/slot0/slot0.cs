function HealSkillsGM::Action(%this, %client, %guiSlot)
{
  if (%this.cooling_)
  {
    return;
  }

  %player = %client.getControlObject();

  if (%player.isField("silenceSet_"))
  {
    if (%player.silenceSet_.getCount() > 0)
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
      /*if (%obj == %player)
      {
        continue;
      }*/

      if (!%obj.isField("healSet_"))
      {
        %obj.healSet_ = new SimSet();
        %this.healSets_.add(%obj.healSet_);
      }

      %targetEmitterNode = new ParticleEmitterNode()
      {
        datablock = HealEmitterNodeData;
        emitter = HealEmitter;
        active = true;
        velocity = 0.0;
      };

      %heal = new ScriptObject()
      {
        class = "HealInstanceSkillsGM";
        emitterNode_ = %targetEmitterNode;
      };

      %obj.mountObject(%targetEmitterNode, 1, MatrixCreate("0 0 1", "1 0 0 0"));

      %obj.healSet_.add(%heal);

      %heal.schedule(%this.duration_ * 1000, "delete");

      /*%objArmor = 0;

      if (%obj.isField("shieldSet_"))
      {
        for (%x = 0; %x < %obj.shieldSet_.getCount(); %x++)
        {
          %objArmor += %obj.shieldSet_.getObject(%x).power_;
        }
      }*/

      %obj.applyRepair(%this.power_);
    }
  }

  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }

  %this.emitterNode_ = new ParticleEmitterNode()
  {
    datablock = HealEmitterNodeData;
    emitter = HealEmitter;
    active = true;
    velocity = 0.0;
  };

  %player.mountObject(%this.emitterNode_, 1, MatrixCreate("0 0 1", "1 0 0 0"));

  %this.schedule(%this.emitterDuration_ * 1000, "RemoveEmitter");

  %this.cooling_ = true;
  //%this.guiSlot_.setText(%this.coolDownTime_);
  %this.coolDownElapsedTime_ = 0.0;
  %this.schedule(1000, "CoolDown");

  %player.setActionThread("Celebrate_01", false);
}

function HealInstanceSkillsGM::onRemove(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function HealSkillsGM::onAdd(%this)
{
  %this.healSets_ = new SimSet();
}

function HealSkillsGM::onRemove(%this)
{
  if (isObject(%this.healSets_))
  {
    %this.healSets_.callOnChildren("deleteAllObjects");
    %this.healSets_.deleteAllObjects();
    %this.healSets_.delete();
  }

  cancel(%this.cdSchedule_);
}

function HealSkillsGM::RemoveEmitter(%this)
{
  if (isObject(%this.emitterNode_))
  {
    %this.emitterNode_.delete();
  }
}

function HealSkillsGM::CoolDown(%this)
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
    class = "HealSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 1.0;
    coolDownElapsedTime_ = 0.0;
    cooling_ = false;
    duration_ = 1.0;
    healSets_ = "";
    cdSchedule_ = "";
    emitterNode_ = "";
    emitterDuration_ = 1.0;
    emitterSchedule_ = "";
    sphereCastRadius_ = 2.0;
    power_ = 10.0;
  };
}
