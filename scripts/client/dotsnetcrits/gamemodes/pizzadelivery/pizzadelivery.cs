function PizzaDeliveryGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PizzaDeliveryGMClientQueue";

}

function PizzaDeliveryGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }
  echo("PizzaDeliveryGMClient go bye bye");
}

if (isObject(PizzaDeliveryGMClientSO))
{
  PizzaDeliveryGMClientSO.delete();
}
else
{
  new ScriptObject(PizzaDeliveryGMClientSO)
  {
    class = "PizzaDeliveryGMClient";
    EventManager_ = "";
  };
}
