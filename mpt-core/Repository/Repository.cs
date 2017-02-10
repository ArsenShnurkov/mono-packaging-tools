using System;
using System.Collections.Generic;
using System.IO;
using MetaSpecTools;

namespace mptcore
{
	public class Repository : ISolutionContext, IProjectContext
	{
		protected string directory;
		protected List<Upstream> upstreams;
		protected SolutionList solutions;
		protected List<CSharpLibraryProject> projects;
		public string FullPath { get { return directory; } set { SetDirectory(value); } }
		public IEnumerable<Upstream> Upstreams { get { return upstreams; } }
		public SolutionList Solutions { get { return solutions; } }
		public IEnumerable<CSharpLibraryProject> Projects { get { return projects; } }

		IEnumerable<Project> IProjectContext.Projects
		{
			get
			{
				List<Project> res = projects.ConvertAll(item => (Project)item); ;
				return res;
			}
		}

		IEnumerable<SolutionFile> ISolutionContext.Solutions
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IProjectContext ProjectContext
		{
			get
			{
				return this;
			}
		}

		public Repository()
		{
			InitFields();
		}

		public Repository(string dir)
		{
			InitFields();
			SetDirectory(dir);
		}

		protected void InitFields()
		{
			solutions = new SolutionList();
			projects = new List<CSharpLibraryProject>();
			upstreams = new List<Upstream>();
		}

		protected void SetDirectory(string dir)
		{
			this.directory = dir;

			// populate list of upstreams
			upstreams.Clear();
			//ScanUpstreams(dir);

			// Scan for solutions and projects
			solutions.Clear();
			projects.Clear();
			ScanForProjects(dir);
			ScanForSolutions(dir);
		}

		void ScanForProjects(string dir)
		{
			SearchOption searchOption = SearchOption.AllDirectories;
			var fileInfos = new DirectoryInfo(dir).GetFiles("*.csproj", searchOption);
			foreach (var file in fileInfos)
			{
				string filename = file.FullName;
				//string guid = CSharpLibraryProject.GetGuidFromFile(filename);
				CSharpLibraryProject item = (CSharpLibraryProject)this.LoadProject(filename);
				this.projects.Add(item);
			}
		}

		void ScanForSolutions(string dir)
		{
			SearchOption searchOption = SearchOption.AllDirectories;
			var fileInfos = new DirectoryInfo(dir).GetFiles("*" + SolutionFile.DefaultExtension, searchOption);
			foreach (var file in fileInfos)
			{
				SolutionFile item = SolutionFile.FromFile(file.FullName, this);
				this.solutions.Add(item);
			}
		}

		SolutionFile ISolutionContext.LoadSolution(string path)
		{
			throw new NotImplementedException();
		}

		public Project LoadProject(string path)
		{
			var p = new CSharpLibraryProject(this, path);
			this.projects.Add(p);
			return p;
		}
	}
}

