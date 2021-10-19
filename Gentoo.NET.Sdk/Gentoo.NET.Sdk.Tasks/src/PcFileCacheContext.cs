using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Mono.PkgConfig;

class PcFileCacheContext : IPcFileCacheContext<LibraryPackageInfo>
{
	public static TaskLoggingHelper Log = null;

	// In the implementation of this method, the host application can extract
	// information from the pc file and store it in the PackageInfo object
	public void StoreCustomData (PcFile pcfile, LibraryPackageInfo pkg)
	{
	}

	// Should return false if the provided package does not have required
	// custom data
	public bool IsCustomDataComplete (string pcfile, LibraryPackageInfo pkg)
	{
		return true;
	}

	// Called to report errors
	public void ReportError (string message, Exception ex)
	{
		// Log message is not an extension method
		// https://github.com/microsoft/msbuild/blob/d993d35514934744f0ef69674d17809c49572799/src/Shared/TaskLoggingHelper.cs#L271
		// so "Log" static member should be initialized
		if (Log != null)
		{
			Log.LogMessage (MessageImportance.Low, "Error loading pkg-config files: {0} : {1}", 
				message,
				ex.ToString ());
		}
	}
}
