function QuickTypeGMServer::NewWord(%this)
{
  %this.curEntry_ = getRandom(0, %this.entryCount_);

  %word = %this.dictionary_.getValue(%this.curEntry_);

  bottomPrintAll( %word, 5000, 1 );

  %this.wordSchedule_ = %this.schedule(5000, "NewWord");
}

function QuickTypeGMServer::onAdd(%this)
{
  %this.EventManager_ = new EventManager();

  %this.EventManager_.queue = "QuickTypeGMServerQueue";

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

  %this.wordSchedule_ = %this.schedule(5000, "NewWord");
}

function QuickTypeGMServer::onRemove(%this)
{
  if (isObject(%this.EventManager_))
  {
    %this.EventManager_.delete();
  }

  if (isObject(%this.dictionary_))
  {
    %this.dictionary_.delete();
  }

  cancel(%this.wordSchedule_);
}

function QuickTypeGMServer::onChat(%this, %client, %text)
{
  %word = %this.dictionary_.getValue(%this.curEntry_);

  if (%text $= %word)
  {
    Game.incScore(%client, 1, false);

    cancel(%this.wordSchedule_);
    %this.NewWord();
  }
}

if (isObject(QuickTypeGMServerSO))
{
  QuickTypeGMServerSO.delete();
}
else
{
  new ScriptObject(QuickTypeGMServerSO)
  {
    class = "QuickTypeGMServer";
    EventManager_ = "";
    entryCount_ = 0;
    curEntry_ = 0;
    dictionary_ = "";
    wordSchedule_ = "";
  };

  DNCServer.chatListeners_.add(QuickTypeGMServerSO);
  MissionCleanup.add(QuickTypeGMServerSO);
}
