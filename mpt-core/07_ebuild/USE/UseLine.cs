namespace Ebuild
{
	using System.Collections.Generic;
	public class UseLine
	{
		List<UseFlag> flags;
		public UseLine()
		{
			flags = new List<UseFlag>();
		}
		public List<UseFlag> Flags
		{
			get
			{
				return flags;
			}
		}

	}
}

