using System;
using System.Collections.Generic;
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
			var verbose = (string)null;
			var remove_warnings_as_errors = (string)null;
			var dump_project_files = (string)null;
			var list = (string)null;
			var no_recurse = (string)null;
			var as_unified_patch = (string)null;
			string base_dir = null;
			string dir = null;
			var optionSet = new OptionSet()
			{
				{ "verbose", b => verbose = b },
				{ "h|?|help", v => ShowHelp() },
				{ "list", b => list = b },
				{ "dump-project-files", b => dump_project_files = b },
				{ "in:", str => AddProjectFile(str) },
				{ "basedir=", str => base_dir = str },
				{ "remove-warnings-as-errors", b => remove_warnings_as_errors = b },
				{ "no-recurse", b => no_recurse = b },
				{ "dir=", str => dir = str },
				{ "as-unified-patch=", b => as_unified_patch = b },
			};
			optionSet.Parse(args);
			if (list != null)
			{
				if (String.IsNullOrWhiteSpace(dir) == false)
				{
					dir = (new DirectoryInfo(dir)).FullName;
				}
				else
				{
					dir = Directory.GetCurrentDirectory();
				}
				// find all *.csproj in directory and add to listOfCsproj
				SearchOption searchOption = (no_recurse != null) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
				var fileInfos = new DirectoryInfo(dir).GetFiles("*.csproj", searchOption);
				foreach (var file in fileInfos)
				{
					AddProjectFile(file.FullName);
				}
				foreach (var csproj_file in listOfCsproj)
				{
					Console.WriteLine($"{csproj_file}");
					Dictionary<string, string> outputs = ProjectTools.GetMainOutputs(csproj_file);
					foreach (var output in outputs)
					{
						Console.WriteLine($"{output.Key} -> ${output.Value}");
					}
				}
			}
			if (dump_project_files != null)
			{
				if (String.IsNullOrWhiteSpace(base_dir) == false)
				{
					base_dir = (new DirectoryInfo(base_dir)).FullName;
				}
				else
				{
					if (listOfCsproj.Count == 0)
					{
						return (int)ExitCode.NoInputFileSpecified;
					}
					var csproj_file = listOfCsproj[0]; // list is not emty because this is checked above
					base_dir = (new FileInfo(csproj_file)).Directory.FullName;
				}
				foreach (var csproj_file in listOfCsproj)
				{
					ProjectTools.DumpFiles(csproj_file, base_dir);
				}
			}
			if (remove_warnings_as_errors != null)
			{
				if (String.IsNullOrWhiteSpace(dir) == false)
				{
					dir = (new DirectoryInfo(dir)).FullName;
					// find all *.csproj in directory and add to listOfCsproj
					SearchOption searchOption = (no_recurse != null) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
					var fileInfos = new DirectoryInfo(dir).GetFiles("*.csproj", searchOption);
					foreach (var file in fileInfos)
					{
						AddProjectFile(file.FullName);
					}
				}
				if (listOfCsproj.Count == 0)
				{
					return (int)ExitCode.NoInputFileSpecified;
				}
				foreach (var csproj_file in listOfCsproj)
				{
					if (verbose != null)
					{
						string output_or_inplace = (as_unified_patch == null) ? ", inplace conversion" : String.Format(" >> {0}", as_unified_patch);
						Console.WriteLine($"{csproj_file}{output_or_inplace}");
					}
					ProjectTools.RemoveWarningsAsErrors(csproj_file, as_unified_patch);
				}
			}
			return (int)ExitCode.Success;
		}
		public static void ShowHelp()
		{
			var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine($"mpt-cspoj.exe, version {version}");
			Console.WriteLine("Usage: ");
			Console.WriteLine("\tmpt-csproj --dump-project-files --in=work/src/my.csproj");
			Console.WriteLine("\tmpt-csproj --dump-project-files --in=work/src/my.csproj --basedir=work");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --in=work/src/my.csproj");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --in=work/src/my.csproj --as-unified-patch my.patch");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --dir=work");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --dir=work --recursive");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --dir=work --recursive --as-unified-patch my.patch");
		}
		static List<string> listOfCsproj = new List<string>();
		static void AddProjectFile(string filename)
		{
			if (String.IsNullOrWhiteSpace(filename))
			{
				throw new ArgumentException("Filename is null or empty", nameof(filename));
			}
			var csproj_file = (new FileInfo(filename)).FullName;
			if (File.Exists(csproj_file) == false)
			{
				throw new FileNotFoundException(csproj_file);
			}
			listOfCsproj.Add(csproj_file);
		}
	}
}
