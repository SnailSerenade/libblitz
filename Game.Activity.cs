/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;
using System;
using Sandbox;

public abstract partial class Game : Sandbox.Game, IGameData
{
	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		foreach ( var player in Players )
		{
			if ( player.HasClient( cl ) ) // this if isn't required but it gives information
				player.Client = cl;
		}

		Activity.Current?.PlayerListUpdate();
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		foreach ( var player in Players )
		{
			if ( player.HasClient( cl ) )
				player.Client = null;
		}

		Activity.Current?.PlayerListUpdate();
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Activity.Current?.Simulate( cl );
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		Activity.Current?.FrameSimulate( cl );
	}
}
