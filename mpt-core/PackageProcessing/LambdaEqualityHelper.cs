// (c) 2015 Oren Novotny, MIT, https://raw.githubusercontent.com/onovotny/ReferenceGenerator/f17905224655e482f0e57174f83b12852b6043c0/LICENSE.txt
using System;

namespace mptcore
{
	public class LambdaEqualityHelper<T>
	{
		Func<T, object>[] equalityContributorAccessors;

		public LambdaEqualityHelper(params Func<T, object>[] equalityContributorAccessors)
		{
			this.equalityContributorAccessors = equalityContributorAccessors;
		}

		public bool Equals(T instance, T other)
		{
			if (ReferenceEquals(null, instance) || ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(instance, other))
			{
				return true;
			}

			if (instance.GetType() != other.GetType())
			{
				return false;
			}

			foreach (var accessor in equalityContributorAccessors)
			{
				if (!Equals(accessor(instance), accessor(other)))
				{
					return false;
				}
			}

			return true;
		}

		public int GetHashCode(T instance)
		{
			var hashCode = GetType().GetHashCode();

			unchecked
			{
				foreach (var accessor in equalityContributorAccessors)
				{
					var item = accessor(instance);
					hashCode = (hashCode * 397) ^ (item != null ? item.GetHashCode() : 0);
				}
			}

			return hashCode;
		}
	}
}

