namespace Bash
{
	using System.Collections.Generic;
	public class SimpleInvocationOfExternalTool : IBashCommand
	{
		public SimpleInvocationOfExternalTool (string name) : this()
		{
			this.ToolNameOfPathName = name;
		}
		protected SimpleInvocationOfExternalTool ()
		{
			this.Parameters = new List<ICommandLineParameter>();
		}
		public string ToolNameOfPathName { get; set; }
		List<ICommandLineParameter> Parameters { get; }
	}
}

