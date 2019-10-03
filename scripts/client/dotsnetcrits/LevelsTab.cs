function LevelList::onVisible(%this, %state)
{
  if (%state)
  {
    %this.clear();

    %dirList = getDirectoryList("scripts/client/dotsnetcrits/levels/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      %this.addRow(%x, getField(%dirList, %x));
    }
  }
}

function LevelButt::onClick(%this)
{
  %text = LevelList.getRowText(LevelList.getSelectedRow());

  commandToServer('LevelVoteDNC', %text);
}

function LevelList::onSelect(%this, %cellID, %text)
{
  exec("scripts/client/dotsnetcrits/levels/" @ %text @ "/preview.cs");
}
