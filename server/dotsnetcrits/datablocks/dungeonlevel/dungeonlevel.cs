function DungeonLevel::onAdd(%this)
{
  %this.shapePaths_ = new ArrayObject();

  %count = 0;

  for( %file = findFirstFile( "art/shapes/dotsnetcrits/levels/dungeon/*.cached.dts" ); %file !$= ""; %file = findNextFile() )
  {
    if (%file $= "art/shapes/dotsnetcrits/levels/dungeon/floorTile.cached.dts")
    {
      continue;
    }

    %this.shapePaths_.add(%count, %file);
    %count += 1;
  }

  DNCServer.EventManager_.registerEvent("DungeonLevelShapeSpawn");
  DNCServer.EventManager_.subscribe(%this, "DungeonLevelShapeSpawn");
}

function DungeonLevel::onDungeonLevelShapeSpawn(%this, %data)
{
  %position = %data.getValue(%data.getIndexFromKey("position"));

  for (%x = 0; %x < 4; %x++)
  {
    %shapePath = %this.shapePaths_.getValue(getRandom(0, %this.shapePaths_.count() - 1));

    %randyOffset = getRandom(-9, 9) SPC getRandom(-9, 9) SPC 0;
    %randyPos = VectorAdd(%position, %randyOffset);

    %prop = new TSStatic()
    {
      shapeName = %shapePath;
      position = %randyPos;
      collisionType = "Visible Mesh";
      decalType = "Visible Mesh";
      allowPlayerStep = "1";
    };
  }
}

function DungeonLevel::onRemove(%this)
{
  %this.shapePaths_.delete();
}

function DungeonTileTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    %floorTile = new TSStatic()
    {
      shapeName = "art/shapes/dotsnetcrits/levels/dungeon/floorTile.cached.dts";
      position = %trigger.position;
      collisionType = "Visible Mesh";
      decalType = "Visible Mesh";
      allowPlayerStep = "1";
    };

    %data = new ArrayObject();
    %data.add("position", %trigger.position);
    DNCServer.EventManager_.postEvent("DungeonLevelShapeSpawn", %data);
    %data.delete();

    %trigger.schedule(0, "delete");
  }
}

/*function DungeonTileTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    //
  }
}*/
