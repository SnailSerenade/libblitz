using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace libblitz;

public partial class Game
{
	[Net] public List<ActivityDescription> ActivityStack { get; } = new();
	[Net] public List<BaseActivity> Activities { get; } = new();

	private ActivityResult _activityResult;

	private bool _shouldInitActivityInstance = false;

	/// <summary>
	/// Should call <see cref="BaseActivity"/>.ActivityActive if requested (_shouldInitActivityInstance) and all
	/// players are ready
	/// </summary>
	private void AttemptActivityInit()
	{
		if ( _activityResult == null )
		{
			Log.Warning( "_activityResult null at time of AttemptActivityInit" );
		}

		if ( !_shouldInitActivityInstance )
		{
			return;
		}

		if ( ActivityStack.Count == 0 )
		{
			return;
		}

		if ( !ActivityStack.Last().AllMembersReady )
		{
			return;
		}

		var activity = ActivityStack.Last().Activity;
		if ( activity == null )
		{
			return;
		}

		activity.ActivityStart( _activityResult );
		_shouldInitActivityInstance = false;
	}

	/// <summary>
	/// Use provided <see cref="BaseActivity"/> and activate it with provided result.
	/// The result (<see cref="ActivityResult"/>) should be provided by the previous activity.
	/// </summary>
	/// <param name="activity">Activity instance</param>
	/// <param name="result">Result of previous activity</param>
	/// <typeparam name="T">Result type</typeparam>
	public void PushActivity<T>( BaseActivity activity, T result ) where T : ActivityResult
	{
		Host.AssertServer();

		if ( Activities.Contains( activity ) )
		{
			throw new InvalidOperationException( "Activity instance already contained in list" );
		}

		Activities.Add( activity );
		ActivityStack.Add( activity.CreateDescription() );

		_activityResult = result;

		_shouldInitActivityInstance = true;
		AttemptActivityInit();
	}

	/// <summary>
	/// Use provided <see cref="ActivityDescription"/> to create a new activity instance and activate it
	/// with provided result.
	/// The result (<see cref="ActivityResult"/>) should be provided by the previous activity.
	/// </summary>
	/// <param name="description">Activity description to create instance for</param>
	/// <param name="result">Result of previous activity</param>
	/// <typeparam name="T">Result type</typeparam>
	public void PushActivity<T>( ActivityDescription description, T result ) where T : ActivityResult
	{
		Host.AssertServer();

		if ( Activities.Contains( description.Activity ) )
		{
			throw new InvalidOperationException( "Activity instance already contained in list" );
		}

		Activities.Add( description.CreateInstance<BaseActivity>() );
		ActivityStack.Add( description );

		_activityResult = result;

		_shouldInitActivityInstance = true;
		AttemptActivityInit();
	}

	/// <summary>
	/// Delete the current activity instance and go back to the activity a layer up.
	/// </summary>
	/// <param name="result">Result of current activity (preferably)</param>
	/// <typeparam name="T">Result type</typeparam>
	public void PopActivity<T>( T result ) where T : ActivityResult
	{
		Host.AssertServer();

		if ( ActivityStack.Count == 0 )
		{
			throw new InvalidOperationException( "No activity to pop" );
		}

		// Call ActivityEnd on current activity
		{
			var current = ActivityStack.Last();
			current.Activity.ActivityEnd();

			// Remove / delete activity if required
			if ( !current.Activity.KeepAlive )
			{
				Activities.Remove( current.Activity );
				// if we ever make BaseActivity an Entity, this should call Delete
			}
		}

		// Remove current activity description from stack
		ActivityStack.RemoveAt( ActivityStack.Count - 1 );

		// Use previous activity if possible
		if ( ActivityStack.Count == 0 )
		{
			return;
		}

		// Prepare new activity
		{
			var current = ActivityStack.Last();
			if ( current.Activity != null )
				return;
			var activity = current.CreateInstance<BaseActivity>();
			Activities.Add( activity );

			_shouldInitActivityInstance = true;
			_activityResult = result;
			AttemptActivityInit();
		}
	}
}
