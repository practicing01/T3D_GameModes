//-----------------------------------------------------------------------------
// bathnuke weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the bathnukeImage is used.

datablock SFXProfile(bathnukeFireSound)
{
   filename = "art/sound/dotsnetcrits/trip03.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ItemData(bathnuke)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/bathnuke/bathnuke.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a bathnuke";
    description = "bathnuke";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 4;
    directDamage = 1000;
    image = bathnukeImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// bathnuke image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(bathnukeImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/bathnuke/bathnuke.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/bathnuke/bathnuke.cached.dts";
   emap = false;

   item = bathnuke;

   infiniteAmmo = true;

   //imageAnimPrefix = "bathnuke";
   //imageAnimPrefixFP = "bathnuke";

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
   stateSound[3]                    = bathnukeFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = bathnukeReloadSound;

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
   //stateSound[6]                    = bathnukeFireEmptySound;
};


//-----------------------------------------------------------------------------

function bathnukeImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function bathnukeImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function bathnukeClass::ProcessAI(%this)
{
  bathnukeAI.onReachDestination(%this);

  %this.target_.damage(%this, %this.position, 10, "bathnuke");

  %this.aiSchedule_ = %this.schedule(1 * 1000, "ProcessAI");
}

function bathnukeImage::Spawnbathnuke( %this, %obj, %slot )
{
  %pos = %obj.position;

  %bathnuke = new AiPlayer()
  {
    dataBlock = bathnukeAI;
    class = bathnukeClass;
    //mMoveTolerance = 1.0;
    //moveStuckTolerance = 1.0;
    client_ = %obj.client;
    target_ = %obj;
    canAttack_ = true;
    aiSchedule_ = "";
  };

  MissionCleanup.add(%bathnuke);

  %teleDir = %obj.getForwardVector();
  %teleDir = VectorScale(%teleDir, -1);

  %size = %obj.getObjectBox();
  %scale = %obj.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %bathnuke.rotation = %obj.rotation;

  %sizeTarget = %bathnuke.getObjectBox();
  %scaleTarget = %bathnuke.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %bathnuke.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

  %bathnuke.setMoveDestination(%obj.getPosition());
  %bathnuke.setAimObject(%bathnuke.target_);

  %bathnuke.aiSchedule_ = %bathnuke.schedule(1 * 1000, "ProcessAI");
}

function bathnukeImage::onFire(%this, %obj, %slot)
{
  %this.schedule(0, "Spawnbathnuke", %obj, %slot);//spawning within causes crash

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

function bathnukeAI::onReachDestination(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    //%ai.target_ = ClientGroup.getRandom().getControlObject();
    //%ai.playAudio(0, chickenCluckSound);
    cancel(%ai.aiSchedule_);
    %ai.schedule(500, "delete");
    return;
  }

  %targPos = %ai.target_.getPosition();

  //setField(%targPos, 2, getField(%ai.getPosition(), 2));

  %ai.setMoveDestination(%targPos);

  %ai.setAimObject(%ai.target_);
}

function bathnukeAI::onMoveStuck(%this, %ai)
{
  if (!isObject(%ai.target_))
  {
    //%ai.target_ = ClientGroup.getRandom().getControlObject();
    //%ai.playAudio(0, chickenCluckSound);
    cancel(%ai.aiSchedule_);
    %ai.schedule(500, "delete");
    return;
  }

  %targPos = %ai.target_.getPosition();

  //setField(%targPos, 2, getField(%ai.getPosition(), 2));

  %ai.setMoveDestination(%targPos);

  %ai.setAimObject(%ai.target_);
}

function bathnukeAI::onDisabled(%this, %obj, %state)
{
  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %client = ClientGroup.getObject(%x);
    %player = %client.getControlObject();
    %player.damage(%obj, %obj.position, 1000, "bathnuke");
  }

  //%obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  cancel(%obj.aiSchedule_);
  %obj.schedule(500, "delete");
}

function bathnukeClass::AttackCD(%this)
{
  %this.canAttack_ = true;
}

DefaultPlayerData.maxInv[bathnuke] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = bathnuke;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
