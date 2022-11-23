using System.Collections.Generic;
using Sandbox;

namespace libblitz;

public partial class GameMember
{
	[Net] public IList<Entity> Pawns { get; private set; } = new List<Entity>();

	public Entity Pawn => Client.Pawn;

	/// <summary>
	/// Get or create new instance of provided type pawn 
	/// </summary>
	/// <typeparam name="T">Pawn type</typeparam>
	/// <returns>Existing or new pawn on server, Existing pawn or null on client</returns>
	public T GetOrCreatePawn<T>( object[] args = null ) where T : Entity, new()
	{
		foreach ( var entity in Pawns )
		{
			if ( entity is T pawn )
			{
				return pawn;
			}
		}

		if ( !Host.IsServer )
		{
			return null;
		}

		{
			var pawn = TypeLibrary.GetDescription<T>().Create<T>( args );
			Pawns.Add( pawn );
			return pawn;
		}
	}

	public void UsePawn( Entity entity )
	{
		if ( CurrentClient == null )
		{
			return;
		}

		Log.Info( "SETTING. YAHOO!" );
		CurrentClient.Pawn = entity;
	}
}
