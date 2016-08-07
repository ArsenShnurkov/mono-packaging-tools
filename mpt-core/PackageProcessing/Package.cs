// (c) 2015 Oren Novotny, MIT, https://raw.githubusercontent.com/onovotny/ReferenceGenerator/f17905224655e482f0e57174f83b12852b6043c0/LICENSE.txt
using System;

namespace mptcore
{
	public class Package : IEquatable<Package>, IComparable<Package>
	{
		public Package(string id, string version)
		{
			Id = id;
			Version = SemanticVersion.Parse(version, null);
			VersionString = version;
		}

		public string Id
		{
			get;
			private set;
		}

		// This needs a SemVer comparer for sorting
		public SemanticVersion Version
		{
			get;
			private set;
		}

		// we use the string for round-tripping
		public string VersionString
		{
			get; private set;
		}


		public int CompareTo(Package other)
		{
			// sort on name for now
			return StringComparer.OrdinalIgnoreCase.Compare(Id, other?.Id);
		}

		public bool Equals(Package other)
		{
			if (ReferenceEquals(other, null))
				return false;

			return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase) &&
				Equals(Version, other.Version);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Package);
		}

		public override int GetHashCode()
		{
			return unchecked(Id?.GetHashCode() ?? 1 ^ Version?.GetHashCode() ?? 1);
		}

		public override string ToString()
		{
			return Id + ", " + VersionString;
		}
	}
}
