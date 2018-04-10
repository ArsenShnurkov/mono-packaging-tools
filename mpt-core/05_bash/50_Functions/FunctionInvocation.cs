namespace Bash
{
	using System.Collections.Generic;
	public class FunctionInvocation
	{
		protected List<FunctionParameterValue> parameters = new List<FunctionParameterValue>();
		public FunctionInvocation (BashDocument script, string functionName)
		{
			this.Function = script.Functions[functionName];
		}
		public FunctionInvocation (BashFunction function)
		{
			this.Function = function;
		}
		public BashFunction Function { get; }
		public List<FunctionParameterValue> Parameters
		{ 
			get
			{
				return parameters;
			}
		}
	}
}

