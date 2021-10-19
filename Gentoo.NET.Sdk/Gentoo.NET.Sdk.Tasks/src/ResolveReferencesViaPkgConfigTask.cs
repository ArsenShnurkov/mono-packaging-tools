using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Mono.PkgConfig;

public class ResolveReferencesViaPkgConfig : Task
{
	private ITaskItem[] references;

        // The Required attribute indicates the following to MSBuild:
        //	     - if the parameter is a scalar type, and it is not supplied, fail the build immediately
        //	     - if the parameter is an array type, and it is not supplied, pass in an empty array
        // In this case the parameter is an array type, so if a project fails to pass in a value for the
        // References parameter, the task will get invoked, but this implementation will do nothing,
        // because the array will be empty.
	[Required]
        // References to resolve.
        public ITaskItem[] References
        {
            get
            {
                return references;
            }

            set
            {
                references = value;
            }
        }

        private ITaskItem[] referencePaths;
	// The Output attribute indicates to MSBuild that the value of this property can be gathered after the
        // task has returned from Execute(), 
	// if the project has an <Output> tag under this task's element for this property.
        [Output]
        // A project may need the subset of the inputs that were actually created, so make that available here.
        public ITaskItem[] ReferencePaths
        {
            get
            {
                return referencePaths;
            }
        }

	string[]	targetFrameworkDirectories = new string [] {};
	string[]	searchPaths = new string [] {
		"{HintPathFromItem}",
		"{CandidateAssemblyFiles}",
		"{Registry:$(FrameworkRegistryBase), $(TargetFrameworkVersion), $(AssemblyFoldersSuffix), $(AssemblyFoldersExConditions)}",
		"{AssemblyFolders}",
		"{GAC}",
		"{PkgConfig}",
		"{RawFileName}",
		 };

		AssemblyResolver assembly_resolver = new AssemblyResolver ();

		string[]	allowedAssemblyExtensions;
		public string[] AllowedAssemblyExtensions {
			get { return allowedAssemblyExtensions; }
			set { allowedAssemblyExtensions = value; }
		}
		static string []	default_assembly_extensions = new string [] { ".dll", ".exe" };

		// Use @search_paths to resolve the reference
		ResolvedReference ResolveReference (ITaskItem item, IEnumerable<string> search_paths, bool set_copy_local)
		{
			ResolvedReference resolved = null;
			bool specific_version;

			assembly_resolver.ResetSearchLogger ();

			if (!TryGetSpecificVersionValue (item, out specific_version))
				return null;

			var spath_index  = 0;
			foreach (string spath in search_paths) {
				if (string.IsNullOrEmpty (spath))
					continue;
				assembly_resolver.LogSearchMessage ("For searchpath {0}", spath);

				// The first value of search_paths can be the parent assembly directory.
				// In that case the value would be treated as a directory.
				// This code checks if we should treat the value as a TargetFramework assembly.
				// Doing so avoids CopyLocal beeing set to true.
				if (spath_index++ == 0 && targetFrameworkDirectories != null) {
					foreach (string fpath in targetFrameworkDirectories) {
						if (string.IsNullOrEmpty (fpath))
							continue;
						if (String.Compare (
								Path.GetFullPath (spath).TrimEnd (Path.DirectorySeparatorChar),
								Path.GetFullPath (fpath).TrimEnd (Path.DirectorySeparatorChar),
								StringComparison.InvariantCulture) != 0)
							continue;

						resolved = assembly_resolver.FindInTargetFramework (item,
							fpath, specific_version);

						break;
					}

					if  (resolved != null)
						break;
				}

				if (String.Compare (spath, "{HintPathFromItem}") == 0) {
					resolved = assembly_resolver.ResolveHintPathReference (item, specific_version);
				} else if (String.Compare (spath, "{TargetFrameworkDirectory}") == 0) {
					if (targetFrameworkDirectories == null)
						continue;
					foreach (string fpath in targetFrameworkDirectories) {
						resolved = assembly_resolver.FindInTargetFramework (item,
								fpath, specific_version);
						if (resolved != null)
							break;
					}
				} else if (String.Compare (spath, "{GAC}") == 0) {
					resolved = assembly_resolver.ResolveGacReference (item, specific_version);
				} else if (String.Compare (spath, "{RawFileName}") == 0) {
					//FIXME: identify assembly names, as extract the name, and try with that?
					AssemblyName aname;
					if (assembly_resolver.TryGetAssemblyNameFromFile (item.ItemSpec, out aname))
						resolved = assembly_resolver.GetResolvedReference (item, item.ItemSpec, aname, true,
								SearchPath.RawFileName);
				} else if (String.Compare (spath, "{CandidateAssemblyFiles}") == 0) {
					assembly_resolver.LogSearchMessage (
							"Warning: {{CandidateAssemblyFiles}} not supported currently");
				} else if (String.Compare (spath, "{PkgConfig}") == 0) {
					resolved = assembly_resolver.ResolvePkgConfigReference (item, specific_version);
				} else {
					resolved = assembly_resolver.FindInDirectory (
							item, spath,
							allowedAssemblyExtensions ?? default_assembly_extensions,
							specific_version);
				}

				if (resolved != null)
					break;
			}

			if (resolved != null && set_copy_local)
				SetCopyLocal (resolved.TaskItem, resolved.CopyLocal.ToString ());

			return resolved;
		}

