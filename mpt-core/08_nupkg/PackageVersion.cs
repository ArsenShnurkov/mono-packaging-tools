namespace BuildAutomation
{
	public class PackageVersion
	{
		protected string version_prefix;
		protected string version_suffix;
		public PackageVersion (string version_prefix, string version_suffix = null)
		{
			this.VersionPrefix = version_prefix;
			this.VersionSuffix = version_suffix;
		}
		public string VersionPrefix
		{ 
			get { return version_prefix; }  
			set { version_prefix = (value == null)?string.Empty:value; } 
		}
		public string VersionSuffix 
		{ 
			get { return version_suffix; }  
			set { version_suffix = (value == null)?string.Empty:value; } 
		}
	}
}
