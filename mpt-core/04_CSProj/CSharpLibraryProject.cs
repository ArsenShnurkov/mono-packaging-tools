using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using MetaSpecTools;

public class CSharpLibraryProject : Project
{
	ConfigurationHashList configurations = null;
	public ConfigurationHashList Configurations { get { return configurations;	} }

	public CSharpLibraryProject(IProjectContext context, string filename)
		: base (context, filename)
	{
		configurations = new ConfigurationHashList(this);
	}

	public void InjectProjectImport(string import_name)
	{
		// construct new import
		MSBuildImport newImport = base.CreateImport();
		newImport.Project = import_name;
		// insert import
		MSBuildImport existingImport = base.FindImport("$(MSBuildBinPath)\\Microsoft.CSharp.targets");
		if (existingImport == null)
		{
			base.InsertImport(newImport);
		}
		else
		{
			base.InsertImportAfter(existingImport, newImport);
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
		MSBuildTarget targ = base.CreateTarget();
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
		base.EnsureTargetExists("BeforeBuild");
		base.InsertTarget(targ);
		base.AddDependOnTarget("BeforeBuild", targ.Name);
	}

	// http://stackoverflow.com/questions/30943342/how-to-use-internalsvisibleto-attribute-with-strongly-named-assembly
	public void InjectInternalsVisibleTo(string assemblyName, string assemblyPublicKey)
	{
		MSBuildTarget targ = base.CreateTarget();
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
		base.EnsureTargetExists("BeforeBuild");
		base.InsertTarget(targ);
		base.AddDependOnTarget("BeforeBuild", targ.Name);
	}
}
