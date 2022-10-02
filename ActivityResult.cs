/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;

using System;
using Sandbox;

public interface IActivityResult
{
	public Guid WinnerUid { get; }
}

public class ActivityResult : BaseNetworkable, IActivityResult
{
	public Guid WinnerUid { get; set; }
}