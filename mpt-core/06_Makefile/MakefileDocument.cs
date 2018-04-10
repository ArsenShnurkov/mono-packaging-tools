namespace Makefile
{
	using System.Collections.Generic;
	public class MakefileDocument
	{
		List<Rule> rules = new List<Rule>();
		List<Rule> Rules {
			get
			{
				return rules;
			}
		}
	}
}
