using System;
//using System.IO;
//using System.Security;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Mono.PkgConfig;

public class ResolvePackageReferencesViaPkgConfig : Task
{
	private ITaskItem[] packageReferences;

        // The Required attribute indicates the following to MSBuild:
        //	     - if the parameter is a scalar type, and it is not supplied, fail the build immediately
        //	     - if the parameter is an array type, and it is not supplied, pass in an empty array
        // In this case the parameter is an array type, so if a project fails to pass in a value for the
        // PackageReferences parameter, the task will get invoked, but this implementation will do nothing,
        // because the array will be empty.
	[Required]
        // PackageReferences to resolve.
        public ITaskItem[] PackageReferences
        {
            get
            {
                return packageReferences;
            }

            set
            {
                packageReferences = value;
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

	/// <summary>
        /// Execute is part of the Microsoft.Build.Framework.ITask interface.
        /// When it's called, any input parameters have already been set on the task's properties.
        /// It returns true or false to indicate success or failure.
        /// </summary>
        public override bool Execute()
        {
		var assembly_resolver = new AssemblyResolver ();
		assembly_resolver.Log = this.Log;
		assembly_resolver.ResetSearchLogger ();
		//
		var outputItems = new List<ITaskItem>();
		foreach (var packageReference in PackageReferences)
		{
			Log.LogMessage ("PackageReference \"{0}\"", $"{packageReference.ItemSpec}");

			TaskItem reference = new TaskItem() { ItemSpec = packageReference.ItemSpec };
			string specific_version = packageReference.GetMetadata("version");
			Log.LogMessage ("specific_version \"{0}\"", $"{specific_version}");
			IEnumerable<ResolvedReference> refs = assembly_resolver.GetReferencesForPackage(reference, specific_version);
			int count = 0;
			foreach (ResolvedReference rr in refs)
			{
				TaskItem referencePath = new TaskItem() { ItemSpec = rr.TaskItem.ItemSpec };
				outputItems.Add(referencePath);
				Log.LogMessage ("referencePath \"{0}\"", $"{referencePath.ItemSpec}");
				count ++;
			}
			Log.LogMessage("{0} refs added", count);
		}
		referencePaths = outputItems.ToArray();
		return true;
	}
}
