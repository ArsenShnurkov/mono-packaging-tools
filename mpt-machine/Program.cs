using System;
﻿using Mono.Options;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace mptmachine
{
	class MainClass
	{
		enum ExitCode : int {
			Success = 0,
			NoInputFileSpecified = 1,
			NoDataProviderNameSpecified = 2,
			NothingToDo = 3,
		}

		public static int Main (string[] args)
		{
			int result = 0; // success
			string in_file_name = null;
			string out_file_name = null;
			string data_provider_name = null;
			string invariant_name = null;
			string dll_name = null;
			var p = new OptionSet ()
			{
				{ "h|?|help", v => ShowHelp() },
				{ "p|print_full_file_name_of_machine_config", v => Console.WriteLine(GetConfigFilename()) },
				{ "in=", str => in_file_name = str },
				{ "out=", str => out_file_name = str },
				{ "name=", str => data_provider_name = str },
				{ "invariant=", str => invariant_name = str },
				{ "dll=", str => dll_name = str },
			};
			p.Parse(args);
			if (String.IsNullOrWhiteSpace (in_file_name))
			{
				return (int)ExitCode.NoInputFileSpecified;
			}
			if (String.IsNullOrWhiteSpace (out_file_name))
			{
				out_file_name = in_file_name;
			}
			if (String.IsNullOrWhiteSpace (invariant_name) && String.IsNullOrWhiteSpace (dll_name))
			{
				return (int)ExitCode.NothingToDo;
			}
			if (String.IsNullOrWhiteSpace (data_provider_name)) {
				Console.WriteLine ("name of data provider should be defined with --name");
				return (int)ExitCode.NoDataProviderNameSpecified;
			}
			var stream = new MemoryStream(File.ReadAllBytes(in_file_name));
			var document = XDocument.Load(stream);
			XElement subsectionElement = document.XPathSelectElement("/configuration/system.data/DbProviderFactories");
			if (!String.IsNullOrWhiteSpace (invariant_name))
			{
				// remove specified invariant
				var listAdd = document.XPathSelectElements("/configuration/system.data/DbProviderFactories/add[@invariant='" + invariant_name + "']");
				foreach (var item in listAdd)
				{
					Console.WriteLine ("-" + item.ToString ());
					item.Remove ();
				}
				var listRemove = document.XPathSelectElements("/configuration/system.data/DbProviderFactories/remove[@invariant='" + invariant_name + "']");
				foreach (var item in listRemove)
				{
					Console.WriteLine ("-" + item.ToString ());
					item.Remove ();
				}
			}
			// debug output of list
			IEnumerable<XElement> allChildElements = subsectionElement.Elements();
			foreach (var el in allChildElements)
			{
				Console.WriteLine ("=" + el.ToString ());
			}
			if (!String.IsNullOrWhiteSpace (dll_name))
			{
				string name_of_type_to_load = "???type";
				string name_of_invariant = "???invariant";
				var dll = Assembly.LoadFrom(dll_name);
				foreach (Type type in dll.GetTypes())
				{
					if (string.Compare (type.BaseType.Name, "DbProviderFactory") == 0) {
						name_of_type_to_load = type.FullName + ", " + type.Assembly.FullName;
						name_of_invariant = type.Namespace;
					}
				}
				var descriptionAttribute = dll
					.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
					.OfType<AssemblyDescriptionAttribute>()
					.FirstOrDefault();
				// add data from dll
				var n = new XElement ("add",
					new XAttribute ("name", data_provider_name),
					new XAttribute ("invariant", name_of_invariant),
					new XAttribute ("description", descriptionAttribute.Description),
					new XAttribute ("type", name_of_type_to_load)
		        );
				Console.WriteLine ("+" + n.ToString ());
				subsectionElement.Add(n);
			}
			// save to resulting file
			document.Save(out_file_name);
			return result;
		}
		public static void ShowHelp()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine("\tmpt-machine --help");
			Console.WriteLine("\tmpt-machine --print_full_file_name_of_machine_config");
			Console.WriteLine("\tmpt-machine --in=/etc/mono/4.5/machine.config --out=/etc/mono/4.5/._cfg0000_machine.config --name=\"Npgsql Data Provider\" --invariant=Npgsql --dll=filename.dll");
		}
		public static string GetConfigFilename()
		{
			string configFileName = RuntimeEnvironment.SystemConfigurationFile;
			return configFileName;
		}
	}
}
