namespace Bash
{
	using System.Collections.Generic;
	public interface IScopeOfVisibility
	{
		Dictionary<string, BashVariable> Variables { get; }
	}
}

