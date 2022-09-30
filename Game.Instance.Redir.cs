/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using Sandbox;

namespace libblitz;

public abstract partial class Game
{
	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		foreach ( var player in Players )
		{
			if ( cl.IsBot && player.CanBeBot && player.Client == null )
			{
				player.Client = cl;
				break;
			}

			if ( player.HasClient( cl ) ) // this if isn't required but it gives information
				player.Client = cl;
		}

		Activity?.PlayerChange();
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		foreach ( var player in Players )
		{
			if ( player.HasClient( cl ) )
				player.Client = null;
		}

		Activity?.PlayerChange();
	}
}