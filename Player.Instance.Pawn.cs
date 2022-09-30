/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sandbox;

namespace libblitz;

public partial class Player
{
	[JsonIgnore]
	[Net]
	public IList<Entity> Pawns { get; } = new List<Entity>();

	[JsonIgnore]
	[Net]
	private Entity InternalPawn { get; set; }
	public Entity Pawn
	{
		get => InternalPawn;
		set => SetPawn( value );
	}

	public void SetPawnByType( Type type )
	{
		if ( type == null )
			throw new ArgumentNullException( nameof( type ), "Pawn type was null" );
		foreach ( var pawn in Pawns )
		{
			Log.Info( $"{pawn.GetType()} == {type}, {pawn.GetType() == type}" );
			if ( pawn.GetType() == type )
			{
				Pawn = pawn;

				if ( Client != null )
					Client.Pawn = Pawn;

				return;
			}
		}
		throw new KeyNotFoundException( $"Player has no pawn of type {type.Name}" );
	}

	public void AddPawn( Entity entity )
	{
		// Compare: does the Pawns list already contain this entity?
		if ( Pawns.Contains( entity ) )
		{
			Log.Error( $"Tried to add existing pawn! {entity.GetType().Name}" );
			return;
		}

		// Compare: does the Pawns list already have this type of entity?
		for ( int i = 0; i < Pawns.Count; i++ )
		{
			Entity storedPawn = Pawns[i];
			if ( storedPawn.GetType() == entity.GetType() )
			{
				// This is the same type of entity, so clean up old one and replace it
				Log.Info( $"Replacing existing pawn of type {entity.GetType().Name}" );

				// Clean up
				Pawns[i].Delete();

				// Replace
				Pawns[i] = entity;
				return;
			}
		}

		// Add entity to list if comparisons fail
		Pawns.Add( entity );
	}

	public void SetPawn( Entity entity )
	{
		if ( Pawns.Contains( entity ) )
		{
			InternalPawn = entity;
			return;
		}

		AddPawn( entity );
		SetPawnByType( entity.GetType() );
	}
}