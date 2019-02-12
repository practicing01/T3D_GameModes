function PizzaDeliveryPizzaClass::UseObject(%this, %player)
{
  %player.mountObject(%this, 0, MatrixCreate("0 0 0", "1 0 0 0"));
  %this.hidden = true;

  %player.mountObject(%this.parent_.pizzaDummy_, 0, MatrixCreate("0 0 0", "1 0 0 0"));
  %this.parent_.pizzaDummy_.hidden = false;
}

function PizzaDeliveryMicrowaveClass::UseObject(%this, %player)
{
  if (!%this.parent_.pizza_.isMounted())
  {
    return;
  }

  %pizzaMount = %this.parent_.pizza_.getObjectMount();

  if (%player == %pizzaMount)
  {
    Game.incScore(%player.client, 10, false);

    %this.parent_.pizza_.unmount();
    %this.parent_.pizza_.hidden = false;

    %this.parent_.pizzaDummy_.unmount();
    %this.parent_.pizzaDummy_.hidden = true;

    %this.parent_.RandySpawnTrans(%this.parent_.pizza_);

    %this.parent_.RandySpawnTrans(%this);
  }
}

function PizzaDeliveryGMServer::SpawnMicrowave(%this)
{
  %this.microwave_ = new StaticShape()
  {
     dataBlock = "PizzaDeliveryMicrowaveStaticShapeData";
     class = "PizzaDeliveryMicrowaveClass";
     parent_ = %this;
  };

  %this.RandySpawnTrans(%this.microwave_);
}

function PizzaDeliveryGMServer::SpawnPizza(%this)
{
  %this.pizza_ = new StaticShape()
  {
     dataBlock = "PizzaDeliveryPizzaStaticShapeData";
     class = "PizzaDeliveryPizzaClass";
     parent_ = %this;
  };

  %this.RandySpawnTrans(%this.pizza_);

  %this.pizzaDummy_ = new TSStatic()
  {
    shapeName = "art/shapes/dotsnetcrits/gamemodes/pizzadelivery/pizza/pizza.dae";
    collisionType = "none";
    parent_ = %this;
    hidden = true;
  };
}

function PizzaDeliveryGMServer::RandySpawnTrans(%this, %obj)
{
  %spawnPoint = PlayerDropPoints.getRandom();
  %transform = GameCore::pickPointInSpawnSphere(%obj, %spawnPoint);
  %obj.setTransform(%transform);
}

function PizzaDeliveryGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PizzaDeliveryGMServerQueue";

  %this.SpawnMicrowave();

  %this.SpawnPizza();
}

function PizzaDeliveryGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.pizzaDummy_))
  {
    %this.pizzaDummy_.unmount();
    %this.pizzaDummy_.delete();
  }

  if (isObject(%this.pizza_))
  {
    %this.pizza_.unmount();
    %this.pizza_.delete();
  }

  if (isObject(%this.microwave_))
  {
    %this.microwave_.delete();
  }
}

function PizzaDeliveryGMServer::loadOut(%this, %player)
{
  //
}

if (isObject(PizzaDeliveryGMServerSO))
{
  PizzaDeliveryGMServerSO.delete();
}
else
{
  new ScriptObject(PizzaDeliveryGMServerSO)
  {
    class = "PizzaDeliveryGMServer";
    EventManager_ = "";
    pizza_ = "";
    pizzaDummy_ = "";
    microwave_ = "";
  };

  DNCServer.loadOutListeners_.add(PizzaDeliveryGMServerSO);
  MissionCleanup.add(PizzaDeliveryGMServerSO);
}
