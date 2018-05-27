namespace BuildAutomation
{
	using System.Text;
	using System.Text.RegularExpressions;

	class AssemblyVersionSigned : AssemblyVersion, IAssemblyVersion
	{
		public string PublicKeyToken { get; set; }
		public AssemblyVersionSigned (string AssemblyName) : base (AssemblyName)
		{
			ParseInternal (AssemblyName);
		}
		public string Generate ()
		{
			var sb = new StringBuilder ();
			sb.Append ($"{Name}");
			if (!string.IsNullOrEmpty (Version))
				sb.Append ($", Version={Version}");
			if (!string.IsNullOrEmpty (Culture))
				sb.Append ($", Culture={Culture}");
			if (!string.IsNullOrEmpty (PublicKeyToken))
				sb.Append ($", PublicKeyToken={PublicKeyToken}");
			return sb.ToString ();
		}
		static readonly string v = "Version *= *(?<Version>[^, ]*)";
		static readonly string c = "Culture *= *(?<Culture>[^, ]*)";
		static readonly string k = "PublicKeyToken *= *(?<PublicKeyToken>[^, ]*)";
		static readonly Regex regex = new Regex ("(?<Name>[^,]*)" +
												"( *, *((" + v + ")|(" + c + ")|(" + k + ")))*",
			RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		protected void ParseInternal (string AssemblyName)
		{
			var m = regex.Match (AssemblyName);
			Name = m.Groups ["Name"].Value;
			Version = m.Groups ["Version"].Value;
			Culture = m.Groups ["Culture"].Value;
			PublicKeyToken = m.Groups ["PublicKeyToken"].Value;
		}
		void IAssemblyVersion.Parse (string AssemblyName)
		{
			ParseInternal(AssemblyName);
		}

		bool IAssemblyVersion.IsSigned {
			get {
				return true;
			}
		}

		public string AssemblyName {
			get {
				return Generate ();
			}
			set {
				ParseInternal (value);
			}
		}
	}

}
