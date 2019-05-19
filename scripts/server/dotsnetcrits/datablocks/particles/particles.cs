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
   sizes[0] = "4";
   sizes[1] = "4";
   sizes[2] = "4";
   sizes[3] = "4";
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
   sizes[0] = "4";
   sizes[1] = "4";
   sizes[2] = "4";
   sizes[3] = "4";
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

datablock ParticleEmitterNodeData(TeleportDNCEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(TeleportDNCParticle))
{
  datablock ParticleData(TeleportDNCParticle)
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

if (!isObject(TeleportDNCEmitter))
{
  datablock ParticleEmitterData(TeleportDNCEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "TeleportDNCParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(HealEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(HealParticle))
{
  datablock ParticleData(HealParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/dpsheal.png";
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

     animTexName = "art/particles/dotsnetcrits/skills/dpsheal.png";
  };
}

if (!isObject(HealEmitter))
{
  datablock ParticleEmitterData(HealEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "HealParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(PoisonEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(PoisonParticle))
{
  datablock ParticleData(PoisonParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/dot.png";
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

     animTexName = "art/particles/dotsnetcrits/skills/dot.png";
  };
}

if (!isObject(PoisonEmitter))
{
  datablock ParticleEmitterData(PoisonEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "PoisonParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(HOTEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(HOTParticle))
{
  datablock ParticleData(HOTParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/hot.png";
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

     animTexName = "art/particles/dotsnetcrits/skills/hot.png";
  };
}

if (!isObject(HOTEmitter))
{
  datablock ParticleEmitterData(HOTEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "HOTParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(AOEEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(AOEParticle))
{
  datablock ParticleData(AOEParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/aoe.png";
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

     animTexName = "art/particles/dotsnetcrits/skills/aoe.png";
  };
}

if (!isObject(AOEEmitter))
{
  datablock ParticleEmitterData(AOEEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "AOEParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(AOEHealEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(AOEHealParticle))
{
  datablock ParticleData(AOEHealParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/aoeheal.png";
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

     animTexName = "art/particles/dotsnetcrits/skills/aoeheal.png";
  };
}

if (!isObject(AOEHealEmitter))
{
  datablock ParticleEmitterData(AOEHealEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "AOEHealParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(RangedEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(RangedParticle))
{
  datablock ParticleData(RangedParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/ranged.png";
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

     animTexName = "art/particles/dotsnetcrits/skills/ranged.png";
  };
}

if (!isObject(RangedEmitter))
{
  datablock ParticleEmitterData(RangedEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "RangedParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(CleanseEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(CleanseParticle))
{
  datablock ParticleData(CleanseParticle)
  {
     textureName = "art/particles/dotsnetcrits/skills/cleanse.png";
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

     animTexName = "art/particles/dotsnetcrits/skills/cleanse.png";
  };
}

if (!isObject(CleanseEmitter))
{
  datablock ParticleEmitterData(CleanseEmitter)
  {
     ejectionPeriodMS = "50";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0.2";
     thetaMax = "40";
     particles = "CleanseParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     softnessDistance = "1";
  };
}

datablock ParticleEmitterNodeData(SummonCircle0EmitterNodeData)
{
  timeMultiple = 10.0;
};

if (!isObject(SummonCircle0Particle))
{
  datablock ParticleData(SummonCircle0Particle)
  {
     textureName = "art/particles/dotsnetcrits/skills/circle4a.png";
     dragCoefficient = "0";
     gravityCoefficient = "0";
     inheritedVelFactor = "0";
     constantAcceleration = "0";
     lifetimeMS = "1000";
     lifetimeVarianceMS = "0";
     useInvAlpha = true;
     spinRandomMin = "1";
     spinRandomMax = "1";
     spinSpeed = "1";

     colors[0] = "1 1 1 1";
     colors[1] = "1 1 1 1";
     colors[2] = "1 1 1 1";
     colors[3] = "1 1 1 1";

     sizes[0] = "1";
     sizes[1] = "1";
     sizes[2] = "1";
     sizes[3] = "1";

     times[0] = "1";
     times[1] = "1";
     times[2] = "1";
     times[3] = "1";

     animTexName = "art/particles/dotsnetcrits/skills/circle4a.png";
  };
}

if (!isObject(SummonCircle0Emitter))
{
  datablock ParticleEmitterData(SummonCircle0Emitter)
  {
     ejectionPeriodMS = "1000";
     ejectionVelocity = "0";
     velocityVariance = "0";
     ejectionOffset = "0";
     particles = "SummonCircle0Particle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     alignParticles = "1";
     alignDirection = "0 0 1";
     lifetimeVarianceMS = "0";
  };
}

datablock ParticleEmitterNodeData(SummonCircle1EmitterNodeData)
{
  timeMultiple = 10.0;
};

if (!isObject(SummonCircle1Particle))
{
  datablock ParticleData(SummonCircle1Particle)
  {
     textureName = "art/particles/dotsnetcrits/skills/circle5a.png";
     dragCoefficient = "0";
     gravityCoefficient = "0";
     inheritedVelFactor = "0";
     constantAcceleration = "0";
     lifetimeMS = "1000";
     lifetimeVarianceMS = "0";
     useInvAlpha = true;
     spinRandomMin = "1";
     spinRandomMax = "1";
     spinSpeed = "1";

     colors[0] = "1 1 1 1";
     colors[1] = "1 1 1 1";
     colors[2] = "1 1 1 1";
     colors[3] = "1 1 1 1";

     sizes[0] = "1";
     sizes[1] = "1";
     sizes[2] = "1";
     sizes[3] = "1";

     times[0] = "1";
     times[1] = "1";
     times[2] = "1";
     times[3] = "1";

     animTexName = "art/particles/dotsnetcrits/skills/circle5a.png";
  };
}

if (!isObject(SummonCircle1Emitter))
{
  datablock ParticleEmitterData(SummonCircle1Emitter)
  {
     ejectionPeriodMS = "1000";
     ejectionVelocity = "0";
     velocityVariance = "0";
     ejectionOffset = "0";
     particles = "SummonCircle1Particle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     alignParticles = "1";
     alignDirection = "0 0 1";
     lifetimeVarianceMS = "0";
  };
}

datablock ParticleEmitterNodeData(SummonCircle2EmitterNodeData)
{
  timeMultiple = 10.0;
};

if (!isObject(SummonCircle2Particle))
{
  datablock ParticleData(SummonCircle2Particle)
  {
     textureName = "art/particles/dotsnetcrits/skills/circle6a.png";
     dragCoefficient = "0";
     gravityCoefficient = "0";
     inheritedVelFactor = "0";
     constantAcceleration = "0";
     lifetimeMS = "1000";
     lifetimeVarianceMS = "0";
     useInvAlpha = true;
     spinRandomMin = "1";
     spinRandomMax = "1";
     spinSpeed = "1";

     colors[0] = "1 1 1 1";
     colors[1] = "1 1 1 1";
     colors[2] = "1 1 1 1";
     colors[3] = "1 1 1 1";

     sizes[0] = "1";
     sizes[1] = "1";
     sizes[2] = "1";
     sizes[3] = "1";

     times[0] = "1";
     times[1] = "1";
     times[2] = "1";
     times[3] = "1";

     animTexName = "art/particles/dotsnetcrits/skills/circle6a.png";
  };
}

if (!isObject(SummonCircle2Emitter))
{
  datablock ParticleEmitterData(SummonCircle2Emitter)
  {
     ejectionPeriodMS = "1000";
     ejectionVelocity = "0";
     velocityVariance = "0";
     ejectionOffset = "0";
     particles = "SummonCircle2Particle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     alignParticles = "1";
     alignDirection = "0 0 1";
     lifetimeVarianceMS = "0";
  };
}

datablock ParticleEmitterNodeData(SummonCircle3EmitterNodeData)
{
  timeMultiple = 10.0;
};

if (!isObject(SummonCircle3Particle))
{
  datablock ParticleData(SummonCircle3Particle)
  {
     textureName = "art/particles/dotsnetcrits/skills/circle7a.png";
     dragCoefficient = "0";
     gravityCoefficient = "0";
     inheritedVelFactor = "0";
     constantAcceleration = "0";
     lifetimeMS = "1000";
     lifetimeVarianceMS = "0";
     useInvAlpha = true;
     spinRandomMin = "1";
     spinRandomMax = "1";
     spinSpeed = "1";

     colors[0] = "1 1 1 1";
     colors[1] = "1 1 1 1";
     colors[2] = "1 1 1 1";
     colors[3] = "1 1 1 1";

     sizes[0] = "1";
     sizes[1] = "1";
     sizes[2] = "1";
     sizes[3] = "1";

     times[0] = "1";
     times[1] = "1";
     times[2] = "1";
     times[3] = "1";

     animTexName = "art/particles/dotsnetcrits/skills/circle7a.png";
  };
}

if (!isObject(SummonCircle3Emitter))
{
  datablock ParticleEmitterData(SummonCircle3Emitter)
  {
     ejectionPeriodMS = "1000";
     ejectionVelocity = "0";
     velocityVariance = "0";
     ejectionOffset = "0";
     particles = "SummonCircle3Particle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     alignParticles = "1";
     alignDirection = "0 0 1";
     lifetimeVarianceMS = "0";
  };
}

datablock ParticleEmitterNodeData(FuzzHellPuffEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(FuzzHellPuffParticle))
{
  datablock ParticleData(FuzzHellPuffParticle)
  {
     textureName = "art/shapes/dotsnetcrits/gamemodes/fuzzhell/puffcloud.png";
     dragCoefficient = "0";
     gravityCoefficient = "0";
     inheritedVelFactor = "0";
     constantAcceleration = "0";
     lifetimeMS = "1000";
     lifetimeVarianceMS = "0";
     useInvAlpha = true;
     spinRandomMin = "1";
     spinRandomMax = "1";
     spinSpeed = "1";

     colors[0] = "1 1 1 1";
     colors[1] = "1 1 1 1";
     colors[2] = "1 1 1 1";
     colors[3] = "1 1 1 1";

     sizes[0] = "1";
     sizes[1] = "1";
     sizes[2] = "1";
     sizes[3] = "1";

     times[0] = "1";
     times[1] = "1";
     times[2] = "1";
     times[3] = "1";

     animateTexture = true;
     animTexName = "art/shapes/dotsnetcrits/gamemodes/fuzzhell/puffcloud.png";
     animTexFrames = "0 1 2 3 4";
     animTexTiling = "5 1";
     framesPerSec = 5;
  };
}

if (!isObject(FuzzHellPuffEmitter))
{
  datablock ParticleEmitterData(FuzzHellPuffEmitter)
  {
     ejectionPeriodMS = "1000";
     ejectionVelocity = "1";
     velocityVariance = "0";
     ejectionOffset = "0";
     particles = "FuzzHellPuffParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     //alignParticles = "1";
     //alignDirection = "0 0 1";
     lifetimeVarianceMS = "0";
     orientParticles = true;
  };
}

datablock ParticleEmitterNodeData(Flame0EmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(Flame0Particle))
{
  datablock ParticleData(Flame0Particle)
  {
     textureName = "art/shapes/dotsnetcrits/weapons/match/flames.png";
     dragCoefficient = "0";
     gravityCoefficient = "0";
     inheritedVelFactor = "0";
     constantAcceleration = "0";
     lifetimeMS = "1000";
     lifetimeVarianceMS = "0";
     useInvAlpha = true;
     spinRandomMin = "1";
     spinRandomMax = "1";
     spinSpeed = "1";

     colors[0] = "1 1 1 1";
     colors[1] = "1 1 1 1";
     colors[2] = "1 1 1 1";
     colors[3] = "1 1 1 1";

     sizes[0] = "1";
     sizes[1] = "1";
     sizes[2] = "1";
     sizes[3] = "1";

     times[0] = "1";
     times[1] = "1";
     times[2] = "1";
     times[3] = "1";

     animateTexture = true;
     animTexName = "art/shapes/dotsnetcrits/weapons/match/flames.png";
     animTexFrames = "0 1 2 3 4 5 6 7 8 9 10 11";
     animTexTiling = "4 3";
     framesPerSec = 4;
  };
}

if (!isObject(Flame0Emitter))
{
  datablock ParticleEmitterData(Flame0Emitter)
  {
     ejectionPeriodMS = "1";
     ejectionVelocity = "10.42";
     velocityVariance = "0";
     ejectionOffset = "0";
     particles = "Flame0Particle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     //alignParticles = "1";
     //alignDirection = "0 0 1";
     lifetimeVarianceMS = "0";
     orientParticles = true;
     thetaMin = "90";
     thetaMax = "180";
     originalName = "Flame0Emitter";
  };
}

datablock ParticleEmitterNodeData(WatererEmitterNodeData)
{
  timeMultiple = 1.0;
};

if (!isObject(WatererParticle))
{
  datablock ParticleData(WatererParticle)
  {
     textureName = "art/particles/bubble.png";
     dragCoefficient = "0";
     gravityCoefficient = "1";
     inheritedVelFactor = "1";
     constantAcceleration = "1";
     lifetimeMS = "1000";
     lifetimeVarianceMS = "0";
     useInvAlpha = true;
     spinRandomMin = "1";
     spinRandomMax = "1";
     spinSpeed = "1";

     colors[0] = "1 0.888 0.008 1";
     colors[1] = "1 0.888 0.008 1";
     colors[2] = "1 0.888 0.008 1";
     colors[3] = "1 0.888 0.008 1";

     sizes[0] = "0.5";
     sizes[1] = "0.5";
     sizes[2] = "0.5";
     sizes[3] = "0.5";

     times[0] = "1";
     times[1] = "1";
     times[2] = "1";
     times[3] = "1";
  };
}

if (!isObject(WatererEmitter))
{
  datablock ParticleEmitterData(WatererEmitter)
  {
     ejectionPeriodMS = "1";
     ejectionVelocity = "5";
     velocityVariance = "0";
     ejectionOffset = "0";
     particles = "WatererParticle";
     blendStyle = "ADDITIVE";
     softParticles = "0";
     //alignParticles = "1";
     //alignDirection = "0 1 0";
     lifetimeVarianceMS = "0";
     //orientParticles = true;
     //orientOnVelocity = true;
     thetaMin = "0";
     thetaMax = "0";
     phiVariance = "0";
     originalName = "WatererEmitter";
  };
}
