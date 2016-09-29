using System;
using System.Text.RegularExpressions;
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

	public static System.Text.Encoding GetEncoding(string filePath)
	{
		System.Text.Encoding enc = null;
		using (var file = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			if (file.CanSeek)
			{
				byte[] bom = new byte[4]; // Get the byte-order mark, if there is one 
				file.Read(bom, 0, 4);
				if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) // utf-8 
				{
					enc = System.Text.Encoding.UTF8;
				}
				else
				if (bom[0] == 0xfe && bom[1] == 0xff) // utf-16 and ucs-2
				{
					enc = System.Text.Encoding.Unicode;
				}
				else
				if ((bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le 
					(bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)) // ucs-4 
				{
					enc = System.Text.Encoding.Unicode;
				}
				else
				{
					enc = System.Text.Encoding.ASCII;
				}

				// Now reposition the file cursor back to the start of the file 
				file.Seek(0, System.IO.SeekOrigin.Begin);
			}
			else
			{
				// The file cannot be randomly accessed, so you need to decide what to set the default to 
				// based on the data provided. If you're expecting data from a lot of older applications, 
				// default your encoding to Encoding.ASCII. If you're expecting data from a lot of newer 
				// applications, default your encoding to Encoding.Unicode. Also, since binary files are 
				// single byte-based, so you will want to use Encoding.ASCII, even though you'll probably 
				// never need to use the encoding then since the Encoding classes are really meant to get 
				// strings from the byte array that is the file. 

				enc = System.Text.Encoding.ASCII;
			}
		}
		return enc;
	}

	public static void RemoveWarningsAsErrors(string csproj_file, string as_unified_patch)
	{
		System.Text.Encoding enc = GetEncoding(csproj_file);
		var old_version = File.ReadAllText(csproj_file, enc);
		var start_of_line = @"( |\t)*";
		var end_of_line = @"(\n\r|\n|\r)";
		var r = new Regex(start_of_line + "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>" + end_of_line);
		var collection = r.Matches(old_version);
		Match[] mc = new Match[collection.Count];
		collection.CopyTo(mc, 0);
		var new_version = old_version;
		for (int i = mc.Length - 1; i >= 0; --i)
		{
			Match m = mc[i];
			new_version = new_version.Remove(m.Index, m.Length);
		}
		if (string.Compare(old_version, new_version) != 0)
		{
			if (as_unified_patch == null)
			{
				File.WriteAllText(csproj_file, new_version, enc);
				Console.WriteLine("\tchanged");
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
