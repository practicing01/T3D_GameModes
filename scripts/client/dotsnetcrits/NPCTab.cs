function NPCList::onVisible(%this, %state)
{
  if (%state)
  {
    %this.clear();

    %dirList = getDirectoryList("scripts/client/dotsnetcrits/npcs/", 1);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      %this.addRow(%x, getField(%dirList, %x));
    }
  }
}

function NPCButt::onClick(%this)
{
  %text = NPCList.getRowText(NPCList.getSelectedRow());

  commandToServer('NPCLoadDNC', %text);
}

function NPCList::onSelect(%this, %cellID, %text)
{
  exec("scripts/client/dotsnetcrits/npcs/" @ %text @ "/preview.cs");
}
