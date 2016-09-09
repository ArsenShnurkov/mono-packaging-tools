using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

public class ProjectTools
{
	private const string namespaceName = "http://schemas.microsoft.com/developer/msbuild/2003";
	public static void DumpFiles(string projectFilename, string baseDirectory)
	{
		List<string> files = new List<string>();
		files.Add(projectFilename);
		var stream = new MemoryStream(File.ReadAllBytes(projectFilename)); // cache file in memoty
		var document = XDocument.Load(stream);
		var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
		xmlNamespaceManager.AddNamespace("ns", namespaceName);
		IEnumerable<XElement> listOfSourceFiles = document.XPathSelectElements("/ns:Project/ns:ItemGroup/ns:Compile[@Include]", xmlNamespaceManager);
		foreach (var el in listOfSourceFiles)
		{
			files.Add(el.Attribute("Include").Value);
		}
		IEnumerable<XElement> listOfContentFiles = document.XPathSelectElements("/ns:Project/ns:ItemGroup/ns:Content[@Include]", xmlNamespaceManager);
		foreach (var el in listOfContentFiles)
		{
			files.Add(el.Attribute("Include").Value);
		}
		IEnumerable<XElement> listOfResourceFiles = document.XPathSelectElements("/ns:Project/ns:ItemGroup/ns:EmbeddedResource[@Include]", xmlNamespaceManager);
		foreach (var el in listOfResourceFiles)
		{
			files.Add(el.Attribute("Include").Value);
		}
		string separator = new string(Path.DirectorySeparatorChar, 1);
		string projDir = (new FileInfo(projectFilename)).Directory.FullName;
		foreach (string relativeName in files)
		{
			var correctedRelativeName = relativeName.Replace("\\", separator);
			string fullFileName = Path.Combine(projDir, correctedRelativeName);
			fullFileName = new FileInfo(fullFileName).FullName;
			string shortName;
			if (fullFileName.StartsWith(baseDirectory))
			{
				shortName = fullFileName.Substring(baseDirectory.Length);
				if (shortName.StartsWith(separator))
				{
					shortName = shortName.Substring(1);
				}
			}
			else
			{
				shortName = fullFileName;
			}
			Console.WriteLine(shortName);
		}
	}
}
