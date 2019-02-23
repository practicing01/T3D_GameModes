datablock ExplosionData(RangedSkillsGMExplosion : GrenadeLauncherExplosion)
{
  shakeCamera = false;
  debris = "";
  emitter[0] = "";
  emitter[1] = "";
  emitter[2] = "";
  ParticleEmitter = "";
  subExplosion[0] = "";
};

datablock ProjectileData(RangedSkillsGMProjectile)
{
   projectileShapeName = "art/shapes/weapons/shared/rocket.dts";
   directDamage = 10;
   radiusDamage = 10;
   damageRadius = 5;
   areaImpulse = 2000;

   explosion = RangedSkillsGMExplosion;
   waterExplosion = "";//GrenadeLauncherWaterExplosion;

   decal = "";//ScorchRXDecal;
   splash = "";//GrenadeSplash;

   particleEmitter = GrenadeProjSmokeTrailEmitter;
   particleWaterEmitter = "";//GrenadeTrailWaterEmitter;

   muzzleVelocity = 1;
   velInheritFactor = 0.3;

   armingDelay = 1;
   lifetime = 60000;
   fadeDelay = 4500;

   bounceElasticity = 1.0;
   bounceFriction = 0.0;
   isBallistic = true;
   gravityMod = 0;

   lightDesc = "";//GrenadeLauncherLightDesc;

   damageType = "RangedSkillsGM";
};

/*******************************************************************************/

datablock ProjectileData(SkeletalDragonDungeonLevelProjectile : RangedSkillsGMProjectile)
{
  armingDelay = 1000;
};

/*******************************************************************************/

datablock ExplosionData(FuzzHellGMExplosion)
{
  shakeCamera = false;
  debris = "";
  emitter[0] = "";
  emitter[1] = "";
  emitter[2] = "";
  ParticleEmitter = "FuzzHellPuffEmitter";
  subExplosion[0] = "";
  particleDensity = 1;
  faceViewer = true;
  offset = 0.0;
  particleRadius = 0.0;
};

datablock ProjectileData(FuzzHellGMProjectile)
{
   projectileShapeName = "art/shapes/dotsnetcrits/gamemodes/fuzzhell/fuzz.cached.dts";
   directDamage = 0;
   radiusDamage = 0;
   damageRadius = 1;
   areaImpulse = 0;

   explosion = FuzzHellGMExplosion;
   waterExplosion = "";//GrenadeLauncherWaterExplosion;

   decal = "";//ScorchRXDecal;
   splash = "";//GrenadeSplash;

   particleEmitter = "";//GrenadeProjSmokeTrailEmitter;
   particleWaterEmitter = "";//GrenadeTrailWaterEmitter;

   muzzleVelocity = 1;
   velInheritFactor = 0.3;

   armingDelay = 1;
   lifetime = 5000;
   fadeDelay = 5000;

   bounceElasticity = 1.0;
   bounceFriction = 0.0;
   isBallistic = false;
   gravityMod = 0;

   lightDesc = "";//GrenadeLauncherLightDesc;

   damageType = "FuzzHellGM";
};

/*******************************************************************************/

datablock ExplosionData(CambangerExplosion)
{
  shakeCamera = true;
  camShakeAmp = "10 10 10";
  camShakeDuration = 10;
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

datablock ProjectileData(CambangerProjectile)
{
   projectileShapeName = "art/shapes/dotsnetcrits/weapons/cambanger/musicalnote.cached.dts";
   directDamage = 0;
   radiusDamage = 0;
   damageRadius = 1;
   areaImpulse = 0;

   explosion = CambangerExplosion;
   waterExplosion = "";//GrenadeLauncherWaterExplosion;

   decal = "";//ScorchRXDecal;
   splash = "";//GrenadeSplash;

   particleEmitter = "";//GrenadeProjSmokeTrailEmitter;
   particleWaterEmitter = "";//GrenadeTrailWaterEmitter;

   muzzleVelocity = 1;
   velInheritFactor = 0.3;

   armingDelay = 1;
   lifetime = 5000;
   fadeDelay = 5000;

   bounceElasticity = 1.0;
   bounceFriction = 0.0;
   isBallistic = false;
   gravityMod = 0;

   lightDesc = "";//GrenadeLauncherLightDesc;

   damageType = "cambanger";
};
