function StaticamGMClient::Move(%this, %dir)
{
  commandToServer('MoveStaticamGM', %dir);
}

function StaticamGMClient::Rotate(%this, %dir)
{
  commandToServer('RotateStaticamGM', %dir);
}

function StaticamGMClient::Fire(%this)
{
  commandToServer('FireStaticamGM');
}

function clientCmdRebindStaticamGM()
{
  if (isObject(StaticamGMClientSO))
  {
    StaticamGMClientSO.Rebind();
  }
}

function StaticamGMClient::Rebind(%this)
{
  %this.actionMap_.bindCmd(keyboard, "w", %this @ ".Move(1);", %this @ ".Move(0);");
  %this.actionMap_.bindCmd(keyboard, "a", %this @ ".Rotate(-1);", %this @ ".Rotate(0);");
  %this.actionMap_.bindCmd(keyboard, "s", %this @ ".Move(-1);", %this @ ".Move(0);");
  %this.actionMap_.bindCmd(keyboard, "d", %this @ ".Rotate(1);", %this @ ".Rotate(0);");
  %this.actionMap_.bindCmd(mouse0, "button0", "", %this @ ".Fire();");
  %this.actionMap_.push();
}

function StaticamGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "StaticamGMClientQueue";

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "w", %this @ ".Move(1);", %this @ ".Move(0);");
  %this.actionMap_.bindCmd(keyboard, "a", %this @ ".Rotate(-1);", %this @ ".Rotate(0);");
  %this.actionMap_.bindCmd(keyboard, "s", %this @ ".Move(-1);", %this @ ".Move(0);");
  %this.actionMap_.bindCmd(keyboard, "d", %this @ ".Rotate(1);", %this @ ".Rotate(0);");
  %this.actionMap_.bindCmd(mouse0, "button0", "", %this @ ".Fire();");
  %this.actionMap_.push();
}

function StaticamGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.actionMap_))
  {
    %this.actionMap_.pop();
    %this.actionMap_.delete();

    if (isObject(DNCClient))
    {
      DNCClient.actionMap_.push();
    }
  }

  echo("StaticamGMClient go bye bye");
}

if (isObject(StaticamGMClientSO))
{
  StaticamGMClientSO.delete();
}
else
{
  new ScriptObject(StaticamGMClientSO)
  {
    class = "StaticamGMClient";
    EventManager_ = "";
    actionMap_ = "";
  };
}
