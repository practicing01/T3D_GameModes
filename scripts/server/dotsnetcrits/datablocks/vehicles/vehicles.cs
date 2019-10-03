datablock HoverVehicleData(HoverWhaleDB)
{
   category = "Vehicles";
   shapeFile = "art/shapes/dotsnetcrits/npcs/hoverwhale/whale.dae";
   emap = 1;

   dragForce = 0.01;
   stabLenMin = 0.5;
   stabLenMax = 2.0;
   normalForce = 30;
   stabSpringConstant = 30;
   mainThrustForce = 100.0;
   reverseThrustForce = 100.0;
};

datablock HoverVehicleData(ShortbusHVD)
{
   category = "Vehicles";
   shapeFile = "art/shapes/dotsnetcrits/gamemodes/shortbus/shortbus.dae";

   mass = "0.1";
   dragForce = 0;
   floatingGravMag = 0.1;
   gyroDrag = 30;
   mainThrustForce = 50;
   reverseThrustForce = 50;
   strafeThrustForce = 0;
   steeringForce = 25;
   normalForce = 1;
   pitchForce = 2.5;
   rollForce = 2.5;
   stabLenMin = 0.5;
   stabLenMax = 2;
   stabSpringConstant = 0.1;
   stabDampingConstant = 0.1;
   vertFactor = 1;
};
