using System;
using Eto.Parse;
using System.Diagnostics;
using System.Collections.Generic;

namespace mptgitmodules
{
	public struct TextLocation
	{
		public int line;
		public int position;
	}

	public static class GrammarExtensions
	{
		public static void CheckAbsentRules(this Grammar grammar)
		{
			foreach (var rule in grammar.Children()) {
				if (grammar.Contains (rule) == false) {
					throw new NotImplementedException ();
				}
			}
		}
		public static IEnumerable<Match> FindUniq(this Match ast, string id, bool deep = false)
		{
			var offset_to_match = new SortedDictionary<int, Match> ();
			var matches = ast.Find(id, deep);
			foreach (var m in matches)
			{
				int offset = m.Index;
				if (offset_to_match.ContainsKey(offset) == false)
				{
					offset_to_match.Add(offset, m);
				}
			}
			return offset_to_match.Values;
		}

		public static TextLocation GetTextLocation(this Match ast, int offset)
		{
			Trace.WriteLine (string.Format("Searching line for offset {0}", offset));
			// build an unique list sorted by position in source
			var linebreaks = ast.FindUniq("line_separator_raw", true);
			// find
			int line = 0;
			int lineoffset = 0;
			foreach (var m in linebreaks) {
				if (m.Index + m.Length > offset)
				{
					var res = new TextLocation ();
					res.line = line;
					res.position = offset - lineoffset;
					return res;
				}
				line++;
				lineoffset = m.Index + m.Length;
			}
			return new TextLocation{line=0, position=offset};
		}
	}
}

