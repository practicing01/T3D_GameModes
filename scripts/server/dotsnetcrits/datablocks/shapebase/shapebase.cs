function ShapeBase::setDamageDt(%this, %damageAmount, %damageType, %interval)
{
   if (%this.getState() !$= "Dead")
   {
      %this.damage(0, "0 0 0", %damageAmount, %damageType);
      %this.damageSchedule = %this.schedule(%interval, "setDamageDt", %damageAmount, %damageType, %interval);
   }
   else
      %this.damageSchedule = "";
}
