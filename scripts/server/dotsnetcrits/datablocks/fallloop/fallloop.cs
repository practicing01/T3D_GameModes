function FallloopTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  %randyOffset = getRandom(-100, 100) SPC getRandom(-100, 100) SPC 100;
  //%randyPos = VectorAdd(%obj.position, %randyOffset);

  %obj.position = %randyOffset;
}
