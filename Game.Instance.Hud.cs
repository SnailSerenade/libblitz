/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace libblitz;

public abstract partial class Game
{
	private class GameHudHost : Sandbox.HudEntity<Sandbox.UI.RootPanel> { }
	[JsonIgnore]
	private GameHudHost Hud { get; set; }

	[JsonIgnore]
	public IList<Sandbox.UI.Panel> Panels { get; } = new List<Sandbox.UI.Panel>();

	[JsonIgnore]
	private Sandbox.UI.Panel InternalPanel { get; set; }
	public Sandbox.UI.Panel Panel
	{
		get => InternalPanel;
		set => SetPanel( value );
	}

	public void AddPanel( Sandbox.UI.Panel panel )
	{
		// Compare: does the Panels list already contain this panel?
		if ( Panels.Contains( panel ) )
		{
			Log.Error( $"Tried to add existing panel! {panel.GetType().Name}" );
			return;
		}

		// Compare: does the Panels list already have this type of panel?
		for ( int i = 0; i < Panels.Count; i++ )
		{
			Sandbox.UI.Panel storedPanel = Panels[i];
			if ( storedPanel.GetType() == panel.GetType() )
			{
				// This is the same type of panel, so clean up old one and replace it
				Log.Info( $"Replacing existing panel of type {storedPanel.GetType().Name}" );

				// Clean up
				Panels[i].Delete();

				// Replace
				Panels[i] = panel;
				return;
			}
		}

		// Add entity to list if comparisons fail
		Panels.Add( panel );
	}

	public void SetPanelByType( Type type )
	{
		if ( type == null )
			throw new ArgumentNullException( nameof( type ), "Panel type was null" );
		foreach ( var panel in Panels )
		{
			if ( panel.GetType() == type )
			{
				if ( InternalPanel != null )
					Log.Info( $"Replacing panel {InternalPanel.GetType().Name}" );
				InternalPanel = panel;
				if ( InternalPanel != null )
					InternalPanel.Parent = Hud.RootPanel;
				return;
			}
		}
		throw new KeyNotFoundException( $"Game has no panel of type {type.Name}" );
	}

	public void SetPanel( Sandbox.UI.Panel panel )
	{
		if ( Panels.Contains( panel ) )
		{
			InternalPanel = panel;
			return;
		}

		AddPanel( panel );
		SetPanelByType( panel.GetType() );
	}
}