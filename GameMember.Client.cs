using System.Collections.Generic;
using Sandbox;

namespace libblitz;

public partial class GameMember
{
	[Net] public List<long> ClientIds { get; set; }
	[Net] private Client CurrentClient { get; set; }
	public new Client Client => CurrentClient;

	/// <summary>
	/// Allow a client to play as this GameMember and set if possible
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
	/// Allow a client to play as this GameMember
	/// </summary>
	/// <param name="clientId">Client ID (Steam ID)</param>
	public void AddClient( long clientId )
	{
		Host.AssertServer();

		if ( ClientIds.Contains( clientId ) )
		{
			return;
		}

		ClientIds.Add( clientId );

		UpdateCurrentClient( false );
	}

	public void HandleClientDisconnect( Client cl )
	{
		if ( cl == CurrentClient )
			CurrentClient = null;
	}

	/// <summary>
	/// Set CurrentClient based on known clients
	/// </summary>
	/// <param name="allowBot">Can UpdateCurrentClient create a bot if required?</param>
	public void UpdateCurrentClient( bool allowBot = true )
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

				Log.Info( client );

				if ( !ClientIds.Contains( client.GetSteamId() ) )
				{
					continue;
				}

				Log.Info( "setting client" );
				CurrentClient = client;
				return;
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
