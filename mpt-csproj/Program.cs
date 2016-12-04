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
			var list_inputs = (string)null;
			var list_outputs = (string)null;
			var no_recurse = (string)null;
			var as_unified_patch = (string)null;
			string base_dir = null;
			string dir = null;
			string reference_name = null;
			var optionSet = new OptionSet()
			{
				/* General options */
				// output usage summary and exit
				{ "h|?|help", v => ShowHelp() },
				// give more explainations during work
				{ "verbose", b => verbose = b },

				/* searching options */
				// options to create list of projects for processing
				{ "in:", str => AddProjectFile(str) },
				{ "dir=", str => dir = str },
				{ "no-recurse", b => no_recurse = b },

				/* readonly options */
				{ "basedir=", str => base_dir = str },
				// lists inputs of .csproj files 
				{ "list-inputs", b => list_inputs = b },
				// lists outputs of .csproj files
				{ "list-outputs", b => list_outputs = b },

				/* transformation options */
				{ "as-unified-patch=", b => as_unified_patch = b },
				// remove elements from .csproj files
				{ "remove-warnings-as-errors", b => remove_warnings_as_errors = b },
				// replace reference(s) in .csproj files
				{ "replace-reference=", str => reference_name = str },
			};
			optionSet.Parse(args);
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
			if (listOfCsproj.Count == 0)
			{
				return (int)ExitCode.NoInputFileSpecified;
			}
			if (list_outputs != null)
			{
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
			if (list_inputs != null)
			{
				foreach (var csproj_file in listOfCsproj)
				{
					string csproj_base_dir;
					if (String.IsNullOrWhiteSpace(base_dir) == false)
					{
						csproj_base_dir = (new DirectoryInfo(base_dir)).FullName;
					}
					else
					{
						csproj_base_dir = (new FileInfo(csproj_file)).Directory.FullName;
					}
					ProjectTools.DumpFiles(csproj_file, csproj_base_dir);
				}
			}
			if (remove_warnings_as_errors != null)
			{
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
			if (reference_name != null)
			{
				Console.WriteLine($"Removing reference {reference_name}");
				foreach (var csproj_file in listOfCsproj)
				{
					ProjectTools.ReplaceReference(csproj_file, reference_name);
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
