using System;
using System.Collections.Generic;
using CWDev.SLNTools.Core;

public class SolutionTools
{
	public static void RemoveProject(string solutionFullPath, string projectName)
	{
		Console.WriteLine($"Removing {projectName} from {solutionFullPath}");
		var listToRemove = new List<Project>();
		var sln = SolutionFile.FromFile(solutionFullPath);
		var projList = sln.Projects;
		foreach (var p in projList)
		{
			if (string.Compare(p.ProjectName, projectName) == 0)
			{
				listToRemove.Add(p);
			}
		}
		foreach (var r in listToRemove)
		{
			sln.Projects.Remove(r);
		}
		sln.Save(); // or .SaveAs("NewName.sln");
	}

	public static void ProcessReferences(string solutionFullPath)
	{
		var sln = SolutionFile.FromFile(solutionFullPath);
		var projList = sln.Projects;
		foreach (var p in projList)
		{
			string fileNath = p.FullPath;
			Console.WriteLine(fileNath);
			var cslib = new CSharpLibraryProject(fileNath);
			foreach (var configuration in cslib.Configurations)
			{
				Console.WriteLine($"{configuration.Name} -> { configuration.GetAssemblyName()}");
			}
		}
	}
}
