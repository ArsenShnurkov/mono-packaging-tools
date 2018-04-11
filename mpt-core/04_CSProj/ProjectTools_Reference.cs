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
		protected static bool RemoveReference(XDocument document, string reference_name)
		{
			bool bWasRemoved = false;
			var newname = new AssemblyNameInGAC(reference_name);
			var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("ns", MSBuildFile.NamespaceName);

			// remove ProjectReference
			var itemsToRemove2 = new List<XElement>();
			IEnumerable<XElement> listOfReferences2 = document.XPathSelectElements("/ns:Project/ns:ItemGroup/ns:ProjectReference[@Include]", xmlNamespaceManager);
			foreach (var el in listOfReferences2)
			{
				string assRef = el.Attribute("Include").Value;
				if (string.Compare(newname.Name, assRef) == 0)
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


			var itemsToRemove = new List<XElement>();
			IEnumerable<XElement> listOfReferences = document.XPathSelectElements("/ns:Project/ns:ItemGroup/ns:Reference[@Include]", xmlNamespaceManager);
			foreach (var el in listOfReferences)
			{
				string assRef = el.Attribute("Include").Value;
				var name = new AssemblyNameInGAC(assRef);
				if (string.Compare(newname.Name, name.Name) == 0)
				{
					itemsToRemove.Add(el);
				}
			}
			foreach (var item in itemsToRemove)
			{
				Console.WriteLine($"Reference removed {item.ToString()}");
				item.Remove();
				bWasRemoved = true;
			}

			return bWasRemoved;
		}

		public static bool RemoveReference(string csproj_file, string reference_name)
		{
			var stream = new MemoryStream(File.ReadAllBytes(csproj_file)); // cache file in memoty
			var document = XDocument.Load(stream);
			bool bWasRemoved = RemoveReference(document, reference_name);
			if (bWasRemoved)
			{
				document.Save(csproj_file);
			}
			return bWasRemoved;
		}

		public static void ReplaceReference(string csproj_file, string reference_name, bool force)
		{
			var stream = new MemoryStream(File.ReadAllBytes(csproj_file)); // cache file in memory
			var document = XDocument.Load(stream);

			bool bWasRemoved = RemoveReference(document, reference_name);
			bool bRequiresSave = bWasRemoved;

			var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("ns", MSBuildFile.NamespaceName);
			XElement group = document.XPathSelectElement("/ns:Project/ns:ItemGroup", xmlNamespaceManager);
			if (group == null)
			{
				// insert new item group
				throw new NotImplementedException();
			}

			// insert new reference
			if (force || bWasRemoved)
			{
				XNamespace ns = document.Root.Name.Namespace;
				var newEl = new XElement(ns + "Reference");
				var newname = new AssemblyNameInGAC(reference_name);
				newEl.Add(new XAttribute("Include", newname.Generate()));
				group.Add(newEl);
				bRequiresSave = true;
			}

			if (bRequiresSave)
			{
				document.Save(csproj_file);
			}
		}
		public static void CreateReferenceHintPath(string csproj_file, string reference_name, string hint_path, bool force)
		{
			Console.WriteLine($"Creating hint path for {reference_name} in {csproj_file} + as {hint_path}");
			var stream = new MemoryStream(File.ReadAllBytes(csproj_file)); // cache file in memory
			var document = XDocument.Load(stream);

			var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("ns", MSBuildFile.NamespaceName);
			IEnumerable<XElement> listOfReferences = document.XPathSelectElements("/ns:Project/ns:ItemGroup/ns:Reference[@Include]", xmlNamespaceManager);
			bool bInserted = false;
			foreach (XElement xmlNode in listOfReferences)
			{
				string assRef = xmlNode.Attribute("Include").Value;
				Console.WriteLine("Found reference " + assRef);
				if (IsMatched(reference_name, assRef))
				{
					Console.WriteLine("matched");
					XNamespace space = MSBuildFile.NamespaceName;
					XElement el = new XElement(space + "HintPath", hint_path);
					xmlNode.Add(el);
					bInserted = true;
				}
			}
			if (bInserted == false)
			{
				Console.WriteLine("No Reference node found");
				if (force)
				{
					ReplaceReference(csproj_file, reference_name, force);
					CreateReferenceHintPath(csproj_file, reference_name, hint_path, force);
					return;
				}
			}
			if (bInserted)
			{
				document.Save(csproj_file);
			}
			else
			{
				Console.WriteLine("No changes was made");
			}
		}
		static bool IsMatched(string reference_name, string reference_spec)
		{
			if (reference_spec.StartsWith(reference_name, StringComparison.InvariantCulture) == false)
			{
				return false;
			}
			if (reference_name.Length == reference_spec.Length)
			{
				return true;
			}
			if (string.Compare(reference_spec.Substring(reference_name.Length, 1), ",", StringComparison.InvariantCulture) == 0)
			{
				return true;
			}
			if (string.Compare(reference_spec.Substring(reference_name.Length, 1), " ", StringComparison.InvariantCulture) == 0)
			{
				return true;
			}
			return false;
		}
	}
}
