/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;

using System;
using Sandbox;
using System.Collections.Generic;

public interface IPlayerData
{
	/// <summary>
	/// Unique ID for player
	/// </summary>
	public Guid Uid { get; }

	/// <summary>
	/// Player nickname / display name
	/// </summary>
	public string DisplayName { get; set; }

	/// <summary>
	/// Steam IDs (as of now) of clients that can take over this Player
	/// </summary>
	public IList<long> PlayedBy { get; }

	/// <summary>
	/// Whether or not this player is allowed to be a bot
	/// </summary>
	public bool CanBeBot { get; set; }
}

public interface IPlayerGameData
{
	public int Coins { get; set; }
	public int SpecialCoins { get; set; }
	public string SavedTileName { get; set; }
}

public partial class Player : Entity, IPlayerData, IPlayerGameData
{
	public Player()
	{
		Transmit = TransmitType.Always;
		Uid = Guid.NewGuid();
	}

	[Net]
	public Guid Uid { get; }
	[Net]
	public string DisplayName { get; set; } = "Unknown";
	[Net]
	public bool CanBeBot { get; set; } = false;
	[Net]
	public IList<long> PlayedBy { get; } = new List<long>();

	[Net]
	public int Coins { get; set; }
	[Net]
	public int SpecialCoins { get; set; }
	[Net]
	public string SavedTileName { get; set; }
}
