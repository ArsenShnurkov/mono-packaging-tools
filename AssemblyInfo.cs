using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.

[assembly: AssemblyTitle ("mono-packaging-tools")]
[assembly: AssemblyCopyright ("2015, Gentoo Foundation")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]
[assembly: AssemblyLicenseName ("GPL-3")]
[assembly: AssemblyLicenseURL ("https://github.com/ArsenShnurkov/mono-packaging-tools/blob/master/LICENSE")]

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple= true)]
public class AssemblyLicenseURL : Attribute
{
	private readonly String licenseName;

	public AssemblyLicenseURL(String name)
	{
		this.licenseName = name;
	}

	public String License
	{
		get { return licenseName; }
	}
}

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple= true)]
public class AssemblyLicenseName : Attribute
{
	private readonly String licenseName;

	public AssemblyLicenseName(String name)
	{
		this.licenseName = name;
	}

	public String LicenseName
	{
		get { return licenseName; }
	}
}
