// (c) 2015 Oren Novotny, MIT, https://raw.githubusercontent.com/onovotny/ReferenceGenerator/f17905224655e482f0e57174f83b12852b6043c0/LICENSE.txt
using System;

namespace mptcore
{
	class Reference : IEquatable<Reference>
	{
		public Reference(string name, Version version)
		{
			Name = name;
			Version = version;
		}

		public string Name
		{
			get;
			private set;
		}

		public Version Version
		{
			get;
			private set;
		}

		public bool Equals(Reference other)
		{
			if (ReferenceEquals(other, null))
				return false;

			return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
				Equals(Version, other.Version);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Reference);
		}

		public override int GetHashCode()
		{
			return unchecked(Name?.GetHashCode() ?? 1 ^ Version?.GetHashCode() ?? 1);
		}

		public override string ToString()
		{
			return Name + ", " + Version;
		}
	}
}
