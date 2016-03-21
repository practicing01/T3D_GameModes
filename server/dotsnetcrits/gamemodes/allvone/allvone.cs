function ALLVONEGMServer::onAdd(%this)
{
  MissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "ALLVONEGMServerQueue";

  for (%x = 0; %x < ClientGroup.getCount(); %x++)
  {
    %obj = ClientGroup.getObject(%x);
    
    %data = new ArrayObject();
    %data.add("client", %obj);
    %data.add("team", 0);
    DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);
    
    %data.delete();
  }
  
  %this.theOne_ = ClientGroup.getRandom();
  
  %data = new ArrayObject();
  %data.add("client", %this.theOne_);
  %data.add("team", 1);
  DNCServer.EventManager_.postEvent("TeamJoinRequest", %data);
  
  %data.delete();

}

function ALLVONEGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

}

function ALLVONEGMServer::loadOut(%this, %player)
{
  echo("ALLVONEGMServer::loadOut");
}

if (isObject(ALLVONEGMServerSO))
{
  ALLVONEGMServerSO.delete();
}
else
{
  %gmSO = new ScriptObject()
  {
    class = "ALLVONEGMServer";
  };
  
  DNCServer.loadOutListeners_.add(%gmSO);
  MissionCleanup.add(%gmSO);

  new ScriptObject(ALLVONEGMServerSO)
  {
    class = "ALLVONEGMServer";
    EventManager_ = "";
    theOne_ = "";
  };
}
