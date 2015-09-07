using System;
using Eto.Parse;
using Eto.Parse.Grammars;
using System.Diagnostics;

namespace mptgitmodules
{
	public class Parser
	{
		Grammar myGrammar;
		public Parser(string textGrammar, string root_rule)
		{
			var ebnfGrammar = new Eto.Parse.Grammars.EbnfGrammar( (EbnfStyle)
				( EbnfStyle.Iso14977 
			      - EbnfStyle.WhitespaceSeparator
					+ EbnfStyle.EscapeTerminalStrings )
			);
			myGrammar = ebnfGrammar.Build(textGrammar, root_rule); 
		}

		public void DoProcessing(string textToParse)
		{
			var ast = myGrammar.Match (textToParse);
			var m = ast.Matches ["variable_value", true].Matches; // for "evaluate" window of debugger
			if (ast.Success == false) {
				Trace.WriteLine (ast.ErrorMessage);
				Console.ReadLine ();
			}
		}
	}
}

