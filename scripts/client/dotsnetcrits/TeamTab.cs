function TeamButt::onClick(%this)
{
  %team = 1;//Team B

  if (%this.getText() $= "Team A")//Team A
  {
    %team = 0;
  }

  commandToServer('JoinTeamDNC', %team);
}
