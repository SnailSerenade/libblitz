/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;
using System;
using System.Collections.Generic;
using Sandbox;

public interface IGameData
{
	/// <summary>
	/// Unique ID for library game info
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
	public Game()
	{
		Uid = Guid.NewGuid();
		if ( Host.IsServer )
			FileSystem.OrganizationData.CreateDirectory( StorageLocation );
	}

	public static new Game Current => Sandbox.Game.Current as Game;

	[Net] public Guid Uid { get; set; }

	[Net] public string DisplayName { get; set; } = "Unknown";

	[Net] public IList<Player> Players { get; private set; } = new List<Player>();
}
