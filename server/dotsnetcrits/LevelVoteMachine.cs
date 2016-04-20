function LevelVoteMachine::onAdd(%this)
{
  %this.tally_ = new ArrayObject();
}

function LevelVoteMachine::onRemove(%this)
{
  if (isObject(%this.tally_))
  {
    %this.tally_.delete();
  }
}

function LevelVoteMachine::onLevelVoteCast(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %index = %this.tally_.getIndexFromKey(%client);
  %level = %data.getValue(%data.getIndexFromKey("level"));

  if (%index == -1)
  {
    %this.tally_.add(%client, %level);
  }
  else
  {
    %this.tally_.setValue(%level, %index);
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

        %levelArray = new ArrayObject();

        for (%x = 0; %x < %uniqueValueArray.count(); %x++)
        {
          %lvl = %uniqueValueArray.getValue(%x);
          %levelArray.add(%lvl, %this.tally_.countValue(%lvl));
        }

        %levelArray.sortnd();

        %chosenLevel = %levelArray.getKey(0);

        %uniqueValueArray.delete();
        %levelArray.delete();

        if (%chosenLevel !$= "")
        {
          DNCServer.EventManager_.postEvent("LevelVoteTallied", %chosenLevel);
        }

      }

      %this.tally_.empty();
    }

  }
}
