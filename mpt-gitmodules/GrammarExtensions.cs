using System;
using Eto.Parse;

namespace mptgitmodules
{
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
	}
}

