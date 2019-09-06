function DotsNetCritsServer::GetRayHitPos(%this, %dir, %dist, %offsetScale, %mask, %obj)
{
  if (!isObject(%obj))//order of operations clusterfuck with triggers returns false positives
  {
    return "0 0 0";
  }

  %startPos = %obj.position;
  %offset = VectorScale(%dir, %dist);
  %endPos = VectorAdd(%startPos, %offset);
  %rayResult = containerRayCast(%startPos, %endPos, %mask, %obj);
  %objTarget = firstWord(%rayResult);

  if (isObject(%objTarget))
  {
    %hitpos = getWords(%rayResult, 1, 3);

    %size = %obj.getObjectBox();
    %scale = %obj.getScale();
    %sizex = (getWord(%size, 3) - getWord(%size, 0)) * getWord(%scale, 0);
    %sizex *= 1.5;

    %inverseDir = VectorScale(%dir, -1);

    %offset = VectorAdd( %hitpos, VectorScale(%inverseDir, %sizex * %offsetScale) );

    return %offset;
  }

  return %startPos;
}
