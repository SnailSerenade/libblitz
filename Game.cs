using System;
using System.Collections.Generic;
using System.Text.Json;
using Sandbox;
using Sandbox.UI;

namespace libblitz;

public abstract partial class Game : Sandbox.Game
{
	public new static Game Current => Sandbox.Game.Current as Game;

	[Net] public Guid Uid { get; set; }
	[Net] public string DisplayName { get; set; }
	[Net] public IList<GameMember> Members { get; set; } = new List<GameMember>();
	[Net] public int Turns { get; set; }
	private Guid LastTurnPlayer { get; set; }

	public class SaveData : ISaveData
	{
		public Guid Uid { get; set; }
		public string DisplayName { get; set; }
		public int Turns { get; set; }
		public Guid LastTurnPlayer { get; set; }
		public IList<GameMember.SaveData> Members { get; set; }
		public IList<ActivityDescription.SaveData> ActivityStack { get; set; }
		public string ActivityResultTypeName { get; set; }
		public string ActivityResultSerialized { get; set; }
	}

	protected Game()
	{
		if ( Host.IsClient )
		{
			Local.Hud = new RootPanel();
		}

		Transmit = TransmitType.Always;
		Uid = Guid.NewGuid();
		Turns = 15; // placeholder
	}
}
