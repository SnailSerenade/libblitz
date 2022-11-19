using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sandbox;

namespace libblitz;

public partial class GameMember
{
	[Net] private List<long> ClientIds { get; set; }
	[Net] private Client CurrentClient { get; set; }

	/// <summary>
	/// Allow a client to play as this GameMember
	/// </summary>
	/// <param name="client">Client</param>
	public void AddClient( Client client )
	{
		Host.AssertServer();

		if ( ClientIds.Contains( client.GetSteamId() ) )
		{
			return;
		}

		ClientIds.Add( client.GetSteamId() );

		UpdateCurrentClient( false );
	}

	/// <summary>
	/// Set CurrentClient based on known clients
	/// </summary>
	/// <param name="allowBot">Can UpdateCurrentClient create a bot if required?</param>
	private void UpdateCurrentClient( bool allowBot = true )
	{
		Host.AssertServer();

		if ( CurrentClient == null || CurrentClient.IsBot )
		{
			foreach ( var client in Client.All )
			{
				if ( client.Pawn != null )
				{
					continue;
				}

				if ( ClientIds.Contains( client.GetSteamId() ) )
				{
					CurrentClient = client;
				}
			}
		}

		if ( CurrentClient != null || !allowBot )
		{
			Log.Info( $"No suitable client found for {this}." );
			return;
		}

		Log.Info( $"No real client found for {this}. Creating bot..." );
		CurrentClient = new Bot().Client;
	}
}
