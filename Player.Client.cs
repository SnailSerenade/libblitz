/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;
using System.Collections.Generic;
using Sandbox;

public partial class Player : BaseNetworkable, IPlayerData, IPlayerStatus
{
	/// <summary>
	/// Steam IDs (as of now) of clients that can take over this LibPlayer
	/// Uses Sandbox.Client.PlayerId
	/// note: This doesn't really need to be networked at the moment
	/// </summary>
	[Net] public List<long> PlayedBy { get; private set; }

	/// <summary>
	/// Check if provided client is allowed to play using this player
	/// </summary>
	/// <param name="client">Client</param>
	/// <returns>True if provided client can play this player</returns>
	public bool HasClient( Client client ) => PlayedBy.Contains( client.PlayerId );

	/// <summary>
	/// Allow a client to play using this player
	/// </summary>
	/// <param name="client">Client</param>
	public void AddClient( Client client ) => PlayedBy.Add( client.PlayerId );
}