namespace BuildAutomation
{
	using System;

	public class AssemblyVersionSpecification
	{
		public static IAssemblyVersion Parse(string some_string)
		{
			if (some_string.IndexOf(",", StringComparison.InvariantCulture) < 0)
			{
				return new AssemblyVersion(some_string);
			}
			else
			{
				return new AssemblyVersionSigned(some_string);
			}
		}
	}
}
