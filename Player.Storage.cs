/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;

using System;
using Sandbox;
using System.Collections.Generic;

[SelectCopyIncludeAll]
public class PlayerData : IPlayerData
{
	public Guid Uid { get; set; }
	public string DisplayName { get; set; }
	public bool CanBeBot { get; set; }
	public IList<long> PlayedBy { get; set; }
	public int Coins { get; set; }
	public int SpecialCoins { get; set; }
	public string SavedTileName { get; set; }
}

public partial class Player : Entity, IPlayerData, IPlayerGameData
{
	public void Save()
	{
		PlayerData playerData = new();

		// Copy player data into playerData
		StorageUtil.SelectCopyTo( this, playerData );

		// Save to storage
		if ( Game.Current.PlayerStorage == null )
			throw new Exception( "Game storage not initialized yet." );

		try
		{
			Game.Current.PlayerStorage.WriteJson( $"{Uid}", playerData );
		}
		catch ( Exception e )
		{
			// todo: find right exception & throw custom exception
			Log.Error( e );
			throw new Exception( "Failed to write player data" );
		}
	}

	public void Load()
	{
		// Make sure game storage is initialized
		if ( Game.Current.PlayerStorage == null )
			throw new Exception( "Game storage not initialized yet." );

		// Attempt to read
		PlayerData data;
		try
		{
			data = Game.Current.PlayerStorage.ReadJson<PlayerData>( $"{Uid}" );
		}
		catch ( Exception e )
		{
			// todo: find right exception & throw custom exception
			Log.Error( e );
			throw new Exception( "Failed to read player data" );
		}

		// Copy player data into this player
		StorageUtil.SelectCopyTo( data, this );
	}

	public static Player FromGameStorage( Guid uid )
	{
		Player player = new( uid );
		player.Load();
		return player;
	}
}
