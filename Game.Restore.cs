using System;
using System.Collections.Generic;
using System.Text.Json;

namespace libblitz;

public partial class Game
{
	public class SaveData : ISaveData
	{
		public Guid Uid;
		public string DisplayName;

		public IList<GameMember.SaveData> Members { get; set; }
		public IList<ActivityDescription.SaveData> ActivityStack { get; set; }
		public string ActivityResultTypeName;
		public string ActivityResultSerialized;

		public string Serialize() => JsonSerializer.Serialize( this );
		public static SaveData From( string data ) => JsonSerializer.Deserialize<SaveData>( data );
	}

	public Game( SaveData saveData ) => LoadSaveData( saveData );

	private void LoadSaveData( SaveData saveData )
	{
		Uid = saveData.Uid;
		DisplayName = saveData.DisplayName;

		ActivityStack.Clear();

		// Populate activity stack
		foreach ( var actStackSave in saveData.ActivityStack )
		{
			ActivityStack.Add( new ActivityDescription( actStackSave ) );
		}

		// Attempt to copy over result
		if ( saveData.ActivityResultTypeName != null )
		{
			var typeDesc = TypeLibrary.GetDescription( saveData.ActivityResultTypeName );
			_activityResult = TypeLibrary.Create<ActivityResult>( saveData.ActivityResultTypeName );
			NonGenericJson.DeserializeTo( typeDesc.TargetType, saveData.ActivityResultSerialized, typeDesc.TargetType,
				_activityResult );
		}

		// Jump to creating new activity
		_shouldInitActivityInstance = true;
		AttemptActivityInit();
	}

	public SaveData ToSaveData()
	{
		var instance = new SaveData
		{
			Uid = Uid,
			DisplayName = DisplayName,
			ActivityStack = new List<ActivityDescription.SaveData>(),
			Members = new List<GameMember.SaveData>()
		};

		// set up ActivityStack
		foreach ( var description in ActivityStack )
		{
			instance.ActivityStack.Add( description.ToSaveData() );
		}

		// set up ActivityResult*
		if ( _activityResult != null )
		{
			var typeDesc = TypeLibrary.GetDescription( _activityResult.GetType() );
			instance.ActivityResultTypeName = typeDesc.TargetType.FullName;
			instance.ActivityResultSerialized = NonGenericJson.Serialize( typeDesc.TargetType, _activityResult );
		}

		// set up Members
		foreach ( var member in Members )
		{
			instance.Members.Add( member.ToSaveData() );
		}

		return instance;
	}
}
