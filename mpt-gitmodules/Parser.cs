using Eto.Parse.Writers;

namespace mptgitmodules
{
	using System;
	using System.IO;
	using System.Text;
	using System.Diagnostics;
	using Eto.Parse;
	using Eto.Parse.Grammars;

	public class Parser
	{
		Grammar grammar;
		public Parser()
		{
			grammar = LoadGrammar ("/var/calculate/remote/distfiles/egit-src/mono-packaging-tools.git/mpt-gitmodules/bnf2/syntax3.ebnf");
		}

		string LoadFile(string filename)
		{
			string fileContent = String.Empty;
			using (var s = new FileStream (filename, FileMode.Open, FileAccess.Read)) {
				using (var sr = new StreamReader (s)) {
					fileContent = sr.ReadToEnd ();
					return fileContent;
				}
			}
		}

		Grammar LoadGrammar(string filename)
		{
			string myEbnfString = LoadFile(filename);
			var ebnfGrammar = new Eto.Parse.Grammars.EbnfGrammar(
				(EbnfStyle)(EbnfStyle.Iso14977 - EbnfStyle.WhitespaceSeparator));
			var myGrammar = ebnfGrammar.Build(myEbnfString, "file_content"); 
			return myGrammar;
		}
			
		void Dump(Match ast, StringBuilder res)
		{
			Trace.WriteLine(new DisplayParserWriter ().Write (ast.Parser));
			foreach (var subnode in ast.Matches)
			{
				res.Append (subnode.StringValue);
			}
		}

		void Cut(Match ast, StringBuilder res, string SectionType, string SectionName)
		{
			foreach (var subnode in ast.Matches)
			{
				if (IsSubnodeToRemove (subnode, SectionType, SectionName) == false)
				{
					res.Append (subnode.StringValue);
				}
			}
		}

		public string Parse (string filename, string SectionType, string SectionName)
		{
			var fileContent = LoadFile (filename);
			var res = new StringBuilder (fileContent.Length);

			fileContent = "\nnn-nn\n---n-n-\n";

			var ast = grammar.Match (fileContent);
			if (ast.Success == false) {
				Trace.WriteLine (ast.ErrorMessage);
				Dump(ast, res);
				Trace.WriteLine (res.ToString());
				Console.ReadLine ();
				return String.Empty;
			}
			var m = ast.Matches ["eol", true].Matches;
			Dump(ast, res); 
			Cut(ast, res, SectionType, SectionName);
			return res.ToString ();
		}

		public bool IsSubnodeToRemove(Eto.Parse.Match node, string SectionType, string SectionName)
		{
			var type = node["sectiontitle",true].StringValue.ToLowerInvariant();
			var name = node["subsectionheader",true].StringValue.ToLowerInvariant();
			if (type.CompareTo (SectionType.ToLowerInvariant ()) == 0
			    && name.CompareTo (SectionType.ToLowerInvariant ()) == 0)
			{
				return true;
			}
			return false;
		}
	}
}

