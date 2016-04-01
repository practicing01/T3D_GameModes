function SkillsGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "SkillsGMServerQueue";

  if (!isObject(Skillbar))
  {
    exec("art/gui/dotsnetcrits/skills/Skills.gui");
  }

  %this.skillbars_ = new SimSet();

  for (%x = 0; %x < Skillbar.getCount(); %x++)
  {
    %bar = new SimSet();
    %this.skillbars_.add(%bar);

    %dirList = getDirectoryList("scripts/server/dotsnetcrits/gamemodes/skills/bar" @ %x @ "/", 1);

    %orderedFileArray = new ArrayObject();

    for (%y = 0; %y < getFieldCount(%dirList); %y++)
    {
      %file = getField(%dirList, %y);
      %file = strlwr(%file);

      %orderedFileArray.add(%file, 0);
    }

    %orderedFileArray.sortka();

    for (%y = 0; %y < %orderedFileArray.count(); %y++)
    {
      %file = %orderedFileArray.getKey(%y);
      exec("scripts/server/dotsnetcrits/gamemodes/skills/bar" @ %x @ "/" @ %file @ "/" @ %file @ ".cs");
      %bar.add($skill);
    }

    %orderedFileArray.delete();

    /*for (%y = 0; %y < getFieldCount(%dirList); %y++)
    {
      %file = getField(%dirList, %y);
      %file = strlwr(%file);
      exec("scripts/server/dotsnetcrits/gamemodes/skills/bar" @ %x @ "/" @ %file @ "/" @ %file @ ".cs");

      %bar.add($skill);
    }*/
  }

  DNCServer.loadOutListeners_.add(%this);
}

function SkillsGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.skillbars_))
  {
    for (%x = 0; %x < %this.skillbars_.getCount(); %x++)
    {
      %this.skillbars_.getObject(%x).deleteAllObjects();
    }

    %this.skillbars_.deleteAllObjects();

    %this.skillbars_.delete();
  }

  if (isObject(Skillbar))
  {
    Skillbar.delete();
  }
}

function SkillsGMServer::SkillAction(%this, %client, %curbar, %slot)
{
  %slot--;

  if (%slot < 0)
  {
    %slot = 9;
  }

  %bar = %this.skillbars_.getObject(%curbar);

  if (%slot >= %bar.getCount())
  {
    return;
  }

  %skill = %bar.getObject(%slot);

  %skill.Action(%client, Skillbar.getObject(%curbar).getObject(%slot));
}

function serverCmdSkillActionTagGM(%client, %curbar, %slot)
{
  if (isObject(SkillsGMServerSO))
  {
    SkillsGMServerSO.SkillAction(%client, %curbar, %slot);
  }
}

function SkillsGMServer::loadOut(%this, %player)
{
  commandToClient(%player.client, 'ReloadActionMapSkillsGM', false);
}

if (isObject(SkillsGMServerSO))
{
  SkillsGMServerSO.delete();
}
else
{
  new ScriptObject(SkillsGMServerSO)
  {
    class = "SkillsGMServer";
    EventManager_ = "";
    skillbars_ = "";
  };
}
