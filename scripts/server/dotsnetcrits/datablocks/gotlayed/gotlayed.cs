//-----------------------------------------------------------------------------
// gotlayed weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the gotlayedImage is used.

datablock SFXProfile(gotlayedExplosionSound)
{
   filename = "art/sound/dotsnetcrits/wubitog_3-popping-pops.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock SFXProfile(gotlayedFireSound)
{
   filename = "art/sound/dotsnetcrits/tieswijnen_gliding-beep-short.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ExplosionData(gotlayedExplosion)
{
   soundProfile = gotlayedExplosionSound;
   lifeTimeMS = 400; // Quick flash, short burn, and moderate dispersal

   // Volume particles
   particleEmitter = GrenadeExpFireEmitter;
   particleDensity = 75;
   particleRadius = 2.25;

   // Point emission
   emitter[0] = GrenadeExpDustEmitter;
   emitter[1] = GrenadeExpSparksEmitter;
   emitter[2] = GrenadeExpSmokeEmitter;

   // Sub explosion objects
   subExplosion[0] = GrenadeSubExplosion;

   // Camera Shaking
   shakeCamera = true;
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
   lightNormalOffset = 2.0;
};

datablock ProximityMineData(gotlayed)
{
   // Mission editor category
   category = "Weapon";
   explosion = gotlayedExplosion;

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   //className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/gotlayed/gotlayed.cached.dts";
   mass = 1.0;
   elasticity = 0.2;
   friction = 0.0;
   sticky = false;
   simpleServerCollision = false;

   armingDelay = 0.0;
   autoTriggerDelay = 2.0;
   triggerOnOwner = true;
   triggerRadius = 3.0;
   triggerDelay = 0.0;
   explosionOffset = 0.1;

    // Dynamic properties defined by the scripts
    pickUpName = "a gotlayed";
    description = "gotlayed";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 8;
    radiusDamage = 300;
    areaImpulse = 2000;
    image = gotlayedImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
    gravityMod = 0.1;
    bounceElasticity = 10.0;
    bounceFriction = 0.0;
};


//--------------------------------------------------------------------------
// gotlayed image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(gotlayedImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/gotlayed/gotlayed.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/gotlayed/gotlayed.cached.dts";
   emap = false;

   item = gotlayed;

   infiniteAmmo = true;

   //imageAnimPrefix = "gotlayed";
   //imageAnimPrefixFP = "gotlayed";

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   //eyeOffset = "0.5 0.0 0.0";
   //rotation = "1 0 0 90";
   scale = "0.1 0.1 0.1";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.
   correctMuzzleVector = false;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   class = "WeaponImage";
   className = "WeaponImage";

   item = gotlayed;

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
   stateSound[3]                    = gotlayedFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = gotlayedReloadSound;

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
   stateSound[6]                    = gotlayedFireEmptySound;
};


//-----------------------------------------------------------------------------

datablock ProjectileData(chipcrispProjectile : GrenadeLauncherProjectile)
{
  projectileShapeName = "art/shapes/dotsnetcrits/weapons/gotlayed/crispchip/crispchip.cached.dts";
  armingDelay = 0;
  areaImpulse = 10000;
};

//-----------------------------------------------------------------------------

function gotlayed::onUse(%this, %obj)
{
   // Act like a weapon on use
   Weapon::onUse( %this, %obj );
}

function gotlayed::onPickup( %this, %obj, %shape, %amount )
{
   // Act like a weapon on pickup
   Weapon::onPickup( %this, %obj, %shape, %amount );
}

function gotlayed::onInventory( %this, %obj, %amount )
{
   %obj.client.setAmmoAmountHud( 1, %amount );

   if (!%amount)
   {
     %obj.setInventory(gotlayed, 1);
     return;
   }

   // Cycle weapons if we are out of ammo
   if ( !%amount && ( %slot = %obj.getMountSlot( %this.image ) ) != -1 )
      %obj.cycleWeapon( "prev" );
}

function gotlayedImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function gotlayedImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function gotlayedImage::onFire( %this, %obj, %slot )
{
   %mine = new ProximityMine()
   {
      datablock = %this.item;
      sourceObject = %obj;
      rotation = "0 0 1 "@ (getRandom() * 360);
      static = false;
      client = %obj.client;
   };
   MissionCleanup.add(%mine);

   %throwForce = 100.0;

   // Start with the shape's eye vector...
   %eye = %obj.getEyeVector();
   %vec = vectorScale(%eye, %throwForce);

   // Add a vertical component to give the object a better arc
   %verticalForce = %throwForce / 2;
   %dot = vectorDot("0 0 1", %eye);
   if (%dot < 0)
      %dot = -%dot;
   %vec = vectorAdd(%vec, vectorScale("0 0 "@%verticalForce, 1 - %dot));

   // Add the shape's velocity
   %vec = vectorAdd(%vec, %obj.getVelocity());

   // Set the object's position and initial velocity
   %pos = getBoxCenter(%obj.getWorldBox());
   %mine.setTransform(%pos);
   %mine.applyImpulse(%pos, %vec);

   // Since the object is thrown from the center of the shape,
   // the object needs to avoid colliding with it's thrower.
   %mine.setCollisionTimeout(%obj);

   %mine.schedule(5000, "explode");
}

function gotlayed::onExplode( %this, %obj, %position )
{
   // Damage objects within the mine's damage radius
   if ( %this.damageRadius > 0 )
   {
     radiusDamage( %obj, %position, %this.damageRadius, %this.radiusDamage, %this.damageType, %this.areaImpulse );
   }

   %velocity = VectorScale(-(%obj.getUpVector()), 1000.0);

   for (%y = -50; %y < 60; %y+=10)
   {
     for (%x = -50; %x < 60; %x+=10)
     {
       %offset = %x SPC %y SPC "0.0";

       %projectile = new Projectile()
       {
         datablock = chipcrispProjectile;
         initialPosition = VectorAdd(%position, %offset);
         initialVelocity = %velocity;
         sourceObject = %obj;
         sourceSlot = 0;
         client = %obj.client;
       };
     }
   }
}

DefaultPlayerData.maxInv[gotlayed] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = gotlayed;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
