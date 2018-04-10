namespace Ebuild
{
	using System.Collections.Generic;
	public class InheritLine
	{
		List<EClassDependency> eclasses;
		public InheritLine()
		{
			eclasses = new List<EClassDependency>();
		}
		public List<EClassDependency> EClasses
		{
			get
			{
				return eclasses;
			}
		}
	}
}

