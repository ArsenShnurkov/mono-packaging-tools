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
			var references = (string)null; //boolean
			var p = new OptionSet()
			{
				{ "h|?|help", v => ShowHelp() },
				{ "sln-file=", str => sln_file = str },
				{ "remove-proj=", str => remove_proj = str },
				{ "references", b => references = b },
			};
			p.Parse(args);
			if (String.IsNullOrWhiteSpace (sln_file) == true)
			{
				Console.WriteLine("--sln-file=some.sln is not given");
				return (int)ExitCode.NoSolutionFileSpecified;
			}
			try
			{
				if (String.IsNullOrWhiteSpace(references) == false)
				{
					SolutionTools.ProcessReferences(sln_file);
				}
				if (String.IsNullOrWhiteSpace(remove_proj) == false)
				{
					SolutionTools.RemoveProject(sln_file, remove_proj);
				}
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
