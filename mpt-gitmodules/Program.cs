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
			//Parse_AutoSpaces ("bnf3/syntax1");
			// Parse_ManualSpaces ("bnf3/syntax2");
			Parse_ManualSpaces ("bnf3/syntax3");
		}

		readonly static string location = 
			"/var/calculate/remote/distfiles/egit-src/mono-packaging-tools.git/mpt-gitmodules";
		
		public static void Parse_AutoSpaces(string variant)
		{
			var grammar = string.Format("{0}/{1}.ebnf", location, variant);
			var textGrammar = LoadFile (grammar);
			var content = string.Format("{0}/{1}.txt", location, variant);
			var textContent = LoadFile (content);
			var root_rule = "file_content";
			EbnfStyle style = (EbnfStyle)(
				(uint)EbnfStyle.Iso14977 
				| (uint) EbnfStyle.EscapeTerminalStrings);
			var Parser = new Parser (textGrammar, root_rule, style);
			Parser.DoProcessing (textContent);
		}

		public static void Parse_ManualSpaces(string variant)
		{
			var grammar = string.Format("{0}/{1}.ebnf", location, variant);
			var textGrammar = LoadFile (grammar);
			var content = string.Format("{0}/{1}.txt", location, variant);
			var textContent = LoadFile (content);
			var root_rule = "file_content";
			EbnfStyle style = (EbnfStyle)(
				(uint)EbnfStyle.Iso14977 
				& ~(uint) EbnfStyle.WhitespaceSeparator	
				| (uint) EbnfStyle.EscapeTerminalStrings);
			var Parser = new Parser (textGrammar, root_rule, style);
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
