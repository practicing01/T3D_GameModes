//exec("art/shapes/dotsnetcrits/gamemodes/randicon/materials.cs");

/*datablock RigidShapeData( RandiconRB )
{
   category = "RigidShape";

   shapeFile = "art/shapes/dotsnetcrits/gamemodes/randicon/randicon.dae";
   isInvincible = 0;
   maxDamage = 50;
   destroyedLevel = 50;
   disabledLevel = 50;
   density = 1;
   // Rigid Body
   mass = 200;
   massCenter = "0 0 0";    // Center of mass for rigid body
   massBox = "0 0 0";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.2;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance

   minRollSpeed = 10;

   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
};*/

function RandiconGMServer::SpawnRandicon(%this)
{
  if (isObject(%this.randicon_))
  {
    if (isObject(%this.randicon_.renderMesh_))
    {
      %this.randicon_.renderMesh_.delete();
    }

    %this.randicon_.delete();
  }

  %this.randicon_ = new AiPlayer()
  {
     dataBlock = "RandiconAI";
     class = "RandiconAIClass";
     state_ = "idle";
     renderMesh_ = "";
  };

  %this.randicon_.isRenderEnabled = false;

  //%spawnPoint = PlayerDropPoints.getRandom();
  //%this.randicon_.setTransform(%spawnPoint.getTransform());

  %player = ClientGroup.getRandom().getControlObject();
  %this.randicon_.setTransform(%player.getTransform());
  %this.randicon_.followObject(%player, 1);

  %mat = %this.textureFileNames_.getRandom();
  //%mapTo = %this.randicon_.getTargetName( 0 );
  //%this.randicon_.changeMaterial( %mapTo, 0, %mat );

  %this.randicon_.renderMesh_ = new RenderMeshExample()
  {
    material = %mat;
    scale = "1 1 1";
  };

  %this.randicon_.mountObject(%this.randicon_.renderMesh_, 0, MatrixCreate("0 0 0.5", "1 0 0 0"));
}

function RandiconGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "RandiconGMServerQueue";

  %this.textureFileNames_ = new SimSet();

  //%fileWrite = new FileObject();
  //%result = %fileWrite.OpenForWrite("./test.txt");

  for( %file = findFirstFile( "art/shapes/dotsnetcrits/gamemodes/randicon/*.png" ); %file !$= ""; %file = findNextFile() )
  {
    %filename = fileName(%file);
    %filename = strreplace(%filename, ".png", "");
    //%file = strreplace(%file, ".png", "");

    /*singleton Material(%filename)
    {
      diffuseMap[0] = %file;
    };*/

    //%fileWrite.writeLine("singleton Material(" @ %filename @ ")");
    //%fileWrite.writeLine("{");
    //%fileWrite.writeLine("diffuseMap[0] = \"" @ %file @ "\";");
    //%fileWrite.writeLine("};");

    %this.textureFileNames_.add(%filename);
  }

  //%fileWrite.close();
  //%fileWrite.delete();

  %this.SpawnRandicon();
}

function RandiconGMServer::onRemove(%this)
{
  if (isObject(%this.textureFileNames_))
  {
    %this.textureFileNames_.delete();
  }

  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.randicon_))
  {
    if (isObject(%this.randicon_.renderMesh_))
    {
      %this.randicon_.renderMesh_.delete();
    }

    %this.randicon_.delete();
  }
}

function RandiconAI::onDisabled(%this, %obj, %state)
{
  if (isObject(RandiconGMServerSO))
  {
    RandiconGMServerSO.schedule(1000, "SpawnRandicon");
  }
}

/*function RandiconRB::onCollision(%this, %obj, %collObj, %vec, %len)
{
  %mat = RandiconGMServerSO.textureFileNames_.getRandom();
  %mapTo = %obj.getTargetName( 0 );
  %obj.changeMaterial( %mapTo, 0, %mat );
  //%obj.setHidden(true);//crash
  //%obj.setHidden(false);
  //%obj.setCloaked(true);
  //%obj.setCloaked(false);
  //%obj.schedule(1000, "setHidden", false);
}*/

function RandiconGMServer::loadOut(%this, %player)
{
  //
}

if (isObject(RandiconGMServerSO))
{
  if (isObject(RandiconGMServerSO.textureFileNames_))
  {
    RandiconGMServerSO.textureFileNames_.delete();
  }

  RandiconGMServerSO.delete();
}
else
{
  new ScriptObject(RandiconGMServerSO)
  {
    class = "RandiconGMServer";
    EventManager_ = "";
    randicon_ = "";
    textureFileNames_ = "";
  };

  DNCServer.loadOutListeners_.add(RandiconGMServerSO);
  MissionCleanup.add(RandiconGMServerSO);
}
