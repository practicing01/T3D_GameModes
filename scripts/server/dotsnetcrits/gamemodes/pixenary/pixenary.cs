function PixenaryStaticShapeData::damage(%this, %shape, %sourceObject, %position, %amount, %damageType)
{
  if (%shape.color_ == false)
  {
    %shape.color_ = true;
    %shape.setSkinName("black");
  }
  else
  {
    %shape.color_ = false;
    %shape.setSkinName("base");
  }
}

function PixenaryGMServer::NewWord(%this)
{
  for (%x = 0; %x < %this.pixels_.getCount(); %x++)
  {
    %pixel = %this.pixels_.getObject(%x);

    if (%pixel.color_ != false)
    {
      %pixel.color_ = false;
      %pixel.setSkinName("base");
    }
  }

  clearBottomPrint( %this.curArtistClient_ );

  %this.curArtistClient_ = ClientGroup.getRandom();

  %this.curEntry_ = getRandom(0, %this.entryCount_);

  %word = %this.dictionary_.getValue(%this.curEntry_);

  bottomPrint( %this.curArtistClient_, %word, %this.drawTime_, 1 );

  %this.wordSchedule_ = %this.schedule(%this.drawTime_, "NewWord");
}

function PixenaryGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "PixenaryGMServerQueue";

  %this.dictionary_ = new ArrayObject();

  %file = new FileStreamObject();
  %file.open("art/shapes/dotsnetcrits/gamemodes/quicktype/quicktype.txt", "read");

  while( !%file.isEOF() )
  {
    %line = %file.readLine();

    if (%line $= "")
    {
      continue;
    }

    %line = strlwr(%line);

    %this.entryCount_++;
    %this.dictionary_.add(%this.entryCount_, %line);
  }

  %file.close();
  %file.delete();

  %this.pixels_ = new SimSet();

  %this.curArtistClient_ = ClientGroup.getRandom();

  %player = %this.curArtistClient_.getControlObject();

  %teleDir = %player.getForwardVector();

  %scaledDir = VectorScale(%teleDir, 10);

  for (%y = 0; %y < 16; %y++)
  {
    for (%x = -8; %x < 8; %x++)
    {
      %offset = VectorAdd( %player.position, (%x * 0.2) SPC 0 SPC (%y * 0.2) );
      %projection = VectorAdd( %offset, %scaledDir );

      %pixel = new StaticShape()
      {
        dataBlock = "PixenaryStaticShapeData";
        class = "PixenaryClass";
        parent_ = %this;
        position = %projection;
        scale = "0.1 0.1 0.1";
        color_ = false;
      };

      %this.pixels_.add(%pixel);
    }
  }

  %this.NewWord();
}

function PixenaryGMServer::onRemove(%this)
{
  clearBottomPrint( %this.curArtistClient_ );

  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.dictionary_))
  {
    %this.dictionary_.delete();
  }

  if (isObject(%this.pixels_))
  {
    %this.pixels_.deleteAllObjects();
    %this.pixels_.delete();
  }

  cancel(%this.wordSchedule_);
}

function PixenaryGMServer::onChat(%this, %client, %text)
{
  %word = %this.dictionary_.getValue(%this.curEntry_);

  if (%text $= %word)
  {
    if (%client != %this.curArtistClient_)
    {
      Game.incScore(%client, 1, false);
    }

    cancel(%this.wordSchedule_);
    %this.NewWord();
  }
}

if (isObject(PixenaryGMServerSO))
{
  PixenaryGMServerSO.delete();
}
else
{
  new ScriptObject(PixenaryGMServerSO)
  {
    class = "PixenaryGMServer";
    EventManager_ = "";
    entryCount_ = 0;
    curEntry_ = 0;
    dictionary_ = "";
    wordSchedule_ = "";
    pixels_ = "";
    curArtistClient_ = "";
    drawTime_ = 120000;
  };

  DNCServer.chatListeners_.add(PixenaryGMServerSO);
  MissionCleanup.add(PixenaryGMServerSO);
}
