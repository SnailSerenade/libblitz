/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using System;
using System.Collections.Generic;
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
	/// List of Players for this game
	/// </summary>
	public IList<Player> Players { get; }
}

public abstract partial class Game : Sandbox.Game, IGameData
{
	public static new Game Current => Sandbox.Game.Current as Game;

	public Game()
	{
		Uid = Guid.NewGuid();

		InitializeStorage();

		if ( Host.IsClient )
			Hud = new();
	}

	[Net]
	public Guid Uid { get; private set; }

	[Net]
	public string DisplayName { get; set; }

	[Net]
	public IList<Player> Players { get; private set; }
}