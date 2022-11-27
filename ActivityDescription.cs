using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace libblitz;

public partial class ActivityDescription : Entity
{
	public ActivityDescription() => Transmit = TransmitType.Always;

	public class SaveData : ISaveData
	{
		public string Name { get; set; }
		public Guid Uid { get; set; }
		public IList<Guid> MemberUids { get; set; }
		public IList<Guid> ActorUids { get; set; }

		public string Serialize()
		{
			throw new NotImplementedException();
		}
	}

	public ActivityDescription( SaveData saveData )
	{
		Transmit = TransmitType.Always;
		
		ISaveData.CopyToOutput( saveData, this );
	}

	public SaveData ToSaveData()
	{
		var instance = new SaveData();
		ISaveData.CopyFromOutput( instance, this );
		return instance;
	}

	/// <summary>
	/// Activity type name
	/// </summary>
	[Net]
	public new string Name { get; set; }

	/// <summary>
	/// Activity instance UID
	/// </summary>
	[Net]
	public Guid Uid { get; set; } = Guid.NewGuid();

	/// <summary>
	/// Activity members / players
	/// Includes spectators.
	/// </summary>
	[Net]
	public IList<Guid> MemberUids { get; set; }

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
	public IList<Guid> ActorUids { get; set; }

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
	public new ActivityDescription Transform( string name )
	{
		Log.Warning( "Depreciated Transform used" );
		return new ActivityDescription
		{
			Name = name, Uid = Guid.NewGuid(), MemberUids = MemberUids.ToList(), ActorUids = ActorUids.ToList(),
		};
	}

	public new ActivityDescription Transform<T>() => new()
	{
		Name = typeof(T).FullName, Uid = Guid.NewGuid(), MemberUids = MemberUids.ToList(), ActorUids = ActorUids.ToList(),
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

	public static ActivityDescription For<T>()
	{
		var description = new ActivityDescription { Name = typeof(T).FullName };
		return description;
	}
}
