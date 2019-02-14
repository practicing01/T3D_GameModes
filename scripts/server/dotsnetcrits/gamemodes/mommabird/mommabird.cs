function MommaBirdAI::onDisabled(%this, %ai, %state)
{
  %this.onDamage(%ai, 1000);

  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
}

function MommaBirdAI::onDamage(%this, %ai, %delta)
{
  %player = %ai.client.getControlObject();
  %player.damage(%ai, "0 0 0", %delta, "MommaBirdAI");
}

function MommaBirdGMServer::TransformNPC(%this, %player, %npc)
{
  %teleDir = %player.getForwardVector();

  %size = %player.getObjectBox();
  %scale = %player.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %npc.rotation = %player.rotation;

  %sizeTarget = %npc.getObjectBox();
  %scaleTarget = %npc.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %npc.position = VectorAdd( %player.position, VectorScale(%teleDir, %sizex + %sizexTarget) );
}

function MommaBirdGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "MommaBirdGMServerQueue";

  %this.dictionary_ = new ArrayObject();
  %this.npcs_ = new SimSet();

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %player = %client.getControlObject();

    %npc = new AiPlayer()
    {
      dataBlock = MommaBirdAI;
      class = MommaBirdAIClass;
      client = %client;
    };

    %this.npcs_.add(%npc);

    %this.TransformNPC(%player, %npc);

    %npc.followObject(%player, 1);

    %this.dictionary_.add(%client, %npc);
  }
}

function MommaBirdGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.npcs_))
  {
    %this.npcs_.deleteAllObjects();
    %this.npcs_.delete();
  }

  if (isObject(%this.dictionary_))
  {
    %this.dictionary_.delete();
  }
}

function MommaBirdGMServerSO::loadOut(%this, %player)
{
  %client = %player.client;
  %npc = %this.dictionary_.getValue(%this.dictionary_.getIndexFromKey(%client));

  %this.TransformNPC(%player, %npc);
  %npc.followObject(%player, 1);
}

if (isObject(MommaBirdGMServerSO))
{
  MommaBirdGMServerSO.delete();
}
else
{
  new ScriptObject(MommaBirdGMServerSO)
  {
    class = "MommaBirdGMServer";
    EventManager_ = "";
    npcs_ = "";
    dictionary_ = "";
  };

  DNCServer.loadOutListeners_.add(MommaBirdGMServerSO);
  MissionCleanup.add(MommaBirdGMServerSO);
}
