/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using System;
using System.Collections.Generic;
using Sandbox;

namespace libblitz;

public abstract partial class Game : Sandbox.Game, IGameData
{
	public const string StorageLocation = "libblitz/games_v000";

	private static void InitializeStorage()
	{
		FileSystem.OrganizationData.CreateDirectory( StorageLocation );
	}

	/// <summary>
	/// Load game state from file on disk
	/// </summary>
	/// <param name="filename">File name (relative to fileSystem)</param>
	/// <param name="customFileSystem">File subsystem to use</param>
	public void LoadFrom( string filename, BaseFileSystem customFileSystem = null )
	{
		if ( Host.IsClient )
			return;

		BaseFileSystem fileSystem = customFileSystem ?? FileSystem.OrganizationData;

		string normalizedFileName = FileSystem.NormalizeFilename( filename );

		// Make sure game data exists
		if ( !fileSystem.FileExists( normalizedFileName ) )
			throw new Exception( "Game data not found" );

		// Attempt to read game data
		IGameData gameData;
		try
		{
			gameData = fileSystem.ReadJson<IGameData>(
				FileSystem.NormalizeFilename( normalizedFileName )
			);
		}
		catch ( Exception e )
		{
			// todo: find right exception & throw custom exception
			Log.Error( e );
			throw new Exception( "Failed to read game data" );
		}

		// Set current game data
		Uid = gameData.Uid;
		DisplayName = gameData.DisplayName;
		Players = gameData.Players;

		// Debug log
		Log.Info( $"Loaded game data from {filename}" );
	}

	/// <summary>
	/// Save game state to file on disk
	/// </summary>
	/// <param name="filename">File name (relative to fileSystem)</param>
	/// <param name="customFileSystem">File subsystem to use</param>
	public void SaveAs( string filename, BaseFileSystem customFileSystem = null )
	{
		if ( Host.IsClient )
			return;

		BaseFileSystem fileSystem = customFileSystem ?? FileSystem.OrganizationData;

		string normalizedFileName = FileSystem.NormalizeFilename( filename );

		// Attempt to write game data
		try
		{
			fileSystem.WriteJson<IGameData>(
				FileSystem.NormalizeFilename( normalizedFileName ),
				this
			);
		}
		catch ( Exception e )
		{
			// todo: find right exception & throw custom exception
			Log.Error( e );
			throw new Exception( "Failed to write game data" );
		}
	}

	/// <summary>
	/// Load game state from game UID
	/// </summary>
	/// <param name="uid">Game UID</param>
	public void Load( Guid uid ) => LoadFrom( $"{StorageLocation}/{uid}" );

	/// <summary>
	/// Save game state with game UID
	/// </summary>
	public void Save() => SaveAs( $"{StorageLocation}/{Uid}" );
}