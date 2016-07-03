function GlassPitTrigger::onEnterTrigger(%this, %trigger, %obj)
{
  %obj.damage(%trigger, "0 0 0", 1000, "sand");
}
