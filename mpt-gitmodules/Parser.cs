

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
			grammar = LoadGrammar ("/var/calculate/remote/distfiles/egit-src/mono-packaging-tools.git/mpt-gitmodules/bnf/syntax1.ebnf");
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
			var ebnfGrammar = new Eto.Parse.Grammars.EbnfGrammar(EbnfStyle.Iso14977);
			var myGrammar = ebnfGrammar.Build(myEbnfString, "filecontent"); 
			return myGrammar;
		}
			
		public string Parse (string filename, string SectionType, string SectionName)
		{
			var fileContent = LoadFile (filename);
			var res = new StringBuilder (fileContent.Length);
			var ast = grammar.Match (fileContent);
			if (ast.Success == false) {
				Trace.WriteLine (ast.ErrorMessage);
				Console.ReadLine ();
				return String.Empty;
			}
			foreach (var subnode in ast.Matches)
			{
				if (IsSubnodeToRemove (subnode, SectionType, SectionName) == false)
				{
					res.Append (subnode.StringValue);
				}
			}
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

