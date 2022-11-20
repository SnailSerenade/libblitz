using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace libblitz;

public abstract partial class Game : Sandbox.Game
{
	public new static Game Current => Sandbox.Game.Current as Game;

	[Net] public Guid Uid { get; }
	[Net] public List<GameMember> Members { get; } = new();

	protected Game()
	{
		if ( Host.IsClient )
		{
			Local.Hud = new RootPanel();
		}

		Uid = Guid.NewGuid();
	}
}
