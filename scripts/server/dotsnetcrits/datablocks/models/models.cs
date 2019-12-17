
function StaticShapeData::onAdd(%data, %obj)
{
   //echo("c4StaticShapeData::onAdd("@%data.getName()@", "@%obj@")");

   // For 'simple' shapes there's not much for us to do here, but you could
   // initialize dynamic properties, activate a startup sequence, set energy,
   // recharge rate, repair rate, prepare for interaction, etc...
}

function StaticShapeData::damage(%data, %obj, %sourceObject, %position, %amount, %damageType)
{
   //echo("c4StaticShapeData::damage("@%data.getName()@", "@%obj@", "@sourceObject@", "@%position@", "@%amount@", "@%damageType@")");

   if (%obj.isDestroyed())
   {
      //echo("object already destroyed, returning");
      return;
   }

   %obj.applyDamage(%amount);
}

function StaticShapeData::onDamage(%data, %obj)
{
   //echo("c4StaticShapeData::onDamage("@%data@", "@%obj@")");

   // Set damage state based on current damage level, we are comparing amount
   // of damage sustained to the damage levels described in the datablock and
   // setting the approppriate damageState
   %damage = %obj.getDamageLevel();
   if (%damage >= %data.destroyedLevel)
   {
      if (%obj.getDamageState() !$= "Destroyed")
      {
         %obj.setDamageState(Destroyed);
         %obj.setDamageLevel(%data.maxDamage);
      }
   }
   else if(%damage >= %data.disabledLevel)
   {
      // you could call an animation sequence here to represent the deformation
      // of the object by damage.  You can have as many sequences as you want,
      // so long as you set up your damage amount to damage level comparisons
      if (%obj.getDamageState() !$= "Disabled")
         %obj.setDamageState(Disabled);
   }
   else
   {
      // we're just assuming that the object is still nice and healthy
      if (%obj.getDamageState() !$= "Enabled")
         %obj.setDamageState(Enabled);
   }
}

function StaticShapeData::onEnabled(%data, %obj, %state)
{
   //echo("c4StaticShapeData::onEnabled("@%data@", "@%obj@", "@%state@")");

   // We could do things here like establishing a power connection, activation
   // sounds, play a start up sequence, add effects, etc.
}

function StaticShapeData::onDisabled(%data, %obj, %state)
{
   //echo("c4StaticShapeData::onDisabled("@%data@", "@%obj@", "@%state@")");

   // We could do things here like disabling power, shutdown sounds, play a
   // damage sequence, swap to an alternate model shape, add effects, etc.
}

function StaticShapeData::onDestroyed(%data, %obj, %prevState)
{
   //echo("c4StaticShapeData::onDestroyed("@%data@", "@%obj@", "@%prevState@")");

   // If this is set to false then we delete the object when it is destroyed,
   // we do so while it is still obscured by the explosion fx
   if (!%data.renderWhenDestroyed)
      %obj.schedule(200, "delete");
}

datablock StaticShapeData(throneKOTHGM)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/koth/toiletSeat/toilet0.cached.dts";
  isInvincible = "1";
};

datablock StaticShapeData(plungerKOTHGM : throneKOTHGM)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/koth/plunger/plungerMod.cached.dts";
};

datablock StaticShapeData(toiletPaperAmmolessGM)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/ammoless/toiletPaper/toiletPaper.cached.dts";
  isInvincible = "1";
};

datablock StaticShapeData(balldummyHydroballGM)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/hydroball/rubberduck/rubberduckNoCol.cached.dts";
  isInvincible = "1";
};

datablock RigidShapeData(ballHydroballGM : BouncingBoulder)
{
   shapeFile = "art/shapes/dotsnetcrits/gamemodes/hydroball/rubberduck/rubberduck.cached.dts";
   mass = "0.1";
   isInvincible = "1";
   density = "1";
};

datablock StaticShapeData(bombDiffusalGM)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/diffusal/banana/banana.cached.dts";
  isInvincible = "1";
};


datablock StaticShapeData(flagCTFGM)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/ctf/cookies/cookies.cached.dts";
  isInvincible = "1";
};

datablock StaticShapeData(ketchupbottleSkillsGM)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/skills/ketchupbottle/ketchupbottle.cached.dts";
  isInvincible = "1";
};

datablock StaticShapeData(glassBottleStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/weapons/glassBottle/glassBottle.cached.dts";
  isInvincible = "1";
};

datablock StaticShapeData(teleMirrorStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/weapons/teleMirror/teleMirror.cached.dts";
  isInvincible = "1";
};

datablock StaticShapeData(glasspanelStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/levels/glass/glasspanel.dae";
  isInvincible = "0";
  maxDamage = 50;
  destroyedLevel = 50;
  disabledLevel = 50;
  renderWhenDestroyed = 0;
};

datablock StaticShapeData(groundCubeStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/levels/trenches/groundCube/groundCube.dae";
  isInvincible = "0";
  maxDamage = 50;
  destroyedLevel = 50;
  disabledLevel = 50;
  renderWhenDestroyed = 0;
};

datablock StaticShapeData(SupplyRunPaperStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/supplyrun/copypaper/copypaper.dae";
  isInvincible = "1";
};

datablock StaticShapeData(BamboomStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/bamboom/bamboo.dae";
  isInvincible = "1";
};

datablock StaticShapeData(PizzaDeliveryMicrowaveStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/pizzadelivery/microwave/microwave.dae";
  isInvincible = "1";
};

datablock StaticShapeData(PizzaDeliveryPizzaStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/pizzadelivery/pizza/pizza.dae";
  isInvincible = "1";
};

datablock StaticShapeData(ScorpionHatPyramidStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/scorpionhat/pyramid.dae";
  isInvincible = "1";
};

datablock StaticShapeData(HexagonSwitchStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/hexagon/switch.dae";
  isInvincible = "1";
};

datablock StaticShapeData(HydrobubbleGoalStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/bubbleball/goalpost.dae";
  isInvincible = "1";
};

datablock StaticShapeData(PumptreeStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/pumptree/pumptree.dae";
  isInvincible = "0";
  maxDamage = 50;
  destroyedLevel = 100;
  disabledLevel = 50;
  renderWhenDestroyed = 1;
};

datablock ItemData(GooshballItemData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/gooshball/gooshball.dae";
  isInvincible = "1";
  pickupName = "gooshball";
};

datablock StaticShapeData(PixenaryStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/pixenary/pixenary.dae";
  //isInvincible = "1";
};

datablock StaticShapeData(SkiTreeStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/skit3d/tree.dae";
  isInvincible = "1";
};

datablock StaticShapeData(MinaryStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/minary/minary.dae";
  isInvincible = "1";
};

datablock StaticShapeData(MinaryCrystalStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/minary/crystal.dae";
  isInvincible = "1";
};

datablock StaticShapeData(VoidBallStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/shadowrescue/voidball/voidball.dae";
  isInvincible = "1";
};

datablock StaticShapeData(BalloonyStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/balloony/balloon/balloon.dae";
};

datablock StaticShapeData(DirtMoundStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/excavator/dirtmound.dae";
};

datablock StaticShapeData(KeyPointAtoBStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/pointatob/key.dae";
  isInvincible = "1";
};

datablock StaticShapeData(DoorPointAtoBStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/pointatob/doorpatb.dae";
  isInvincible = "1";
};

datablock StaticShapeData(StealthCubeStaticShapeData)
{
  shapeFile = "art/shapes/dotsnetcrits/gamemodes/stackstealth/stealthcube/stealthcube.dae";
  isInvincible = "1";
};
