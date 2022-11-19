using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sandbox;

namespace libblitz;

public partial class GameMember
{
	[Net] public List<Guid> ActivityStack { get; set; }

	public void JoinActivity( BaseActivity activity )
	{
		Host.AssertServer();

		ActivityStack.Add( activity.Uid );
	}

	public void LeaveCurrentActivity()
	{
		Host.AssertServer();

		(ActivityStack as IList<Guid>).RemoveAt( ActivityStack.Count - 1 );
	}
}
