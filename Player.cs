/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;
using System;
using System.Collections.Generic;
using Sandbox;

public interface IPlayerData
{
	public Guid Uid { get; }
	public string DisplayName { get; set; }
	public List<long> PlayedBy { get; }
	public bool HasClient( Client client );
	public void AddClient( Client client );
}

public interface IPlayerStatus
{
	public Entity Pawn { get; }
}

public partial class Player : BaseNetworkable, IPlayerData, IPlayerStatus
{
	public Player() { }

	/// <summary>
	/// Unique ID for player
	/// </summary>
	[Net] public Guid Uid { get; set; }

	/// <summary>
	/// Player nickname / display name
	/// </summary>
	[Net] public string DisplayName { get; set; } = "Unknown";
}