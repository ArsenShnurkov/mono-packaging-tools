namespace BuildAutomation
{
	public class ProjectAssemblyProjectReference
	{
		protected string relative_path;

		public string Name { get; set; }

		public ProjectAssemblyProjectReference (string textOfInclude)
		{
			this.relative_path = textOfInclude;
		}

		public static string GetKeyForItem(ProjectAssemblyProjectReference item)
		{
			return item.Name;
		}
	}
}

