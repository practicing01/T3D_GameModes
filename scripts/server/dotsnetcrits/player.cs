function PlayerData::onDamage(%this, %obj, %delta)
{
   // This method is invoked by the ShapeBase code whenever the
   // object's damage level changes.
   if (%delta > 0 && %obj.getState() !$= "Dead")
   {
      // Apply a damage flash
      %obj.setDamageFlash(1);

      // If the pain is excessive, let's hear about it.
      if (%delta > 10)
      {
        %datablock = %obj.getDataBlock();

        if (!%datablock.isField("painSound_"))
        {
          return;
        }

        %obj.playAudio(0, %datablock.getFieldValue("painSound_"));
      }
   }
}

function Player::playDeathCry(%this)
{
  %datablock = %this.getDataBlock();

  if (!%datablock.isField("deathSound_"))
  {
    return;
  }

  %this.playAudio(0, %this.getDataBlock().getFieldValue("deathSound_"));
}

function Player::playPain(%this)
{
  %datablock = %this.getDataBlock();

  if (!%datablock.isField("painSound_"))
  {
    return;
  }

  %this.playAudio(0, %datablock.getFieldValue("painSound_"));
}

function Player::FlashlightToggle(%this, %state)
{
  if (%state == true)
  {
    %light = new SpotLight()
    {
      range = "40";
      cookie = "art/lights/corona.png";
    };

    //%this.mountObject(%light, 0, "0.05 0.68 -0.09");
    %this.mountObject(%light, 0, MatrixCreate("0 0 1", "1 0 0 0"));
    %this.flashlight_ = %light;
  }
  else
  {
    if (isObject(%this.flashlight_))
    {
      %this.flashlight_.delete();
    }
  }
}

function Player::UseObject(%this)
{
  %pos = %this.getPosition();

  %rayResult = %this.doRaycast(3.0, $TypeMasks::ShapeBaseObjectType);

  %objTarget = firstWord(%rayResult);

  if (!isObject(%objTarget))
  {
    return;
  }

  if (%objTarget.isMethod("UseObject"))
  {
    %objTarget.UseObject(%this);
  }
}
