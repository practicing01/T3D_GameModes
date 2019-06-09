function Skit3dGMClient::Move(%this, %dir, %state)
{
  commandToServer('MoveSkit3dGM', %dir, %state);
}

function Skit3dGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "Skit3dGMClientQueue";

  %this.actionMap_ = new ActionMap();
  %this.actionMap_.bindCmd(keyboard, "w", %this @ ".Move(0, 1);", %this @ ".Move(0, 0);");
  %this.actionMap_.bindCmd(keyboard, "a", %this @ ".Move(1, 1);", %this @ ".Move(1, 0);");
  %this.actionMap_.bindCmd(keyboard, "s", %this @ ".Move(2, 1);", %this @ ".Move(2, 0);");
  %this.actionMap_.bindCmd(keyboard, "d", %this @ ".Move(3, 1);", %this @ ".Move(3, 0);");
  %this.actionMap_.push();

  DNCCrosshair.visible = false;
}

function Skit3dGMClient::onRemove(%this)
{
  DNCCrosshair.visible = true;

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

  echo("Skit3dGMClient go bye bye");
}

if (isObject(Skit3dGMClientSO))
{
  Skit3dGMClientSO.delete();
}
else
{
  new ScriptObject(Skit3dGMClientSO)
  {
    class = "Skit3dGMClient";
    EventManager_ = "";
    actionMap_ = "";
  };
}
