namespace Bash
{
	using System;
	using System.Collections.Generic;
	public class BashFunction : IPartOfScript, IScopeOfVisibility
	{
		public BashFunction ()
		{
			Body = new List<ICommandStatementOrVariable>();
		}
		public string Name {get; set; }
		public List<ICommandStatementOrVariable> Body { get; }

		public Dictionary<string, BashVariable> Variables
		{
			get
			{
				var variables = new Dictionary<string, BashVariable>();
				foreach (var part in Body)
				{
					if (part is BashVariable)
					{
						BashVariable item = part as BashVariable;
						string name = item.Name;
						variables.Add(name, item);
					}
				}
				return variables;
			}
		}
	}
}
