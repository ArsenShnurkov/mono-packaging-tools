namespace Ebuild
{
	using System.Collections.Generic;
	using Bash;

	public class EbuildDocument
	{
		protected BashDocument baseDocument;
		public EbuildDocument () : this (new BashDocument())
		{
		}
		public EbuildDocument (BashDocument doc)
		{
			baseDocument = doc;
			UseLine = new UseLine();
			InheritLines = new InheritanceDeclaration();
			HelperFunctions = new List<BashFunction>();
			Phases = new List<PhaseFunction>();
		}
		public BashDocument UnderlayingObject {
			get
			{
				return baseDocument;
			}
		}
		public UseLine UseLine { get; }
		public InheritanceDeclaration InheritLines {get;}
		public List<BashFunction> HelperFunctions { get; }
		public List<PhaseFunction> Phases { get; }
		public void SaveTo(string fileName)
		{
			UnderlayingObject.SaveTo(fileName);
		}
	}
}

