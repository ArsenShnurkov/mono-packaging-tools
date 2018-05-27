namespace BuildAutomation
{
	using System;

	public class AssemblyVersionSpecification
	{
		IAssemblyVersion version;
		public AssemblyVersionSpecification (string specification)
		{
			Parse(specification);
		}
		public static IAssemblyVersion Parse(string some_string)
		{
			throw new NotImplementedException();
		}
	}
}
