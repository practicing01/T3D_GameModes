datablock ExplosionData(RustyBeamWrenchExplosion)
{
  shakeCamera = false;
  debris = "";
  emitter[0] = "";
  emitter[1] = "";
  emitter[2] = "";
  ParticleEmitter = "";
  subExplosion[0] = "";
  particleDensity = 1;
  faceViewer = true;
  offset = 0.0;
  particleRadius = 0.0;
};

datablock ProjectileData(RustyBeamWrenchProjectile)
{
   projectileShapeName = "art/shapes/dotsnetcrits/levels/rustybeams/wrench.cached.dts";
   directDamage = 0;
   radiusDamage = 0;
   damageRadius = 1;
   areaImpulse = 5000;

   explosion = RustyBeamWrenchExplosion;
   waterExplosion = "";//GrenadeLauncherWaterExplosion;

   decal = "";//ScorchRXDecal;
   splash = "";//GrenadeSplash;

   particleEmitter = "";//GrenadeProjSmokeTrailEmitter;
   particleWaterEmitter = "";//GrenadeTrailWaterEmitter;

   muzzleVelocity = 1;
   velInheritFactor = 0.3;

   armingDelay = 1;
   lifetime = 30000;
   fadeDelay = 30000;

   bounceElasticity = 1.0;
   bounceFriction = 0.0;
   isBallistic = false;
   gravityMod = 0;

   lightDesc = "";//GrenadeLauncherLightDesc;

   damageType = "RustyBeamWrench";
};

function RustyBeamsSO::onRemove(%this)
{
  cancel(%this.schedule_);
}

function RustyBeamsSO::SpawnProjectile(%this)
{
  %client = ClientGroup.getRandom();

  %player = %client.getControlObject();

  %dir = VectorSub(%player.position, "0 0 50");

  %dir = VectorNormalize(%dir);

  %dir = VectorScale(%dir, 10);

  %projectile = new Projectile()
  {
    datablock = RustyBeamWrenchProjectile;
    initialPosition = "0 0 50";
    initialVelocity = %dir;
    sourceObject = %player;
    sourceSlot = 0;
    client = %client;
  };

  %this.schedule(5000, "SpawnProjectile");
}

function RustyBeamsClass::onAdd(%this)
{
  %rustybeamsSO = new ScriptObject()
  {
    schedule_ = "";
    class = "RustyBeamsSO";
  };

  MissionGroup.add(%rustybeamsSO);

  %rustybeamsSO.schedule(5000, "SpawnProjectile");
}
