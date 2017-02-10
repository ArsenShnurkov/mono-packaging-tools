using System;
using System.Collections.Generic;
using System.IO;
using MetaSpecTools;

namespace mptcore
{

	public class SolutionTools
	{

		public static void RemoveProject(string solutionFullPath, string projectName)
		{
			FileInfo f = new FileInfo(solutionFullPath);
			Console.WriteLine($"Removing {projectName} from {solutionFullPath}");
			var listToRemove = new List<Project>();
			var sln = SolutionFile.FromFile(solutionFullPath, new Repository(f.DirectoryName));
			var projList = sln.Projects;
			foreach (Project p in projList)
			{
				if (string.Compare(p.ProjectName, projectName) == 0)
				{
					listToRemove.Add(p);
				}
			}
			foreach (Project r in listToRemove)
			{
				sln.Projects.Remove(r);
			}
			sln.Save(); // or .SaveAs("NewName" + SolutionFile.DefaultExtension);
		}

		public static void ProcessReferences(string solutionFullPath)
		{
			FileInfo f = new FileInfo(solutionFullPath);
			var context = new Repository(f.DirectoryName);
			var sln = SolutionFile.FromFile(solutionFullPath, context);
			var projList = sln.Projects;
			foreach (Project p in projList)
			{
				string filename = p.FullPath;
				Console.WriteLine(filename);
				//string guid = CSharpLibraryProject.GetGuidFromFile(filename);
				var cslib = (CSharpLibraryProject)context.LoadProject(filename);
				foreach (var configuration in cslib.Configurations)
				{
					Console.WriteLine($"{configuration.Name} -> { configuration.GetAssemblyName()}");
				}
			}
		}
	}

}