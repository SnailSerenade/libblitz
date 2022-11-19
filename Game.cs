using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace libblitz;

public partial class Game : Sandbox.Game
{
	public new static Sandbox.Game Current => Sandbox.Game.Current as Sandbox.Game;

	[Net] public Guid Uid { get; private set; }
	[Net] public List<GameMember> Members { get; private set; }
	[Net] public List<BaseActivity> Activities { get; init; }

	public Game()
	{
		Activities = new List<BaseActivity>();

		if ( Host.IsClient )
		{
			Local.Hud = new RootPanel();
		}
	}
}
