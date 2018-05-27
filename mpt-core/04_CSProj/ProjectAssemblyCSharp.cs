namespace BuildAutomation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;
	using CWDev.SLNTools.Core;

	public class ProjectAssemblyCSharp : IDisposable
	{
		MSBuildFile uo;

		ConfigurationHashList configurations = null;
		public ConfigurationHashList Configurations { get { return configurations; } }

		public string FileName { get { return uo.FileName; } set { uo.FileName = value; } }

		public ProjectAssemblyCSharp (string csproj_file)
		{
			configurations = new ConfigurationHashList (this);
			uo = new MSBuildFile (csproj_file);
			PrepareReferences ();
		}

		void PrepareReferences ()
		{
			string filename = uo.FileName;
			if (!File.Exists (filename)) {
				throw new FileNotFoundException ($"Cannot detect references of project '{filename}' because the project file cannot be found.");
			}

			var docManaged = uo.UnderlyingObject;

			var xmlManager = new XmlNamespaceManager (docManaged.NameTable);
			xmlManager.AddNamespace ("prefix", "http://schemas.microsoft.com/developer/msbuild/2003");

			foreach (XmlNode xmlNode in docManaged.SelectNodes (@"//prefix:Reference", xmlManager)) {
				string referenceInclude = xmlNode.Attributes.GetNamedItem ("Include").InnerText;
				string referencePackage = xmlNode.SelectSingleNode (@"prefix:Package", xmlManager)?.InnerText.Trim (); // TODO handle null
				references.Add (new ProjectAssemblyReference (
					referenceInclude,
					null,
					referencePackage));
			}
		}

		public void Dispose()
		{
			uo.Dispose();
		}

		public void InjectProjectImport(string import_name)
		{
			// construct new import
			MSBuildImport newImport = uo.CreateImport();
			newImport.Project = import_name;
			// insert import
			MSBuildImport existingImport = uo.FindImport("$(MSBuildBinPath)\\Microsoft.CSharp.targets");
			if (existingImport == null)
			{
				uo.InsertImport(newImport);
			}
			else
			{
				uo.InsertImportAfter(existingImport, newImport);
			}
		}

		public void InjectVersioning(string versionPropertyName)
		{
			/*
				<Target Name="MyAssemblyVersion" Outputs="@(GeneratedVersion)">
					<MakeDir Directories="$(IntermediateOutputPath)" />
					<AssemblyInfo CodeLanguage="CS"
						AssemblyCompany="MyCompanyName"
						AssemblyCopyright="Copyright $(CompanyName), All rights reserved."
						AssemblyVersion="12.34.56.78"
						AssemblyFileVersion="3.3.3.3"
						OutputFile="$(IntermediateOutputPath)file1.cs">
						<Output TaskParameter="OutputFile" ItemName="Compile" />
					</AssemblyInfo>
				</Target>
			*/
			MSBuildTarget targ = uo.CreateTarget();
			targ.Name = "MyAssemblyVersion";
			{
				MSBuildTask task = targ.CreateTask();
				task.Name = "MakeDir";
				task.AddParameter("Directories", "$(IntermediateOutputPath)");
				targ.AppendTask(task);
			}
			{
				MSBuildPropertyGroup group = targ.CreatePropertyGroup();
				group.Condition = " '$(" + versionPropertyName + ")' == '' ";
				group.AddProperty(versionPropertyName, "1.0.0.0");
				targ.AppendPropertyGroup(group);
			}
			{
				MSBuildTask task = targ.CreateTask();
				task.Name = "AssemblyInfo";
				task.AddParameter("CodeLanguage", "CS");

				task.AddParameter("AssemblyVersion", "$(" + versionPropertyName + ")"); // System.Reflection.AssemblyVersion
				task.AddParameter("AssemblyFileVersion", "$(" + versionPropertyName + ")"); // System.Reflection.AssemblyFileVersion
				task.AddParameter("AssemblyInformationalVersion", "$(" + versionPropertyName + ")"); // System.Reflection.AssemblyInformationalVersion

				task.AddParameter("OutputFile", "$(IntermediateOutputPath)AssemblyVersion.Generated.cs");
				{
					MSBuildTaskResultItem resultItem = task.CreateResultItem();
					resultItem.TaskParameter = "OutputFile";
					resultItem.ItemName = "Compile";
					task.AppendResultItem(resultItem);
				}
				targ.AppendTask(task);
			}
			uo.EnsureTargetExists("BeforeBuild");
			uo.InsertTarget(targ);
			uo.AddDependOnTarget("BeforeBuild", targ.Name);
		}

		// http://stackoverflow.com/questions/30943342/how-to-use-internalsvisibleto-attribute-with-strongly-named-assembly
		public void InjectInternalsVisibleTo(string assemblyName, string assemblyPublicKey)
		{
			MSBuildTarget targ = uo.CreateTarget();
			targ.Name = "MyInsertInternalsTo";
			{
				MSBuildTask task = targ.CreateTask(); // '$(SignAssembly)' == 'true'
				task.Name = "AssemblyInfo";
				task.AddParameter("CodeLanguage", "CS");

				task.Condition = "'$(SignAssembly)' == 'true'";
				task.AddParameter("InternalsVisibleTo", assemblyName + ", PublicKey=" + assemblyPublicKey);
				task.AddParameter("OutputFile", "$(IntermediateOutputPath)" + assemblyName + ".IVT.Generated.cs");
				{
					MSBuildTaskResultItem resultItem = task.CreateResultItem();
					resultItem.TaskParameter = "OutputFile";
					resultItem.ItemName = "Compile";
					task.AppendResultItem(resultItem);
				}
				targ.AppendTask(task);
			}
			{
				MSBuildTask task = targ.CreateTask(); // '$(SignAssembly)' == 'false'
				task.Name = "AssemblyInfo";
				task.AddParameter("CodeLanguage", "CS");

				task.Condition = "'$(SignAssembly)' != 'true'";
				task.AddParameter("InternalsVisibleTo", assemblyName);
				task.AddParameter("OutputFile", "$(IntermediateOutputPath)" + assemblyName + ".IVT.Generated.cs");
				{
					MSBuildTaskResultItem resultItem = task.CreateResultItem();
					resultItem.TaskParameter = "OutputFile";
					resultItem.ItemName = "Compile";
					task.AppendResultItem(resultItem);
				}
				targ.AppendTask(task);
			}
			uo.EnsureTargetExists("BeforeBuild");
			uo.InsertTarget(targ);
			uo.AddDependOnTarget("BeforeBuild", targ.Name);
		}

		FuncKeyedCollection<string, ProjectAssemblyReference> references
			= new FuncKeyedCollection<string, ProjectAssemblyReference>(ProjectAssemblyReference.GetKeyForItem);

		public IEnumerable<ProjectAssemblyReference> References
		{
			get
			{
				return references;
			}
		}
		public IEnumerable<ProjectAssemblyCSharp> Dependencies
		{
			get
			{
				throw new NotImplementedException();
				/*
				var docManaged = this.uo.UnderlyingObject;

				var xmlManager = new XmlNamespaceManager(docManaged.NameTable);
				xmlManager.AddNamespace("prefix", "http://schemas.microsoft.com/developer/msbuild/2003");

				foreach (XmlNode xmlNode in docManaged.SelectNodes(@"//prefix:ProjectReference", xmlManager))
				{
					// SelectSingleNode - Selects the first XmlNode that matches the XPath expression.
					var nodeProject = xmlNode.SelectSingleNode(@"prefix:Project", xmlManager);
					if (nodeProject == null)
					{
						Console.WriteLine($"Unexpected syntax of reference {xmlNode.OuterXml}");
						continue;
					}
					string dependencyGuid = nodeProject.InnerText == null ? string.Empty : nodeProject.InnerText.Trim(); // TODO handle null
					var nodeName = xmlNode.SelectSingleNode(@"prefix:Name", xmlManager);
					string dependencyName = nodeName.InnerText == null ? string.Empty : nodeName.InnerText.Trim(); // TODO handle null
					yield return FindProjectInContainer(
								dependencyGuid,
								"Cannot find one of the dependency of project '{0}'.\nProject guid: {1}\nDependency guid: {2}\nDependency name: {3}\nReference found in: ProjectReference node of file '{4}'",
								m_projectName,
								r_projectGuid,
								dependencyGuid,
								dependencyName,
								this.FullPath);
				}
				*/
			}
		}
	}
}