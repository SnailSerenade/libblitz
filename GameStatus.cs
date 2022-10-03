/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;

public enum GameStatus
{
	NONE, // Unknown status or nothing is happening yet
	INTRODUCTION_NEEDED, // Players need the introduction first before turns can happen
	IN_PROGRESS, // Game is in-progress (as in player turns are happening)
	COMPLETED // Game has fully finished
}