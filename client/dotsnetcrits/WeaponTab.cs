function WeaponList::onVisible(%this, %state)
{
  if (%state)
  {
    %this.clear();

    %dirList = getDirectoryList("scripts/client/dotsnetcrits/weapons/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      %this.addRow(%x, getField(%dirList, %x));
    }
  }
}

function WeaponButt::onClick(%this)
{
  %text = WeaponList.getRowText(WeaponList.getSelectedRow());

  commandToServer('WeaponLoadDNC', %text);
}
