singleton Material(floor0seabits)
{
   mapTo = "floor0seabits";
   terrainMaterials = "1";
   ShowDust = "1";
   showFootprints = "1";
   materialTag0 = "Terrain";
};

new TerrainMaterial()
{
   internalName = "floor0seabits";
   diffuseMap = "art/shapes/dotsnetcrits/levels/seabits/floor0.png";
   detailMap = "art/shapes/dotsnetcrits/levels/seabits/floor0_d.png";
   normalMap = "art/shapes/dotsnetcrits/levels/seabits/floor0_n.png";
   detailSize = "1";
   isManaged = "1";
   detailBrightness = "1";
   Enabled = "1";
   diffuseSize = "1";
};

singleton Material(floor1seabits)
{
   mapTo = "floor1seabits";
   terrainMaterials = "1";
   ShowDust = "1";
   showFootprints = "1";
   materialTag0 = "Terrain";
};

new TerrainMaterial()
{
   internalName = "floor1seabits";
   diffuseMap = "art/shapes/dotsnetcrits/levels/seabits/floor1.png";
   detailMap = "art/shapes/dotsnetcrits/levels/seabits/floor1_d.png";
   normalMap = "art/shapes/dotsnetcrits/levels/seabits/floor1_n.png";
   detailSize = "1";
   isManaged = "1";
   detailBrightness = "1";
   Enabled = "1";
   diffuseSize = "1";
};

singleton Material(floor2seabits)
{
   mapTo = "floor2seabits";
   terrainMaterials = "1";
   ShowDust = "1";
   showFootprints = "1";
   materialTag0 = "Terrain";
};

new TerrainMaterial()
{
   internalName = "floor2seabits";
   diffuseMap = "art/shapes/dotsnetcrits/levels/seabits/floor2.png";
   detailMap = "art/shapes/dotsnetcrits/levels/seabits/floor2_d.png";
   normalMap = "art/shapes/dotsnetcrits/levels/seabits/floor2_n.png";
   detailSize = "1";
   isManaged = "1";
   detailBrightness = "1";
   Enabled = "1";
   diffuseSize = "1";
};

singleton Material(coral0seabits)
{
   diffuseMap[0] = "art/shapes/dotsnetcrits/levels/seabits/coral0.png";
   //detailMap[0] = "art/shapes/dotsnetcrits/levels/seabits/coral0.png";
   normalMap[0] = "art/shapes/dotsnetcrits/levels/seabits/coral0_n.png";
   translucent = false;
   materialTag0 = "Foliage";
   mapTo = "coral0.png";
   castShadows = "0";
   alphaTest = "1";
   alphaRef = "255";
};

singleton Material(coral1seabits)
{
   diffuseMap[0] = "art/shapes/dotsnetcrits/levels/seabits/coral1.png";
   //detailMap[0] = "art/shapes/dotsnetcrits/levels/seabits/coral1.png";
   normalMap[0] = "art/shapes/dotsnetcrits/levels/seabits/coral1_n.png";
   translucent = false;
   materialTag0 = "Foliage";
   mapTo = "coral1.png";
   castShadows = "0";
   alphaTest = "1";
   alphaRef = "255";
};

singleton Material(rockseabits)
{
   diffuseMap[0] = "art/shapes/dotsnetcrits/levels/seabits/rock.png";
   //detailMap[0] = "art/shapes/dotsnetcrits/levels/seabits/rock.png";
   normalMap[0] = "art/shapes/dotsnetcrits/levels/seabits/rock_n.png";
   translucent = false;
   materialTag0 = "Foliage";
   mapTo = "rock.png";
   castShadows = "0";
   alphaTest = "1";
   alphaRef = "255";
};

singleton Material(seaweed0seabits)
{
   diffuseMap[0] = "art/shapes/dotsnetcrits/levels/seabits/seaweed0.png";
   //detailMap[0] = "art/shapes/dotsnetcrits/levels/seabits/seaweed0.png";
   normalMap[0] = "art/shapes/dotsnetcrits/levels/seabits/seaweed0_n.png";
   translucent = false;
   materialTag0 = "Foliage";
   mapTo = "seaweed0.png";
   castShadows = "0";
   alphaTest = "1";
   alphaRef = "255";
};

singleton Material(barnacleseabits)
{
   diffuseMap[0] = "art/shapes/dotsnetcrits/levels/seabits/barnacle.png";
   //detailMap[0] = "art/shapes/dotsnetcrits/levels/seabits/barnacle.png";
   normalMap[0] = "art/shapes/dotsnetcrits/levels/seabits/barnacle_n.png";
   translucent = false;
   mapTo = "barnacle.png";
   castShadows = "0";
   alphaTest = "1";
   alphaRef = "255";
};

datablock DecalData(barnacleDecalseabits)
{
   material = barnacleseabits;
   lifeSpan = -1;
   fadeTime = -1;
   size = 0.1;
   texCols = 1;
   texRows = 1;
   textureCoordCount = 1;
};

singleton Material(shellseabits)
{
   diffuseMap[0] = "art/shapes/dotsnetcrits/levels/seabits/shell.png";
   //detailMap[0] = "art/shapes/dotsnetcrits/levels/seabits/shell.png";
   normalMap[0] = "art/shapes/dotsnetcrits/levels/seabits/shell_n.png";
   translucent = false;
   mapTo = "shell.png";
   castShadows = "0";
   alphaTest = "1";
   alphaRef = "255";
};

datablock DecalData(shellDecalseabits)
{
   material = shellseabits;
   fadeTime = -1;
   lifeSpan = -1;
   size = 0.1;
   texCols = 1;
   texRows = 1;
   textureCoordCount = 1;
};
