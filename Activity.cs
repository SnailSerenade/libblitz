using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Sandbox;

namespace libblitz;

public abstract class ActivityResult : BaseNetworkable
{
	public string Name => GetType().Name;

	public virtual string Serialize() => JsonSerializer.Serialize( this );
	public virtual T Deserialize<T>( string data ) where T : ActivityResult => JsonSerializer.Deserialize<T>( data );
}

public abstract class BaseActivity : BaseNetworkable
{
	[Net] public Guid Uid { get; init; }

	[Net] private List<Guid> MemberUids { get; init; } = new();

	// todo: OPTIMIZE!!!!
	public IEnumerable<GameMember> Members => Game.Current.Members.Where( v => MemberUids.Contains( v.Uid ) );

	[Net] private List<Guid> ActorUids { get; init; } = new();

	// todo: OPTIMIZE!!!!
	public IEnumerable<GameMember> Actors => Game.Current.Members.Where( v => ActorUids.Contains( v.Uid ) );

	/// <summary>
	/// Whether or not the activity instance should be deleted after switching to a different activity
	/// </summary>
	public virtual bool KeepAlive => false;

	protected BaseActivity()
	{
		Uid = Guid.NewGuid();
	}

	protected BaseActivity( List<GameMember> actors )
	{
		foreach ( var actor in actors )
		{
			MemberUids.Add( actor.Uid );
			ActorUids.Add( actor.Uid );
		}

		Uid = Guid.NewGuid();
	}

	protected BaseActivity( List<GameMember> actors, List<GameMember> spectators )
	{
		foreach ( var actor in actors )
		{
			MemberUids.Add( actor.Uid );
			ActorUids.Add( actor.Uid );
		}

		foreach ( var spectator in spectators )
		{
			MemberUids.Add( spectator.Uid );
		}

		Uid = Guid.NewGuid();
	}

	protected BaseActivity( Guid uid, IEnumerable<Guid> actorUids, IEnumerable<Guid> memberUids )
	{
		foreach ( var actor in actorUids )
		{
			ActorUids.Add( actor );
		}

		foreach ( var member in memberUids )
		{
			MemberUids.Add( member );
		}

		Uid = uid;
	}

	public ActivityDescription CreateDescription() =>
		new()
		{
			ActorUids = ActorUids, MemberUids = MemberUids, Name = GetType().Name, Uid = Uid,
		};

	public virtual void ActivityStart( ActivityResult result ) { }
	public virtual void ActivityEnd() { }

	public virtual void MemberConnect( Client cl ) { }
	public virtual void MemberDisconnect( Client cl, NetworkDisconnectionReason reason ) { }
	public virtual void Simulate( Client cl ) { }
	public virtual void FrameSimulate( Client cl ) { }
}

public class Activity : BaseActivity
{
	protected Activity( List<GameMember> actors ) : base( actors )
	{
	}

	protected Activity( List<GameMember> actors, List<GameMember> spectators ) : base( actors, spectators )
	{
	}

	protected Activity( ActivityDescription description ) : base( description.Uid, description.ActorUids,
		description.MemberUids )
	{
	}

	protected Activity()
	{
	}
}

[AttributeUsage( AttributeTargets.Class )]
public class RegisteredActivityAttribute : Attribute
{
}
