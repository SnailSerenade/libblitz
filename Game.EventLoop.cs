﻿using System.Linq;
using Sandbox;

namespace libblitz;

public partial class Game
{
	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		foreach ( var member in Members )
		{
			member.UpdateCurrentClient( false );
		}

		foreach ( var activity in Activities )
		{
			activity.MemberConnect( cl );
		}

		AttemptActivityInit();
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		foreach ( var member in Members.Where( member => member.Client == cl ) )
		{
			member.HandleClientDisconnect( cl );
		}

		foreach ( var activity in Activities )
		{
			activity.MemberDisconnect( cl, reason );
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		for ( var i = Activities.Count - 1; i >= 0; i-- )
		{
			var activity = Activities[i];
			activity?.Simulate( cl );
		}
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		for ( var i = Activities.Count - 1; i >= 0; i-- )
		{
			var activity = Activities[i];
			activity?.FrameSimulate( cl );
		}
	}
}
