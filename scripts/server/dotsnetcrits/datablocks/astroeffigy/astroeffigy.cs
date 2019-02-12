//-----------------------------------------------------------------------------
// astroeffigy weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the astroeffigyImage is used.

datablock SFXProfile(astroeffigyFireSound)
{
   filename = "art/sound/dotsnetcrits/laser8.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ItemData(astroeffigy)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/astroeffigy/astronaut.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a astroeffigy";
    description = "astroeffigy";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 0.1;
    directDamage = 1;
    image = astroeffigyImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// astroeffigy image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(astroeffigyImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/astroeffigy/astronaut.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/astroeffigy/astronaut.cached.dts";
   emap = false;

   item = astroeffigy;

   infiniteAmmo = true;

   //imageAnimPrefix = "astroeffigy";
   //imageAnimPrefixFP = "astroeffigy";

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
   stateSound[3]                    = astroeffigyFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = astroeffigyReloadSound;

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
   stateSound[6]                    = astroeffigyFireEmptySound;
};


//-----------------------------------------------------------------------------

function astroeffigyImage::RemoveZone(%this, %zone)
{
  if (isObject(%zone))
  {
    if (isObject(%zone.particleEmitter_))
    {
      %zone.particleEmitter_.delete();
    }
    %zone.delete();
  }
}

function astroeffigyImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function astroeffigyImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function astroeffigyImage::onFire(%this, %obj, %slot)
{
   %pos = %obj.getPosition();

   %mask = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
   $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
   $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
   $TypeMasks::CorpseObjectType;

   %rayResult = %obj.doRaycast(1000.0, %mask);

   %objTarget = firstWord(%rayResult);

   if (!isObject(%objTarget))
   {
     return;
   }

   %objPos = getWords(%rayResult, 1, 3);
   //%objDir = getWords(%rayResult, 4, 6);

   //%objDir = VectorScale(%objDir, 5);
   //%objPos = VectorAdd(%objPos, %objDir);

   %emitter = new ParticleEmitterNode()
   {
     datablock = EmberNode;
     emitter = EmberEmitter;
     active = true;
     position = %objPos;
   };

   %zone = new PhysicalZone() {
     velocityMod = "1";
     gravityMod = "0";
     //appliedForce = "0 0 20000";
     polyhedron = "-0.5000000 0.5000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
     position = %objPos;
     rotation = "1 0 0 0";
     scale = "10 10 10";
     isRenderEnabled = "true";
     canSaveDynamicFields = "1";
     enabled = "1";
     renderZones = true;
     particleEmitter_ = %emitter;
   };

   %this.schedule(10000, "RemoveZone", %zone);
}

DefaultPlayerData.maxInv[astroeffigy] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = astroeffigy;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
