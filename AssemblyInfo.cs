using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.

[assembly: AssemblyTitle ("mono-packaging-tools")]
[assembly: AssemblyCopyright ("2015-2018, Gentoo Foundation")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]
[assembly: AssemblyLicenseName ("GPL-3")]
[assembly: AssemblyLicenseURL ("https://github.com/ArsenShnurkov/mono-packaging-tools/blob/master/LICENSE")]
[assembly: InternalsVisibleTo("mpt-core.tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010079159977d2d03a8e6bea7a2e74e8d1afcc93e8851974952bb480a12c9134474d04062447c37e0e68c080536fcf3c3fbe2ff9c979ce998475e506e8ce82dd5b0f350dc10e93bf2eeecf874b24770c5081dbea7447fddafa277b22de47d6ffea449674a4f9fccf84d15069089380284dbdd35f46cdff12a1bd78e4ef0065d016df")]

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
