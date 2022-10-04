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

	public Sandbox.UI.Panel RootPanel => Hud.RootPanel;

	[JsonIgnore]
	public IList<Sandbox.UI.Panel> Panels { get; } = new List<Sandbox.UI.Panel>();

	[JsonIgnore]
	public Sandbox.UI.Panel Panel
	{
		get => RootPanel;
		set => SetPanel( value );
	}

	public void AddPanel( Sandbox.UI.Panel panel )
	{
		// Compare: does the Panels list already contain this panel?
		if ( Panels.Contains( panel ) )
		{
			Log.Warning( $"Tried to add existing panel! {panel.GetType().Name}" );
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
				panel.Parent = RootPanel;
				return;
			}
		}

		// Add entity to list if comparisons fail
		Panels.Add( panel );
		panel.Parent = RootPanel;
	}

	public void RemovePanelByType( Type type )
	{
		if ( type == null )
			return;
		for ( int i = Panels.Count - 1; i >= 0; i-- )
		{
			Sandbox.UI.Panel panel = Panels[i];
			if ( panel.GetType() == type )
			{
				panel.Delete();
				Panels.RemoveAt( i );
				Log.Info( $"Removed panel of type {type.Name}" );
			}
		}
	}

	public void SetPanel( Sandbox.UI.Panel panel )
	{
		RemovePanelByType( panel.GetType() );
		AddPanel( panel );
	}
}