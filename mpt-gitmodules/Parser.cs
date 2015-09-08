using System;
using Eto.Parse;
using Eto.Parse.Grammars;
using System.Diagnostics;

namespace mptgitmodules
{
	public class Parser
	{
		Grammar myGrammar;
		public Parser(string textGrammar, string root_rule, EbnfStyle style)
		{
			var ebnfGrammar = new Eto.Parse.Grammars.EbnfGrammar( style	);
			myGrammar = ebnfGrammar.Build(textGrammar, root_rule);
			myGrammar.CheckAbsentRules ();
		}

		public void DoProcessing(string textToParse)
		{
			var ast = myGrammar.Match (textToParse);
			var mmm = ast.Matches ["variable_value", true];
			var mm = ast.Matches ["list_of_subsections", true];
			var m = ast.Matches ["section_body", true].Matches; // for "evaluate" window of debugger
			if (ast.Success == false) {
				Trace.WriteLine (ast.ErrorMessage);
				Console.ReadLine ();
			}
		}
	}
}

