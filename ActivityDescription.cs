using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace libblitz;

public partial class ActivityDescription : BaseNetworkable
{
	/// <summary>
	/// Activity type name
	/// </summary>
	[Net]
	public string Name { get; set; }

	/// <summary>
	/// Activity instance UID
	/// </summary>
	[Net]
	public Guid Uid { get; set; }

	/// <summary>
	/// Activity members / players
	/// Includes spectators.
	/// </summary>
	[Net]
	public List<Guid> MemberUids { get; set; }

	public IEnumerable<GameMember> Members
	{
		get => Game.Current.Members.Where( v => MemberUids.Contains( v.Uid ) );
		set => MemberUids = value.Select( v => v.Uid ).ToList();
	}

	/// <summary>
	/// Activity acting members.
	/// Doesn't include spectators.
	/// </summary>
	[Net]
	public List<Guid> ActorUids { get; set; }

	public IEnumerable<GameMember> Actors
	{
		get => Game.Current.Members.Where( v => ActorUids.Contains( v.Uid ) );
		set => ActorUids = value.Select( v => v.Uid ).ToList();
	}


	/// <summary>
	/// Get <see cref="BaseActivity"/> from the Game activity list
	/// </summary>
	public BaseActivity Activity => Game.Current?.Activities?.SingleOrDefault( v => v.Uid == Uid );

	/// <summary>
	/// Create duplicate <see cref="ActivityDescription"/> instance with new activity type name and UID 
	/// </summary>
	/// <param name="name">New type name</param>
	/// <returns>New ActivityDescription</returns>
	public ActivityDescription Transform( string name ) => new()
	{
		Name = name, Uid = Guid.NewGuid(), MemberUids = MemberUids.ToList(), ActorUids = ActorUids.ToList(),
	};

	/// <summary>
	/// Whether or not all members are ready with a suitable client
	/// </summary>
	public bool AllMembersReady
	{
		get
		{
			var ready = true;
			foreach ( var member in from uid in MemberUids
			         where Game.Current?.Members != null
			         select Game.Current.Members.SingleOrDefault( v => v.Uid == uid )
			         into member
			         where member?.Client == null
			         select member )
			{
				ready = false;
			}

			return ready;
		}
	}

	/// <summary>
	/// Create Activity instance from this ActivityDescription
	/// </summary>
	/// <typeparam name="T">Activity type</typeparam>
	/// <returns>New activity instance</returns>
	/// <exception cref="TypeAccessException">If activity type unusable</exception>
	/// <exception cref="Exception">If activity couldn't be created</exception>
	public T CreateInstance<T>() where T : BaseActivity
	{
		var typeDescription = TypeLibrary.GetDescription( Name );
		if ( typeDescription == null )
		{
			throw new Exception( $"Activity type {Name} not found or inaccessible" );
		}

		var activity = typeDescription.Create<T>( new object[] { this } );

		if ( activity == null )
		{
			throw new Exception( $"Failed to create activity of type {Name}" );
		}

		return activity;
	}
}
