//-----------------------------------------------------------------------------
// excavaber weapon. This file contains all the items related to this weapon
// including explosions, ammo, the item and the weapon item image.
// These objects rely on the item & inventory support system defined
// in item.cs and inventory.cs
//-----------------------------------------------------------------------------
//http://www.garagegames.com/community/resources/view/9230

//--------------------------------------------------------------------------
// Weapon Item.  This is the item that exists in the world, i.e. when it's
// been dropped, thrown or is acting as re-spawnable item.  When the weapon
// is mounted onto a shape, the excavaberImage is used.

function DirtMoundTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  if (%obj.dataBlock $= "ExcavatorAI")
  {
    %obj.AddTarget(%trigger.dirtMound_);
  }
}

function DirtMoundTrigger::onTickTrigger(%this, %trigger)
{
  for (%x = 0; %x < %trigger.getNumObjects(); %x++)
  {
    %obj = %trigger.getObject(%x);

    if (%obj.dataBlock $= "ExcavatorAI")
    {
      if (!%obj.targets_.isMember(%trigger.dirtMound_))
      {
        %obj.AddTarget(%trigger.dirtMound_);
      }
    }
  }
}

function DirtMoundStaticShapeData::onRemove(%this, %shape)
{
    for (%x = 0; %x < %shape.trigger_.getNumObjects(); %x++)
    {
      %obj = %shape.trigger_.getObject(%x);

      if (%obj.dataBlock $= "ExcavatorAI")
      {
        %obj.targets_.remove(%shape);
      }
    }

    %shape.trigger_.delete();
}

function DirtMoundStaticShapeData::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  %scale = VectorScale(%shape.scale, 0.9);

  if (%scale.x < 1 || %scale.y < 1 || %scale.z < 1)
  {
    %shape.delete();
  }
  else
  {
    %shape.scale = %scale;
    %shape.trigger_.scale = %scale;
  }
}

datablock SFXProfile(excavaberFireSound)
{
   filename = "art/sound/dotsnetcrits/wdfourtee_mxl993-yarstick-swoosh-5.ogg";
   description = AudioDefault3d;
   preload = true;
};

datablock ItemData(excavaber)
{
   // Mission editor category
   category = "Weapon";

   // Hook into Item Weapon class hierarchy. The weapon namespace
   // provides common weapon handling functions in addition to hooks
   // into the inventory system.
   className = "Weapon";

   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/excavaber/excavaber.cached.dts";
   mass = 1;
   elasticity = 0.2;
   friction = 0.6;
   emap = true;

    // Dynamic properties defined by the scripts
    pickUpName = "a excavaber";
    description = "excavaber";
    maxInventory = 1;
    damageType = "meleeDamage";
    damageRadius = 5;
    directDamage = 0;
    image = excavaberImage;
    reticle = "crossHair";
    zoomReticle = "crossHairZoomed";
};


//--------------------------------------------------------------------------
// excavaber image which does all the work.  Images do not normally exist in
// the world, they can only be mounted on ShapeBase objects.

datablock ShapeBaseImageData(excavaberImage)
{
   // Basic Item properties
   shapeFile = "art/shapes/dotsnetcrits/weapons/excavaber/excavaber.cached.dts";
   //shapeFileFP = "art/shapes/dotsnetcrits/weapons/excavaber/excavaber.cached.dts";
   emap = false;

   item = excavaber;

   infiniteAmmo = true;

   //imageAnimPrefix = "excavaber";
   //imageAnimPrefixFP = "excavaber";

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
   stateSound[3]                    = excavaberFireSound;
   stateShapeSequence[3]            = "shoot";

   // Play the reload animation, and transition into
   stateName[4]                     = "Reload";
   stateTransitionOnNoAmmo[4]       = "Ready";
   stateTransitionOnTimeout[4]      = "Ready";
   stateTimeoutValue[4]             = 0.8;
   stateAllowImageChange[4]         = false;
   stateSequence[4]                 = "Reload";
   //stateEjectShell[4]               = true;
   //stateSound[4]                    = excavaberReloadSound;

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
   //stateSound[6]                    = excavaberFireEmptySound;
};


//-----------------------------------------------------------------------------

function excavaberImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   %obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function excavaberImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function excavaberImage::onFire(%this, %obj, %slot)
{
   %pos = %obj.getPosition();

   initContainerRadiusSearch(%pos, %this.item.damageRadius, $TypeMasks::ShapeBaseObjectType);

   while ( (%targetObject = containerSearchNext()) != 0 )
   {
     if(%targetObject.dataBlock $= "DirtMoundStaticShapeData")
     {
       %scale = VectorScale(%targetObject.scale, 1.1);
       %targetObject.scale = %scale;
       %targetObject.trigger_.scale = %scale;
      return;
     }
   }

   %trigger = new Trigger()
   {
     dataBlock = "DirtMoundTrigger";
     class = "DirtMoundTriggerClass";
     polyhedron = "-0.5 0.5 0.0 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
     position = %pos;
     dirtMound_ = "";
   };

   %dirtmound = new StaticShape()
   {
     dataBlock = "DirtMoundStaticShapeData";
     class = "DirtMoundClass";
     position = %pos;
     trigger_ = %trigger;
   };

   %trigger.dirtMound_ = %dirtmound;

   MissionCleanup.add(%dirtmound);
}

DefaultPlayerData.maxInv[excavaber] = 1;

%weaponSO = new ScriptObject()
{
  class = "WeaponLoader";
  weapon_ = excavaber;
};

DNCServer.loadOutListeners_.add(%weaponSO);
DNCServer.loadedWeapons_.add(%weaponSO);
