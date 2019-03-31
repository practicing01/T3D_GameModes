/*
Simset tileGroup;
ScriptObject tile;

struct
{
  Vector3 position_;
  float distanceToGoal_;
  tile connectedTiles_[];
  bool deadend_;
}tile;
*/

//function GimpStar::SortCallback(%this, %a, %b)
function GimpStarSort(%a, %b)
{
  if (%a.distanceToGoal_ < %b.distanceToGoal_)
  {
    return -1;
  }
  else if (%a.distanceToGoal_ > %b.distanceToGoal_)
  {
    return 1;
  }
  else if (%a.distanceToGoal_ == %b.distanceToGoal_)
  {
    return 0;
  }
}

function GimpStar::GetCloser(%this, %curTile, %endTile, %curPath)
{
  if (%curTile.connectedTiles_.getCount() == 0)
  {
    return %curTile;
  }

  if (%curTile == %endTile)
  {
    return %endTile;
  }

  %sortedList = new SimSet();

  for (%x = 0; %x < %curTile.connectedTiles_.getCount(); %x++)
  {
    %testTile = %curTile.connectedTiles_.getObject(%x);

    if (%testTile.deadend_ == true || %curPath.isMember(%testTile) == true)
    {
      continue;
    }

    %sortedList.add(%testTile);
  }

  if (%sortedList.getCount())
  {
    %sortedList.sort("GimpStarSort");
    //%sortedList.sort("GimpStar::SortCallback");

    %nextTile = %sortedList.getObject(0);
    %sortedList.delete();
    return %nextTile;
  }
  else
  {
    %sortedList.delete();
    return %curTile;
  }
}

function GimpStar::ClearDeadEnds(%this, %tileGroup)
{
  for (%x = 0; %x < %tileGroup.getCount(); %x++)
  {
    %tile = %tileGroup.getObject(%x);
    %tile.deadend_ = false;
  }
}

function GimpStarPathSort(%a, %b)
{
  if (%a.getCount() < %b.getCount())
  {
    return -1;
  }
  else if (%a.getCount() > %b.getCount())
  {
    return 1;
  }
  else if (%a.getCount() == %b.getCount())
  {
    return 0;
  }
}

function GimpStar::GetPath(%this, %tileGroup, %startTile, %endTile)
{
  %this.ClearDeadEnds(%tileGroup);
  
  if (%startTile.connectedTiles_.getCount() == 0)
  {
    return -1;
  }

  for (%x = 0; %x < %tileGroup.getCount(); %x++)
  {
    %tile = %tileGroup.getObject(%x);

    %tile.distanceToGoal_ = VectorDist(%tile.position_, %endTile.position_);
  }

  %paths = new SimSet();

  for (%x = 0; %x < %startTile.connectedTiles_.getCount(); %x++)
  {
    //%this.ClearDeadEnds(%tileGroup);//this gives erroneous results
    %connectedTile = %startTile.connectedTiles_.getObject(%x);
    %curPath = new SimSet();
    %curPath.add(%connectedTile);
    %paths.add(%curPath);

    %curTile = %connectedTile;
    %index = 0;

    while (1)
    {
      %nextTile = %this.GetCloser(%curTile, %endTile, %curPath);

      if (%nextTile == %curTile && %nextTile != %endTile)//dead end
      {
        %curTile.deadend_ = true;

        if (%index == 0)
        {
          break;
        }
        else
        {
          %index--;
          %curTile = %curPath.getObject(%index);
        }
      }
      else if (%nextTile == %curTile && %nextTile == %endTile)//yay!
      {
        break;
      }
      else if (%nextTile != %curTile)
      {
        %curPath.add(%nextTile);
        %curTile = %nextTile;
      }
    }
  }

  for (%x = 0; %x < %paths.getCount(); %x++)
  {
    %path = %paths.getObject(%x);

    %tile = %path.getObject(%path.getCount() - 1);

    if (%tile != %endTile)
    {
      %paths.remove(%path);
      %path.delete();
      %x = -1;
    }
  }

  if (%paths.getCount())
  {
    %paths.sort("GimpStarPathSort");
    %shortestPath = %paths.getObject(0);
    %paths.remove(%shortestPath);
    %paths.deleteAllObjects();
    %paths.delete();

    return %shortestPath;
  }
  else
  {
    return -1;
  }
}
