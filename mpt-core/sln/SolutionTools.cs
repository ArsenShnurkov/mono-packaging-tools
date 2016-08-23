using System;
using CWDev.SLNTools.Core;

public class SolutionTools
{
	public static void RemoveProject(string solutionFullPath, string projectName)
	{
		Console.WriteLine($"Removing {projectName} from {solutionFullPath}");
		var sln = SolutionFile.FromFile(solutionFullPath);
		var projList = sln.Projects;
		var proj = sln.Projects.FindByFullName(projectName);
		sln.Projects.Remove(proj);
		sln.Save(); // or .SaveAs("NewName.sln");
	}
}
