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
	[SelectCopyIncluded]
	public Guid Uid { get; }

	/// <summary>
	/// Game nickname / display name
	/// </summary>
	[SelectCopyIncluded]
	public string DisplayName { get; set; }
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
}