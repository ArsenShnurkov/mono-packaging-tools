namespace BuildAutomation
{
	public class PackageVersionSpecification
	{
		protected string package_name;
		public PackageVersionSpecification (string package_name)
		{
			this.PackageName = package_name;
		}
		public string PackageName
		{
			get { return package_name; }
			set { package_name = string.IsNullOrEmpty(value)?string.Empty:value; }
		}
	}
}

