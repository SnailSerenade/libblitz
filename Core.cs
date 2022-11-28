using System;
using System.Text.Json;
using Sandbox;

namespace libblitz;

public interface ISaveData
{
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

			saveProp.SetValue( data, outputProp.GetValue( output ) );
		}
	}
}

public static class NonGenericJson
{
	private interface IGeneratedJsonSerializer
	{
		public string Serialize( object instance );
		public void DeserializeTo( string data, Type outputType, object output );
		public void DeserializeTo<TOutput>( string data, TOutput output );
	}

	private class GeneratedJsonSerializer<TInput> : IGeneratedJsonSerializer
	{
		public string Serialize( object instance ) => JsonSerializer.Serialize( (TInput)instance );

		public void DeserializeTo( string data, Type outputType, object output )
		{
			var instance = JsonSerializer.Deserialize<TInput>( data );

			var inputTypeDesc = TypeLibrary.GetDescription<TInput>();
			var outputTypeDesc = TypeLibrary.GetDescription( outputType );

			if ( inputTypeDesc == null )
			{
				throw new Exception( "Couldn't get type description for input data type" );
			}

			if ( outputTypeDesc == null )
			{
				throw new Exception( "Couldn't get type description for output data type" );
			}

			foreach ( var inputProp in inputTypeDesc.Properties )
			{
				var outputProp = outputTypeDesc.GetProperty( inputProp.Name );

				if ( outputProp == null )
				{
					Log.Warning( $"Property {inputProp.Name} not found on output type" );
					continue;
				}

				if ( outputProp.Name == "NetworkIdent" )
				{
					continue; // just skip warning for NetworkIdent 
				}

				if ( !outputProp.CanWrite )
				{
					Log.Warning( $"Can't write to output property {outputProp.Name}" );
					continue;
				}

				try
				{
					outputProp.SetValue( output, inputProp.GetValue( instance ) );
				}
				catch ( Exception e )
				{
					Log.Info( $"DeserializeTo failure: {e}" );
				}
			}
		}

		public void DeserializeTo<TOutput>( string data, TOutput output ) =>
			DeserializeTo( data, typeof(TOutput), output );
	}

	private static TypeDescription _gjs;

	[Event.Hotload]
	private static void FixGjs() => _gjs = null;

	/// <summary>
	/// Don't use this.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="instance"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public static string Serialize( Type type, object instance )
	{
		_gjs ??= TypeLibrary.GetDescription( typeof(GeneratedJsonSerializer<>) );

		if ( _gjs == null )
		{
			throw new Exception( "Failed to find GeneratedJsonSerializer" );
		}

		var generic = _gjs.CreateGeneric<IGeneratedJsonSerializer>( new[] { type } );
		return generic.Serialize( instance );
	}

	/// <summary>
	/// Deserialize data of provided <see cref="Type"/> and copy shared properties to the output
	/// </summary>
	/// <param name="inputType">Input data type</param>
	/// <param name="data">Data (as string)</param>
	/// <param name="outputType">Output type</param>
	/// <param name="output">Output object</param>
	/// <exception cref="Exception">TypeLibrary failure</exception>
	public static void DeserializeTo( Type inputType, string data, Type outputType,
		object output )
	{
		_gjs ??= TypeLibrary.GetDescription( typeof(GeneratedJsonSerializer<>) );

		if ( _gjs == null )
		{
			throw new Exception( "Failed to find GeneratedJsonSerializer" );
		}

		var generic = _gjs.CreateGeneric<IGeneratedJsonSerializer>( new[] { inputType } );
		generic.DeserializeTo( data, outputType, output );
	}

	/// <summary>
	/// Deserialize data of provided <see cref="Type"/> and copy shared properties to the output
	/// </summary>
	/// <param name="type">Type of provided data</param>
	/// <param name="data">Data (as string)</param>
	/// <param name="output">Output object</param>
	/// <typeparam name="TOutput">Output type</typeparam>
	/// <exception cref="Exception">TypeLibrary failure</exception>
	public static void DeserializeTo<TOutput>( Type type, string data, TOutput output )
	{
		_gjs ??= TypeLibrary.GetDescription( typeof(GeneratedJsonSerializer<>) );

		if ( _gjs == null )
		{
			throw new Exception( "Failed to find GeneratedJsonSerializer" );
		}

		var generic = _gjs.CreateGeneric<IGeneratedJsonSerializer>( new[] { type } );
		generic.DeserializeTo( data, output );
	}
}
