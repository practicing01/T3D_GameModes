//Globals are so naughty :0!
function clientCmdSensitiveSnake(%sensitivity)
{
  $pref::Input::LinkMouseSensitivity = %sensitivity;
  schedule(2000, 0, "ResetSensitiveSnake");
}

function ResetSensitiveSnake()
{
  $pref::Input::LinkMouseSensitivity = 0.214615;
}
