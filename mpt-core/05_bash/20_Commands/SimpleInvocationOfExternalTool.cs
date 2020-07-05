namespace Bash
{
	using System.Collections.Generic;

	public class SimpleInvocationOfExternalTool : IPartOfScript /*, IBashCommand */
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
		public ICollection<ICommandLineParameter> Parameters { get; }
	}
}

