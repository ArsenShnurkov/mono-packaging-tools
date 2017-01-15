using System;

public class CSharpLibraryProject : IDisposable
{
	MSBuildFile uo;

	ConfigurationHashList configurations = null;
	public ConfigurationHashList Configurations { get { return configurations;	} }

	public string FileName { get { return uo.FileName; } set { uo.FileName = value; } }

	public CSharpLibraryProject(string csproj_file)
	{
		configurations = new ConfigurationHashList(this);
		uo = new MSBuildFile(csproj_file);
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

	public void InjectVersioning()
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
			MSBuildTask task = targ.CreateTask();
			task.Name = "AssemblyInfo";
			task.AddParameter("CodeLanguage", "CS");

			task.AddParameter("AssemblyVersion", "$(VersionNumber)"); // System.Reflection.AssemblyVersion
			task.AddParameter("AssemblyFileVersion", "$(VersionNumber)"); // System.Reflection.AssemblyFileVersion
			task.AddParameter("AssemblyInformationalVersion", "$(VersionNumber)"); // System.Reflection.AssemblyInformationalVersion

			task.AddParameter("OutputFile", "$(IntermediateOutputPath)AssemblyVersion.Generated.cs");
			{
				MSBuildTaskResultItem resultItem = task.CreateResultItem();
				resultItem.TaskParameter = "OutputFile";
				resultItem.ItemName = "Compile";
				task.AppendResultItem(resultItem);
			}
			targ.AppendTask(task);
		}
		uo.InsertTarget(targ);
	}

	// http://stackoverflow.com/questions/30943342/how-to-use-internalsvisibleto-attribute-with-strongly-named-assembly
	public void InjectInternalsVisibleTo(string assemblyName, string assemblyPublicKey)
	{
		MSBuildTarget targ = uo.CreateTarget();
		targ.Name = "MyInsertInternalsTo";
		{
			MSBuildPropertyGroup group = targ.CreatePropertyGroup();
			group.Condition = " '$(VersionNumber)' == '' ";
			group.AddProperty("VersionNumber", "1.0.0.0");
			targ.AppendPropertyGroup(group);
		}
		{
			MSBuildTask task = targ.CreateTask(); // '$(SignAssembly)' == 'true'
			task.Name = "AssemblyInfo";
			task.Condition = "'$(SignAssembly)' == 'true'";
			task.AddParameter("InternalsVisibleTo", assemblyName + ", PublicKey=" + assemblyPublicKey);
			task.AddParameter("OutputFile", "$(IntermediateOutputPath)" + assemblyName + ".IVT.Generated.cs");
			targ.AppendTask(task);
		}
		{
			MSBuildTask task = targ.CreateTask(); // '$(SignAssembly)' == 'false'
			task.Name = "AssemblyInfo";
			task.Condition = "'$(SignAssembly)' != 'true'";
			task.AddParameter("InternalsVisibleTo", assemblyName);
			task.AddParameter("OutputFile", "$(IntermediateOutputPath)" + assemblyName + ".IVT.Generated.cs");
			targ.AppendTask(task);
		}
		uo.InsertTarget(targ);
	}
}
