datablock ExplosionData(RangedSkillsGMExplosion : GrenadeLauncherExplosion)
{
  shakeCamera = false;
};

datablock ProjectileData(RangedSkillsGMProjectile)
{
   projectileShapeName = "art/shapes/weapons/shared/rocket.dts";
   directDamage = 10;
   radiusDamage = 10;
   damageRadius = 5;
   areaImpulse = 2000;

   explosion = RangedSkillsGMExplosion;
   waterExplosion = GrenadeLauncherWaterExplosion;

   decal = ScorchRXDecal;
   splash = GrenadeSplash;

   particleEmitter = GrenadeProjSmokeTrailEmitter;
   particleWaterEmitter = GrenadeTrailWaterEmitter;

   muzzleVelocity = 1;
   velInheritFactor = 0.3;

   armingDelay = 1;
   lifetime = 60000;
   fadeDelay = 4500;

   bounceElasticity = 1.0;
   bounceFriction = 0.0;
   isBallistic = true;
   gravityMod = 0;

   lightDesc = GrenadeLauncherLightDesc;

   damageType = "RangedSkillsGM";
};
