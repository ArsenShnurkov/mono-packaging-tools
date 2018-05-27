using System;

namespace BuildAutomation
{
	/// <remarks>
	/// How this class is different from AssemblyInfo?
	/// <br />
	/// This one contains only version number, and no other meta information
	/// <br />
	/// How this class usage is different from AssemblyVersionSpecification?
	/// <br />
	/// This class represents actual version of assembly,
	/// ProjectAssembly should use AssemblyVersionSpecification
	/// </remarks>
	public class AssemblyVersion : IAssemblyVersion
	{
		protected string name;
		protected string version;
		protected string culture;
		public AssemblyVersion (string name, string version = null, string culture = null)
		{
			this.Name = name;
			this.Version = version;
			this.Culture = culture;
		}
		public string Name
		{
			get {return name;}
			set {name = string.IsNullOrEmpty(value)?string.Empty:value;}
		}
		public string Version
		{
			get {return version;}
			set {version = string.IsNullOrEmpty(value)?"1.0.0.*":value;}
		}
		public string Culture
		{
			get {return culture;}
			set {culture = string.IsNullOrEmpty(value)?string.Empty:value;}
		}

		bool IAssemblyVersion.IsSigned {
			get {
				return false;
			}
		}

		void IAssemblyVersion.Parse (string value)
		{
			throw new NotImplementedException ();
		}

		string IAssemblyVersion.ToString ()
		{
			throw new NotImplementedException ();
		}
	}
}
