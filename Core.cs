using System;

namespace libblitz;

public interface ISaveData
{
	public string Serialize();

	public static void CopyToOutput<TSave, TOutput>( TSave data, TOutput output ) where TSave : ISaveData
	{
		var saveTypeDesc = TypeLibrary.GetDescription<TSave>();
		var outputTypeDesc = TypeLibrary.GetDescription<TOutput>();

		if ( saveTypeDesc == null )
		{
			throw new Exception( "Couldn't get type description for save data type" );
		}

		if ( outputTypeDesc == null )
		{
			throw new Exception( "Couldn't get type description for output data type" );
		}

		foreach ( var saveProp in saveTypeDesc.Properties )
		{
			var outputProp = outputTypeDesc.GetProperty( saveProp.Name );

			if ( outputProp == null )
			{
				Log.Warning( $"Property {saveProp.Name} not found on output type" );
				continue;
			}

			outputProp.SetValue( output, saveProp.GetValue( data ) );
		}
	}

	public static void CopyFromOutput<TSave, TOutput>( TSave data, TOutput output ) where TSave : ISaveData
	{
		var saveTypeDesc = TypeLibrary.GetDescription<TSave>();
		var outputTypeDesc = TypeLibrary.GetDescription<TOutput>();

		if ( saveTypeDesc == null )
		{
			throw new Exception( "Couldn't get type description for save data type" );
		}

		if ( outputTypeDesc == null )
		{
			throw new Exception( "Couldn't get type description for output data type" );
		}

		foreach ( var saveProp in saveTypeDesc.Properties )
		{
			var outputProp = outputTypeDesc.GetProperty( saveProp.Name );

			if ( outputProp == null )
			{
				Log.Warning( $"Property {saveProp.Name} not found on output type" );
				continue;
			}

			saveProp.SetValue( output, outputProp.GetValue( data ) );
		}
	}
}
