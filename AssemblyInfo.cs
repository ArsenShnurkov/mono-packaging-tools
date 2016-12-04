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
[assembly: InternalsVisibleTo("tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001009f2fe9783596542068aeeaa37ad37285be244f38d38b58fe9b3218e0baf12d8765b6008e304b9523dd2a838713b99b46bbfa9afb7bbe922acdf6747586e2efe24282faa518603350413f44afafb23825b212393a0a89cf1f5a5438ba8b5c3bdab0455edf0df8f3a662448763c00d06a852acefa177282774fdec7cff1fcaebc9")]

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
