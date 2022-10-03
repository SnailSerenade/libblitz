/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using System;
using System.Collections.Generic;
using Sandbox;

namespace libblitz;

[SelectCopyIncludeAll]
public class GameData : IGameData
{
	public Guid Uid { get; set; }
	public string DisplayName { get; set; }
	public ActivityResult PreviousActivityResult { get; set; }
}

public abstract partial class Game : Sandbox.Game, IGameData
{
	public const string StorageLocation = "libblitz/games_v001";
	public const string GameDataName = "game.lblitz";
	public BaseFileSystem GameStorage { get; private set; }
	public BaseFileSystem PlayerStorage { get; private set; }

	private void InitializeStorage( Guid uid )
	{
		// Create directory tree
		FileSystem.OrganizationData.CreateDirectory( $"{StorageLocation}/{uid}/players" );

		// Set file systems
		GameStorage = FileSystem.OrganizationData.CreateSubSystem( $"{StorageLocation}/{uid}" );
		PlayerStorage = FileSystem.OrganizationData.CreateSubSystem( $"{StorageLocation}/{uid}/players" );
	}

	/// <summary>
	/// Load game state from storage
	/// </summary>
	/// <param name="uid"></param>
	/// <exception cref="Exception"></exception>
	public void Load( Guid uid )
	{
		if ( Host.IsClient )
			return;

		// Make sure game data exists
		if ( !FileSystem.OrganizationData.DirectoryExists( $"{StorageLocation}/{uid}" ) )
			throw new Exception( $"Game {uid} not found" );

		// Use this game data as storage
		InitializeStorage( uid );

		// Attempt to read game data
		GameData gameData;
		try
		{
			gameData = GameStorage.ReadJson<GameData>(
				GameDataName
			);
		}
		catch ( Exception e )
		{
			Log.Error( e );
			throw new Exception( "Failed to read game data" );
		}

		if ( Players == null )
			Players = new List<Player>();

		// Load player data
		// For each player UID we can load...
		foreach ( var playerFileName in PlayerStorage.FindFile( "" ) )
		{
			Guid playerUid;

			// Get player UID as Guid
			try
			{
				playerUid = Guid.Parse( playerFileName );
			}
			catch ( Exception )
			{
				Log.Error( $"Unknown file in player storage {playerFileName}" );
				continue;
			}

			bool playerAlreadyExists = false;
			foreach ( Player player in Players )
			{
				if ( player.Uid == playerUid )
				{
					playerAlreadyExists = true;
					Log.Info( $"loading existing {player.Uid}" );
					player.Load();
				}
			}

			// Player doesn't exist in this game already. Load and add them
			if ( !playerAlreadyExists )
				Players.Add( Player.FromGameStorage( playerUid ) );
		}

		// Set current game data
		Log.Info( gameData.Uid );
		Log.Info( gameData.DisplayName );
		StorageUtil.SelectCopyTo( gameData, this );

		// Debug log
		Log.Info( $"Loaded game data from {uid}" );
		DebugOverlay.ScreenText( "Loaded!", Vector2.One * 20, 0, Color.Black, 3.0f );
	}

	/// <summary>
	/// Save game state to file on disk
	/// </summary>
	public void Save()
	{
		if ( Host.IsClient )
			return;

		InitializeStorage( Uid );

		GameData gameData = new();
		StorageUtil.SelectCopyTo( this, gameData );

		// Attempt to write game data
		try
		{
			GameStorage.WriteJson<IGameData>(
				GameDataName,
				gameData
			);
		}
		catch ( Exception e )
		{
			// todo: find right exception & throw custom exception
			Log.Error( e );
			throw new Exception( "Failed to write game data" );
		}

		// Write player data
		foreach ( var player in Players )
		{
			player.Save();
		}

		DebugOverlay.ScreenText( "Saved!", Vector2.One * 20, 0, Color.Black, 3.0f );
	}
}