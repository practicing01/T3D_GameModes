function TrenchesLevel::onAdd(%this)
{
  //DNCServer.loadOutListeners_.add(%this);
}

/*function TrenchesLevel::onRemove(%this)
{
  //
}*/

/*function TrenchesLevel::loadOut(%this, %player)
{
  %cubeSet = Cubes;//MissionGroup.findObjectByInternalName("Triggers");

  %cube = %cubeSet.getObject(0);
  %box = %cube.getObjectBox();
  %xAbs = mAbs(getWord(%box, 0) - getWord(%box, 3));
  %yAbs = mAbs(getWord(%box, 1) - getWord(%box, 4));
  %zAbs = mAbs(getWord(%box, 2) - getWord(%box, 5));

  for (%z = 0; %z < 2; %z++)
  {
    for (%y = 0; %y < 50; %y++)
    {
      for (%x = 0; %x < 50; %x++)
      {
        %clone = %cube.clone();
        %clone.name = "";
        Cubes.add(%clone);

        %clone.position = %x SPC %y SPC %z;
      }
    }
  }

  %cube.delete();

}*/
