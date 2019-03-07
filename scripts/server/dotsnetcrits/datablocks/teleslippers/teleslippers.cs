//-----------------------------------------------------------------------------
// teleslippers weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the teleslippersImage is used.
datablock SFXProfile(teleslippersExplosionSound)
{
   filename = "art/sound/dotsnetcrits/step008-cloth1.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock SFXProfile(teleslippersFireSound)
{
   filename = "art/sound/dotsnetcrits/step008-cloth1.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ExplosionData(teleslippersExplosion)
{
   soundProfile = teleslippersExplosionSound;
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
   lightNormalOffset = 2.0;
};

datablock ProximityMineData(teleslippers)
{
   // Mission editor category
   category = "Weapon";
   explosion = teleslippersExplosion;

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   //className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/teleslippers/teleslippers.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   sticky = true;
   simpleServerCollision = false;

   armingDelay = 0.0;
   autoTriggerDelay = 0;
   triggerOnOwner = true;
   triggerRadius = 1.0;
   triggerDelay = 0.0;
   explosionOffset = 0.1;

    // Dynamic properties defined by the scripts
    pickUpName = "a teleslippers";
    description = "teleslippers";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 8;
    radiusDamage = 1000;
    areaImpulse = 0;
    image = teleslippersImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
    //gravityMod = 0.0;
    bounceElasticity = 10.0;
    bounceFriction = 0.0;
};


//--------------------------------------------------------------------------
// teleslippers image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(teleslippersImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/teleslippers/teleslippers.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/teleslippers/teleslippers.cached.dts";
   emap = false;

   item = teleslippers;

   infiniteAmmo = true;

   //imageAnimPrefix = "teleslippers";
   //imageAnimPrefixFP = "teleslippers";

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

   item = teleslippers;

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
   stateSound[3]                    = teleslippersFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = teleslippersReloadSound;

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
   //stateSound[6]                    = teleslippersFireEmptySound;
};


//-----------------------------------------------------------------------------


function teleslippers::onUse(%this, %obj)
{
   // Act like a weapon on use
   Weapon::onUse( %this, %obj );
}

function teleslippers::onPickup( %this, %obj, %shape, %amount )
{
   // Act like a weapon on pickup
   Weapon::onPickup( %this, %obj, %shape, %amount );
}

function teleslippers::onInventory( %this, %obj, %amount )
{
   %obj.client.setAmmoAmountHud( 1, %amount );

   if (!%amount)
   {
     %obj.setInventory(teleslippers, 1);
     return;
   }

   // Cycle weapons if we are out of ammo
   if ( !%amount && ( %slot = %obj.getMountSlot( %this.image ) ) != -1 )
      %obj.cycleWeapon( "prev" );
}

function teleslippersImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function teleslippersImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function teleslippersImage::onFire( %this, %obj, %slot )
{
   // To fire a deployable mine is to throw it
   %obj.throw( %this.item );
}

function teleslippers::onTriggered( %this, %mine, %target )
{
  if (!isObject(%mine.client))
  {
    return;
  }

  if (%mine.client == %target.client)
  {
    return;
  }

  %player = %mine.client.getControlObject();

  %targetTrans = %target.getTransform();

  %target.setTransform(%player.getTransform());
  %player.setTransform(%targetTrans);
}

function teleslippers::onExplode( %this, %obj, %position )
{
  //
}

DefaultPlayerData.maxInv[teleslippers] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = teleslippers;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
