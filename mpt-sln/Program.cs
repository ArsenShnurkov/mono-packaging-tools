using System;
using Mono.Options;

namespace mptsln
{
	class MptSln
	{
		enum ExitCode : int {
			Success = 0,
			NoSolutionFileSpecified = 1,
			NoProjNameSpecified = 2,
			ProcessingError = 3
		}

		public static int Main (string[] args)
		{
			string sln_file = null;
			string remove_proj = null;
			var p = new OptionSet ()
			{
				{ "h|?|help", v => ShowHelp() },
				{ "sln-file=", str => sln_file = str },
				{ "remove-proj=", str => remove_proj = str },
			};
			p.Parse(args);
			if (String.IsNullOrWhiteSpace (sln_file))
			{
				Console.WriteLine("--sln-file is not set");
				return (int)ExitCode.NoSolutionFileSpecified;
			}
			if (String.IsNullOrWhiteSpace (remove_proj))
			{
				Console.WriteLine("--remove-proj name is not given");
				return (int)ExitCode.NoProjNameSpecified;
			}
			try
			{
				SolutionTools.RemoveProject(sln_file, remove_proj);
				return (int)ExitCode.Success;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return (int)ExitCode.ProcessingError;
			}
		}
		public static void ShowHelp()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine("\tmpt-sln --help");
			Console.WriteLine("\tmpt-sln --sln-file <filename> --remove-proj <projname>");
		}
	}
}
