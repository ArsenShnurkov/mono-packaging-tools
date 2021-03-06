﻿namespace BuildAutomation
{
	using System;
	using System.Xml;

	public class MSBuildProperty
	{
		XmlElement uo;

		public XmlElement UnderlyingObject { get { return uo; } }

		public string Name { get { return uo.LocalName; } set { SetName(value); } }
		public string Value { get { return uo.InnerText; } set { uo.InnerText = value; } }

		public MSBuildProperty(MSBuildPropertyGroup parent)
		{
			XmlDocument doc = parent.UnderlyingObject.OwnerDocument;
			uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, "UndefilnedPropertyName", MSBuildFile.NamespaceName);
		}

		public MSBuildProperty(MSBuildPropertyGroup parent, XmlElement uo)
		{
			this.uo = uo;
		}

		void SetName(string name)
		{
			// replace underlaying object to change it's name
			XmlElement oldItem = uo;
			XmlDocument doc = oldItem.OwnerDocument;
			// replace name
			uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, name, MSBuildFile.NamespaceName);
			//  what about node's text content ?
			uo.InnerText = oldItem.InnerText;
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
			if (oldItem.ParentNode != null)
			{
				oldItem.ParentNode.ReplaceChild(uo, oldItem);
			}
		}

		public static string GetKeyForItem(MSBuildProperty item)
		{
			return item.Name;
		}
	}
}
