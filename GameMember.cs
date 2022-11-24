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

	public GameMember( SaveData saveData )
	{
		Transmit = TransmitType.Always;

		LoadSaveData( saveData );

		UpdateCurrentClient( false );
	}

	public GameMember()
	{
		Transmit = TransmitType.Always;

		Uid = Guid.NewGuid();
	}

	public class SaveData : ISaveData
	{
		public Guid Uid;
		public string DisplayName;
		public int Coins;
		public string CurrentTileName;

		public IList<long> ClientIds;

		public string Serialize() => JsonSerializer.Serialize( this );
		public static SaveData From( string data ) => JsonSerializer.Deserialize<SaveData>( data );
	}

	private void LoadSaveData( SaveData saveData )
	{
		Uid = saveData.Uid;
		DisplayName = saveData.DisplayName;
		Coins = saveData.Coins;
		CurrentTileName = saveData.CurrentTileName;
		ClientIds = saveData.ClientIds.ToList();
	}

	public SaveData ToSaveData() =>
		new SaveData
		{
			Uid = Uid,
			DisplayName = DisplayName,
			Coins = Coins,
			ClientIds = ClientIds,
		};
}
