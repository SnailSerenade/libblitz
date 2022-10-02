/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using System;
using System.Text.Json.Serialization;
using Sandbox;

namespace libblitz;

public partial class Player
{
	[JsonIgnore]
	[Net]
	private Client InternalClient { get; set; }

	[JsonIgnore]
	public new Client Client
	{
		get => InternalClient;
		set
		{
			if ( Host.IsClient )
				return;
			if ( !value.IsBot && value != null && !HasClient( value ) )
				throw new ArgumentException( "Tried to set player client to unassociated client" );
			InternalClient = value;
			HandleNewClient( InternalClient );
		}
	}

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
	public void AddClient( Client client ) { PlayedBy.Add( client.PlayerId ); }

	/// <summary>
	/// Allow a client to play using this player and start using the client
	/// </summary>
	/// <param name="client">Client</param>
	public void SetClient( Client client )
	{
		AddClient( client );
		Client = client;
	}

	private void HandleNewClient( Client client )
	{
		if ( Game.Current.Activity != null && !Game.Current.Activity.PreparedForInitialize )
			SetPawnByType( Game.Current.Activity.PawnType );
	}
}