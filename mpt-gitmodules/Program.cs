using System;
using System.Diagnostics;
using Eto.Parse.Grammars;
using Eto.Parse.Writers;
using Eto.Parse;
using System.Text;

namespace mptgitmodules
{
	class MainClass
	{
		readonly static string myEbnfString = @"
file_content := { file_unit } ;

file_unit := eol | nbody | myws ;

eol := ? Terminals.Eol ? ;

nbody := { 'A' } ;

myws := '-' ;
";
		readonly static string fileContent = "\nA-AA\n-AAAA-AA-A-\n";

		public static void Main (string[] args)
		{
			var ebnfGrammar = new Eto.Parse.Grammars.EbnfGrammar(
				EbnfStyle.Iso14977);
			var myGrammar = ebnfGrammar.Build(myEbnfString, "file_content"); 

			var ast = myGrammar.Match (fileContent);
			if (ast.Success == false) {
				Trace.WriteLine (ast.ErrorMessage);
				Console.ReadLine ();
			}
			var m = ast.Matches ["eol", true].Matches; // for "evaluate" window of debugger
		}
	}
}
