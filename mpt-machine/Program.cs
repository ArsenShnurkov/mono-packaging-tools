using System;
﻿using Mono.Options;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;

namespace mptmachine
{
	class MainClass
	{
		enum ExitCode : int {
			Success = 0,
			NoInputFileSpecified = 1,
			NothingToDo = 2,
		}

		public static int Main (string[] args)
		{
			int result = 0; // success
			string in_file_name = null;
			string out_file_name = null;
			string invariant_name = null;
			string dll_name = null;
			var p = new OptionSet ()
			{
				{ "h|?|help", v => ShowHelp() },
				{ "p|print_full_file_name_of_machine_config", v => Console.WriteLine(GetConfigFilename()) },
				{ "in=", str => in_file_name = str },
				{ "out=", str => out_file_name = str },
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
			var stream = new MemoryStream(File.ReadAllBytes(in_file_name));
			var document = XDocument.Load(stream);
			XElement subsectionElement = document.XPathSelectElement("/configuration/system.data/DbProviderFactories");
			if (!String.IsNullOrWhiteSpace (invariant_name))
			{
				// remove specified invariant
				var list = document.XPathSelectElements("/configuration/system.data/DbProviderFactories/add[@invariant='" + invariant_name + "']");
				foreach (var item in list)
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
				// add data from dll
				var n = new XElement ("add",
					        new XAttribute ("name", "???name"),
					        new XAttribute ("invariant", "???invariant"),
					        new XAttribute ("description", "???description"),
					        new XAttribute ("type", "???type")
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
			Console.WriteLine("\tmpt-machine --in=/etc/mono/4.5/machine.config --out=/etc/mono/4.5/._cfg0000_machine.config --invariant=Npgsql");
			Console.WriteLine("\tmpt-machine --in=/etc/mono/4.5/machine.config --out=/etc/mono/4.5/._cfg0000_machine.config --dll=filename.dll");
		}
		public static string GetConfigFilename()
		{
			string configFileName = RuntimeEnvironment.SystemConfigurationFile;
			return configFileName;
		}
	}
}
