namespace BuildAutomation
{
	public class ProjectAssemblyPackageReference
	{
		protected string name_of_package;
		protected string version_of_package;

		public string Name { get; set; }

		public ProjectAssemblyPackageReference (string name_of_package, string version_of_package)
		{
			this.name_of_package = name_of_package;
			this.version_of_package = version_of_package;
		}

		public static string GetKeyForItem(ProjectAssemblyPackageReference item)
		{
			return item.Name;
		}
	}
}

