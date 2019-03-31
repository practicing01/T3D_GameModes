//-----------------------------------------------------------------------------
// lobster weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the lobsterImage is used.

datablock SFXProfile(lobsterFireSound)
{
   filename = "art/sound/dotsnetcrits/bodily-eating04-eating_chips_1.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ItemData(lobster)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/lobster/lobster.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a lobster";
    description = "lobster";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 4;
    directDamage = 100;
    image = lobsterImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// lobster image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(lobsterImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/lobster/lobster.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/lobster/lobster.cached.dts";
   emap = false;

   item = lobster;

   infiniteAmmo = true;

   //imageAnimPrefix = "lobster";
   //imageAnimPrefixFP = "lobster";

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
   //stateSound[3]                    = lobsterFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = lobsterReloadSound;

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
   //stateSound[6]                    = lobsterFireEmptySound;
};


//-----------------------------------------------------------------------------

function lobsterImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function lobsterImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function lobsterClass::ProcessAI(%this)
{
  lobsterAI.onReachDestination(%this);

  %this.aiSchedule_ = %this.schedule(1 * 1000, "ProcessAI");
}

function lobsterImage::SpawnLobster( %this, %obj, %slot )
{
   %pos = %obj.getPosition();

   %rayResult = %obj.doRaycast(1000.0, $TypeMasks::ShapeBaseObjectType);

   %objTarget = firstWord(%rayResult);

   if (!isObject(%objTarget))
   {
     return;
   }

  %lobster = new AiPlayer()
  {
    dataBlock = lobsterAI;
    class = lobsterClass;
    //mMoveTolerance = 1.0;
    //moveStuckTolerance = 1.0;
    target_ = %objTarget;
    canAttack_ = true;
    aiSchedule_ = "";
  };

  MissionCleanup.add(%lobster);

  %teleDir = %obj.getForwardVector();

  %size = %obj.getObjectBox();
  %scale = %obj.getScale();
  %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
  %sizex *= 1.5;

  %lobster.rotation = %obj.rotation;

  %sizeTarget = %lobster.getObjectBox();
  %scaleTarget = %lobster.getScale();
  %sizexTarget = (getWord(%sizeTarget, 3) - getWord(%sizeTarget, 0)) * getWord(%scaleTarget, 0);
  %sizexTarget *= 1.5;

  %lobster.position = VectorAdd( %pos, VectorScale(%teleDir, %sizex + %sizexTarget) );

  %lobster.setMoveDestination(%objTarget.getPosition());
  %lobster.setAimObject(%lobster.target_);

  %lobster.playAudio(0, lobsterFireSound);

  %lobster.aiSchedule_ = %lobster.schedule(1 * 1000, "ProcessAI");
}

function lobsterImage::onFire(%this, %obj, %slot)
{
  %this.schedule(0, "SpawnLobster", %obj, %slot);//spawning within causes crash

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

function lobsterAI::onReachDestination(%this, %ai)
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

function lobsterAI::onMoveStuck(%this, %ai)
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

function lobsterAI::onDisabled(%this, %obj, %state)
{
  initContainerRadiusSearch(%obj.position, 4.0, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    //if(%targetObject != %obj)
    //{
     //%targetObject.damage(%obj, %pos, %this.item.directDamage, "waterer");
     %targetObject.applyRepair(100);

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

  //%obj.playAudio(0, chickenCluckSound);
  //parent::onDisabled(%this, %obj, %state);
  cancel(%obj.aiSchedule_);
  %obj.schedule(500, "delete");
}

function lobsterClass::AttackCD(%this)
{
  %this.canAttack_ = true;
}

function lobsterAI::onCollision(%this, %obj, %collObj, %vec, %len)
{
  parent::onCollision(%this, %obj, %collObj, %vec, %len);

  if (!isObject(%collObj))
  {
    return;
  }

  //if (%collObj.getClassName() !$= "Player")// && %collObj.getClassName() !$= "AIPlayer")
  if (!(%collObj.getType() & ($TypeMasks::ShapeBaseObjectType)))
  {
    return;
  }

  %damageState = %obj.getDamageState();
  if (%damageState $= "Disabled" || %damageState $= "Destroyed")
  {
    return;
  }

  if (%obj.canAttack_ == false)
  {
    return;
  }

  /*if (%obj.getClassName() $= %collObj.getClassName())
  {
    return;
  }*/

  %damageState = %collObj.getDamageState();
  if (%damageState $= "Disabled" || %damageState $= "Destroyed")
  {
    //%obj.playAudio(0, chickenCluckSound);
    //parent::onDisabled(%this, %obj, %state);
    cancel(%obj.aiSchedule_);
    %obj.schedule(500, "delete");
    return;
  }

  //%collObj.damage(%obj, %vec, 10, "melee");

  initContainerRadiusSearch(%obj.position, 4.0, $TypeMasks::ShapeBaseObjectType);

  while ( (%targetObject = containerSearchNext()) != 0 )
  {
    //if(%targetObject != %obj)
    //{
     //%targetObject.damage(%obj, %pos, %this.item.directDamage, "waterer");
     %targetObject.applyRepair(100);

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

  %obj.canAttack_ = false;
  %obj.schedule(1000, "AttackCD");

  %obj.setActionThread("melee");
}

DefaultPlayerData.maxInv[lobster] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = lobster;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
