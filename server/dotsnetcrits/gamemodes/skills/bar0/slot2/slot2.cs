function KnockbackSkillsGM::Action(%this, %client, %guiSlot)
{
  %this.guiSlot_ = %guiSlot;
  echo(%this.guiSlot_.getName());
}

$skill = "";

if (isObject(SkillsGMServerSO))
{
  $skill = new ScriptObject()
  {
    class = "KnockbackSkillsGM";
    guiSlot_ = "";
    coolDownTime_ = 1.0;
    cooling_ = false;
  };
}