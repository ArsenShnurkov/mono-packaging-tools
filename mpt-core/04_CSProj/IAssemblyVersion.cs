namespace BuildAutomation
{
	public interface IAssemblyVersion
	{
		bool IsSigned { get; }
		/// <remarks>
		/// Use AssemblyVersionSpecification.Parse as factory method
		/// </remarks>
		/// <param name="value">Value.</param>
		void Parse(string value);
		string ToString();
	}
}
