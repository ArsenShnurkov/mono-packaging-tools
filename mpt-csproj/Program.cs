using System;
using System.IO;
using Mono.Options;

namespace mptcsproj
{
	class MainClass
	{
		enum ExitCode : int
		{
			Success = 0,
			NoInputFileSpecified = 1,
			NoDataProviderNameSpecified = 2,
			NothingToDo = 3,
		}		
		public static int Main (string[] args)
		{
			string csproj_file = null;
			string base_dir = null;
			var p = new OptionSet()
			{
				{ "h|?|help", v => ShowHelp() },
				{ "in=", str => csproj_file = str },
				{ "basedir=", str => base_dir = str },
			};
			p.Parse(args);
			if (String.IsNullOrWhiteSpace(csproj_file))
			{
				return (int)ExitCode.NoInputFileSpecified;
			}
			else
			{
				csproj_file = (new FileInfo(csproj_file)).FullName;
			}
			if (String.IsNullOrWhiteSpace(base_dir))
			{
				base_dir = (new FileInfo(csproj_file)).Directory.FullName;
			}
			else
			{
				base_dir = (new DirectoryInfo(base_dir)).FullName;
			}
			Console.WriteLine($"in={csproj_file}");
			Console.WriteLine($"basedir={base_dir}");
			ProjectTools.DumpFiles(csproj_file, base_dir);
			return (int)ExitCode.Success;
		}
		public static void ShowHelp()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine("\tmpt-csproj --in=work/src/my.csproj --basedir=work");
		}
	}
}
