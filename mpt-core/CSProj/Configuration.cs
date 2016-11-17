using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

public class Configuration
{
	CSharpLibraryProject r_container;

	public Configuration(CSharpLibraryProject r_container, Configuration item)
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

public class ConfigurationHashList : KeyedCollection<string, Configuration>
{
	private CSharpLibraryProject r_container = null;
	//
	// Properties
	//
	public CSharpLibraryProject Container
	{
		get
		{
			return this.r_container;
		}
	}

	//
	// Constructors
	//
	public ConfigurationHashList(CSharpLibraryProject container) : base(StringComparer.InvariantCultureIgnoreCase)
	{
		if (container == null)
		{
			throw new ArgumentNullException("container");
		}
		this.r_container = container;
	}

	public ConfigurationHashList(CSharpLibraryProject container, IEnumerable<Configuration> items) : this(container)
	{
		this.AddRange(items);
	}

	//
	// Methods
	//
	public void AddRange(IEnumerable<Configuration> items)
	{
		if (items != null)
		{
			foreach (var current in items)
			{
				base.Add(current);
			}
		}
	}

	public Configuration FindByName(string name)
	{
		Configuration result;
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

	protected override string GetKeyForItem(Configuration item)
	{
		return item.Name;
	}

	protected override void InsertItem(int index, Configuration item)
	{
		base.InsertItem(index, new Configuration(this.r_container, item));
	}

	protected override void SetItem(int index, Configuration item)
	{
		base.SetItem(index, new Configuration(this.r_container, item));
	}

	public void Sort()
	{
		this.Sort((Configuration p1, Configuration p2) => StringComparer.InvariantCultureIgnoreCase.Compare(p1.Name, p2.Name));
	}

	public void Sort(Comparison<Configuration> comparer)
	{
		var list = new List<Configuration>(this);
		list.Sort(comparer);
		base.Clear();
		this.AddRange(list);
	}
}

