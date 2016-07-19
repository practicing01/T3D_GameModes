function AmmolessGMServer::GetRandyAmmoSpawnVector(%this)
{
  for (%x = 0; %x < MissionGroup.getCount(); %x++)
  {
    %obj = MissionGroup.getObject(%x);
    if (%obj.getName() $= "ammoSpawnGroupAmmolessGM")
    {
      %randy = getRandom(%obj.getCount() - 1);
      return (%obj.getObject(%randy).position);
    }
  }
  return ClientGroup.getObject(0).getControlObject().getPosition();
}

function AmmolessGMServer::ReduceAmmo(%this, %player)
{
  %player = ClientGroup.getObject(%x).getControlObject();

  for (%i=0; %i< %player.totalCycledWeapons; %i++)
  {
    %weapon = %player.cycleWeapon[%i];

    %player.setInventory(%weapon, 1);

    %image = %weapon.getFieldValue("image");

    if (%image.isField("ammo"))
    {
      %player.setInventory(%image.getFieldValue("ammo"), 1);
    }

    if (%image.isField("clip"))
    {
      %player.setInventory(%image.getFieldValue("clip"), 1);
    }

  }
}

function AmmolessGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "AmmolessGMServerQueue";

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %obj = ClientGroup.getObject(%x).getControlObject();

    %this.ReduceAmmo(%obj);
  }

  %pos = %this.GetRandyAmmoSpawnVector();

  %this.ammo_ = new StaticShape()
  {
    dataBlock = "toiletPaperAmmolessGM";
    position = %pos;
    rotation = "1 0 0 0";
    scale = "0.2 0.2 0.2";
  };

}

function AmmolessGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.ammo_))
  {
    %this.ammo_.delete();
  }

}

function toiletPaperAmmolessGM::onCollision(%this, %obj, %collObj, %vec, %len)
{
  for (%i=0; %i< %collObj.totalCycledWeapons; %i++)
  {
    %weapon = %collObj.cycleWeapon[%i];

    %collObj.setInventory(%weapon, 1);

    %image = %weapon.getFieldValue("image");

    if (%image.isField("ammo"))
    {
      %collObj.setInventory(%image.getFieldValue("ammo"), 1);
    }

    if (%image.isField("clip"))
    {
      %collObj.setInventory(%image.getFieldValue("clip"), 1);
    }

    %collObj.playAudio(0, ammolessRechargeSound);

  }

  if (isObject(AmmolessGMServerSO))
  {
    %obj.position = AmmolessGMServerSO.GetRandyAmmoSpawnVector();
  }
}

function AmmolessGMServer::loadOut(%this, %player)
{
  %this.ReduceAmmo(%player);
}

if (isObject(AmmolessGMServerSO))
{
  AmmolessGMServerSO.delete();
}
else
{
  new ScriptObject(AmmolessGMServerSO)
  {
    class = "AmmolessGMServer";
    EventManager_ = "";
    ammo_ = "";
  };

  DNCServer.loadOutListeners_.add(AmmolessGMServerSO);
  MissionCleanup.add(AmmolessGMServerSO);
}
