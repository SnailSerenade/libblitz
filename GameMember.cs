using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using BonitoBlitz.Entities.CoreBoard;
using Sandbox;

namespace libblitz;

public partial class GameMember : Entity
{
	[Net] public Guid Uid { get; private set; }
	[Net] public string DisplayName { get; set; }
	[Net] public int Coins { get; set; }
	[Net] public string CurrentTileName { get; set; }

	public BaseTile CurrentTile
	{
		get => BaseTile.FromName( CurrentTileName );
		set => CurrentTileName = value?.Name;
	}

	public GameMember()
	{
		Transmit = TransmitType.Always;

		Uid = Guid.NewGuid();
	}

	public class SaveData : ISaveData
	{
		public Guid Uid { get; set; }
		public string DisplayName { get; set; }
		public int Coins { get; set; }
		public string CurrentTileName { get; set; }

		public IList<long> ClientIds { get; set; }

		public string Serialize() => JsonSerializer.Serialize( this );
		public static SaveData From( string data ) => JsonSerializer.Deserialize<SaveData>( data );
	}

	public GameMember( SaveData saveData )
	{
		Transmit = TransmitType.Always;

		ISaveData.CopyToOutput( saveData, this );

		UpdateCurrentClient( false );
	}

	public SaveData ToSaveData()
	{
		var instance = new SaveData();
		ISaveData.CopyFromOutput( instance, this );
		return instance;
	}
}
