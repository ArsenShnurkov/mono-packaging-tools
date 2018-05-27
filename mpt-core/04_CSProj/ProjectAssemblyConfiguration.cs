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
	}

	public class ConfigurationHashList : KeyedCollection<string, ProjectAssemblyConfiguration>
	{
		private ProjectAssemblyCSharp r_container = null;
		//
		// Properties
		//
		public ProjectAssemblyCSharp Container
		{
			get
			{
				return this.r_container;
			}
		}

		//
		// Constructors
		//
		public ConfigurationHashList(ProjectAssemblyCSharp container) : base(StringComparer.InvariantCultureIgnoreCase)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			this.r_container = container;
		}

		public ConfigurationHashList(ProjectAssemblyCSharp container, IEnumerable<ProjectAssemblyConfiguration> items) : this(container)
		{
			this.AddRange(items);
		}

		//
		// Methods
		//
		public void AddRange(IEnumerable<ProjectAssemblyConfiguration> items)
		{
			if (items != null)
			{
				foreach (var current in items)
				{
					base.Add(current);
				}
			}
		}

		public ProjectAssemblyConfiguration FindByName(string name)
		{
			ProjectAssemblyConfiguration result;
			foreach (var current in this)
			{
				if (string.Compare(current.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}

		protected override string GetKeyForItem(ProjectAssemblyConfiguration item)
		{
			return item.Name;
		}

		protected override void InsertItem(int index, ProjectAssemblyConfiguration item)
		{
			base.InsertItem(index, new ProjectAssemblyConfiguration(this.r_container, item));
		}

		protected override void SetItem(int index, ProjectAssemblyConfiguration item)
		{
			base.SetItem(index, new ProjectAssemblyConfiguration(this.r_container, item));
		}

		public void Sort()
		{
			this.Sort((ProjectAssemblyConfiguration p1, ProjectAssemblyConfiguration p2) => StringComparer.InvariantCultureIgnoreCase.Compare(p1.Name, p2.Name));
		}

		public void Sort(Comparison<ProjectAssemblyConfiguration> comparer)
		{
			var list = new List<ProjectAssemblyConfiguration>(this);
			list.Sort(comparer);
			base.Clear();
			this.AddRange(list);
		}
	}
}
