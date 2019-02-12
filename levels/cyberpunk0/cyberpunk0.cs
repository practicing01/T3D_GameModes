function Cyberpunk0Level::onAdd(%this)
{
  %gridWidth = 10;
  %gridLength = 10;
  %gridHeight = 10;
  %tileWidth = 25;
  %tileLength = 25;
  %tileHeight = 12;

  %this.vectorSet_ = new SimSet();

  for (%y = 0; %y < %gridLength; %y++)
  {
    for (%x = 0; %x < %gridWidth; %x++)
    {
      %vectorSO = new ScriptObject()
      {
        position_ = %x SPC %y;
        height_ = getRandom(1, %gridHeight);
      };

      %this.vectorSet_.add(%vectorSO);
    }
  }

  %count = %this.vectorSet_.getCount();
  %count = %count * 0.5;

  for (%i = 0; %i < %count; %i++)
  {
    %randy = %this.vectorSet_.getRandom();

    %x = getWord(%randy.position_, 0);
    %x = %x * %tileWidth;
    %y = getWord(%randy.position_, 1);
    %y = %y * %tileLength;
    %position = %x SPC %y SPC "2";

    %spawnPoint = new SpawnSphere() {
       autoSpawn = "0";
       spawnTransform = "0";
       radius = "5";
       sphereWeight = "1";
       indoorWeight = "1";
       outdoorWeight = "1";
       isAIControlled = "0";
       dataBlock = "SpawnSphereMarker";
       position = %position;
       rotation = "1 0 0 0";
       scale = "1 1 1";
       canSave = "1";
       canSaveDynamicFields = "1";
          enabled = "1";
          homingCount = "0";
          lockCount = "0";
    };

    PlayerDropPoints.add(%spawnPoint);

    %this.vectorSet_.remove(%randy);
  }

  %count = %this.vectorSet_.getCount();

  for (%i = 0; %i < %count; %i++)
  {
    %vectorSO = %this.vectorSet_.getObject(%i);

    %x = getWord(%vectorSO.position_, 0);
    %x = %x * %tileWidth;
    %y = getWord(%vectorSO.position_, 1);
    %y = %y * %tileLength;

    for (%z = 0; %z < %vectorSO.height_; %z++)
    {
      %position = %x SPC %y SPC (%z * %tileHeight);

      %tile = new TSStatic() {
         shapeName = "art/shapes/dotsnetcrits/levels/cyberpunk0/buildingTile.dae";
         playAmbient = "1";
         meshCulling = "0";
         originSort = "0";
         collisionType = "Visible Mesh";
         decalType = "Visible Mesh";
         allowPlayerStep = "1";
         alphaFadeEnable = "0";
         alphaFadeStart = "100";
         alphaFadeEnd = "150";
         alphaFadeInverse = "0";
         renderNormals = "0";
         forceDetail = "-1";
         position = %position;
         rotation = "1 0 0 0";
         scale = "1 1 1";
         canSave = "1";
         canSaveDynamicFields = "1";
      };

      MissionGroup.add(%tile);
    }
  }

}

function Cyberpunk0Level::onRemove(%this, %ai)
{
  %this.vectorSet_.deleteAllObjects();
  %this.vectorSet_.delete();
}
