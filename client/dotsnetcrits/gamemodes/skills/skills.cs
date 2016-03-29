function SkillsGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "SkillsGMClientQueue";

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.delete();
  }

  %this.actionMap_ = new ActionMap();

  for (%x = 0; %x < 10; %x++)
  {
    %this.actionMap_.bindCmd(keyboard, "" @ %x, "", %this @ ".SkillAction(" @ %x @ ");");
  }

  %this.actionMap_.bind(mouse0, "zaxis", SkillbarCycleSkillsGMClient);
  %this.actionMap_.push();

  if (!isObject(GuiModelessControlProfile))
  {
    new GuiControlProfile( GuiModelessControlProfile : GuiDefaultProfile )
    {
      modal = false;
    };
  }

  if (!isObject(GuiModelessButtonProfile))
  {
    new GuiControlProfile( GuiModelessButtonProfile : GuiButtonProfile )
    {
      modal = false;
      justify = "center";
    };
  }

  exec("art/gui/dotsnetcrits/skills/Skills.gui");

  Canvas.pushDialog(Skillbar);
}

function SkillsGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.pop();
    %this.actionMap_.delete();
  }

  if (isObject(Skillbar))
  {
    Canvas.popDialog(Skillbar);
    Skillbar.delete();
  }
  echo("SkillsGMClient go bye bye");
}

function SkillsGMClient::SkillAction(%this, %slot)
{
  if (%slot == 1)
  {
    echo("slought");
  }
  commandToServer('SkillActionTagGM');
}

//function SkillsGMClient::SkillbarCycle(%this, %val)
function SkillbarCycleSkillsGMClient(%val)
{
  if (!isObject(Skillbar))
  {
    return;
  }

  Skillbar.getObject(SkillsGMClientSO.curBar_).setVisible(false);

  if (%val < 0)//next
  {
    SkillsGMClientSO.curBar_++;

    if (SkillsGMClientSO.curBar_ >= Skillbar.getCount())
    {
      SkillsGMClientSO.curBar_ = 0;
    }
  }
  else if (%val > 0)//prev
  {
    SkillsGMClientSO.curBar_--;

    if (SkillsGMClientSO.curBar_ < 0)
    {
      SkillsGMClientSO.curBar_ = Skillbar.getCount() - 1;
    }
  }

  Skillbar.getObject(SkillsGMClientSO.curBar_).setVisible(true);
}

if (isObject(SkillsGMClientSO))
{
  SkillsGMClientSO.delete();
}
else
{
  new ScriptObject(SkillsGMClientSO)
  {
    class = "SkillsGMClient";
    EventManager_ = "";
    actionMap_ = "";
    curBar_ = 0;
  };
}
