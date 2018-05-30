namespace BuildAutomation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;
	using CWDev.SLNTools.Core;

	public class ProjectAssemblyCSharp : IDisposable
	{
		MSBuildFile underlaying_object;

		FuncKeyedCollection<string, ProjectAssemblyConfiguration> configurations
		= new FuncKeyedCollection<string, ProjectAssemblyConfiguration>(ProjectAssemblyConfiguration.GetKeyForItem);

		FuncKeyedCollection<string, ProjectAssemblyReference> references
		= new FuncKeyedCollection<string, ProjectAssemblyReference>(ProjectAssemblyReference.GetKeyForItem);

		FuncKeyedCollection<string, ProjectAssemblyProjectReference> project_references
		= new FuncKeyedCollection<string, ProjectAssemblyProjectReference>(ProjectAssemblyProjectReference.GetKeyForItem);

		FuncKeyedCollection<string, ProjectAssemblyPackageReference> package_references
		= new FuncKeyedCollection<string, ProjectAssemblyPackageReference>(ProjectAssemblyPackageReference.GetKeyForItem);

		public MSBuildFile UnderlyingObject
		{
			get
			{
				return underlaying_object;
			}
		}

		public string FileName
		{
			get { return this.UnderlyingObject.FileName; }
			set { this.UnderlyingObject.FileName = value; }
		}

		public ICollection<ProjectAssemblyConfiguration> Configurations { get { return configurations; } }
		public ICollection<ProjectAssemblyReference> References { get { return references; } }
		public ICollection<ProjectAssemblyProjectReference> ProjectReferences { get { return project_references; } }
		public ICollection<ProjectAssemblyPackageReference> PackageReferences { get { return package_references; } }

		public ProjectAssemblyCSharp ()
		{
			this.underlaying_object = new MSBuildFile ();
		}

		public virtual void Load (string filename)
		{
			if (!File.Exists (filename)) {
				throw new FileNotFoundException ($"Cannot detect references of project '{filename}' because the project file cannot be found.");
			}
			this.UnderlyingObject.Load(filename);
			ParseConfigurations ();
			ParseCSharpProperties();
			ParseReferences ();
			ParseProjectReferences ();
			ParsePackageReferences ();
		}

		void ParseConfigurations ()
		{
		}

		void ParseCSharpProperties ()
		{
		}

		void ParseReferences ()
		{
			foreach (XmlElement xmlNode in this.UnderlyingObject.UnderlyingObject.SelectNodes (@"//*[local-name() = 'Reference']")) {
				var par = new ProjectAssemblyReference (this, xmlNode);
				if (references.Contains(par.AssemblyVersion.AssemblyName) == false)
				{
					references.Add (par);
				}
			}
		}

		void ParseProjectReferences ()
		{
			foreach (XmlNode xmlNode in this.UnderlyingObject.UnderlyingObject.SelectNodes (@"//*[local-name() = 'ProjectReference']")) {
				string textOfInclude = xmlNode.Attributes.GetNamedItem ("Include").InnerText;
				project_references.Add (new ProjectAssemblyProjectReference (textOfInclude));
			}
		}

		void ParsePackageReferences ()
		{
			foreach (XmlNode xmlNode in this.UnderlyingObject.UnderlyingObject.SelectNodes (@"//*[local-name() = 'PackageReference']")) {
				string name_of_package = xmlNode.Attributes.GetNamedItem ("Include").InnerText;
				string version_of_package = xmlNode.Attributes.GetNamedItem ("Version").InnerText;
				package_references.Add (new ProjectAssemblyPackageReference (name_of_package, version_of_package));
			}
		}

		public void Dispose()
		{
			this.UnderlyingObject.Dispose();
		}

		public void InjectProjectImport(string import_name)
		{
			// construct new import
			MSBuildImport newImport = this.UnderlyingObject.CreateImport();
			newImport.Project = import_name;
			// insert import
			MSBuildImport existingImport = this.UnderlyingObject.FindImport("$(MSBuildBinPath)\\Microsoft.CSharp.targets");
			if (existingImport == null)
			{
				this.UnderlyingObject.InsertImport(newImport);
			}
			else
			{
				this.UnderlyingObject.InsertImportAfter(existingImport, newImport);
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
			MSBuildTarget targ = this.UnderlyingObject.CreateTarget();
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
			this.UnderlyingObject.EnsureTargetExists("BeforeBuild");
			this.UnderlyingObject.InsertTarget(targ);
			this.UnderlyingObject.AddDependOnTarget("BeforeBuild", targ.Name);
		}

		// http://stackoverflow.com/questions/30943342/how-to-use-internalsvisibleto-attribute-with-strongly-named-assembly
		public void InjectInternalsVisibleTo(string assemblyName, string assemblyPublicKey)
		{
			MSBuildTarget targ = this.UnderlyingObject.CreateTarget();
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
			this.UnderlyingObject.EnsureTargetExists("BeforeBuild");
			this.UnderlyingObject.InsertTarget(targ);
			this.UnderlyingObject.AddDependOnTarget("BeforeBuild", targ.Name);
		}
	}
}