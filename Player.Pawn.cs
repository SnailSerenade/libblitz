/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;
using System;
using System.Collections.Generic;
using Sandbox;

public partial class Player : BaseNetworkable, IPlayerData, IPlayerStatus
{
	/// <summary>
	/// Pawns associated with this player
	/// </summary>
	[Net] public IList<Entity> Pawns { get; private set; } = new List<Entity>();

	/// <summary>
	/// Pawn being used by player at the moment
	/// </summary>
	[Net] private Entity CurrentPawn { get; set; }

	/// <summary>
	/// Type of pawn currently saved
	/// </summary> 
	private Type CurrentPawnType;

	/// <summary>
	/// Pawn being used by current minigame
	/// </summary>
	public Entity Pawn
	{
		get
		{
			var minigamePawnType = Minigame.Current.PawnType;
			if ( Host.IsServer && (CurrentPawn == null || minigamePawnType != CurrentPawnType) )
			{
				CurrentPawn = GetPawn( minigamePawnType );
				CurrentPawnType = minigamePawnType;
				return CurrentPawn;
			}

			if ( CurrentPawn != null )
				return CurrentPawn;

			return null;
		}
	}

	/// <summary>
	/// Find associated pawn of provided type
	/// </summary>
	/// <param name="pawnType">Type of pawn to find</param>
	/// <returns>Entity</returns>
	/// <exception cref="Exception">Thrown when no pawn found</exception>
	public Entity GetPawn( Type pawnType )
	{
		foreach ( var i in Pawns )
			if ( i.GetType() == pawnType )
				return i;
		// todo: throw custom exception
		throw new Exception( $"Player has no {pawnType.FullName} type pawn" );
	}
}