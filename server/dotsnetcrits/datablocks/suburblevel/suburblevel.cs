function SuburbLevel::onAdd(%this)
{
  DNCServer.loadOutListeners_.add(%this);

  %this.unoccupiedTriggers_ = new SimSet();
  %this.moveableTiles_ = new SimSet();
}

function SuburbLevel::onRemove(%this)
{
  %this.unoccupiedTriggers_.delete();
  %this.moveableTiles_.delete();
}

function SuburbLevel::loadOut(%this, %player)
{
  %this.unoccupiedTriggers_.clear();
  %this.moveableTiles_.clear();

  %triggerSet = Triggers;//MissionGroup.findObjectByInternalName("Triggers");

  for (%x = 0; %x < %triggerSet.getCount(); %x++)
  {
    %trigger = %triggerSet.getObject(%x);

    if (%trigger.occupied_ == 0)
    {
      %this.unoccupiedTriggers_.add(%trigger);
    }
  }

  %tileSet = tiles;//MissionGroup.findObjectByInternalName("tiles");

  for (%x = 0; %x < %this.unoccupiedTriggers_.getCount(); %x++)
  {
    %trigger = %this.unoccupiedTriggers_.getObject(%x);

    for (%y = 0; %y < %tileSet.getCount(); %y++)
    {
      %tile = %tileSet.getObject(%y);

      if (%trigger.position $= %tile.position)
      {
        %this.moveableTiles_.add(%tile);
      }
    }
  }

  while (%this.moveableTiles_.getCount() > 1)
  {
    %tileA = %this.moveableTiles_.getRandom();
    %this.moveableTiles_.remove(%tileA);
    %tileB = %this.moveableTiles_.getRandom();
    %this.moveableTiles_.remove(%tileB);

    %tmpPos = %tileA.position;

    %tileA.position = %tileB.position;
    %tileB.position = %tmpPos;
  }
}

function SuburbTileTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (!%trigger.isField("occupied_"))
  {
    %trigger.occupied_ = 0;
  }

  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    %trigger.occupied_++;
  }
}

function SuburbTileTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer")
  {
    %trigger.occupied_--;
  }
}
