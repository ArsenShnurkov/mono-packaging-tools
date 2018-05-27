using System;
using System.IO;
using System.Collections.Generic;

namespace mptcore
{
	public class GacTools
	{
		SortedList<string, string> assemblies = new SortedList<string, string>();

		public GacTools()
		{
			BuildListOfAssemblies();
		}

		/// http://stackoverflow.com/questions/1599575/enumerating-assemblies-in-gac
		/// http://stackoverflow.com/a/1557981/6017919
		void BuildListOfAssemblies()
		{
			string path = "/usr/lib/mono/gac";

			if (Directory.Exists(path))
			{
				string[] assemblyFolders = Directory.GetDirectories(path);
				foreach (string assemblyFolder in assemblyFolders)
				{
					assemblies.Add(assemblyFolder.Substring(path.Length + 1).ToLower(), assemblyFolder);
				}
			}
		}
		public bool IsAssemblyInGAC(string assemblyName)
		{
			if (assemblies.ContainsKey(assemblyName.ToLower()))
			{
				return true;
			}
			return false;
		}
	}
}

