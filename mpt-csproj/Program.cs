using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using BuildAutomation;

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
			HelpOrVersion = 5,
			Exception = 6,
		}
		enum Action : int
		{
			Change = 0,
			Create = 1,
			DeleteReference = 2,
			DeleteProjectReference = 3,
		}
		public static int Main(string[] args)
		{
			try
			{
				return (int)MainProcessing(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				ShowVersion();
				for (int i = 0; i < args.Length; i++)
				{
					if (i > 0)
					{
						Console.Write(" ");
					}
					Console.Write(args[i]);
				}
				Console.WriteLine();
			}
			return (int)ExitCode.Exception;
		}
		static ExitCode MainProcessing(string[] args)
		{
			var verbose = (string)null;
			var remove_warnings_as_errors = (string)null;
			var remove_signing = (string)null;
			var package_hintpath = (string)null;
			var list_refs = (string)null;
			var list_projrefs = (string)null;
			var list_inputs = (string)null;
			var list_outputs = (string)null;
			var no_recurse = (string)null;
			var as_unified_patch = (string)null;
			string base_dir = null;
			string dir = null;
			string reference_name = null;
			Action referenceAction = Action.Change;
			string import_name = null;
			string version_string = null;
			string friend_assembly_name = null;
			string AssemblyKeyContainerName = null;
			string AssemblyOriginatorKeyFile = null;

			var optionSet = new OptionSet()
			{
				/* General options */
				// output usage summary and exit
				{ "h|?|help", v => { ShowHelp(); System.Environment.Exit((int)ExitCode.HelpOrVersion); } },
				{ "v|V|version", v => { ShowVersion(); System.Environment.Exit((int)ExitCode.HelpOrVersion); } },
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
				// Prepend <Package> xml element with <HintPath> xml element
				{ "package-hintpath=", str => package_hintpath = str },
				// remove reference(s) from .csproj files
				{ "remove-projectreference=", str => { reference_name = str; referenceAction = Action.DeleteProjectReference; } },
				// replace reference(s) in .csproj files
				{ "remove-reference=", str => { reference_name = str; referenceAction = Action.DeleteReference; } },
				// replace reference(s) in .csproj files
				{ "replace-reference=", str => { reference_name = str; referenceAction = Action.Change; } },
				// insert reference(s) in .csproj files
				{ "inject-reference=", str => { reference_name = str; referenceAction = Action.Create; } },
				// insert project import into .csproj files
				{ "inject-import=", str => { import_name = str; } },
				// insert task for versioning into .csproj files
				{ "inject-versioning=", str => { version_string = str; } },
				// insert project import into .csproj files
				{ "inject-InternalsVisibleTo=", str => { friend_assembly_name = str; } },
				// insert project import into .csproj files
				{ "AssemblyKeyContainerName=", str => { AssemblyKeyContainerName = str; } },
				{ "AssemblyOriginatorKeyFile=", str => { AssemblyOriginatorKeyFile = str; } },
			};

			var listOfUnparsedParameters = optionSet.Parse(args);
			foreach (var strUnparsedParameter in listOfUnparsedParameters)
			{
				bool bFile = false;
				if (File.Exists(strUnparsedParameter))
				{
					AddProjectFile(strUnparsedParameter);
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
					return ExitCode.WrongUsage;
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
				return ExitCode.NoInputFileSpecified;
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
				Console.WriteLine($"Removing warnings as errors");
				foreach (var csproj_file in listOfCsproj)
				{
					Console.WriteLine($"from file {csproj_file}");
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
				Console.WriteLine($"Removing signing");
				foreach (var csproj_file in listOfCsproj)
				{
					Console.WriteLine($"from file {csproj_file}");
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
				Console.WriteLine($"Processing reference {reference_name}");
				foreach (var csproj_file in listOfCsproj)
				{
					Console.WriteLine($"in file {csproj_file}");
					if (referenceAction == Action.DeleteReference)
					{
						ProjectTools.RemoveReference(csproj_file, reference_name);
					}
					else if (referenceAction == Action.DeleteProjectReference)
					{
						ProjectTools.RemoveProjectReference(csproj_file, reference_name);
					}
					else
					{
						bool bForceReferenceAppending = referenceAction == Action.Create;
						if (package_hintpath != null)
						{
							Console.WriteLine ($"package_hintpath is {package_hintpath}");
							ProjectTools.CreateReferenceHintPath(csproj_file, reference_name, package_hintpath, bForceReferenceAppending);
						}
						else
						{
							ProjectTools.ReplaceReference(csproj_file, reference_name, bForceReferenceAppending);
						}

					}
				}
			}
			else
			{
				if (package_hintpath != null)
				{
					Console.WriteLine($"Reference is not specified");
				}
			}
			if (import_name != null)
			{
				Console.WriteLine($"Injecting import of project {import_name}");
				foreach (var csproj_file in listOfCsproj)
				{
					Console.WriteLine($"into file {csproj_file}");
					using (CSharpLibraryProject file = new CSharpLibraryProject(csproj_file))
					{
						file.InjectProjectImport(import_name);
					}
				}
			}
			if (version_string != null)
			{
				Console.WriteLine($"Injecting version property {version_string}");
				foreach (var csproj_file in listOfCsproj)
				{
					Console.WriteLine($"into file {csproj_file}");
					using (CSharpLibraryProject file = new CSharpLibraryProject(csproj_file))
					{
						file.InjectVersioning(version_string);
					}
				}
			}
			if (friend_assembly_name != null)
			{
				if (String.IsNullOrWhiteSpace(AssemblyOriginatorKeyFile) && String.IsNullOrWhiteSpace(AssemblyKeyContainerName))
				{
					return ExitCode.WrongUsage;
				}
				Console.WriteLine($"Injecting version property {version_string}");
				foreach (var csproj_file in listOfCsproj)
				{
					Console.WriteLine($"into file {csproj_file}");
					using (CSharpLibraryProject file = new CSharpLibraryProject(csproj_file))
					{
						// null is ok - http://stackoverflow.com/questions/637308/why-is-adding-null-to-a-string-legal
						string publicKey = null;
						if (String.IsNullOrEmpty(AssemblyOriginatorKeyFile) == false)
						{
							publicKey = PublicKeyUtils.GetPublicKeyStringFromFilename(AssemblyOriginatorKeyFile);
						}
						if (String.IsNullOrEmpty(AssemblyKeyContainerName) == false )
						{
							publicKey = PublicKeyUtils.GetPublicKeyStringFromContainer(AssemblyKeyContainerName);
						}
						file.InjectInternalsVisibleTo(friend_assembly_name, publicKey);
					}
				}
			}
			return ExitCode.Success;
		}
		public static void ShowVersion()
		{
			var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine($"mpt-cspoj.exe, version {version}");
		}
		public static void ShowHelp()
		{
			ShowVersion();
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
			Console.WriteLine("\tmpt-csproj --remove-reference=\"MyDll\"");
			Console.WriteLine("\tmpt-csproj --replace-reference=\"MyDll,Version,Culture,PubKeyToken\"");
			Console.WriteLine("\t\treplaces the reference for MyDll of given version");
			Console.WriteLine("\tmpt-csproj --inject-reference=\"MyDll,Version,Culture,PubKeyToken\"");
			Console.WriteLine("\tmpt-csproj --inject-import='$(MSBuildToolsPath)\\MSBuild.Community.Tasks.Targets'");
			Console.WriteLine("\t\tinserts new reference for MyDll of given version, or replaces the old one");
			Console.WriteLine("\tmpt-csproj --inject - versioning=BuildVersion");
			Console.WriteLine("\t\tinserts property with given name $(BuildVersion), and default value 1.0.0.0");
			Console.WriteLine("\tmpt-csproj --inject-InternalsVisibleTo=mytest.dll --AssemblyKeyContainerName=mono");
			Console.WriteLine("\tmpt-csproj --inject-InternalsVisibleTo=mytest.dll --AssemblyOriginatorKeyFile=mono.snk");
			Console.WriteLine("\t\tinserts InternalsVisibleToAttribute");
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
