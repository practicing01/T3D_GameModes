function HexagonSwitchClass::UseObject(%this, %player)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%player, %spawnPoint);
  %player.setTransform(%transform);
}

function HexagonGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "HexagonGMServerQueue";

  %this.hexagons_ = new SimSet();
  %this.switches_ = new SimSet();

  for (%x = 0; %x < PlayerDropPoints.getCount(); %x++)
  {
    %spawnpoint = PlayerDropPoints.getObject(%x);
    %transform = %spawnpoint.getTransform();

    %hexagon = new TSStatic()
    {
      shapeName = "art/shapes/dotsnetcrits/gamemodes/hexagon/hexagon.dae";
      collisionType = "Visible Mesh";
      decalType = "Visible Mesh";
      allowPlayerStep = "1";
      parent_ = %this;
    };

    %hexagon.setTransform(%transform);

    %this.hexagons_.add(%hexagon);

    %switch = new StaticShape()
    {
       dataBlock = "HexagonSwitchStaticShapeData";
       class = "HexagonSwitchClass";
       parent_ = %this;
    };

    %switch.setTransform(%transform);

    %this.switches_.add(%switch);
  }
}

function HexagonGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.hexagons_))
  {
    %this.hexagons_.deleteAllObjects();
    %this.hexagons_.delete();
  }

  if (isObject(%this.switches_))
  {
    %this.switches_.deleteAllObjects();
    %this.switches_.delete();
  }

}

if (isObject(HexagonGMServerSO))
{
  HexagonGMServerSO.delete();
}
else
{
  new ScriptObject(HexagonGMServerSO)
  {
    class = "HexagonGMServer";
    EventManager_ = "";
    hexagons_ = "";
    switches_ = "";
  };

  MissionCleanup.add(HexagonGMServerSO);
}
