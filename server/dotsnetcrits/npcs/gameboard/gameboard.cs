function gameboardScriptMsgListener::onClientLeaveGame(%this, %client)
{
  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %npc = %this.npcArray_.getValue(%index);

    if (isObject(%npc))
    {
      %npc.delete();
    }

    %this.npcArray_.erase(%index);
  }
}

function gameboardScriptMsgListener::onAdd(%this)
{
  %this.npcArray_ = new ArrayObject();

  %dirList = getDirectoryList("art/shapes/dotsnetcrits/tcg/", 1);

  for (%x = 0; %x < getFieldCount(%dirList); %x++)
  {
    %card = getField(%dirList, %x);
    exec("art/shapes/dotsnetcrits/tcg/" @ %card @ "/materials.cs");
  }
}

function gameboardScriptMsgListener::onRemove(%this)
{
  if (isObject(%this.npcArray_))
  {
    for (%x = 0; %x < %this.npcArray_.count(); %x++)
    {
      %npc = %this.npcArray_.getValue(%x);

      if (isObject(%npc))
      {
        %npc.delete();
      }
    }

    %this.npcArray_.delete();
  }
}

function gameboardScriptMsgListener::SpawnNPC(%this, %client)
{
  %fieldList = new ArrayObject();
  return %fieldList;
}

function gameboardScriptMsgListener::onNPCLoadRequest(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %npcName = %data.getValue(%data.getIndexFromKey("npc"));
  %player = %client.getControlObject();

  if (%npcName !$= %this.npc_)
  {
    return;
  }

  commandToClient(%client, 'NPCLoadDNC', %npcName, true);

  %index = %this.npcArray_.getIndexFromKey(%client);

  if (%index != -1)
  {
    %fieldList = %this.npcArray_.getValue(%index);

    if (isObject(%fieldList))
    {
      return;
    }

    //create fieldList.
    %fieldList = %this.SpawnNPC(%client);

    %this.npcArray_.setValue(%fieldList, %index);

    return;
  }

  //add index.
  %fieldList = %this.SpawnNPC(%client);

  %this.npcArray_.add(%client, %fieldList);

}

