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

	/// <summary>
	/// Higher numbers == this player's turn is further back in the turn order.
	/// (0 == first to play, int64_max == last to play)
	/// </summary>
	public long TurnOrderIndex { get; }
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
		TurnOrderIndex = Random.Shared.NextInt64();
	}

	public Player( Guid uid )
	{
		Transmit = TransmitType.Always;
		Uid = uid;
	}

	[SelectCopyIncluded]
	[Net]
	public Guid Uid { get; private set; }

	[SelectCopyIncluded]
	[Net]
	public string DisplayName { get; set; } = "Unknown";

	[SelectCopyIncluded]
	[Net]
	public bool CanBeBot { get; set; } = false;

	[SelectCopyIncluded]
	[Net]
	public IList<long> PlayedBy { get; private set; } = new List<long>();

	[SelectCopyIncluded]
	[Net]
	public int Coins { get; set; }

	[SelectCopyIncluded]
	[Net]
	public int SpecialCoins { get; set; }

	[SelectCopyIncluded]
	[Net]
	public string SavedTileName { get; set; }

	[SelectCopyIncluded]
	[Net]
	public long TurnOrderIndex { get; private set; }
}
