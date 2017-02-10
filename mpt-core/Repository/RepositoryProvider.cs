using System;

namespace mptcore
{
	public interface IRepositoryProvider
	{
		string GetUrl(string accountName, string repoName);
	}
	public class GitHub : IRepositoryProvider
	{
		public string GetUrl(string accountName, string repoName)
		{
			return "https://github.com/" + accountName + "/" + repoName + ".git";
		}
	}
	public class GitLab : IRepositoryProvider
	{
		public string GetUrl(string accountName, string repoName)
		{
			return "https://gitlab.com/" + accountName + "/" + repoName + ".git";
		}
	}
	public class BitBucket : IRepositoryProvider
	{
		public string GetUrl(string accountName, string repoName)
		{
			return "https://bitbucket.com/" + accountName + "/" + repoName + ".git";
		}
	}
	public class CodePlex : IRepositoryProvider
	{
		public string GetUrl(string accountName, string repoName)
		{
			return "https://codeplex.com/" + accountName + "/" + repoName + ".git";
		}
	}
	public class InternetInfo
	{
		public static InternetInfo Wikipedia = new InternetInfo();
		public GitHub GitHub = new GitHub();
		public GitLab GitLab = new GitLab();
		public BitBucket BitBucket = new BitBucket();
		public CodePlex CodePlex = new CodePlex();
		public IRepositoryProvider GetRepositoryProvider(string url)
		{
			if (url.Contains("github")) return GitHub;
			if (url.Contains("gitlab")) return GitLab;
			if (url.Contains("bitbucket")) return BitBucket;
			if (url.Contains("codeplex")) return CodePlex;
			throw new NotSupportedException();
		}
	}
}
