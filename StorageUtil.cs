/*
 * part of the BonitoBlitz (w.i.p name) gamemode
 * library used across the board gamemode & minigames
 * - lotuspar, 2022 (github.com/lotuspar)
 */
namespace libblitz;

using System;

[AttributeUsage( AttributeTargets.Property )]
public class SelectCopyIncludedAttribute : Attribute { }

[AttributeUsage( AttributeTargets.Class )]
public class SelectCopyIncludeAllAttribute : Attribute { }

static class StorageUtil
{
	/// <summary>
	/// Copy properties with the SelectCopyIncluded attribute to the output object
	/// </summary>
	/// <typeparam name="O">Output object type</typeparam>
	/// <param name="from">Input object</param>
	/// <param name="to">Output object</param>
	public static void SelectCopyTo<O>( object from, O to )
	{
		bool copyAll = false;
		if ( TypeLibrary.GetAttribute<SelectCopyIncludeAllAttribute>( from.GetType() ) != null )
			copyAll = true;

		foreach ( var propIn in TypeLibrary.GetDescription( from.GetType() ).Properties )
		{
			if ( !copyAll && propIn.GetCustomAttribute<SelectCopyIncludedAttribute>() == null )
				continue;

			foreach ( var propOut in TypeLibrary.GetDescription<O>().Properties )
			{
				if ( propOut.Name != propIn.Name )
					continue;
				try
				{
					propOut.SetValue( to, propIn.GetValue( from ) );
				}
				catch ( ArgumentException )
				{
					Log.Error( $"Couldn't set {propOut.Name}, are you sure it has a setter?" );
				}
			}
		}
	}
}