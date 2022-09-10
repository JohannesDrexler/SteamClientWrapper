using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SteamClientWrapper.Types
{
    /// <summary>
    /// Describes what can be cleaned up or was cleaned up
    /// </summary>
    public class CleanupReport
    {
        /// <summary>
        /// Diskspace in bytes
        /// </summary>
        [Obsolete("Use CalculateSpace() instead")]
        public long Space { get; internal set; }

        /// <summary>
        /// Number of files
        /// </summary>

        [Obsolete("Use FilePaths and AddFile() instead")]
        public int Files { get; internal set; }

        /// <summary>
        /// Number of directories
        /// </summary>
        [Obsolete("Use DirectoryPaths and AddDirectory instead")]
        public int Directories { get; internal set; }

        /// <summary>
        /// Internal list of full file paths
        /// </summary>
        private List<string> FilePathsInternal { get; }
            = new List<string>();

        /// <summary>
        /// List of files
        /// </summary>
        public IReadOnlyList<string> FilePaths => FilePathsInternal.AsReadOnly();

        /// <summary>
        /// Internal list of full directory paths
        /// </summary>
        private List<string> DirectoryPathsInternal { get; }
            = new List<string>();

        /// <summary>
        /// List of Directories
        /// </summary>
        public IReadOnlyList<string> DirectoryPaths => DirectoryPathsInternal.AsReadOnly();

        /// <summary>
        /// Calculates how much space the files take up
        /// </summary>
        /// <returns>Returns total filesize in bytes</returns>
        public long CalculateSpace()
        {
            long space = FilePathsInternal.Select(f => new FileInfo(f)).Sum(f => f.Length);

            return space;
        }

        /// <summary>
        /// Returns a boolean indicating if there is anything to delete
        /// </summary>
        public bool AnythingToDelete()
        {
            return DirectoryPaths.Count > 0 || FilePaths.Count > 0;
        }

        /// <summary>
        /// Merges cleanup reports together
        /// </summary>
        /// <param name="report">Report to include</param>
        public void AddReport(CleanupReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

#pragma warning disable CS0618 // Typ oder Element ist veraltet
            Files += report.Files;
            Directories += report.Directories;
            Space += report.Space;
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            foreach (string file in report.FilePaths)
            {
                AddFile(file);
            }
            foreach (string directory in report.DirectoryPaths)
            {
                AddDirectory(directory);
            }
        }

        /// <summary>
        /// Adds a file
        /// </summary>
        /// <param name="filePath">Full path of the file</param>
        public void AddFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                FilePathsInternal.Add(filePath);
            }
        }

        /// <summary>
        /// Adds a directory
        /// </summary>
        /// <param name="directoryPath">Full path of the directory</param>
        public void AddDirectory(string directoryPath)
        {
            if (!string.IsNullOrEmpty(directoryPath))
            {
                DirectoryPathsInternal.Add(directoryPath);
            }
        }

    }
}
