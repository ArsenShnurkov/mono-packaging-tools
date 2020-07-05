namespace Bash
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class BashDocument : IScopeOfVisibility
	{
		public BashDocument ()
		{
			Parts = new List<IPartOfScript> ();
		}
		public ICollection<IPartOfScript> Parts { get; }
		public Dictionary<string, BashFunction> Functions
		{ 
			get
			{
				var functions = new Dictionary<string, BashFunction>();
				foreach (var part in Parts)
				{
					if (part is BashFunction)
					{
						BashFunction func = part as BashFunction;
						string name = func.Name;
						functions.Add(name, func);
					}
				}
				return functions;
			}
		}
		public Dictionary<string, BashVariable> Variables
		{ 
			get
			{
				var variables = new Dictionary<string, BashVariable>();
				foreach (var part in Parts)
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
		public void SaveTo(string fileName)
		{
			using(StreamWriter writetext = new StreamWriter(fileName))
			{
				foreach (var p in Parts)
				{
					writetext.WriteLine(p.ToString());
				}
			}
		}
	}
}
