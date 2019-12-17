//-----------------------------------------------------------------------------
// troutscepter weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the troutscepterImage is used.

datablock SFXProfile(troutscepterFireSound)
{
   filename = "art/sound/dotsnetcrits/Drink_01.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ItemData(troutscepter)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/troutscepter/troutscepter.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a troutscepter";
    description = "troutscepter";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 0.1;
    directDamage = 1;
    image = troutscepterImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// troutscepter image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(troutscepterImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/troutscepter/troutscepter.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/troutscepter/troutscepter.cached.dts";
   emap = false;

   item = troutscepter;

   infiniteAmmo = true;

   //imageAnimPrefix = "troutscepter";
   //imageAnimPrefixFP = "troutscepter";

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   eyeOffset = "0.5 1.0 -0.5";
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
   stateSound[3]                    = troutscepterFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = troutscepterReloadSound;

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
   //stateSound[6]                    = troutscepterFireEmptySound;
};


//-----------------------------------------------------------------------------

function troutscepterImage::RemoveWater(%this, %waterObj)
{
  if (isObject(%waterObj))
  {
    %waterObj.delete();
  }
}

function troutscepterImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function troutscepterImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function troutscepterImage::onFire(%this, %obj, %slot)
{
   %pos = %obj.getPosition();

   %mask = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|/*$TypeMasks::WaterObjectType|*/
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
   %objDir = getWords(%rayResult, 4, 6);
   /*%transform = MatrixCreateFromEuler(%objDir);
   %mat_rotx = MatrixCreateFromEuler( mAtan( mSqrt( %objDir.x*%objDir.x + %objDir.y*%objDir.y), %objDir.z) SPC "0 0");
   //%mat_localzrot = MatrixCreateFromEuler("0 0" SPC %localzrot);
   //%mat_rotx = MatrixMultiply(%mat_rotx,%mat_localzrot);
   %mat_rotz = MatrixCreateFromEuler("0 0" SPC mAtan(%objDir.x,%objDir.y));
   %transform = MatrixMultiply(%mat_rotz,%mat_rotx);*/

   %objDir = VectorScale(%objDir, 5);
   %objPos = VectorAdd(%objPos, %objDir);

   %waterObj = new WaterBlock() {
      gridElementSize = "5";
      gridSize = "5";
      density = "1";
      viscosity = "1";
      liquidType = "Water";
      baseColor = "45 108 171 255";
      fresnelBias = "0.3";
      fresnelPower = "6";
      specularPower = "48";
      specularColor = "1 1 1 1";
      emissive = "0";
      waveDir[0] = "0 1";
      waveDir[1] = "0.707 0.707";
      waveDir[2] = "0.5 0.86";
      waveSpeed[0] = "1";
      waveSpeed[1] = "1";
      waveSpeed[2] = "1";
      waveMagnitude[0] = "0.2";
      waveMagnitude[1] = "0.2";
      waveMagnitude[2] = "0.2";
      overallWaveMagnitude = "1";
      rippleTex = "art/water/ripple";
      rippleDir[0] = "0 1";
      rippleDir[1] = "0.707 0.707";
      rippleDir[2] = "0.5 0.86";
      rippleSpeed[0] = "0.065";
      rippleSpeed[1] = "0.09";
      rippleSpeed[2] = "0.04";
      rippleTexScale[0] = "7.14 7.14";
      rippleTexScale[1] = "6.25 12.5";
      rippleTexScale[2] = "50 50";
      rippleMagnitude[0] = "1";
      rippleMagnitude[1] = "1";
      rippleMagnitude[2] = "0.3";
      overallRippleMagnitude = "1";
      foamTex = "art/water/foam";
      foamDir[0] = "1 0";
      foamDir[1] = "0 1";
      foamSpeed[0] = "0";
      foamSpeed[1] = "0";
      foamTexScale[0] = "1 1";
      foamTexScale[1] = "3 3";
      foamOpacity[0] = "0";
      foamOpacity[1] = "0";
      overallFoamOpacity = "1";
      foamMaxDepth = "2";
      foamAmbientLerp = "0.5";
      foamRippleInfluence = "0.05";
      fullReflect = "0";
      reflectivity = "0.5";
      reflectPriority = "1";
      reflectMaxRateMs = "15";
      reflectDetailAdjust = "1";
      reflectNormalUp = "1";
      useOcclusionQuery = "1";
      reflectTexSize = "256";
      waterFogDensity = "0";
      waterFogDensityOffset = "0";
      wetDepth = "0";
      wetDarkening = "0";
      depthGradientTex = "art/water/depthcolor_ramp";
      depthGradientMax = "50";
      distortStartDist = "0";
      distortEndDist = "0";
      distortFullDepth = "0";
      clarity = "1";
      underwaterColor = "254 254 252 0";
      position = %objPos;
      rotation = "1 0 0 0";
      scale = "10 10 10";
      canSave = "1";
      canSaveDynamicFields = "1";
   };

   //%waterObj.setTransform(%transform);

   %this.schedule(10000, "RemoveWater", %waterObj);
}

DefaultPlayerData.maxInv[troutscepter] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = troutscepter;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
