using System;
using System.Collections.Generic;
using System.Xml;

public class MSBuildImport
{
	MSBuildFile file;
	XmlElement uo;

	public XmlElement UnderlyingObject
	{
		get
		{
			return uo;
		}
	}

	public string Project { get { return uo.Attributes["Project"].Value; } set { uo.Attributes["Project"].Value = value; } }

	public MSBuildImport(MSBuildFile f, XmlElement el)
	{
		this.file = f;
		uo = el;
	}

	public MSBuildImport(MSBuildFile f)
	{
		this.file = f;
		// string element = "<Import Project=\"" + import_name + "\" />";
		uo = (XmlElement)file.UnderlyingObject.CreateNode(XmlNodeType.Element, "Import", this.uo.NamespaceURI);
	}
}
