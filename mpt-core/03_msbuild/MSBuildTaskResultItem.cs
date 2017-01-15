﻿using System;
using System.Xml;

public class MSBuildTaskResultItem
{
	// https://github.com/mono/mono/blob/master/mcs/class/referencesource/System.Xml/System/Xml/Dom/XmlAttribute.cs
	XmlElement uo;
	MSBuildTask parent;

	public XmlElement UnderlyingObject
	{
		get
		{
			return uo;
		}
	}

	public string TaskParameter { get { return uo.Attributes["TaskParameter"].Value; } set { uo.Attributes["TaskParameter"].Value = value; } }
	public string ItemName { get { return uo.Attributes["ItemName"].Value; } set { uo.Attributes["ItemName"].Value = value; } }

	public MSBuildTaskResultItem(MSBuildTask p)
	{
		this.parent = p;
		XmlDocument doc = parent.UnderlyingObject.OwnerDocument;
		uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, "Output", doc.NamespaceURI);
	}

	void SetName(string name)
	{
		// replace underlaying object to change it's name
		XmlElement oldItem = uo;
		XmlDocument doc = oldItem.OwnerDocument;
		// replace name
		uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, name, doc.NamespaceURI);
		uo.Value = oldItem.Value;
		// copy attributes
		foreach (XmlAttribute a in oldItem.Attributes)
		{
			uo.Attributes.Append((XmlAttribute)a.CloneNode(true));
		}
		// copy childs
		for (XmlNode child = oldItem.FirstChild; child != null; child = child.NextSibling)
		{
			uo.AppendChild(child.CloneNode(true));
		}
		//  what about node's text content ?
		//uo.Value = oldItem.Value;
		oldItem.ParentNode.ReplaceChild(uo, oldItem);
	}
}
