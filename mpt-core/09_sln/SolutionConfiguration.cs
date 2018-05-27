namespace BuildAutomation
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;

	public class SolutionConfiguration
	{
		ProjectAssemblyCSharp r_container;

		public SolutionConfiguration(ProjectAssemblyCSharp r_container, SolutionConfiguration item)
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

	public class SolutionConfigurationHashList : KeyedCollection<string, SolutionConfiguration>
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
		public SolutionConfigurationHashList(ProjectAssemblyCSharp container) : base(StringComparer.InvariantCultureIgnoreCase)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			this.r_container = container;
		}

		public SolutionConfigurationHashList(ProjectAssemblyCSharp container, IEnumerable<SolutionConfiguration> items) : this(container)
		{
			this.AddRange(items);
		}

		//
		// Methods
		//
		public void AddRange(IEnumerable<SolutionConfiguration> items)
		{
			if (items != null)
			{
				foreach (var current in items)
				{
					base.Add(current);
				}
			}
		}

		public SolutionConfiguration FindByName(string name)
		{
			SolutionConfiguration result;
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

		protected override string GetKeyForItem(SolutionConfiguration item)
		{
			return item.Name;
		}

		protected override void InsertItem(int index, SolutionConfiguration item)
		{
			base.InsertItem(index, new SolutionConfiguration(this.r_container, item));
		}

		protected override void SetItem(int index, SolutionConfiguration item)
		{
			base.SetItem(index, new SolutionConfiguration(this.r_container, item));
		}

		public void Sort()
		{
			this.Sort((SolutionConfiguration p1, SolutionConfiguration p2) => StringComparer.InvariantCultureIgnoreCase.Compare(p1.Name, p2.Name));
		}

		public void Sort(Comparison<SolutionConfiguration> comparer)
		{
			var list = new List<SolutionConfiguration>(this);
			list.Sort(comparer);
			base.Clear();
			this.AddRange(list);
		}
	}
}
