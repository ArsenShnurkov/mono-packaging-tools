using System;
using System.Collections.Generic;
using CWDev.SLNTools.Core;

public class CSharpLibraryProject : Project
{
	ConfigurationHashList configurations = null;
	public ConfigurationHashList Configurations
	{
		get
		{
			return configurations;
		}
	}
	public CSharpLibraryProject(SolutionFile container, string projectGuid, string projectTypeGuid, string projectName, string relativePath, string parentFolderGuid, IEnumerable<Section> projectSections, IEnumerable<PropertyLine> versionControlLines, IEnumerable<PropertyLine> projectConfigurationPlatformsLines)
		: base(container, projectGuid, projectTypeGuid, projectName, relativePath, parentFolderGuid, projectSections, versionControlLines, projectConfigurationPlatformsLines)
	{
		configurations = new ConfigurationHashList(this);
	}
}

