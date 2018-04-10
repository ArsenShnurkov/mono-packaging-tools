namespace Ebuild
{
	using System.Collections.Generic;
	public class DownloadLine
	{
		public DownloadLine ()
		{
			Specifications = new List<DistributionArtifactSpecification>();
		}
		public List<DistributionArtifactSpecification> Specifications { get ; }
	}
}

