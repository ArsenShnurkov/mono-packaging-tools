//using System;
//using System.IO;
//using System.Security;
//using System.Collections;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

public class ResolveAssemblyReference : Task
{
	private ITaskItem[] packageReferences;
        private ITaskItem[] referencesComputed;

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

	// The Output attribute indicates to MSBuild that the value of this property can be gathered after the
        // task has returned from Execute(), 
	// if the project has an <Output> tag under this task's element for this property.
        [Output]
        // A project may need the subset of the inputs that were actually created, so make that available here.
        public ITaskItem[] References
        {
            get
            {
                return referencesComputed;
            }
        }

	/// <summary>
        /// Execute is part of the Microsoft.Build.Framework.ITask interface.
        /// When it's called, any input parameters have already been set on the task's properties.
        /// It returns true or false to indicate success or failure.
        /// </summary>
        public override bool Execute()
        {
		return true;
	}
}
