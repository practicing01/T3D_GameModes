function SkillsGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "SkillsGMServerQueue";

}

function SkillsGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

}

function SkillsGMServer::SkillAction(%this, %client)
{
  echo("SkillAction");
}

function serverCmdSkillActionTagGM(%client)
{
  if (isObject(SkillsGMServerSO))
  {
    SkillsGMServerSO.SkillAction(%client);
  }
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
  };
}
