function TeamButt::onClick(%this)
{
  %team = 1;//Team B

  if (%this.getParent().getObject(0) == %this)//Team A
  {
    %team = 0;
  }

  commandToServer('JoinTeamDNC', %team);
}
