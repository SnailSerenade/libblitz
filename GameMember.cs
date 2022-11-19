using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Sandbox;

namespace libblitz;

public partial class GameMember : Entity
{
	[Net] private Guid Uid { get; set; }
	[Net] private string DisplayName { get; set; }
	[Net] private int Coins { get; set; }

	public GameMember( SaveData saveData )
	{
		LoadSaveData( saveData );

		UpdateCurrentClient( true );
	}

	public class SaveData : ISaveData
	{
		public Guid Uid;
		public string DisplayName;
		public int Coins;

		public IList<long> ClientIds;
		public IList<Guid> ActivityStack;

		public string Serialize() => JsonSerializer.Serialize( this );
		public static SaveData From( string data ) => JsonSerializer.Deserialize<SaveData>( data );
	}

	private void LoadSaveData( SaveData saveData )
	{
		Uid = saveData.Uid;
		DisplayName = saveData.DisplayName;
		Coins = saveData.Coins;
		ClientIds = saveData.ClientIds.ToList();
		ActivityStack = saveData.ActivityStack.ToList();
	}

	public SaveData ToSaveData() =>
		new SaveData
		{
			Uid = Uid,
			DisplayName = DisplayName,
			Coins = Coins,
			ClientIds = ClientIds,
			ActivityStack = ActivityStack
		};
}
