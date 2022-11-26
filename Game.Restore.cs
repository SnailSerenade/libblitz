using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace libblitz;

public partial class Game
{
	public class SaveData : ISaveData
	{
		public Guid Uid;
		public string DisplayName;

		public IList<ActivityDescription> ActivityStack { get; set; }
		public ActivityResult Result;

		public string Serialize() => JsonSerializer.Serialize( this );
		public static SaveData From( string data ) => JsonSerializer.Deserialize<SaveData>( data );
	}

	public Game( SaveData saveData )
	{
	}

	private void LoadSaveData( SaveData saveData )
	{
		Uid = saveData.Uid;
		DisplayName = saveData.DisplayName;

		ActivityStack = ActivityStack.ToList();
	}
}
