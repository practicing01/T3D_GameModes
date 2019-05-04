if (isObject(candyhellgm_Bomb.materials_))
{
  candyhellgm_Bomb.materials_.delete();
}
candyhellgm_Bomb.materials_ = new SimSet();

candyhellgm_Bomb.materials_.add(candyhellgm_jellypinkfront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_bluefront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_greenfront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_orangefront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_pinkfront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_purplefront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_redfront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_whitefront);
candyhellgm_Bomb.materials_.add(candyhellgm_bean_yellowfront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_bluefront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_greenfront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_orangefront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_purplefront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_redfront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_tealfront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_whitefront);
candyhellgm_Bomb.materials_.add(candyhellgm_heart_yellowfront);
candyhellgm_Bomb.materials_.add(candyhellgm_jellybig_greenfront);
candyhellgm_Bomb.materials_.add(candyhellgm_jellybig_redfront);
candyhellgm_Bomb.materials_.add(candyhellgm_jellybig_yellowfront);
candyhellgm_Bomb.materials_.add(candyhellgm_jelly_bluefront);
candyhellgm_Bomb.materials_.add(candyhellgm_jelly_greenfront);
candyhellgm_Bomb.materials_.add(candyhellgm_jelly_orangefront);
candyhellgm_Bomb.materials_.add(candyhellgm_jelly_purplefront);
candyhellgm_Bomb.materials_.add(candyhellgm_jelly_redfront);
candyhellgm_Bomb.materials_.add(candyhellgm_jelly_tealfront);
candyhellgm_Bomb.materials_.add(candyhellgm_jelly_yellowfront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_bluefront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_greenfront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_orangefront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_purplefront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_redfront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_tealfront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_whitefront);
candyhellgm_Bomb.materials_.add(candyhellgm_star_yellowfront);
candyhellgm_Bomb.materials_.add(candyhellgm_wrappedtrans_bluefront);
candyhellgm_Bomb.materials_.add(candyhellgm_wrappedtrans_greenfront);
candyhellgm_Bomb.materials_.add(candyhellgm_wrappedtrans_orangefront);
candyhellgm_Bomb.materials_.add(candyhellgm_wrappedtrans_purplefront);
candyhellgm_Bomb.materials_.add(candyhellgm_wrappedtrans_redfront);
candyhellgm_Bomb.materials_.add(candyhellgm_wrappedtrans_tealfront);
candyhellgm_Bomb.materials_.add(candyhellgm_wrappedtrans_yellowfront);

function candyhellgm_Bomb::onUse(%this, %obj)
{
   // Act like a weapon on use
   Weapon::onUse( %this, %obj );
}

function candyhellgm_Bomb::onPickup( %this, %obj, %shape, %amount )
{
   // Act like a weapon on pickup
   Weapon::onPickup( %this, %obj, %shape, %amount );
}

function candyhellgm_Bomb::onInventory( %this, %obj, %amount )
{
   //%obj.client.setAmmoAmountHud( 1, %amount );

   if (!%amount)
   {
     %obj.setInventory(candyhellgm_Bomb, 1);
     return;
   }

   // Cycle weapons if we are out of ammo
   if ( !%amount && ( %slot = %obj.getMountSlot( %this.image ) ) != -1 )
      %obj.cycleWeapon( "prev" );
}

function candyhellgm_BombImage::onMount( %this, %obj, %slot )
{
   // The mine doesn't use ammo from a player's perspective.
   %obj.setImageAmmo( %slot, true );
   %numMines = %obj.getInventory(%this.item);
   //%obj.client.RefreshWeaponHud( 1, %this.item.previewImage, %this.item.reticle, %this.item.zoomReticle, %numMines  );
}

function candyhellgm_BombImage::onUnmount( %this, %obj, %slot )
{
   %obj.client.RefreshWeaponHud( 0, "", "" );
}

