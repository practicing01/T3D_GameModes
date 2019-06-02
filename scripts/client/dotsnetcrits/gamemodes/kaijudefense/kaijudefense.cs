function kaijudefenseGMmouse::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
  %mousePoint.z = 1;
  %unprojection = PlayGui.unproject(%mousePoint);

  commandToServer('FirekaijudefenseGM', %unprojection);
}

function kaijudefenseGMmouse::onRightMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
  %this.prevPos_ = %mousePoint;
}

function kaijudefenseGMmouse::onRightMouseDragged(%this,%modifier,%mousePoint,%mouseClickCount)
{
   %delta = VectorSub(%mousePoint, %this.prevPos_);
   %delta = VectorNormalize(%delta);
   %delta = VectorScale(%delta, 20);
   yaw(%delta.x);
   pitch(%delta.y);

   %this.prevPos_ = %mousePoint;
}

function kaijudefenseGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "kaijudefenseGMClientQueue";

  %this.mouseGui_ = new GuiMouseEventCtrl()
  {
     lockMouse = true;
     class = "kaijudefenseGMmouse";
     horizSizing = "windowRelative";
     vertSizing = "windowRelative";
     extent = PlayGui.extent;
     prevPos_ = "0 0";
  };

  Canvas.pushDialog(%this.mouseGui_);
}

function kaijudefenseGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.mouseGui_))
  {
    Canvas.popDialog(%this.mouseGui_);
    %this.mouseGui_.delete();
  }

  echo("kaijudefenseGMClient go bye bye");
}

if (isObject(kaijudefenseGMClientSO))
{
  kaijudefenseGMClientSO.delete();
}
else
{
  new ScriptObject(kaijudefenseGMClientSO)
  {
    class = "kaijudefenseGMClient";
    EventManager_ = "";
    mouseGui_ = "";
  };
}
