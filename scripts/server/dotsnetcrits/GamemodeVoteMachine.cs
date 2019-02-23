function GameModeVoteMachine::onAdd(%this)
{
  %this.tally_ = new ArrayObject();
}

function GameModeVoteMachine::onRemove(%this)
{
  if (isObject(%this.tally_))
  {
    %this.tally_.delete();
  }
}

function GameModeVoteMachine::onGamemodeVoteCast(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %index = %this.tally_.getIndexFromKey(%client);
  %gamemode = %data.getValue(%data.getIndexFromKey("gamemode"));

  centerPrintAll("Vote called for gamemode" SPC %gamemode, 2, 1);

  if (%index == -1)
  {
    %this.tally_.add(%client, %gamemode);
  }
  else
  {
    %this.tally_.setValue(%gamemode, %index);
  }

  if (isObject(ClientGroup))
  {
    if (%this.tally_.count() == ClientGroup.getCount())
    {
      if (isObject(DNCServer))
      {
        %uniqueValueArray = new ArrayObject();

        %uniqueValueArray.duplicate(%this.tally_);

        %uniqueValueArray.uniqueValue();

        %gamemodeArray = new ArrayObject();

        for (%x = 0; %x < %uniqueValueArray.count(); %x++)
        {
          %gm = %uniqueValueArray.getValue(%x);
          %gamemodeArray.add(%gm, %this.tally_.countValue(%gm));
        }

        %gamemodeArray.sortnd();

        %chosenGM = %gamemodeArray.getKey(0);

        %uniqueValueArray.delete();
        %gamemodeArray.delete();

        if (%chosenGM !$= "")
        {
          DNCServer.EventManager_.postEvent("GamemodeVoteTallied", %chosenGM);
        }

      }

      %this.tally_.empty();
    }

  }
}
