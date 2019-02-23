function ModelList::onVisible(%this, %state)
{
  if (%state)
  {
    %this.clear();

    %dirList = getDirectoryList("art/shapes/dotsnetcrits/actors/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      %this.addRow(%x, getField(%dirList, %x));
    }
  }
}

function ModelButt::onClick(%this)
{
  %text = ModelList.getRowText(ModelList.getSelectedRow());

  commandToServer('ModelLoadDNC', %text);
}
