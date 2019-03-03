datablock SFXProfile(candyhellgm_BombExplosionSound)
{
   filename = "art/sound/dotsnetcrits/wubitog_3-popping-pops.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock SFXProfile(candyhellgm_BombFireSound)
{
   filename = "art/sound/dotsnetcrits/tieswijnen_gliding-beep-short.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ExplosionData(candyhellgm_BombExplosion)
{
   soundProfile = candyhellgm_BombExplosionSound;
   lifeTimeMS = 10; // Quick flash, short burn, and moderate dispersal

   // Volume particles
   /*particleEmitter = GrenadeExpFireEmitter;
   particleDensity = 75;
   particleRadius = 2.25;

   // Point emission
   emitter[0] = GrenadeExpDustEmitter;
   emitter[1] = GrenadeExpSparksEmitter;
   emitter[2] = GrenadeExpSmokeEmitter;

   // Sub explosion objects
   subExplosion[0] = GrenadeSubExplosion;

   // Camera Shaking
   shakeCamera = false;
   camShakeFreq = "10.0 11.0 9.0";
   camShakeAmp = "15.0 15.0 15.0";
   camShakeDuration = 1.5;
   camShakeRadius = 20;

   // Exploding debris
   debris = GrenadeDebris;
   debrisThetaMin = 10;
   debrisThetaMax = 60;
   debrisNum = 4;
   debrisNumVariance = 2;
   debrisVelocity = 25;
   debrisVelocityVariance = 5;

   lightStartRadius = 4.0;
   lightEndRadius = 0.0;
   lightStartColor = "1.0 1.0 1.0";
   lightEndColor = "1.0 1.0 1.0";
   lightStartBrightness = 4.0;
   lightEndBrightness = 0.0;
   lightNormalOffset = 2.0;*/
};

datablock ProximityMineData(candyhellgm_Bomb)
{
   // Mission editor category
   category = "Weapon";
   explosion = candyhellgm_BombExplosion;

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   //className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/gamemodes/candyhell/candy/candyhellgm_bomb.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   sticky = true;
   simpleServerCollision = false;

   armingDelay = 0.5;
   autoTriggerDelay = 30.0;
   triggerOnOwner = true;
   triggerRadius = 1.0;
   triggerDelay = 0.5;
   explosionOffset = 0.1;
   triggerSpeed = 1.0;

    // Dynamic properties defined by the scripts
    pickUpName = "a candyhellgm_Bomb";
    description = "candyhellgm_Bomb";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 4;
    radiusDamage = 100;
    areaImpulse = 0;
    image = candyhellgm_BombImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
    //gravityMod = 0.0;
    bounceElasticity = 10.0;
    bounceFriction = 0.0;
    //rotate = true;
    //static = true;
    //gravityMod = 0.1;

    materials_ = "";
};

//--------------------------------------------------------------------------
// candyhellgm_Bomb image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(candyhellgm_BombImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/gamemodes/candyhell/candy/candyhellgm_bomb.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/gamemodes/candyhell/candy/candyhellgm_bomb.cached.dts";
   emap = false;

   item = candyhellgm_Bomb;

   infiniteAmmo = true;

   //imageAnimPrefix = "candyhellgm_Bomb";
   //imageAnimPrefixFP = "candyhellgm_Bomb";

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   //eyeOffset = "0.5 0.0 0.0";
   //rotation = "1 0 0 90";
   //scale = "0.1 0.1 0.1";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.
   correctMuzzleVector = false;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   class = "WeaponImage";
   className = "WeaponImage";

   item = candyhellgm_Bomb;

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
   stateName[0]                     = "Preactivate";
   stateTransitionOnLoaded[0]       = "Activate";
   stateTransitionOnNoAmmo[0]       = "Activate";

   // Activating the gun.  Called when the weapon is first
   // mounted and there is ammo.
   stateName[1]                     = "Activate";
   stateTransitionOnTimeout[1]      = "Ready";
   stateTimeoutValue[1]             = 0.6;
   stateSequence[1]                 = "Activate";

   // Ready to fire, just waiting for the trigger
   stateName[2]                     = "Ready";
   //stateTransitionOnNoAmmo[2]       = "Ready";
   stateTransitionOnTriggerDown[2]  = "Fire";

   // Fire the weapon. Calls the fire script which does
   // the actual work.
   stateName[3]                     = "Fire";
   stateTransitionOnTimeout[3]      = "Ready";
   stateTimeoutValue[3]             = 1.0;
   stateFire[3]                     = true;
   stateRecoil[3]                   = LightRecoil;
   stateAllowImageChange[3]         = false;
   stateSequence[3]                 = "Fire";
   stateScript[3]                   = "onFire";
   stateSound[3]                    = "";//candyhellgm_BombFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = candyhellgm_BombReloadSound;

   // No ammo in the weapon, just idle until something
   // shows up. Play the dry fire sound if the trigger is
   // pulled.
   stateName[5]                     = "NoAmmo";
   stateTransitionOnAmmo[5]         = "Ready";
   stateSequence[5]                 = "NoAmmo";
   stateTransitionOnTriggerDown[5]  = "Fire";

   // No ammo dry fire
   stateName[6]                     = "DryFire";
   stateTimeoutValue[6]             = 1.0;
   stateTransitionOnTimeout[6]      = "Ready";
   //stateSound[6]                    = candyhellgm_BombFireEmptySound;
};


//-----------------------------------------------------------------------------
