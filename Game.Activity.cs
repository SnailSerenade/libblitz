using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace libblitz;

public partial class Game
{
	[Net] public IList<ActivityDescription> ActivityStack { get; private set; } = new List<ActivityDescription>();
	[Net] public IList<BaseActivity> Activities { get; private set; } = new List<BaseActivity>();

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
		ClientHandleStartActivity( activity.Uid );
		_shouldInitActivityInstance = false;

		// Remove current activity instance from instance list if required
		CleanUnusedActivityInstances();
	}

	/// <summary>
	/// Request activity init
	/// </summary>
	/// <param name="result">Previous activity result to pass to ActivityStart</param>
	private void RequestActivityInit( ActivityResult result )
	{
		_activityResult = result;
		_shouldInitActivityInstance = true;
	}

	/// <summary>
	/// This function has been changed to try to fix bug #1
	/// 
	/// 
	///
	/// **Previous function description:**
	/// Just remove current activity (stack) from main Activities list
	/// todo: optimize this
	/// </summary>
	protected void CleanUnusedActivityInstances()
	{
		if ( ActivityStack.Count == 0 )
		{
			return;
		}

		var cleanedFromStack = 0;
		var cleanedFromInstances = 0;

		foreach ( var description in ActivityStack )
		{
			var activity = description.Activity;

			if ( activity == null )
			{
				continue;
			}

			if ( activity.KeepAlive )
			{
				continue;
			}

			if ( description == ActivityStack.Last() )
			{
				continue;
			}

			Log.Info( $"Deleting unreferenced stack activity {activity.Uid}/{activity.GetType().Name}" );

			activity.ActivityEnd();
			ClientHandleEndActivity( activity.Uid );
			Activities.Remove( activity );
			activity.Delete();

			cleanedFromStack++;
		}

		// todo: figure out why activities can be in instance storage and not the stack?
		for ( var i = Activities.Count - 1; i >= 0; i-- )
		{
			var activity = Activities[i];
			if ( ActivityStack.Any( v => v.Uid == activity.Uid ) )
			{
				continue;
			}

			Log.Info( $"Deleting unreferenced instance activity {activity.Uid}/{activity.GetType().Name}" );

			activity.ActivityEnd();
			ClientHandleEndActivity( activity.Uid );
			Activities.RemoveAt( i );
			activity.Delete();

			cleanedFromInstances++;
		}

		if ( cleanedFromInstances != 0 || cleanedFromStack != 0 )
		{
			Log.Info( $"Cleaned {cleanedFromInstances} instance(s) and {cleanedFromStack} stack object(s)" );
		}
	}

	[ClientRpc]
	private void ClientHandleStartActivity( Guid activityId )
	{
		var activity = Activities.SingleOrDefault( v => v.Uid == activityId );
		if ( activity == null )
		{
			Log.Warning( $"Client has no activity with UID {activityId}" );
			return;
		}

		activity.ActivityClientStart();
	}

	[ClientRpc]
	private void ClientHandleEndActivity( Guid activityId )
	{
		var activity = Activities.SingleOrDefault( v => v.Uid == activityId );
		if ( activity == null )
		{
			Log.Warning( $"Client has no activity with UID {activityId}" );
			return;
		}

		activity.ActivityClientEnd();
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

		CleanUnusedActivityInstances();

		// Create description & add activity
		Activities.Add( activity );
		ActivityStack.Add( activity.CreateDescription() );

		// Request & attempt activity init
		RequestActivityInit( result );
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

		CleanUnusedActivityInstances();

		// Create & add activity
		Activities.Add( description.CreateInstance<BaseActivity>() );
		ActivityStack.Add( description );

		// Request & attempt activity init
		RequestActivityInit( result );
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

		CleanUnusedActivityInstances();

		// Remove current activity description from stack
		ActivityStack.RemoveAt( ActivityStack.Count - 1 );

		// Use previous activity if possible
		if ( ActivityStack.Count == 0 )
		{
			return;
		}

		// Prepare new activity
		var current = ActivityStack.Last();
		if ( current.Activity != null )
		{
			return;
		}

		// Add activity to Activities list
		var activity = current.CreateInstance<BaseActivity>();
		Activities.Add( activity );

		// Request & attempt activity init
		RequestActivityInit( result );
		AttemptActivityInit();
	}
}
