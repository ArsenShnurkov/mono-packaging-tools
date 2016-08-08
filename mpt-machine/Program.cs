using System;
﻿using Mono.Options;

namespace mptmachine
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			int result = 0; // success
			var p = new OptionSet ()
			{
				{ "h|?|help", v => ShowHelp() },
				{ "i=", str => result = InsertFileToMachineConfig(str) },
				{ "r=", str => result = RemoveInvariantFromMachineConfig(str) },
			};
			p.Parse(args);
			return result;
		}
		public static void ShowHelp()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine("\tmpt-machine -i filename.dll");
			Console.WriteLine("\tmpt-machine -d invariant");
			Console.WriteLine("\tmpt-machine -h");
		}
		public static int InsertFileToMachineConfig(string filename)
		{
			Console.WriteLine($"inserting {filename}");
			return 0;
		}
		public static int RemoveInvariantFromMachineConfig(string invariant)
		{
			Console.WriteLine($"removing {invariant}");
			return 0;
		}
	}
}
