/*function DNCButt::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  if (%this.getName() $= "WeaponsButt")
  {
    DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(5).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(0).getObject(1).setVisible(true);
  }
  else if (%this.getName() $= "NPCButt")
  {
    DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(4).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(0).getObject(2).setVisible(true);
  }
  else if (%this.getName() $= "GamemodeButt")
  {
    DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(6).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(0).getObject(0).setVisible(true);
  }
  else if (%this.getName() $= "TeamButt")
  {
    DNCMain.getObject(0).getObject(0).getObject(2).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(3).setVisible(true);
  }
  else if (%this.getName() $= "LevelsButt")
  {
    DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(7).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(0).getObject(3).setVisible(true);
  }
  else if (%this.getName() $= "NodesButt")
  {
    DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(8).setVisible(true);
    DNCMain.getObject(0).getObject(0).getObject(0).getObject(4).setVisible(true);
  }
  else if (%this.getName() $= "MiscellaneousButt")
  {
    DNCMain.getObject(0).getObject(0).getObject(1).setVisible(true);
  }
}*/

function WeaponsTab::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(5).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(0).getObject(1).setVisible(true);
}

function NPCTab::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(4).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(0).getObject(2).setVisible(true);
}

function GamemodeTab::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(6).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(0).getObject(0).setVisible(true);
}

function TeamTab::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  DNCMain.getObject(0).getObject(0).getObject(2).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(3).setVisible(true);
}

function LevelsTab::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(7).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(0).getObject(3).setVisible(true);
}

function NodesTab::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  DNCMain.getObject(0).getObject(0).getObject(0).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(8).setVisible(true);
  DNCMain.getObject(0).getObject(0).getObject(0).getObject(4).setVisible(true);
}

function MiscellaneousTab::onClick(%this)
{
  DNCMain.getObject(0).getObject(0).callOnChildren("setVisible", "false");

  DNCMain.getObject(0).getObject(0).getObject(1).setVisible(true);
}
