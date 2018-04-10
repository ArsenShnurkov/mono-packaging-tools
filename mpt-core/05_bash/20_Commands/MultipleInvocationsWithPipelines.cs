namespace Bash
{
	using System.Collections.Generic;
	public abstract class MultipleInvocationsWithPipelines
	{
		List<SimpleInvocationOfExternalTool> commands = new List<SimpleInvocationOfExternalTool>();
		public MultipleInvocationsWithPipelines ()
		{
		}
		public List<SimpleInvocationOfExternalTool> Commands
		{
			get
			{
				return commands;
			}
		}
	}
}

