namespace BuildAutomation
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;

	public class ProjectAssemblyConfiguration
	{
		ProjectAssemblyCSharp r_container;

		public ProjectAssemblyConfiguration(ProjectAssemblyCSharp r_container, ProjectAssemblyConfiguration item)
		{
			this.r_container = r_container;
			this.Name = item.Name;
		}

		public string Name
		{
			get
			{
				Debug.Assert(r_container != null);
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string GetAssemblyName()
		{
			throw new NotImplementedException();
		}

		public static string GetKeyForItem(ProjectAssemblyConfiguration item)
		{
			return item.Name;
		}
	}
}
