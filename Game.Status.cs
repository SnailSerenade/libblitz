/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using Sandbox;

namespace libblitz;

public abstract partial class Game : Sandbox.Game, IGameData
{
	[Net]
	private GameStatus InternalStatus { get; set; } = GameStatus.NONE;

	[SelectCopyIncluded]
	public GameStatus Status
	{
		get => InternalStatus;
		set
		{
			if ( InternalStatus == GameStatus.NONE )
			{
				Log.Info( $"Setting initial game status value! ({value})" );
				InternalStatus = value;
				return;
			}

			// todo: is this needed / working?
			if ( value != InternalStatus + 1 )
			{
				Log.Info( $"Tried to move from game status {InternalStatus} directly to {value}!" );
				return;
			}

			InternalStatus = value;
		}
	}
}