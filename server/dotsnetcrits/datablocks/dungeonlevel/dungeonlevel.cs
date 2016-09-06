function DungeonLevel::onAdd(%this)
{
  %this.moveableTiles_ = new SimSet();
  echo("DungeonLevel");
}

function DungeonLevel::onRemove(%this)
{
  %this.moveableTiles_.delete();
}

function DungeonTileTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    //
  }
}

/*function DungeonTileTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    //
  }
}*/