function gameboardScriptMsgListener::CommandNPC(%this, %action, %card, %index, %player)
{
  if (%action $= "spawn")
  {
    //check if card exists
    %dirList = getDirectoryList("art/shapes/dotsnetcrits/tcg/", 1);

    %card = strlwr(%card);

    for (%x = 0; %x < getFieldCount(%dirList); %x++)
    {
      if (getField(%dirList, %x) $= %card)//Make sure the card exists.
      {
        //credits to irei1as http://forums.torque3d.org/viewtopic.php?f=12&p=5432&sid=361ab2046ec1826bce5b1435cad3eeb3#p5432
        %rayResult = %player.doRaycast(10000.0, %this.rayMask_);

        %objTarget = firstWord(%rayResult);
        %objPos = getWords(%rayResult, 1, 3);
        %objDir = getWords(%rayResult, 4, 6);
        %transform = MatrixCreateFromEuler(%objDir);
        %mat_rotx = MatrixCreateFromEuler( mAtan( mSqrt( %objDir.x*%objDir.x + %objDir.y*%objDir.y), %objDir.z) SPC "0 0");
        //%mat_localzrot = MatrixCreateFromEuler("0 0" SPC %localzrot);
        //%mat_rotx = MatrixMultiply(%mat_rotx,%mat_localzrot);
        %mat_rotz = MatrixCreateFromEuler("0 0" SPC mAtan(%objDir.x,%objDir.y));
        %transform = MatrixMultiply(%mat_rotz,%mat_rotx);

        %cardModel = new TSStatic()
        {
          shapeName = "art/shapes/dotsnetcrits/tcg/card.cached.dts";
          position = %player.position;
          skin = %card;
        };

        DNCServer.dummyCam_.position = %objPos;
        DNCServer.dummyCam_.lookAt(%player.position);

        %cardModel.setTransform(DNCServer.dummyCam_.getTransform());

        %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

        if (%playerIndex != -1)
        {
          %fieldList = %this.npcArray_.getValue(%playerIndex);

          %fieldCount = %fieldList.count();

          %fieldList.add(%fieldList.count(), %cardModel);

          commandToClient(%player.client, 'NPCActiongameboard', "spawn", %fieldCount, %card, -1);
        }

        break;
      }
    }
  }
  else if (%action $= "remove")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      %cardModel.delete();

      %fieldList.erase(%index);

      commandToClient(%player.client, 'NPCActiongameboard', "remove", -1, -1, %index);
    }
  }
  else if (%action $= "move")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      %rayResult = %player.doRaycast(10000.0, %this.rayMask_);

      %objTarget = firstWord(%rayResult);
      %objPos = getWords(%rayResult, 1, 3);
      %objDir = getWords(%rayResult, 4, 6);
      %transform = MatrixCreateFromEuler(%objDir);
      %mat_rotx = MatrixCreateFromEuler( mAtan( mSqrt( %objDir.x*%objDir.x + %objDir.y*%objDir.y), %objDir.z) SPC "0 0");
      //%mat_localzrot = MatrixCreateFromEuler("0 0" SPC %localzrot);
      //%mat_rotx = MatrixMultiply(%mat_rotx,%mat_localzrot);
      %mat_rotz = MatrixCreateFromEuler("0 0" SPC mAtan(%objDir.x,%objDir.y));
      %transform = MatrixMultiply(%mat_rotz,%mat_rotx);

      DNCServer.dummyCam_.position = %objPos;
      DNCServer.dummyCam_.lookAt(%player.position);

      %cardModel.setTransform(DNCServer.dummyCam_.getTransform());
    }
  }
  else if (%action $= "zcw")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      //credits for math to Dimitris Matsuoliadis
      //https://www.garagegames.com/community/forums/viewthread/132114/1#comment-835555
      %objTransform = %cardModel.getTransform();
      %tryRotation = "0 0 0 0 0 1 " @ mDegToRad(90);
      %newTransform = matrixMultiply(%objTransform, %tryRotation);
      %cardModel.setTransform(%newTransform);
    }
  }
  else if (%action $= "zccw")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      %objTransform = %cardModel.getTransform();
      %tryRotation = "0 0 0 0 0 1 " @ mDegToRad(-90);
      %newTransform = matrixMultiply(%objTransform, %tryRotation);
      %cardModel.setTransform(%newTransform);
    }
  }
  else if (%action $= "xcw")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      %objTransform = %cardModel.getTransform();
      %tryRotation = "0 0 0 1 0 0 " @ mDegToRad(90);
      %newTransform = matrixMultiply(%objTransform, %tryRotation);
      %cardModel.setTransform(%newTransform);
    }
  }
  else if (%action $= "xccw")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      %objTransform = %cardModel.getTransform();
      %tryRotation = "0 0 0 1 0 0 " @ mDegToRad(-90);
      %newTransform = matrixMultiply(%objTransform, %tryRotation);
      %cardModel.setTransform(%newTransform);
    }
  }
  else if (%action $= "ycw")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      %objTransform = %cardModel.getTransform();
      %tryRotation = "0 0 0 0 1 0 " @ mDegToRad(-90);
      %newTransform = matrixMultiply(%objTransform, %tryRotation);
      %cardModel.setTransform(%newTransform);
    }
  }
  else if (%action $= "yccw")
  {
    %playerIndex = %this.npcArray_.getIndexFromKey(%player.client);

    if (%playerIndex != -1)
    {
      %fieldList = %this.npcArray_.getValue(%playerIndex);

      %cardModel = %fieldList.getValue(%index);

      %objTransform = %cardModel.getTransform();
      %tryRotation = "0 0 0 0 1 0 " @ mDegToRad(90);
      %newTransform = matrixMultiply(%objTransform, %tryRotation);
      %cardModel.setTransform(%newTransform);
    }
  }

}

function gameboardScriptMsgListener::onNPCActiongameboard(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %action = %data.getValue(%data.getIndexFromKey("action"));
  %card = %data.getValue(%data.getIndexFromKey("card"));
  %index = %data.getValue(%data.getIndexFromKey("index"));
  %player = %client.getControlObject();

  %this.CommandNPC(%action, %card, %index, %player);
}

function serverCmdNPCActiongameboard(%client, %action, %card, %index)
{
  %data = new ArrayObject();
  %data.add("client", %client);
  %data.add("action", %action);
  %data.add("card", %card);
  %data.add("index", %index);
  DNCServer.EventManager_.postEvent("NPCActiongameboard", %data);

  %data.delete();
}

%NPC = new ScriptMsgListener()
{
  class = "gameboardScriptMsgListener";
  npc_ = "gameboard";
  npcArray_ = "";
  rayMask_ = $TypeMasks::StaticObjectType|$TypeMasks::EnvironmentObjectType|$TypeMasks::WaterObjectType|
  $TypeMasks::ShapeBaseObjectType|$TypeMasks::StaticShapeObjectType|$TypeMasks::DynamicShapeObjectType|
  $TypeMasks::PlayerObjectType|$TypeMasks::ItemObjectType|$TypeMasks::VehicleObjectType|
  $TypeMasks::CorpseObjectType;
};

DNCServer.loadedNPCs_.add(%NPC);
DNCServer.EventManager_.registerEvent("NPCActiongameboard");
DNCServer.EventManager_.subscribe(%NPC, "NPCLoadRequest");
DNCServer.EventManager_.subscribe(%NPC, "NPCActiongameboard");
DNCServer.ClientLeaveListeners_.add(%NPC);
