//-----------------------------------------------------------------------------
// teleMirror weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the teleMirrorImage is used.

datablock ItemData(teleMirror)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/teleMirror/teleMirror.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a teleMirror";
    description = "teleMirror";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 2;
    directDamage = 20;
    image = teleMirrorImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";

    rayMask = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
    $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
    $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
    $TypeMasks::CorpseObjectType;
};


//--------------------------------------------------------------------------
// teleMirror image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(teleMirrorImage)
{
  portalPhase_ = 0;
  inPortal_ = "";
  outPortal_ = "";
  inMirror_ = "";
  outMirror_ = "";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/teleMirror/teleMirror.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/teleMirror/teleMirror.cached.dts";
   emap = false;

   item = teleMirror;

   infiniteAmmo = true;

   //imageAnimPrefix = "teleMirror";
   //imageAnimPrefixFP = "teleMirror";

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
   stateSound[3]                    = teleMirrorFireSound;
   stateShapeSequence[3]            = "Celebrate_01";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = teleMirrorReloadSound;

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
   stateSound[6]                    = teleMirrorFireEmptySound;
};


//-----------------------------------------------------------------------------

function teleMirrorImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function teleMirrorImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function teleMirrorImage::onFire(%this, %obj, %slot)
{
   %rayResult = %obj.doRaycast(10000.0, %this.item.rayMask);

   %objTarget = firstWord(%rayResult);
   %objPos = getWords(%rayResult, 1, 3);
   %objDir = getWords(%rayResult, 4, 6);
   %transform = MatrixCreateFromEuler(%objDir);
   %mat_rotx = MatrixCreateFromEuler( mAtan( mSqrt( %objDir.x*%objDir.x + %objDir.y*%objDir.y), %objDir.z) SPC "0 0");
   //%mat_localzrot = MatrixCreateFromEuler("0 0" SPC %localzrot);
   //%mat_rotx = MatrixMultiply(%mat_rotx,%mat_localzrot);
   %mat_rotz = MatrixCreateFromEuler("0 0" SPC mAtan(%objDir.x,%objDir.y));
   %transform = MatrixMultiply(%mat_rotz,%mat_rotx);

   //%triggerPos = VectorAdd(%objPos, VectorScale(%objDir, 1.5));
   %triggerPos = %objPos;
   %localzrot = 0.0;
   %localxrot = 180.0;
   %mat_localzrot = MatrixCreateFromEuler(%localxrot SPC "0" SPC %localzrot);
   %mat_rotx = MatrixMultiply(%mat_rotx,%mat_localzrot);
   %triggerTransform = MatrixMultiply(%mat_rotz,%mat_rotx);

   if (!isObject(%objTarget))
   {
     return;
   }

   if (!%this.portalPhase_)//in phase
   {
     if (!isObject(%this.inMirror_))//create mirror.
     {
       %this.inMirror_ = new StaticShape()
       {
         dataBlock = "teleMirrorStaticShapeData";
       };

       %this.inMirror_.setTransform(%transform);
       %this.inMirror_.position = %objPos;
     }
     else//transform mirror
     {
       %this.inMirror_.setTransform(%transform);
       %this.inMirror_.position = %objPos;
     }

     if (!isObject(%this.inPortal_))//create portal
     {
       %this.inPortal_ = new Trigger() {
          polyhedron = "-0.5000000 0.5000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
          dataBlock = "TeleporterTrigger";
          //scale = "4 1 4";
          canSave = "1";
          canSaveDynamicFields = "1";
             EntranceEffect = "EntranceEffect";
             Exit = "";
             exiteffect = "EntranceEffect";
             exitVelocityScale = "0";
             oneSided = "1";
             reorientPlayer = "1";
             teleporterCooldown = "0";
       };

       %this.inPortal_.setTransform(%triggerTransform);
       %this.inPortal_.position = %triggerPos;
     }
     else//transform portal
     {
       %this.inPortal_.setTransform(%triggerTransform);
       %this.inPortal_.position = %triggerPos;
     }

   }
   else//out phase
   {
     if (!isObject(%this.outMirror_))//create mirror.
     {
       %this.outMirror_ = new StaticShape()
       {
         dataBlock = "teleMirrorStaticShapeData";
       };

       %this.outMirror_.setTransform(%transform);
       %this.outMirror_.position = %objPos;
     }
     else//transform mirror
     {
       %this.outMirror_.setTransform(%transform);
       %this.outMirror_.position = %objPos;
     }

     if (!isObject(%this.outPortal_))//create portal
     {
       %this.outPortal_ = new Trigger() {
          polyhedron = "-0.5000000 0.5000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
          dataBlock = "TeleporterTrigger";
          //scale = "4 1 4";
          canSave = "1";
          canSaveDynamicFields = "1";
             EntranceEffect = "EntranceEffect";
             Exit = %this.inPortal_;
             exiteffect = "EntranceEffect";
             exitVelocityScale = "0";
             oneSided = "1";
             reorientPlayer = "1";
             teleporterCooldown = "0";
       };

       %this.outPortal_.setTransform(%triggerTransform);
       %this.outPortal_.position = %triggerPos;

       %this.inPortal_.Exit = %this.outPortal_;
     }
     else//transform portal
     {
       %this.outPortal_.setTransform(%triggerTransform);
       %this.outPortal_.position = %triggerPos;
     }

   }

   %this.portalPhase_ = !%this.portalPhase_;

}

DefaultPlayerData.maxInv[teleMirror] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = teleMirror;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
