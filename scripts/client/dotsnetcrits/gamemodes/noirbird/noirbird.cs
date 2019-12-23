function clientCmdNoirBirdPoop(%pos)
{
  if (!isObject(NoirBirdGMClientSO))
  {
    return;
  }

  NoirBirdGMClientSO.Poop(%pos);
}

function NoirBirdGMClient::Poop(%this, %pos)
{
  %norm = "0.0 0.0 1.0";
  %rot = 0.5;
  %scale = 1.0;

  decalManagerAddDecal(%pos, %norm, %rot, %scale, "poopDecalnoirbird", true);
}

function NoirBirdGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "NoirBirdGMClientQueue";

}

function NoirBirdGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  decalManagerClear();

  echo("NoirBirdGMClient go bye bye");
}

if (isObject(NoirBirdGMClientSO))
{
  NoirBirdGMClientSO.delete();
}
else
{
  new ScriptObject(NoirBirdGMClientSO)
  {
    class = "NoirBirdGMClient";
    EventManager_ = "";
  };
}
