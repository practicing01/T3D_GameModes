function GamemodeList::onVisible(%this, %state)
{
  if (%state)
  {
    %this.clear();

    %dirList = getDirectoryList("scripts/client/dotsnetcrits/gamemodes/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      %this.addRow(%x, getField(%dirList, %x));
    }
  }
}

function GamemodeButt::onClick(%this)
{
  %text = GamemodeList.getRowText(GamemodeList.getSelectedRow());

  commandToServer('GamemodeVoteDNC', %text);
}

function GamemodeList::onSelect(%this, %cellID, %text)
{
  exec("scripts/client/dotsnetcrits/gamemodes/" @ %text @ "/preview.cs");
}
