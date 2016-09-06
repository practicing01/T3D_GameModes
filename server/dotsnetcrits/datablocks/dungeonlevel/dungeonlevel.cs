function DungeonLevel::onAdd(%this)
{
  %this.shapePaths_ = new ArrayObject();

  %count = 0;

  for( %file = findFirstFile( "art/shapes/dotsnetcrits/levels/dungeon/*.cached.dts" ); %file !$= ""; %file = findNextFile() )
  {
    %this.shapePaths_.add(%count, %file);
    %count += 1;
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

    schedule(0, "delete", %trigger);
  }
}

/*function DungeonTileTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    //
  }
}*/
