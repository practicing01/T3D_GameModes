//-----------------------------------------------------------------------------
// waterer weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the watererImage is used.

datablock SFXProfile(watererFireSound)
{
   filename = "art/sound/dotsnetcrits/water-pour06-BloodDrips.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ItemData(waterer)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/waterer/waterer.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a waterer";
    description = "waterer";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 10;
    directDamage = 20;
    image = watererImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// waterer image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(watererImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/waterer/waterer.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/waterer/waterer.cached.dts";
   emap = false;

   item = waterer;

   infiniteAmmo = true;

   //imageAnimPrefix = "waterer";
   //imageAnimPrefixFP = "waterer";

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   //eyeOffset = "0.5 0.0 0.0";
   //rotation = "1 0 0 90";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.
   correctMuzzleVector = false;

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
   //stateTransitionOnNoAmmo[2]       = "Ready";
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
   stateSound[3]                    = watererFireSound;
   stateShapeSequence[3]            = "shoot";
   stateEmitter[3]                  = WatererEmitter;
   stateEmitterTime[3]              = 1;

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = watererReloadSound;

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
   //stateSound[6]                    = watererFireEmptySound;
};


//-----------------------------------------------------------------------------

function watererImage::RemoveEmitter(%this, %emitter)
{
  if (isObject(%emitter))
  {
    %emitter.delete();
  }
}

function watererImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function watererImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function watererImage::onFire(%this, %obj, %slot)
{
   %pos = %obj.getPosition();

   /*%emitter = new ParticleEmitterNode()
   {
     datablock = WatererEmitterNodeData;
     emitter = WatererEmitter;
     active = true;
   };

   %obj.mountObject(%emitter, 0, MatrixCreate("0 0 0", "1 0 0 0"));

   %this.schedule(500, "RemoveEmitter", %emitter);*/

   initContainerRadiusSearch(%pos, %this.item.damageRadius, $TypeMasks::ShapeBaseObjectType);

   while ( (%targetObject = containerSearchNext()) != 0 )
   {
     //if(%targetObject != %obj)
     //{
      //%targetObject.damage(%obj, %pos, %this.item.directDamage, "waterer");
      %targetObject.applyRepair(%this.item.directDamage);

      /*%targetEmitter = new ParticleEmitterNode()
      {
        datablock = FireNode;
        emitter = FireEmitter;
        active = true;
      };

      %targetObject.mountObject(%targetEmitter, 1, MatrixCreate("0 0 1", "1 0 0 0"));

      %this.schedule(500, "RemoveEmitter", %targetEmitter);*/
     //}
   }
}

DefaultPlayerData.maxInv[waterer] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = waterer;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
