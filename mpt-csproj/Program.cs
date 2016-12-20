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
			WrongUsage = 1,
			NoInputFileSpecified = 2,
			NoDataProviderNameSpecified = 3,
			NothingToDo = 4,
		}
		public static int Main (string[] args)
		{
			var verbose = (string)null;
			var remove_warnings_as_errors = (string)null;
			var remove_signing = (string)null;
			var list_refs = (string)null;
			var list_projrefs = (string)null;
			var list_inputs = (string)null;
			var list_outputs = (string)null;
			var no_recurse = (string)null;
			var as_unified_patch = (string)null;
			string base_dir = null;
			string dir = null;
			string reference_name = null;
			bool bForceReferenceAppending = false;

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
				// lists references of .csproj files 
				{ "list-refs", b => list_refs = b },
				// lists references of .csproj files 
				{ "list-projrefs", b => list_projrefs = b },
				// lists inputs of .csproj files 
				{ "list-inputs", b => list_inputs = b },
				// lists outputs of .csproj files
				{ "list-outputs", b => list_outputs = b },

				/* transformation options */
				{ "as-unified-patch=", b => as_unified_patch = b },
				// remove elements from .csproj files
				{ "remove-warnings-as-errors", b => remove_warnings_as_errors = b },
				{ "remove-signing", b => remove_signing = b },
				// replace reference(s) in .csproj files
				{ "replace-reference=", str => reference_name = str },
				// replace reference(s) in .csproj files
				{ "inject-reference=", str => { reference_name = str; bForceReferenceAppending = true; } },
			};

			var listOfUnparsedParameters = optionSet.Parse(args);
			foreach (var strUnparsedParameter in listOfUnparsedParameters)
			{
				bool bFile = false;
				if (File.Exists(strUnparsedParameter))
				{
					var name = new FileInfo(strUnparsedParameter).FullName;
					listOfCsproj.Add(name);
					bFile = true;
				}
				bool bDir = false;
				if (Directory.Exists(strUnparsedParameter))
				{
					// find all *.csproj in directory and add to listOfCsproj
					SearchOption searchOption = (no_recurse != null) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
					var fileInfos = new DirectoryInfo(strUnparsedParameter).GetFiles("*.csproj", searchOption);
					foreach (var file in fileInfos)
					{
						AddProjectFile(file.FullName);
					}
					bDir = true;
				}
				if (!bFile && !bDir)
				{
					Console.WriteLine($"unknown parameter {strUnparsedParameter}");
					Environment.Exit((int)ExitCode.WrongUsage);
				}
			}
			if (String.IsNullOrWhiteSpace(dir) == false)
			{
				dir = (new DirectoryInfo(dir)).FullName;
			}
			else
			{
				dir = Directory.GetCurrentDirectory();
			}
			if (listOfCsproj.Count == 0)
			{
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
			if (list_refs != null)
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
					ProjectTools.DumpRefs(csproj_file, csproj_base_dir);
				}
			}
			if (list_projrefs != null)
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
					ProjectTools.DumpProjRefs(csproj_file, csproj_base_dir);
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
			if (remove_signing != null)
			{
				foreach (var csproj_file in listOfCsproj)
				{
					if (verbose != null)
					{
						string output_or_inplace = (as_unified_patch == null) ? ", inplace conversion" : String.Format(" >> {0}", as_unified_patch);
						Console.WriteLine($"{csproj_file}{output_or_inplace}");
					}
					ProjectTools.RemoveSigning(csproj_file, as_unified_patch);
				}
			}
			if (reference_name != null)
			{
				Console.WriteLine($"Replacing reference {reference_name}");
				foreach (var csproj_file in listOfCsproj)
				{
					ProjectTools.ReplaceReference(csproj_file, reference_name, bForceReferenceAppending);
				}
			}
			return (int)ExitCode.Success;
		}
		public static void ShowHelp()
		{
			var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine($"mpt-cspoj.exe, version {version}");
			Console.WriteLine("Usage: ");
			Console.WriteLine("\tmpt-csproj --list-refs");
			Console.WriteLine("\t\tPrints references");
			Console.WriteLine("\tmpt-csproj --list-projrefs");
			Console.WriteLine("\t\tPrints project references");
			Console.WriteLine("\tmpt-csproj --list-inputs");
			Console.WriteLine("\t\tPrints all input files");
			Console.WriteLine("\tmpt-csproj --list-outputs");
			Console.WriteLine("\t\tPrints resulting files");
			Console.WriteLine("\tmpt-csproj --remove-signing");
			Console.WriteLine("\t\tRemoves xml elements related to signing");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --dir=work");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --dir=work --recursive");
			Console.WriteLine("\tmpt-csproj --remove-warnings-as-errors --dir=work --recursive --as-unified-patch my.patch");
			Console.WriteLine("\t\tremoves xml element <WarningsAsErrors>true</WarningsAsErrors>");
			Console.WriteLine("\tmpt-csproj --replace-reference=\"MyDll,Version,Culture,PubKeyToken\"");
			Console.WriteLine("\t\treplaces the reference for MyDll of given version");
			Console.WriteLine("\tmpt-csproj --inject-reference=\"MyDll,Version,Culture,PubKeyToken\"");
			Console.WriteLine("\t\tinserts new reference for MyDll of given version, or replaces the old one");
		}
		static List<string> listOfCsproj = new List<string>();
		static void AddProjectFile(string filename)
		{
			if (String.IsNullOrWhiteSpace(filename))
			{
				throw new ArgumentException("Filename is null or empty", nameof(filename));
			}
			var csproj_file = (new FileInfo(filename)).FullName;
			if (listOfCsproj.Contains(csproj_file) == false)
			{
				if (File.Exists(csproj_file) == false)
				{
					throw new FileNotFoundException(csproj_file);
				}
				listOfCsproj.Add(csproj_file);
			}
		}
	}
}
