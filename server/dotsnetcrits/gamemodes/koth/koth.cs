function KOTHGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "KOTHGMServerQueue";

  %pos = ClientGroup.getObject(0).getControlObject().getPosition();

  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "throneSpawnKOTHGM")
    {
      %pos = %obj.position;
    }
  }

  %this.userpers_ = new SimSet();

  %this.trigger_ = new Trigger()
  {
    dataBlock = "KOTHGMTrigger";
    polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
    position = %pos;
    scale = "20 20 5";
  };

  %this.throne_ = new StaticShape()
  {
    dataBlock = "throneKOTHGM";
    position = %pos;
    rotation = "1 0 0 0";
    scale = "0.2 0.2 0.2";
  };

  %pos.z += 0.5;

  %this.plunger_ = new StaticShape()
  {
    dataBlock = "plungerKOTHGM";
    position = %pos;
    rotation = "1 0 0 0";
    scale = "4.0 4.0 4.0";
  };

}

function KOTHGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.throne_))
  {
    %this.throne_.delete();
  }

  if (isObject(%this.plunger_))
  {
    %this.plunger_.delete();
  }

  if (isObject(%this.trigger_))
  {
    %this.trigger_.delete();
  }

  if (isObject(%this.userpers_))
  {
    %this.userpers_.delete();
  }

}

function KOTHGMTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (isObject(KOTHGMServerSO))
  {
    KOTHGMServerSO.userpers_.add(%obj);

    if (KOTHGMServerSO.userpers_.getcount() == 1)
    {
      %modelFile = %obj.getModelFile();

      %shapeConstructor = "";

      for (%x = 0; %x <  TSShapeConstructorGroup.getCount(); %x++)
      {
        %TSShapeConstructor = TSShapeConstructorGroup.getObject(%x);

        if (%TSShapeConstructor.baseShape $= %modelFile)
        {
          %shapeConstructor = %TSShapeConstructor;
          break;
        }
      }

      %index = -1;

      for (%x = 0; %x <  %shapeConstructor.	getNodeCount(); %x++)
      {
        if (strstr(%shapeConstructor.getNodeName(%x), "mount") != -1)
        {
          %index++;
        }
      }

      %obj.mountObject(KOTHGMServerSO.plunger_, %index);
    }
  }
}

function KOTHGMTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (isObject(KOTHGMServerSO))
  {
    KOTHGMServerSO.userpers_.remove(%obj);

    if (KOTHGMServerSO.userpers_.getcount() > 0)
    {
      KOTHGMServerSO.userpers_.getObject(0).mountObject(KOTHGMServerSO.plunger_, 0);
    }
  }
}

function KOTHGMTrigger::onTickTrigger(%this, %trigger)
{
  Game.incScore(KOTHGMServerSO.userpers_.getObject(0).client, 1, false);
}

if (isObject(KOTHGMServerSO))
{
  KOTHGMServerSO.delete();
}
else
{
  new ScriptObject(KOTHGMServerSO)
  {
    class = "KOTHGMServer";
    EventManager_ = "";
    throne_ = "";
    plunger_ = "";
    trigger_ = "";
    userpers_ = "";
  };
}
