namespace mptgitmodules
{
	using System;
	using System.IO;
	using System.Reflection;

	public class UsageInfo
	{
		string exeName;
		public UsageInfo ()
		{
			string codeBase = Assembly.GetExecutingAssembly().CodeBase;
			exeName = Path.GetFileName(codeBase);
		}
		public void Dump(TextWriter stream)
		{
			stream.WriteLine ("usage: {0} subsection-name", exeName);
		}
	}
}
