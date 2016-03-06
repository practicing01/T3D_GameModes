function WeaponTab::onVisible(%this, %state)
{
  if (%state)
  {
    %list = %this.getObject(0);

    if (!isObject(%list)){return;}

    %list.clear();

    %dirList = getDirectoryList("scripts/client/dotsnetcrits/weapons/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      %list.addRow(%x, getField(%dirList, %x));
    }
  }
}

function WeaponButt::onClick(%this)
{
  %list = %this.getParent().getObject(0);
  %text = %list.getRowText(%list.getSelectedRow());

  commandToServer('WeaponLoadDNC', %text);
}
