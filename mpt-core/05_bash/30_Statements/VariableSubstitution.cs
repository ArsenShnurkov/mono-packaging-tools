namespace Bash
{
	using System;
	public class VariableSubstitution : IBashStatement
	{
		protected BashVariable variable;
		public VariableSubstitution (BashVariable v)
		{
			this.variable = v;
		}
		public VariableSubstitution (IScopeOfVisibility scope, string name)
		{
			this.variable = scope.Variables[name];
		}
		public BashVariable Variable
		{
			get
			{
				return variable;
			}
		}
	}
}

