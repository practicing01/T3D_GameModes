datablock SFXProfile(quechanDeathCrySound)
{
   fileName = "art/sound/dotsnetcrits/reitanna_japanese-huh.ogg";
   description = AudioClose3d;
   preload = true;
};

datablock SFXProfile(quechanPainCrySound)
{
   fileName = "art/sound/dotsnetcrits/mike-bes_nya.ogg";
   description = AudioClose3d;
   preload = true;
};

datablock PlayerData(quechan : DefaultPlayerData)
{
   shapeFile = "art/shapes/dotsnetcrits/actors/quechan/quechan.dae";
   painSound_ = quechanPainCrySound;
   deathSound_ = quechanDeathCrySound;
   availableSkins =  "base Black";
};

datablock PlayerData(quechanhoodie : DefaultPlayerData)
{
   shapeFile = "art/shapes/dotsnetcrits/actors/quechanhoodie/quechanhoodie.dae";
   painSound_ = quechanPainCrySound;
   deathSound_ = quechanDeathCrySound;
};

datablock PlayerData(hardcorechicken : DefaultPlayerData)
{
   shapeFile = "art/shapes/dotsnetcrits/actors/hardcorechicken/hardcore_chickent3d.dae";
   painSound_ = chickenCluckSound;
   deathSound_ = chickenCluckSound;
};
