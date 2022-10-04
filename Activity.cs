/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 * this is inspired by / based on:
 * - https://github.com/Facepunch/sbox-spire/blob/main/code/Gamemodes/BaseGamemode.cs
 *     * (actually, the whole Activity system is based on the above!)
 */
namespace libblitz;

using System;
using System.Collections.Generic;
using Sandbox;

public interface IActivity
{
	/// <summary>
	/// Subset of the global Player list
	/// </summary>
	public IList<Player> Players { get; }

	public Type PawnType { get; }
	public Type HudPanelType { get; }

	/// <summary>
	/// Called when all players are ready.
	/// </summary>
	public void Initialize();

	/// <summary>
	/// Called on every Game.Simulate
	/// "Called when simulating as part of a player's tick. Like if it's a pawn."
	/// </summary>
	/// <param name="cl">Client</param>
	public void Simulate( Client cl );

	/// <summary>
	/// Called on every Game.FrameSimulate
	/// "Called each frame clientside only on Pawn (and anything the pawn decides to call it on)"
	/// </summary>
	/// <param name="cl">Client</param>
	public void FrameSimulate( Client cl );

	/// <summary>
	/// Called when a player (libblitz.Player) has lost / gained a client
	/// </summary>
	public void PlayerChange();

	/// <summary>
	/// Called when the activity becomes the current global activity
	/// </summary>
	public void ActivityActive( Type sender );

	/// <summary>
	/// Called when the activity is no longer the current global activity
	/// </summary>
	public void ActivityDormant();
}

public abstract partial class Activity : Entity, IActivity
{
	public static Activity Current => Game.Current.Activity;

	[Net]
	public IList<Player> Players { get; private set; }

	public abstract Type PawnType { get; }
	public abstract Type HudPanelType { get; }
	public virtual object Storage { get; set; } = null;

	public bool PreparedForActivityActive = true;
	public bool PreparedForInitialize = true;

	public Activity( IList<Player> players )
	{
		Transmit = TransmitType.Always;

		if ( players == null && Host.IsServer )
			Log.Info( "Using player list from game!" );

		Players = players ?? Game.Current.Players;

		if ( Host.IsServer )
			Load();
	}

	public void Load()
	{
		if ( Storage == null )
		{
			Log.Info( $"Skipping load of activity {GetType().Name} storage" );
			return;
		}
		Storage = Game.Current.LoadPerActivityStorage( this );
	}

	public void Save()
	{
		if ( Storage == null )
		{
			Log.Info( $"Skipping write of activity {GetType().Name} storage" );
			return;
		}

		Game.Current.SavePerActivityStorage( this );
	}

	[ClientRpc]
	private void InternalClientActivityActive( Type sender )
	{
		ActivityActive( sender );
	}
	public virtual void ActivityActive( Type sender )
	{
		if ( Host.IsServer )
		{
			if ( PawnType != null )
				foreach ( var player in Players )
					player.SetPawnByType( PawnType );
		}
	}

	// This 3 layered ActivityDormant is certainly annoying...
	public void CallClientActivityDormant()
	{
		foreach ( var player in Players )
		{
			if ( player.Client != null )
				InternalClientActivityDormant( To.Single( player.Client ) );
		}
	}
	[ClientRpc]
	private void InternalClientActivityDormant()
	{
		ActivityDormant();
	}
	public virtual void ActivityDormant() { }

	[ClientRpc]
	private void InternalClientInitialize() { Initialize(); }
	public virtual void Initialize() { }

	public virtual void PlayerChange()
	{
		AttemptStateUpdate();
	}

	public virtual void AttemptStateUpdate()
	{
		if ( Players.Count == 0 )
			return;

		bool ready = true;
		foreach ( var player in Players )
		{
			if ( player.Client == null )
				ready = false;
		}

		if ( ready )
		{
			if ( PreparedForInitialize )
			{
				PreparedForInitialize = false;
				Initialize();
				foreach ( var player in Players )
					InternalClientInitialize( To.Single( player.Client ) );
			}
			if ( PreparedForActivityActive )
			{
				PreparedForActivityActive = false;
				ActivityActive( Game.Current.PreviousActivityType );
				foreach ( var player in Players )
					InternalClientActivityActive( To.Single( player.Client ), Game.Current.PreviousActivityType );
			}
		}
	}
}