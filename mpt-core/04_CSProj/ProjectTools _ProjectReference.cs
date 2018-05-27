namespace BuildAutomation
{
	using System;
	using System.Text.RegularExpressions;
	using System.IO;
	using System.Collections.Generic;
	using System.Xml.Linq;
	using System.Xml.XPath;
	using System.Xml;

	public partial class ProjectTools
	{
		protected static bool RemoveProjectReference(XDocument document, string reference_name)
		{
			bool bWasRemoved = false;
			var newname = new AssemblyVersionSigned (reference_name);
			var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("ns", MSBuildFile.NamespaceName);

			// remove ProjectReference
			var itemsToRemove2 = new List<XElement>();
			IEnumerable<XElement> listOfReferences2 = document.XPathSelectElements("/ns:Project/ns:ItemGroup/ns:ProjectReference[@Include]", xmlNamespaceManager);
			foreach (var el in listOfReferences2)
			{
				string assRef = el.Attribute("Include").Value;
				if (assRef.IndexOf(newname.Name, StringComparison.InvariantCulture) >= 0)
				{
					itemsToRemove2.Add(el);
				}
			}
			foreach (var item in itemsToRemove2)
			{
				Console.WriteLine($"ProjectReference removed {item.ToString()}");
				item.Remove();
				bWasRemoved = true;
			}

			return bWasRemoved;
		}

		public static bool RemoveProjectReference(string csproj_file, string reference_name)
		{
			var stream = new MemoryStream(File.ReadAllBytes(csproj_file)); // cache file in memoty
			var document = XDocument.Load(stream);
			bool bWasRemoved = RemoveProjectReference(document, reference_name);
			if (bWasRemoved)
			{
				document.Save(csproj_file);
			}
			return bWasRemoved;
		}
	}
}