function ShapeBase::throwObjectDir(%this, %data, %dir)
{
    if (%this.getInventory(%data) > 0)
    {
       %obj = %data.onThrow(%this, %amount);
       if (%obj)
       {
         %mat = candyhellgm_Bomb.materials_.getRandom();
         %mapTo = %obj.getTargetName( 0 );
         %obj.changeMaterial( %mapTo, 0, %mat );
         %mapTo = %obj.getTargetName( 1 );
         %obj.changeMaterial( %mapTo, 1, %mat );
         // Throw the given object in the direction the shape is looking.
         // The force value is hardcoded according to the current default
         // object mass and mission gravity (20m/s^2).

         %throwForce = %this.getDataBlock().throwForce;
         if (!%throwForce)
         %throwForce = 20;

         // Start with the shape's eye vector...
         %eye = %dir;//%this.getEyeVector();
         %vec = vectorScale(%eye, %throwForce);

         // Add a vertical component to give the object a better arc
         /*%verticalForce = %throwForce / 2;
         %dot = vectorDot("0 0 1", %eye);
         if (%dot < 0)
         %dot = -%dot;
         %vec = vectorAdd(%vec, vectorScale("0 0 "@%verticalForce, 1 - %dot));*/

         // Add the shape's velocity
         %vec = vectorAdd(%vec, %this.getVelocity());

         // Set the object's position and initial velocity
         %pos = getBoxCenter(%this.getWorldBox());
         %obj.setTransform(%pos);
         %obj.applyImpulse(%pos, %vec);

         // Since the object is thrown from the center of the shape,
         // the object needs to avoid colliding with it's thrower.
         %obj.setCollisionTimeout(%this);
         //serverPlay3D(ThrowSnd, %this.getTransform());
       }
    }
}

function candyhellgm_BombImage::onFire( %this, %obj, %slot )
{
  %backwards = -(%obj.getForwardVector());
  %rightDir = %obj.getRightVector();
  %leftDir = -%rightDir;

   // To fire a deployable mine is to throw it
   %obj.throwObjectDir( %this.item, %backwards);
   %obj.throwObjectDir( %this.item, %rightDir);
   %obj.throwObjectDir( %this.item, %leftDir);
}

/*function candyhellgm_Bomb::onExplode( %this, %obj, %position )
{
  initContainerRadiusSearch(%position, %this.damageRadius, $TypeMasks::ShapeBaseObjectType);

  while ( (%objTarget = containerSearchNext()) != 0 )
  {
    %objTarget.applyRepair(%this.radiusDamage);
  }
}*/

CandyHellAI.maxInv[candyhellgm_Bomb] = 1;

function CandyHellGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "CandyHellGMServerQueue";

  %trans = ClientGroup.getObject(0).getControlObject().getTransform();

  %this.npc_ = new AiPlayer()
  {
    dataBlock = CandyHellAI;
    class = "CandyHellAIClass";
    bombSchedule_ = "";
  };

  %this.npc_.setInventory(candyhellgm_Bomb, 1);
  %this.npc_.addToWeaponCycle(candyhellgm_Bomb);
  %this.npc_.mountImage(candyhellgm_BombImage, 0);

  %this.npc_.setTransform(%trans);

  %this.npc_.setDest();

  %this.npc_.setBomb();

  MissionCleanup.add(%this.npc_);
}

function CandyHellGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if(isObject(%this.npc_))
  {
    cancel(%this.npc_.bombSchedule_);
    %this.npc_.delete();
  }
}

function CandyHellAIClass::setBomb(%this)
{
  %this.fire(true);
  %this.fire(false);
  %this.bombSchedule_ = %this.schedule(1 * 1000, "setBomb");
}

function CandyHellAIClass::setDest(%this)
{
  if (!isObject(PlayerDropPoints))
  {
    return;
  }

  %spawnPoint = PlayerDropPoints.getRandom();
  %result = %this.setPathDestination(%spawnPoint.position);

  if (!%result)
  {
    %this.schedule(1000, "setDest");
    return;
  }
  
  %this.setAimLocation(%spawnPoint);
  %this.clearAim();
}

function CandyHellAI::onDisabled(%this, %ai, %state)
{
  %ai.setDamageLevel(0);
  %ai.setDamageState("Enabled");
}

function CandyHellAI::onReachDestination(%this, %ai)
{
  %ai.setDest();
}

function CandyHellAI::onMoveStuck(%this, %ai)
{
  %ai.setDest();
}

if (isObject(CandyHellGMServerSO))
{
  CandyHellGMServerSO.delete();
}
else
{
  new ScriptObject(CandyHellGMServerSO)
  {
    class = "CandyHellGMServer";
    EventManager_ = "";
    npc_ = "";
  };

  MissionCleanup.add(CandyHellGMServerSO);
}
