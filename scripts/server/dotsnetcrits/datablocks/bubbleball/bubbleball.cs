//-----------------------------------------------------------------------------
// bubbleball weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the bubbleballImage is used.

datablock SFXProfile(bubbleballFireSound)
{
   filename = "art/sound/throw.ogg";
   description = AudioDefault3d;
   preload = true;
};

/*
datablock SFXProfile(WeaponTemplateReloadSound)
{
   filename = "art/sound/weapons/wpn_reload";
   description = AudioClose3D;
   preload = true;
};

datablock SFXProfile(WeaponTemplateSwitchinSound)
{
   filename = "art/sound/weapons/wpn_switchin";
   description = AudioClose3D;
   preload = true;
};

datablock SFXProfile(WeaponTemplateIdleSound)
{
   filename = "art/sound/weapons/wpn_idle";
   description = AudioClose3D;
   preload = true;
};

datablock SFXProfile(WeaponTemplateGrenadeSound)
{
   filename = "art/sound/weapons/wpn_grenadelaunch";
   description = AudioClose3D;
   preload = true;
};

datablock SFXProfile(WeaponTemplateMineSwitchinSound)
{
   filename = "art/sound/weapons/wpn_mine_switchin";
   description = AudioClose3D;
   preload = true;
};

datablock LightDescription( bubbleballBulletProjectileLightDesc )
{
   color  = "0.0 0.5 0.7";
   range = 3.0;
};
*/
datablock ProjectileData( bubbleballBulletProjectile )
{
   projectileShapeName = "art/shapes/dotsnetcrits/gamemodes/bubbleball/bubbleball.cached.dts";

   directDamage        = 0;
   radiusDamage        = 0;
   damageRadius        = 0.5;
   areaImpulse         = 0.5;
   impactForce         = 1;

   explosion           = BulletDirtExplosion;
   decal               = BulletHoleDecal;

   muzzleVelocity      = 15;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 130000;
   fadeDelay           = 130000;
   bounceElasticity    = 1;
   bounceFriction      = 0;
   isBallistic         = true;
   gravityMod          = 0.1;
};

function bubbleballBulletProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
  if (%col.isMethod("onCollision"))
  {
    %col.onCollision(%obj, %pos, %normal);
  }

  %size = %obj.getObjectBox();
  %scale = %obj.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %offset = VectorAdd( %pos, VectorScale(%normal, %sizex) );

  %bubbleball = new Item() {
     static = "1";
     rotate = "1";
     dataBlock = "bubbleballAmmo";
     position = %offset;
     rotation = "1 0 0 0";
     scale = "1 1 1";
  };

  MissionCleanup.add(%bubbleball);

  %obj.schedule(500, "delete");
}

function bubbleballAmmo::onCollision(%this, %obj, %collObj, %vec, %len)
{
  %collObj.use(bubbleball);
  %obj.schedule(500, "delete");
}

/*function bubbleballBulletProjectile::onRemove(%this, %obj)
{
  %bubbleball = new Item() {
     static = "1";
     rotate = "1";
     dataBlock = "bubbleballAmmo";
     position = %obj.position;
     rotation = "1 0 0 0";
     scale = "1 1 1";
  };

  MissionCleanup.add(%bubbleball);
}*/

//-----------------------------------------------------------------------------
// Ammo Item
//-----------------------------------------------------------------------------
datablock ItemData(bubbleballClip)
{
   // Mission editor category
   category = "AmmoClip";

   // Add the Ammo namespace as a parent.  The ammo namespace provides
   // common ammo related functions and hooks into the inventory system.
   className = "AmmoClip";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/gamemodes/bubbleball/bubbleball.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;

   // Dynamic properties defined by the scripts
   pickUpName = "";
   count = 1;
   maxInventory = 1;
};

datablock ItemData(bubbleballAmmo)
{
   // Mission editor category
   category                         = "Ammo";

   // Add the Ammo namespace as a parent.  The ammo namespace provides
   // common ammo related functions and hooks into the inventory system.
   className                        = "Ammo";

   // Basic Item properties
   shapeFile                        = "art/shapes/dotsnetcrits/gamemodes/bubbleball/bubbleball.cached.dts";
   mass                             = 1;
   elasticity                       = 0.2;
   friction                         = 0.6;

   // Dynamic properties defined by the scripts
   pickUpName                       = "Bubbleball";
   maxInventory                     = 1;
   clip = bubbleballClip;
};

datablock ItemData(bubbleball)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/gamemodes/bubbleball/bubbleball.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a bubbleball";
    description = "bubbleball";
    image = bubbleballImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// bubbleball image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(bubbleballImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/gamemodes/bubbleball/bubbleball.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/gamemodes/bubbleball/bubbleball.cached.dts";
   emap = false;

   item = bubbleball;
   ammo = bubbleballAmmo;
   clip = bubbleballClip;
   projectile = bubbleballBulletProjectile;
   projectileType = Projectile;
   projectileSpread = "0.0";
   useRemainderDT = true;
   //casing = BulletShell;
   //shellExitDir                   = "1.0 0.3 1.0";
   //shellExitOffset                = "0.15 -0.56 -0.1";
   //shellExitVariance              = 15.0;
   //shellVelocity                  = 3.0;
   //lightType                      = "";
   //lightColor                     = "0.992126 0.968504 0.708661 1";
   //lightRadius                    = "4";
   //lightDuration                  = "100";
   //lightBrightness                = 2;
   //shakeCamera                    = false;
   //camShakeFreq                   = "0 0 0";
   //camShakeAmp                    = "0 0 0";

   //imageAnimPrefix = "bubbleball";
   //imageAnimPrefixFP = "bubbleball";

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   //eyeOffset = "0.5 0.0 0.0";
   //rotation = "1 0 0 90";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   class = "WeaponImage";
   className = "WeaponImage";

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
   stateTransitionOnNoAmmo[2]       = "Ready";
   stateTransitionOnTriggerDown[2]  = "Fire";

   // Fire the weapon. Calls the fire script which does
   // the actual work.
   stateName[3]                     = "Fire";
   stateTransitionOnTimeout[3]      = "Ready";
   stateTimeoutValue[3]             = 0.2;
   stateFire[3]                     = true;
   stateRecoil[3]                   = LightRecoil;
   stateAllowImageChange[3]         = false;
   stateSequence[3]                 = "Fire";
   stateScript[3]                   = "onFire";
   stateSound[3]                    = bubbleballFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = bubbleballReloadSound;

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
   //stateSound[6]                    = bubbleballFireEmptySound;
};


//-----------------------------------------------------------------------------

DefaultPlayerData.maxInv[bubbleball] = 1;
DefaultPlayerData.maxInv[bubbleballClip] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = bubbleball;
  clip_ = bubbleballClip;
  ammo_ = bubbleballAmmo;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
