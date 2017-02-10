using System;
namespace mptcore
{
	public class MappingPolicy
	{
		public static MappingPolicy Current = new MappingPolicy();

		public virtual string UrlToLocalDirectory(Upstream remoteRepository)
		{
			throw new NotImplementedException();
		}
	}
}
