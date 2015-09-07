using System;
using System.Diagnostics;
using Eto.Parse.Grammars;
using Eto.Parse.Writers;
using Eto.Parse;
using System.Text;
using System.IO;

namespace mptgitmodules
{
	class Globals
	{
		public static void Main (string[] args)
		{
			var grammar = "/var/calculate/remote/distfiles/egit-src/mono-packaging-tools.git/mpt-gitmodules/bnf2/syntax4.ebnf";
			var textGrammar = LoadFile (grammar);
			var root_rule = "file_content";
			var Parser = new Parser (textGrammar, root_rule);

			var content = "/var/calculate/remote/distfiles/egit-src/monodevelop.git/.gitmodules";
			var textContent = LoadFile (content);
			Parser.DoProcessing (textContent);
		}


		public static string LoadFile(string filename)
		{
			string fileContent = String.Empty;
			using (var s = new FileStream (filename, FileMode.Open, FileAccess.Read)) {
				using (var sr = new StreamReader (s)) {
					fileContent = sr.ReadToEnd ();
					return fileContent;
				}
			}
		}
	}
}
