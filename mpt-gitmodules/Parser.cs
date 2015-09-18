using System;
using Eto.Parse;
using Eto.Parse.Grammars;
using System.Diagnostics;
using System.Text;
using System.Globalization;

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

		public void DoProcessing(string textToParse, string[] args)
		{
			var ast = myGrammar.Match (textToParse);
			if (ast.Success == false)
			{
				var loc = ast.GetTextLocation(ast.ErrorIndex);
				var msg = string.Format ("Line {0},{1}: {2}", loc.line, loc.position, ast.ErrorMessage);
				Console.WriteLine (msg);
				Environment.Exit (-1);
			}
			var subsections = ast.FindUniq("subsection", true);

			StringBuilder result = new StringBuilder (textToParse);
			int excluded_total = 0;
			foreach (var sec in subsections)
 			{
				var start = sec.Index;
				var end = start + sec.Length;
				var name = sec.Matches["subsection_title", true].StringValue;
				var startLoc = ast.GetTextLocation (start);
				var endLoc = ast.GetTextLocation (end);
				string msg = string.Format("{0} - from [{1},{2}] to [{3},{4}]", name,
				startLoc.line + 1, startLoc.position + 1, endLoc.line + 1, endLoc.position + 1);
				Trace.WriteLine (msg);
				bool cut = false;
				Trace.WriteLine ("testing name : " + name);
				foreach (var exclude in args)
				{
					Trace.WriteLine (exclude);
					// string strA
					// string strB
					// bool ignoreCase
					// CultureInfo culture
					if (string.Compare (name, exclude, true, CultureInfo.InvariantCulture) == 0)
					{
						cut = true;
						break;
					}
				}
				if (cut) {
					Trace.WriteLine ("match found");
					int pos = sec.Index - excluded_total;
					result.Remove (pos, sec.Length);
					// microhack for removing spaces after section
					// it is hack because it don't use parsing markup
					int spaces_count = 0;
					while (char.IsWhiteSpace(result[pos + spaces_count]))
					{
						spaces_count++;
					}
					result.Remove (pos, spaces_count);
					excluded_total += spaces_count;
					excluded_total += sec.Length;
				} else {
					Trace.WriteLine ("match not found");
				}
			}
			Console.WriteLine (result.ToString ());
		}
	}
}

