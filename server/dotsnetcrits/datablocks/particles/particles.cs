if (!isObject(DefaultParticle))
{
  datablock ParticleData(DefaultParticle)
  {
     textureName = "core/art/defaultParticle";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "core/art/defaultParticle";
  };
}

if (!isObject(DefaultEmitter))
{
  datablock ParticleEmitterData(DefaultEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "DefaultParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleData(teamAParticle : DefaultParticle)
{
   animateTexture = true;
   dragCoefficient = "0";
   inheritedVelFactor = "0";
   lifetimeMS = "1000";
   lifetimeVarianceMS = "0";
   spinSpeed = "0";
   spinRandomMin = "-1";
   spinRandomMax = "0";
   sizes[0] = "2";
   sizes[1] = "2";
   sizes[2] = "2";
   sizes[3] = "2";
   times[0] = "0.0";
   times[1] = "0.25";
   times[2] = "0.5";
   times[3] = "1.0";
   colors[0]     = "1.0 0.0 1.0 0.0";
   colors[1]     = "1.0 0.0 1.0 0.03";
   colors[2]     = "1.0 0.0 1.0 0.04";
   colors[3]     = "1.0 0.0 1.0 0.05";
   textureName = "art/particles/energyAura.png";
   animTexName = "art/particles/energyAura.png";
   animTexFrames = "0 1 2 3";
   animTexTiling = "2 2";
   framesPerSec = "4";
};

datablock ParticleData(teamBParticle : DefaultParticle)
{
   animateTexture = true;
   dragCoefficient = "0";
   inheritedVelFactor = "0";
   lifetimeMS = "1000";
   lifetimeVarianceMS = "0";
   spinSpeed = "0";
   spinRandomMin = "-1";
   spinRandomMax = "0";
   sizes[0] = "2";
   sizes[1] = "2";
   sizes[2] = "2";
   sizes[3] = "2";
   times[0] = "0.0";
   times[1] = "0.25";
   times[2] = "0.5";
   times[3] = "1.0";
   colors[0]     = "0.0 1.0 1.0 0.0";
   colors[1]     = "0.0 1.0 1.0 0.03";
   colors[2]     = "0.0 1.0 1.0 0.04";
   colors[3]     = "0.0 1.0 1.0 0.05";
   textureName = "art/particles/energyAura.png";
   animTexName = "art/particles/energyAura.png";
   animTexFrames = "0 1 2 3";
   animTexTiling = "2 2";
   framesPerSec = "4";
};

datablock ParticleEmitterNodeData(teamEmitterNodeData)
{
  timeMultiple = 1.0;
};

datablock ParticleEmitterData(teamAOutlinerEmitter : DefaultEmitter)
{
   particles = "teamAParticle";
   ejectionPeriodMS = "50";
   ejectionVelocity = "0";
   ejectionOffset = "0";
   thetaMax = "0";
   phiVariance = "0";
};

datablock ParticleEmitterData(teamBOutlinerEmitter : DefaultEmitter)
{
   particles = "teamBParticle";
   ejectionPeriodMS = "50";
   ejectionVelocity = "0";
   ejectionOffset = "0";
   thetaMax = "0";
   phiVariance = "0";
};

datablock ParticleEmitterNodeData(BlindEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(BlindParticle))
{
  datablock ParticleData(BlindParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/blind.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/blind.png";
  };
}

if (!isObject(BlindEmitter))
{
  datablock ParticleEmitterData(BlindEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "BlindParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(SilenceEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(SilenceParticle))
{
  datablock ParticleData(SilenceParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/silence.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/silence.png";
  };
}

if (!isObject(SilenceEmitter))
{
  datablock ParticleEmitterData(SilenceEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "SilenceParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(CritEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(CritParticle))
{
  datablock ParticleData(CritParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/crit.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/crit.png";
  };
}

if (!isObject(CritEmitter))
{
  datablock ParticleEmitterData(CritEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "CritParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(MeleeEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(MeleeParticle))
{
  datablock ParticleData(MeleeParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/melee.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/melee.png";
  };
}

if (!isObject(MeleeEmitter))
{
  datablock ParticleEmitterData(MeleeEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "MeleeParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(KnockbackEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(KnockbackParticle))
{
  datablock ParticleData(KnockbackParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/knockback.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/knockback.png";
  };
}

if (!isObject(KnockbackEmitter))
{
  datablock ParticleEmitterData(KnockbackEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "KnockbackParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(SprintEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(SprintParticle))
{
  datablock ParticleData(SprintParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/sprint.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/sprint.png";
  };
}

if (!isObject(SprintEmitter))
{
  datablock ParticleEmitterData(SprintEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "SprintParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(SnareEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(SnareParticle))
{
  datablock ParticleData(SnareParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/snare.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/snare.png";
  };
}

if (!isObject(SnareEmitter))
{
  datablock ParticleEmitterData(SnareEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "SnareParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(ShieldEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(ShieldParticle))
{
  datablock ParticleData(ShieldParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/shield.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/shield.png";
  };
}

if (!isObject(ShieldEmitter))
{
  datablock ParticleEmitterData(ShieldEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "ShieldParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(CloakEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(CloakParticle))
{
  datablock ParticleData(CloakParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/cloak.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/cloak.png";
  };
}

if (!isObject(CloakEmitter))
{
  datablock ParticleEmitterData(CloakEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "CloakParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(TeleportEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(TeleportParticle))
{
  datablock ParticleData(TeleportParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/teleport.png";
     dragCoefficient = 0.498534;
     gravityCoefficient = 0;
     inheritedVelFactor = 0.499022;
     constantAcceleration = 0.0;
     lifetimeMS = 1313;
     lifetimeVarianceMS = 500;
     useInvAlpha = true;
     spinRandomMin = -360;
     spinRandomMax = 360;
     spinSpeed = 1;

     colors[0] = "0.992126 0.00787402 0.0314961 1";
     colors[1] = "1 0.834646 0 0.645669";
     colors[2] = "1 0.299213 0 0.330709";
     colors[3] = "0.732283 1 0 0";

     sizes[0] = 0;
     sizes[1] = 0.497467;
     sizes[2] = 0.73857;
     sizes[3] = 0.997986;

     times[0] = 0.0;
     times[1] = 0.247059;
     times[2] = 0.494118;
     times[3] = 1;

     animTexName = "art/particles/dotsnetcrits/skills/teleport.png";
  };
}

if (!isObject(TeleportEmitter))
{
  datablock ParticleEmitterData(TeleportEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "TeleportParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}
