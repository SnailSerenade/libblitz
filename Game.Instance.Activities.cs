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
	[Net]
	private Activity InternalActivity { get; set; } = null;

	[JsonIgnore]
	public Activity Activity
	{
		get => InternalActivity;
		set
		{
			if ( InternalActivity != null )
			{
				InternalActivity.ActivityDormant();
				InternalActivity.CallClientActivityDormant();
			}
			InternalActivity = value;
			if ( InternalActivity == null )
				return;
			InternalActivity.PreparedForActivityActive = true;
			InternalActivity.AttemptStateUpdate();
		}
	}

	public Activity GetActivityByType<T>()
	{
		foreach ( var activity in Activities )
			if ( activity.GetType() == typeof( T ) )
				return activity;
		throw new KeyNotFoundException( $"Activity type {typeof( T ).Name} not found." );
	}
	public Activity GetActivityByType( Type type )
	{
		if ( type == null )
			throw new ArgumentNullException( nameof( type ), "Activity type was null" );
		foreach ( var activity in Activities )
			if ( activity.GetType() == type )
				return activity;
		throw new KeyNotFoundException( $"Activity type {type.Name} not found." );
	}

	public void SetActivityByType<T>() => Activity = GetActivityByType<T>();
	public void SetActivityByType( Type type ) => Activity = GetActivityByType( type );
	public void SetActivity( Activity activity )
	{
		AddActivity( activity );
		Activity = activity;
	}

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