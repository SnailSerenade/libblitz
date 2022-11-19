using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sandbox;

namespace libblitz;

public abstract class ActivityResult : BaseNetworkable
{
	public string Name => GetType().Name;

	public abstract string Serialize();
	public abstract T Deserialize<T>( string data );
}

public abstract class BaseActivity : BaseNetworkable
{
	[Net] public Guid Uid { get; init; }

	[Net] private List<GameMember> InternalMembers { get; init; }
	public ReadOnlyCollection<GameMember> Members => InternalMembers.AsReadOnly();

	public Type ResultType { get; init; }

	protected BaseActivity( List<GameMember> members, Type resultType )
	{
		InternalMembers = members;
		ResultType = resultType;
		Uid = Guid.NewGuid();
	}

	protected BaseActivity( Type resultType )
	{
		ResultType = resultType;
		Uid = Guid.NewGuid();
	}
}

public class Activity<TResult> : BaseActivity
{
	protected Activity( List<GameMember> members ) : base( members, typeof(TResult) )
	{
	}

	protected Activity() : base( typeof(TResult) )
	{
	}
}

public class BoardActivity : Activity<BoardActivity.Result>
{
	public class Result : ActivityResult
	{
		public override string Serialize()
		{
			throw new NotImplementedException();
		}

		public override T Deserialize<T>( string data )
		{
			throw new NotImplementedException();
		}
	}
}
