//-----------------------------------------------------------------------------
// cambanger weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the cambangerImage is used.

datablock SFXProfile(cambangerFireSound)
{
   filename = "art/sound/dotsnetcrits/8bit-pickup2.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ItemData(cambanger)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/cambanger/speaker.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a cambanger";
    description = "cambanger";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 2;
    directDamage = 0;
    image = cambangerImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// cambanger image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(cambangerImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/cambanger/speaker.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/cambanger/cambanger.cached.dts";
   emap = false;

   item = cambanger;

   infiniteAmmo = true;

   //imageAnimPrefix = "cambanger";
   //imageAnimPrefixFP = "cambanger";

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   //eyeOffset = "0.5 0.0 0.0";
   //rotation = "1 1 1 90";
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
   stateSound[3]                    = cambangerFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = cambangerReloadSound;

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
   //stateSound[6]                    = cambangerFireEmptySound;
};


//-----------------------------------------------------------------------------

function cambangerImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function cambangerImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function CambangerClass::onRemove(%this)
{
  cancel(%this.aiSchedule_);
}

function CambangerClass::ProcessAI(%this)
{
  %projectileVelocity = VectorScale(%this.getEyeVector(), 10.0);

  %projectile = new Projectile()
  {
    datablock = CambangerProjectile;
    initialPosition = %this.getEyePoint();
    initialVelocity = %projectileVelocity;
    sourceObject = %this;
    sourceSlot = 0;
    client = %this.client;
  };

  %this.aiSchedule_ = %this.schedule(1 * 1000, "ProcessAI");
}

function cambangerImage::SpawnCambanger( %this, %obj, %slot )
{
  %cambanger = new AiPlayer()
  {
    dataBlock = CambangerAI;
    class = CambangerClass;
    aiSchedule_ = "";
    //client = %obj.client;
  };

  MissionCleanup.add(%cambanger);

  %teleDir = %obj.getForwardVector();

  %size = %obj.getObjectBox();
  %scale = %obj.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %cambanger.rotation = %obj.rotation;

  %sizeTarget = %cambanger.getObjectBox();
  %scaleTarget = %cambanger.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %cambanger.position = VectorAdd( %obj.position, VectorScale(%teleDir, %sizex + %sizexTarget) );

  %cambanger.aiSchedule_ = %cambanger.schedule(1000, "ProcessAI");
}

function cambangerImage::onFire(%this, %obj, %slot)
{
  %this.schedule(0, "SpawnCambanger", %obj, %slot);//spawning within causes crash

  /*%emitterNode = new ParticleEmitterNode()
   {
     datablock = TeleportDNCEmitterNodeData;
     emitter = TeleportDNCEmitter;
     active = true;
     velocity = 0.0;
     position = %pos;
   };

   %emitterNode.schedule(1000, "delete");

   %targetEmitterNode = new ParticleEmitterNode()
   {
     datablock = TeleportDNCEmitterNodeData;
     emitter = TeleportDNCEmitter;
     active = true;
     velocity = 0.0;
     position = %objTarget.position;
   };

   %targetEmitterNode.schedule(1000, "delete");*/
}

function CambangerAI::onDisabled(%this, %obj, %state)
{
  //%obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  cancel(%obj.aiSchedule_);
  %obj.schedule(500, "delete");
}

DefaultPlayerData.maxInv[cambanger] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = cambanger;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
