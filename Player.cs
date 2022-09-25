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
	public IList<long> PlayedBy { get; }
	public bool CanBeBot { get; set; }
}

public interface IPlayerStatus
{
	public Entity Pawn { get; }
	public bool Ready { get; }
	public Client Client { get; }
	public bool HasClient( Client client );
	public void AddClient( Client client );
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

	/// <summary>
	/// Whether or not this player is allowed to be a bot
	/// </summary>
	[Net] public bool CanBeBot { get; set; } = false;

	public bool Ready => Client != null || CanBeBot;
}