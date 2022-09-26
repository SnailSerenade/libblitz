/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 * this is inspired by / based on:
 * - https://github.com/Facepunch/sbox-spire/blob/main/code/Gamemodes/BaseGamemode.cs
 */
namespace libblitz;
using System;
using Sandbox;
using System.Collections.Generic;

public interface IActivity
{
	public IList<Player> Players { get; }
	public Type PawnType { get; }

	/// <summary>
	/// Start activity if all criteria are met:
	/// 1. the Activity Player list is initialized and all those players are ready,
	/// 2. the Activity hasn't been initialized yet,
	/// 3. the global current Activity _is_ this one.
	/// If all criteria are met then Initialize and ClientInitialize are called
	/// </summary>
	public void TryInitialize();

	/// <summary>
	/// Called once after TryInitialize succeeds.
	/// This should prepare the Activity for the whole time it exists.
	/// </summary>
	public void Initialize();

	/// <summary>
	/// Called once client-side after TryInitialize succeeds.
	/// This should prepare the Activity for the whole time it exists.
	/// </summary>
	public void ClientInitialize();

	/// <summary>
	/// Called when this Activity becomes the global current Activity
	/// </summary>
	public void BecomeCurrentActivity();

	/// <summary>
	/// Called when this Activity stops being the global current Activity
	/// </summary>
	public void StopBeingCurrentActivity();

	/// <summary>
	/// Called on every Game.Simulate
	/// </summary>
	public void Simulate();

	/// <summary>
	/// Called on every Game.FrameSimulate
	/// </summary>
	public void FrameSimulate();

	/// <summary>
	/// (most likely) Called when Game.Players is changed
	/// </summary>
	public void PlayerListUpdate();
}

/// <summary>
/// Global Activity Data
/// </summary>
public static class Activity
{
	private static IActivity InternalCurrent { get; set; } = null;

	/// <summary>
	/// Current global Activity
	/// </summary>
	public static IActivity Current
	{
		get => InternalCurrent;
		set
		{
			if ( InternalCurrent != null )
				InternalCurrent.StopBeingCurrentActivity();
			InternalCurrent = value;
			if ( InternalCurrent != null )
				InternalCurrent.TryInitialize();
			InternalCurrent.BecomeCurrentActivity();
		}
	}
}

/// <summary>
/// Single activity
/// </summary>
/// <typeparam name="T">Activity pawn type</typeparam>
public abstract partial class Activity<T> : Sandbox.Entity, IActivity where T : Pawn
{
	[Net] public IList<Player> Players { get; set; } = new List<Player>();
	public Type PawnType => typeof( T );
	public bool IsPrepared { get; private set; }

	public Activity( IList<Player> players ) => Players = players;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		// Set this to current Activity if no other activities running
		if ( Activity.Current == null )
		{
			Log.Info( $"No activities active, using {GetType().Name}" );
			Activity.Current = this;
		}

		TryInitialize();
	}

	public abstract void Initialize();

	// "Sandbox.ClientRpc" required until below is fixed
	// https://github.com/Facepunch/sbox-issues/issues/2359
	[Sandbox.ClientRpc] public abstract void ClientInitialize();

	public virtual void Simulate() { }
	public virtual void FrameSimulate() { }

	public virtual void StopBeingCurrentActivity() { }
	public virtual void BecomeCurrentActivity()
	{
		// Set all player pawns to the ones for this activity
		foreach ( var player in Players )
			player.Pawn = player.GetPawn( PawnType );
	}

	public virtual void PlayerListUpdate() => TryInitialize();

	public void TryInitialize()
	{
		if ( Players.Count == 0 )
			return;

		if ( IsPrepared )
			return;

		if ( Activity.Current != this )
			return;

		bool shouldStart = true;

		foreach ( var player in Players )
			if ( player.Client == null )
				shouldStart = false;

		if ( shouldStart )
		{
			Initialize();
			ClientInitialize();
			IsPrepared = true;
		}
	}
}