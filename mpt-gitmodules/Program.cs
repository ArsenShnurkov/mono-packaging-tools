using System;

namespace mptgitmodules
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var pars = new Parser ();
			string res = pars.Parse (
				"/var/calculate/remote/distfiles/egit-src/monodevelop.git/.gitmodules",
				"submodule",
				"main/external/nrefactory");
			Console.WriteLine (res);
		}
	}
}
