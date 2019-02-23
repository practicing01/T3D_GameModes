// Torque Input Map File
if (isObject(vehicleMap)) vehicleMap.delete();
new ActionMap(vehicleMap);
vehicleMap.bindCmd(keyboard, "ctrl x", "commandToServer(\'flipCar\');", "");
vehicleMap.bind(keyboard, "w", moveForward);
vehicleMap.bind(keyboard, "s", movebackward);
vehicleMap.bind(keyboard, "up", moveForward);
vehicleMap.bind(keyboard, "down", movebackward);
vehicleMap.bindCmd(keyboard, "ctrl f", "getout();", "");
vehicleMap.bind(keyboard, "space", brake);
vehicleMap.bindCmd(keyboard, "l", "brakeLights();", "");
vehicleMap.bindCmd(keyboard, "escape", "", "handleEscape();");
vehicleMap.bind(keyboard, "v", toggleFreeLook);
vehicleMap.bind(keyboard, "alt c", toggleCamera);
vehicleMap.bind(mouse0, "xaxis", yaw);
vehicleMap.bind(mouse0, "yaxis", pitch);
vehicleMap.bind(mouse0, "button0", mouseFire);
vehicleMap.bind(mouse0, "button1", altTrigger);
