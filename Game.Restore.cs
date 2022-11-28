using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Sandbox;

namespace libblitz;

public partial class Game
{
	private static BaseFileSystem _saveSubSystem;
	private BaseFileSystem _gameSubSystem;

	public Game( SaveData saveData ) => LoadSaveData( saveData );

	/// <summary>
	/// Delete all entities used by libblitz
	/// </summary>
	private static void FullCleanup()
	{
		foreach ( var entity in Entity.All )
		{
			switch ( entity )
			{
				case ActivityDescription:
				case BaseActivity:
				case GameMember:
					entity.Delete();
					break;
				case Client client:
					client.Pawn = null;
					break;
			}
		}
	}

	private void LoadSaveData( SaveData saveData )
	{
		// Clean up entities
		FullCleanup();

		// Clear previous game data
		Activities.Clear();
		ActivityStack.Clear();
		Members.Clear();

		Uid = saveData.Uid;
		DisplayName = saveData.DisplayName;

		// Populate activity stack
		foreach ( var actStackSave in saveData.ActivityStack )
		{
			ActivityStack.Add( new ActivityDescription( actStackSave ) );
		}

		// Attempt to load saved ActivityResult
		if ( saveData.ActivityResultTypeName != null )
		{
			var typeDesc = TypeLibrary.GetDescription( saveData.ActivityResultTypeName );
			_activityResult = TypeLibrary.Create<ActivityResult>( saveData.ActivityResultTypeName );
			NonGenericJson.DeserializeTo( typeDesc.TargetType, saveData.ActivityResultSerialized, typeDesc.TargetType,
				_activityResult );
		}

		// Restore members
		foreach ( var memberSave in saveData.Members )
		{
			var member = new GameMember( memberSave );
			Members.Add( member );
		}

		// Create activity instance
		if ( ActivityStack.Count != 0 )
		{
			Activities.Add( ActivityStack.Last().CreateInstance<BaseActivity>() );
		}

		// Jump to initializing new activity
		_shouldInitActivityInstance = true;
		AttemptActivityInit();
	}

	private static void InitFileSystems()
	{
		FileSystem.Data.CreateDirectory( "saves" );
		_saveSubSystem ??= FileSystem.Data.CreateSubSystem( "saves" );
	}

	public void LoadPersistent( Guid uid )
	{
		InitFileSystems();

		if ( !_saveSubSystem.DirectoryExists( $"{uid}" ) )
		{
			throw new Exception( $"Save directory for Game {uid} not found" );
		}

		_gameSubSystem = _saveSubSystem.CreateSubSystem( $"{uid}" );

		if ( !_gameSubSystem.FileExists( "latest" ) )
		{
			throw new Exception( $"Couldn't find save data for Game {Uid}" );
		}

		var saveData = _gameSubSystem.ReadJson<SaveData>( "latest" );
		LoadSaveData( saveData );
	}

	public void SavePersistent( Guid uid )
	{
		InitFileSystems();

		if ( !_saveSubSystem.DirectoryExists( $"{uid}" ) )
		{
			Log.Info( $"Creating save directory for unsaved Game {uid}" );
			_saveSubSystem.CreateDirectory( $"{uid}" );
		}

		_gameSubSystem = _saveSubSystem.CreateSubSystem( $"{uid}" );
		_gameSubSystem.WriteJson( "latest", ToSaveData() );
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
			var activityResultType = _activityResult.GetType();
			instance.ActivityResultTypeName = activityResultType.FullName;
			instance.ActivityResultSerialized = NonGenericJson.Serialize( activityResultType, _activityResult );
		}

		// set up Members
		foreach ( var member in Members )
		{
			instance.Members.Add( member.ToSaveData() );
		}

		return instance;
	}
}
