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

public interface IGameData
{
	/// <summary>
	/// Unique ID for game info
	/// </summary>
	public Guid Uid { get; }

	/// <summary>
	/// Game nickname / display name
	/// </summary>
	public string DisplayName { get; set; }

	/// <summary>
	/// Result of the previous activity to run
	/// </summary>
	public ActivityResult PreviousActivityResult { get; }

	/// <summary>
	/// Amount of turns left for this game
	/// </summary>
	public int TurnsLeft { get; }

	/// <summary>
	/// Current status of the game (in progress, complete, etc.)
	/// </summary>
	public GameStatus Status { get; }

	/// <summary>
	/// Unique ID of player that is having their turn
	/// </summary>
	public Guid CurrentTurnPlayer { get; }

	/// <summary>
	/// Activity to (maybe) use when restarting the gamemode
	/// </summary>
	public string RequestedActivity { get; }
}

public abstract partial class Game : Sandbox.Game, IGameData
{
	[JsonIgnore]
	public static new Game Current => Sandbox.Game.Current as Game;

	public Game()
	{
		Uid = Guid.NewGuid();

		if ( Host.IsClient )
			Hud = new();
	}

	[SelectCopyIncluded]
	[Net]
	public Guid Uid { get; private set; }

	[SelectCopyIncluded]
	[Net]
	public string DisplayName { get; set; }

	[SelectCopyIncluded]
	[Net]
	public IList<Player> Players { get; private set; }

	[SelectCopyIncluded]
	[Net]
	public int TurnsLeft { get; set; }

	[SelectCopyIncluded]
	[Net]
	public Guid CurrentTurnPlayer { get; set; }

	[SelectCopyIncluded]
	// [Net] // todo: does this have to be networked?
	public string RequestedActivity { get; set; }
}