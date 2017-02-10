using System;
namespace mptcore
{
	public class Upstream
	{
		public string Url { get; set; }
		public Upstream(string url)
		{
			this.Url = url;
		}
	}
	public class GitUpstream : Upstream
	{
		public GitUpstream() : base(null)
		{
		}
	}
	public class MercurialUpstream : Upstream
	{
		public MercurialUpstream() : base(null)
		{
		}
	}
	public class SubversionUpstream : Upstream
	{
		public SubversionUpstream() : base(null)
		{
		}
	}
	public class CvsUpstream : Upstream
	{
		public CvsUpstream() : base(null)
		{
		}
	}
}