		bool TryGetSpecificVersionValue (ITaskItem item, out bool specific_version)
		{
			specific_version = true;
			string value = item.GetMetadata ("SpecificVersion");
			if (String.IsNullOrEmpty (value)) {
				//AssemblyName name = new AssemblyName (item.ItemSpec);
				// If SpecificVersion is not specified, then
				// it is true if the Include is a strong name else false
				//specific_version = assembly_resolver.IsStrongNamed (name);

				// msbuild seems to just look for a ',' in the name :/
				specific_version = item.ItemSpec.IndexOf (',') >= 0;
				return true;
			}

			if (Boolean.TryParse (value, out specific_version))
				return true;

			Log.LogError ("Item '{0}' has attribute SpecificVersion with invalid value '{1}' " +
					"which could not be converted to a boolean.", item.ItemSpec, value);
			return false;
		}

		void SetCopyLocal (ITaskItem item, string copy_local)
		{
			item.SetMetadata ("CopyLocal", copy_local);

			// Assumed to be valid value
			if (Boolean.Parse (copy_local))
			{
				// tempCopyLocalFiles.AddUniqueFile (item);
			}
		}

	/// <summary>
        /// Execute is part of the Microsoft.Build.Framework.ITask interface.
        /// When it's called, any input parameters have already been set on the task's properties.
        /// It returns true or false to indicate success or failure.
        /// </summary>
        public override bool Execute()
        {
		assembly_resolver.Log = this.Log;
		assembly_resolver.ResetSearchLogger ();
		//
		var outputItems = new List<ITaskItem>();
		foreach (var reference in References)
		{
			Log.LogMessage ("Reference \"{0}\"", $"{reference.ItemSpec}");

			TaskItem referenceTaskItem = new TaskItem() { ItemSpec = reference.ItemSpec };
			string specific_version = referenceTaskItem.GetMetadata("version");
			Log.LogMessage ("specific_version \"{0}\"", $"{specific_version}");

			ResolvedReference rr = ResolveReference(referenceTaskItem, searchPaths, true);
			TaskItem referencePath = new TaskItem() { ItemSpec = rr.TaskItem.ItemSpec };
			outputItems.Add(referencePath);
			Log.LogMessage ("referencePath \"{0}\"", $"{referencePath.ItemSpec}");
		}
		referencePaths = outputItems.ToArray();
		return true;
	}
}
