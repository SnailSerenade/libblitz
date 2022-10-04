/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sandbox;

namespace libblitz;

public abstract partial class Game
{
	[JsonIgnore]
	[Net]
	public IList<Activity> Activities { get; protected set; } = new List<Activity>();

	[JsonIgnore]
	[Net, Change]
	private Activity InternalActivity { get; set; } = null;
	void OnInternalActivityChanged( Activity o, Activity n ) => Event.Run( "libblitz.activityupdate" );

	[SelectCopyIncluded]
	[Net]
	public string PreviousActivityType { get; private set; } = null;

	[SelectCopyIncluded]
	[Net]
	public string PreviousActivityResult { get; private set; } = null;

	[JsonIgnore]
	public Activity Activity
	{
		get => InternalActivity;
		set
		{
			if ( Host.IsClient )
				return;
			if ( InternalActivity != null )
			{
				PreviousActivityType = InternalActivity.GetType().Name;
				PreviousActivityResult = InternalActivity.ActivityDormant();
				InternalActivity.CallClientActivityDormant();
			}
			InternalActivity = value;
			NextActivity = null;
			if ( InternalActivity == null )
				return;
			InternalActivity.PreparedForActivityActive = true;
			InternalActivity.AttemptStateUpdate();
		}
	}

	public Activity GetActivityByType<T>() => GetActivityByType( typeof( T ) );
	public Activity GetActivityByType( Type type )
	{
		if ( type == null )
			throw new ArgumentNullException( nameof( type ), "Activity type was null" );
		foreach ( var activity in Activities )
			if ( activity.GetType() == type )
				return activity;
		throw new KeyNotFoundException( $"Activity type {type.Name} not found." );
	}
	public Activity GetActivityByTypeName( string type )
	{
		if ( type == null )
			throw new ArgumentNullException( nameof( type ), "Activity type was null" );
		foreach ( var activity in Activities )
			if ( activity.GetType().Name == type )
				return activity;
		throw new KeyNotFoundException( $"Activity type {type} not found." );
	}

	public void SetActivityByType<T>() => Activity = GetActivityByType<T>();
	public void SetActivityByType( Type type ) => Activity = GetActivityByType( type );
	public void SetActivityByTypeName( string type ) => Activity = GetActivityByTypeName( type );
	public void SetActivity( Activity activity )
	{
		AddActivity( activity );
		Activity = activity;
	}

	public void SetNextActivityByType<T>() => NextActivity = typeof( T ).Name;
	public void SetNextActivityByType( Type type ) => NextActivity = type.Name;
	public void SetNextActivityByTypeName( string type ) => NextActivity = type;

	public void AddActivity( Activity activity )
	{
		// Compare: does the Activities list already contain this activity?
		if ( Activities.Contains( activity ) )
		{
			Log.Error( $"Tried to add existing activity! {activity.GetType().Name}" );
			return;
		}

		// Compare: does the Activities list already have this type of Activity?
		for ( int i = 0; i < Activities.Count; i++ )
		{
			Activity storedActivity = Activities[i];
			if ( storedActivity.GetType() == activity.GetType() )
			{
				// This is the same type of activity, so clean up old one and replace it
				Log.Info( $"Replacing existing Activity of type {activity.GetType().Name}" );

				// Clean up
				Activities[i].Delete();

				// Replace
				Activities[i] = activity;
				return;
			}
		}

		// Add activity to list if comparisons fail
		Activities.Add( activity );
	}
}