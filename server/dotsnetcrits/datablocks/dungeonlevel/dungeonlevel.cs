exec("scripts/server/dotsnetcrits/gamemodes/skills/bar3/slot2/slot2.cs");
exec("./ai.cs");

if (isObject(SkillsGMServerSO))
{
  $skill.delete();
}

function DungeonLevel::onAdd(%this)
{
  %this.shapePaths_ = new ArrayObject();
  %this.shapeAIStrings_ = new ArrayObject();

  DNCServer.execDirScripts("datablocks/dungeonlevel/ai", "");

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
  %degrees = "0 90 180 270";
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

    %objTransform = %prop.getTransform();
    %tryRotation = "0 0 0 0 0 1 " @ mDegToRad(getWord(%degrees, getRandom(0, 3)));
    %newTransform = matrixMultiply(%objTransform, %tryRotation);
    %prop.setTransform(%newTransform);

    %randyOffset = getRandom(-9, 9) SPC getRandom(-9, 9) SPC 0;
    %randyPos = VectorAdd(%position, %randyOffset);

    %wall = new TSStatic()
    {
      shapeName = "art/shapes/dotsnetcrits/levels/dungeon/wall/wall.cached.dts";
      position = %randyPos;
      collisionType = "Visible Mesh";
      decalType = "Visible Mesh";
      allowPlayerStep = "1";
    };

    %objTransform = %wall.getTransform();
    %tryRotation = "0 0 0 0 0 1 " @ mDegToRad(getWord(%degrees, getRandom(0, 3)));
    %newTransform = matrixMultiply(%objTransform, %tryRotation);
    %wall.setTransform(%newTransform);
  }

  %string = DungeonLevelHandle.shapeAIStrings_.getValue(getRandom(0, %this.shapeAIStrings_.count() - 1));

  %npc = new AiPlayer()
  {
    dataBlock = getWord(%string, 0);
    class = getWord(%string, 1);
    //mMoveTolerance = 1.0;
    //moveStuckTolerance = 1.0;
    //moveStuckTestDelay = 1.0;
    position = %position;
    //rotation = %zombieSpawn.rotation;
    target_ = "";
    canAttack_ = true;
  };

  %randyTarget = ClientGroup.getRandom();

  %npc.target_ = %randyTarget.getControlObject();

  %targPos = %npc.target_.getPosition();

  //setField(%targPos, 2, getField(%zombie.getPosition(), 2));

  %npc.setMoveDestination(%targPos);

  %npc.setAimObject(%npc.target_);

}

function DungeonLevel::onRemove(%this)
{
  %this.shapePaths_.delete();
  %this.shapeAIStrings_.delete();
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
