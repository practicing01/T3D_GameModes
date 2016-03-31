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

  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    if (!%obj.isField("silenceSet_"))
    {
      %obj.silenceSet_ = new SimSet();
      %this.silenceSets_.add(%obj.silenceSet_);
    }

    %silence = new ScriptObject()
    {
      class = "SilenceInstanceSkillsGM";
    };

    %obj.silenceSet_.add(%silence);

    %silence.schedule(%this.duration_ * 1000, "delete");
  }

}

function SilenceInstanceSkillsGM::onRemove(%this)
{
  echo("SilenceInstanceSkillsGM removed");
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
}

$skill = "";

if (isObject(SkillsGMServerSO))
{
  $skill = new ScriptObject()
  {
    class = "SilenceSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 1.0;
    cooling_ = false;
    duration_ = 5.0;
    silenceSets_ = "";
  };
}
