namespace Ebuild
{
	using System.Collections.Generic;
	public class InheritanceDeclaration
	{
		public InheritanceDeclaration ()
		{
			InheritLines = new List<InheritLine>();
		}
		public List<InheritLine> InheritLines { get ; }
	}
}

