namespace BuildAutomation
{
	using System;

	public class ProjectAssemblyReference
	{
		IAssemblyVersion assembly_version;
		string hint_path = string.Empty;
		string package_name = string.Empty;
		public ProjectAssemblyReference(string assembly_version_specification, string hint_path = "", string package_name = "")
		{
			this.AssemblyVersion = AssemblyVersionSpecification.Parse(assembly_version_specification);
			this.HintPath = hint_path;
			this.PackageName = package_name;
		}
		public IAssemblyVersion AssemblyVersion
		{
			get
			{
				return assembly_version;
			}
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException($"{nameof(AssemblyVersion)} has value of {value}");
				}
				assembly_version = value;
			}
		}
		public string HintPath
		{
			get
			{
				return hint_path;
			}
			set
			{
				hint_path = (value==null)?String.Empty:value;
			}
		}
		public string PackageName
		{
			get
			{
				return package_name;
			}
			set
			{
				package_name = (value==null)?String.Empty:value;
			}
		}
		public static string GetKeyForItem(ProjectAssemblyReference item)
		{
			return item.assembly_version.AssemblyName;
		}
	}
}
