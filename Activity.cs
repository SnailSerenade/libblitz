using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Sandbox;

namespace libblitz;

public abstract class ActivityResult : BaseNetworkable
{
}

public abstract partial class BaseActivity : Entity
{
	[Net] public Guid Uid { get; private set; }

	[Net] private IList<Guid> MemberUids { get; set; } = new List<Guid>();

	// todo: OPTIMIZE!!!!
	public IEnumerable<GameMember> Members => Game.Current.Members.Where( v => MemberUids.Contains( v.Uid ) );

	[Net] private IList<Guid> ActorUids { get; set; } = new List<Guid>();

	// todo: OPTIMIZE!!!!
	public IEnumerable<GameMember> Actors => Game.Current.Members.Where( v => ActorUids.Contains( v.Uid ) );

	/// <summary>
	/// Whether or not the activity instance should be deleted after switching to a different activity
	/// </summary>
	public virtual bool KeepAlive => false;

	protected BaseActivity()
	{
		Uid = Guid.NewGuid();

		Transmit = TransmitType.Always;
	}

	protected BaseActivity( List<GameMember> actors )
	{
		foreach ( var actor in actors )
		{
			MemberUids.Add( actor.Uid );
			ActorUids.Add( actor.Uid );
		}

		Uid = Guid.NewGuid();

		Transmit = TransmitType.Always;
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

		Transmit = TransmitType.Always;
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

		Transmit = TransmitType.Always;
	}


	public ActivityDescription CreateDescription() =>
		new()
		{
			ActorUids = ActorUids, MemberUids = MemberUids, Name = GetType().Name, Uid = Uid,
		};

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( Host.IsClient )
		{
			// yep this is a bad fix but it works
			ActivityClientEnd();
		}
	}

	public virtual void ActivityStart( ActivityResult result ) { }
	public virtual void ActivityEnd() { }

	public virtual void ActivityClientStart() { }
	public virtual void ActivityClientEnd() { }

	public virtual void MemberConnect( Client cl ) { }
	public virtual void MemberDisconnect( Client cl, NetworkDisconnectionReason reason ) { }
	public new virtual void Simulate( Client cl ) { }
	public new virtual void FrameSimulate( Client cl ) { }
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

	protected Activity() => Host.AssertClient();
}

[AttributeUsage( AttributeTargets.Class )]
public class RegisteredActivityAttribute : Attribute
{
}
