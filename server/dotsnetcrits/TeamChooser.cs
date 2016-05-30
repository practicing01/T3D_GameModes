function TeamChooser::onAdd(%this)//todo: use 2d simset for arbitrary team count.
{
  %this.teamA_ = new SimSet();
  %this.teamAParticleEmitters_ = new SimSet();
  %this.teamB_ = new SimSet();
  %this.teamBParticleEmitters_ = new SimSet();

}

function TeamChooser::onRemove(%this)
{
  if (isObject(%this.teamA_))
  {
    %this.teamA_.delete();
  }
  if (isObject(%this.teamB_))
  {
    %this.teamB_.delete();
  }
  if (isObject(%this.teamAParticleEmitters_))
  {
    %this.teamAParticleEmitters_.deleteAllObjects();
    %this.teamAParticleEmitters_.delete();
  }
  if (isObject(%this.teamBParticleEmitters_))
  {
    %this.teamBParticleEmitters_.deleteAllObjects();
    %this.teamBParticleEmitters_.delete();
  }
}

function TeamChooser::CreateTeamParticleEmitter(%this, %team, %client)
{
  %emitterNode = "";

  if (%team == 0)
  {
    %emitterNode = new ParticleEmitterNode()
    {
      datablock = teamEmitterNodeData;
      emitter = teamAOutlinerEmitter;
      active = true;
      velocity = 0.0;
    };
  }
  else
  {
    %emitterNode = new ParticleEmitterNode()
    {
      datablock = teamEmitterNodeData;
      emitter = teamBOutlinerEmitter;
      active = true;
      velocity = 0.0;
    };
  }

  %index = DNCServer.ClientLeaveCleanup_.getIndexFromKey(%client);

  if (%index == -1)
  {
    %simset = new SimSet();
    %simset.add(%emitterNode);
    DNCServer.ClientLeaveCleanup_.add(%client, %simset);
  }
  else
  {
    %simset = DNCServer.ClientLeaveCleanup_.getValue(%index);
    %simset.add(%emitterNode);
  }

  return %emitterNode;
}

function TeamChooser::onTeamJoinRequest(%this, %data)
{
  %client = %data.getValue(%data.getIndexFromKey("client"));
  %team = %data.getValue(%data.getIndexFromKey("team"));

  %obj = %client.getControlObject();

  if (%team == 0)
  {
    if (%this.teamA_.isMember(%client))
    {
      %emitterNode = %this.teamAParticleEmitters_.getObject(%this.teamA_.getObjectIndex(%client));
      %this.teamAParticleEmitters_.remove(%emitterNode);
      %emitterNode.delete();
      %this.teamA_.remove(%client);
    }
    else
    {
      %this.teamA_.add(%client);

      %emitterNode = %this.CreateTeamParticleEmitter(%team, %client);
      %this.teamAParticleEmitters_.add(%emitterNode);

      %obj.mountObject(%emitterNode, 1, MatrixCreate("0 0 1", "1 0 0 0"));
    }

    if (%this.teamB_.isMember(%client))
    {
      %particleEmitter = %this.teamBParticleEmitters_.getObject(%this.teamB_.getObjectIndex(%client));
      %this.teamBParticleEmitters_.remove(%particleEmitter);
      %particleEmitter.delete();
      %this.teamB_.remove(%client);
    }
  }
  else
  {
    if (%this.teamB_.isMember(%client))
    {
      %emitterNode = %this.teamBParticleEmitters_.getObject(%this.teamB_.getObjectIndex(%client));
      %this.teamBParticleEmitters_.remove(%emitterNode);
      %emitterNode.delete();
      %this.teamB_.remove(%client);
    }
    else
    {
      %this.teamB_.add(%client);

      %emitterNode = %this.CreateTeamParticleEmitter(%team, %client);
      %this.teamBParticleEmitters_.add(%emitterNode);

      %obj.mountObject(%emitterNode, 1, MatrixCreate("0 0 1", "1 0 0 0"));
    }

    if (%this.teamA_.isMember(%client))
    {
      %particleEmitter = %this.teamAParticleEmitters_.getObject(%this.teamA_.getObjectIndex(%client));
      %this.teamAParticleEmitters_.remove(%particleEmitter);
      %particleEmitter.delete();
      %this.teamA_.remove(%client);
    }
  }

}

function TeamChooser::loadOut(%this, %player)
{
  %client = %player.client;

  if (%this.teamA_.isMember(%client))
  {
    %index = %this.teamA_.getObjectIndex(%client);
    %player.mountObject(%this.teamAParticleEmitters_.getObject(%index), 1, MatrixCreate("0 0 1", "1 0 0 0"));
  }
  else
  {
    %index = %this.teamB_.getObjectIndex(%client);
    %player.mountObject(%this.teamBParticleEmitters_.getObject(%index), 1, MatrixCreate("0 0 1", "1 0 0 0"));
  }

}
