namespace mptgitmodules
{
	using System;
	using System.IO;
	using System.Reflection;

	/// <summary>
	/// Information about program usage
	/// </summary>
	/// <remarks>
	/// called from function main, when args.Length == 0
	/// </remarks>
	public class UsageInfo
	{
		string exeName;
		/// <summary>
		/// Initializes a new instance of the <see cref="mptgitmodules.UsageInfo"/> class.
		/// </summary>
		public UsageInfo ()
		{
			string codeBase = Assembly.GetExecutingAssembly().CodeBase;
			exeName = Path.GetFileName(codeBase);
		}
		/// <summary>
		/// Output instructions to the specified stream.
		/// </summary>
		/// <param name="stream">destination</param>
		public void Dump(TextWriter stream)
		{
			stream.WriteLine ("usage: {0} subsection-name", exeName);
		}
	}
}
