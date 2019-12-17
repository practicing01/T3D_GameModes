function OpenHouseGMmouse::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
  %mousePoint.z = 1;
  %unprojection = PlayGui.unproject(%mousePoint);

  commandToServer('FireOpenHouseGM', %unprojection);
}

function OpenHouseGMClient::onAdd(%this)
{
  ClientMissionCleanup.add(%this);

  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "OpenHouseGMClientQueue";

  %this.mouseGui_ = new GuiMouseEventCtrl()
  {
     lockMouse = true;
     class = "OpenHouseGMmouse";
     horizSizing = "windowRelative";
     vertSizing = "windowRelative";
     extent = PlayGui.extent;
     prevPos_ = "0 0";
  };

  Canvas.pushDialog(%this.mouseGui_);

  %this.cursor_ = new GuiCursor(OpenHouseCursor)
  {
     hotSpot = "31 31";
     renderOffset = "0 0";
     bitmapName = "~/art/gui/dotsnetcrits/crossHair";
  };

  Canvas.setCursor(%this.cursor_);
}

function OpenHouseGMClient::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  Canvas.setCursor(DefaultCursor);

  if (isObject(%this.mouseGui_))
  {
    Canvas.popDialog(%this.mouseGui_);
    %this.mouseGui_.delete();
  }

  if (isObject(%this.cursor_))
  {
    %this.cursor_.delete();
  }

  echo("OpenHouseGMClient go bye bye");
}

if (isObject(OpenHouseGMClientSO))
{
  OpenHouseGMClientSO.delete();
}
else
{
  new ScriptObject(OpenHouseGMClientSO)
  {
    class = "OpenHouseGMClient";
    EventManager_ = "";
    mouseGui_ = "";
    cursor_ = "";
  };
}
