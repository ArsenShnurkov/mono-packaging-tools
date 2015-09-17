using System;
using System.Diagnostics;
using Eto.Parse.Grammars;
using Eto.Parse.Writers;
using Eto.Parse;
using System.Text;
using System.IO;
using System.Reflection;

namespace mptgitmodules
{
	class Globals
	{

		public static void Main (string[] args)
		{
			if (args.Length == 0) {
				var ui = new UsageInfo();
				ui.Dump (Console.Out);
				return;
			}
			Parse_ManualSpaces (args);
		}

		public static void Parse_ManualSpaces(string[] args)
		{
			var textGrammar = Resources("syntax4.ebnf");
			var textContent = LoadReader (Console.In);
			var root_rule = "file_content";
			EbnfStyle style = (EbnfStyle)(
				(uint)EbnfStyle.Iso14977 
				& ~(uint) EbnfStyle.WhitespaceSeparator	
				| (uint) EbnfStyle.EscapeTerminalStrings);
			var Parser = new Parser (textGrammar, root_rule, style);
			Parser.DoProcessing (textContent, args);
		}

		public static string LoadReader(TextReader sr)
		{
			var fileContent = sr.ReadToEnd ();
			return fileContent;
		}

		public static string LoadStream(Stream s)
		{
			using (var sr = new StreamReader (s)) {
				return LoadReader(sr);
			}
		}

		public static string LoadFile(string filename)
		{
			using (var s = new FileStream (filename, FileMode.Open, FileAccess.Read)) {
				return LoadStream (s);
			}
		}

		public static string Resources(string name_of_grammar)
		{
			// Resource ID: mptgitmodules.Resources.syntax4.ebnf
			var fullname = string.Format ("mptgitmodules.Resources.{0}", name_of_grammar);
			using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream (fullname))
			{
				return LoadStream(s);
			}
		}
	}
}
