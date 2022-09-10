using Microsoft.Win32;
using SteamClientWrapper.Configuration;
using SteamClientWrapper.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SteamClientWrapper
{
    /// <summary>
    /// This class accesses steams client
    /// </summary>
    public class SteamInfo
    {
        /// <summary>
        /// Returns wether a steam-installation could be found
        /// </summary>
        public bool SteamInstalled
        {
            get { return !string.IsNullOrEmpty(SteamDirectory); }
        }

        /// <summary>
        /// Path of the Steam-installation. Is String.Empty if no installation was found
        /// </summary>
        public string SteamDirectory { get; private set; }

        private ConfigurationWrapper _configuration;

        /// <summary>
        /// ConfigurationWrapper to read steams configuration
        /// </summary>
        public ConfigurationWrapper Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new ConfigurationWrapper(SteamDirectory);
                }

                return _configuration;
            }
            private set
            {
                _configuration = value;
            }
        }

        /// <summary>
        /// Creates a new instance of SteamInfo. Calls 'Refresh()' on creation
        /// </summary>
        public SteamInfo()
        {
            Refresh();
        }

        /// <summary>
        /// Reloads keys from Registry to check the status of the steam-installation again
        /// and re-initializes other members like 'ConfigurationWrapper'
        /// </summary>
        public void Refresh()
        {
            //reset some properties,fields
            SteamDirectory = string.Empty;
            Configuration = null;

            object installpath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);

            //check if the key was found and then try to read directory-location
            bool stFound = (installpath != null);
            if (stFound)
            {
                string steamDir = (string)installpath;

                // Normalize path
                steamDir = Path.GetFullPath(steamDir);

                // Now make sure the directry as well as steam.exe both exist
                stFound = Directory.Exists(steamDir) && File.Exists(Path.Combine(steamDir, "steam.exe"));

                // Set result if double checking paths was successfull
                if (stFound)
                {
                    SteamDirectory = steamDir;
                }
            }
        }

        /// <summary>
        /// Returns a list of all library paths
        /// </summary>
        public List<string> GetLibraryPaths()
        {
            if (!SteamInstalled)
            {
                return new List<string>();
            }
            else
            {
                List<string> paths = new List<string>
                {
                    Path.Combine(SteamDirectory, "steamapps")
                };

                string hopConfig = Path.Combine(paths[0], "libraryfolders.vdf");
                if (File.Exists(hopConfig))
                {
                    using (var stream = new FileStream(hopConfig, FileMode.Open, FileAccess.Read))
                    {
                        SteamManifest doc = new SteamManifest();
                        doc.Load(stream);

                        paths = GetLibraryPathsFromDocument(doc);
                    }
                }

                //alphabetical order
                paths = paths.OrderBy(str => str).ToList();

                return paths;
            }
        }

        /// <summary>
        /// Reads the library paths from the given libraryfolders.vdf
        /// </summary>
        /// <param name="doc">Docment containing the files content of libraryfolder.vdf</param>
        /// <returns>Returns a list of library paths</returns>
        public List<string> GetLibraryPathsFromDocument(SteamManifest doc)
        {
            List<string> paths = new List<string>();

            var libFoldersNode = doc.GetNode("libraryfolders");
            var pathsValuesPairs = libFoldersNode.ChildNodes.Where(kvp => int.TryParse(kvp.Key, out int tmpVal));
            if (pathsValuesPairs.Any())
            {
                foreach (string rawPath in pathsValuesPairs.Select(kvp => Path.Combine(kvp.Value.Values["path"], "SteamApps")))
                {
                    string refactoredPath = rawPath.Replace("\\\\", "\\").ToLower();
                    paths.Add(refactoredPath);
                }
            }

            return paths;
        }

        /// <summary>
        /// Returns all Libraries
        /// </summary>
        public List<SteamLibrary> GetLibraries()
        {
            List<SteamLibrary> libs = new List<SteamLibrary>();

            IEnumerable<string> libPaths = GetLibraryPaths();
            foreach (string str in libPaths)
            {
                SteamLibrary lib = new SteamLibrary(str, Configuration);
                lib.Refresh();

                libs.Add(lib);
            }

            return libs;
        }

        /// <summary>
        /// Returns all games from all libraries
        /// </summary>
        public List<SteamGame> GetAllGames()
        {
            List<SteamGame> games = new List<SteamGame>();

            var libraries = GetLibraries();
            foreach (SteamLibrary lib in libraries)
            {
                games.AddRange(lib);
            }

            //order games by their name
            games = games.OrderBy(g => g.Name).ToList();

            return games;
        }

        /// <summary>
        /// Returns a filtered List of games that are currently updating or installing
        /// </summary>
        public List<SteamGame> GetUpdatingOrInstallingGames()
        {
            List<SteamGame> games = GetAllGames();

            games = games.Where(g => g.State != GameState.Installed && g.State != GameState.Uninstalled).ToList();

            return games;
        }

        /// <summary>
        /// Counts and optionally deletes everything from the libraries that isn't supposed to be there
        /// </summary>
        /// <param name="delete">Set this to true to delete everything that isn't supposed there</param>
        /// <returns>A summarized report accross all libraries</returns>
        [Obsolete("This function will be removed in a later release. Function can be done better and will be reworked at a later time")]
        public async Task<CleanupReport> CleanupLibrariesAsync(bool delete = false)
        {
            CleanupReport report = await Task.Run(() => this.CleanupLibraries(delete));
            return report;
        }

        /// <summary>
        /// Counts and optionally deletes everything from the libraries that isn't supposed to be there
        /// </summary>
        /// <param name="delete">Set this to true to delete everything that isn't supposed there</param>
        /// <returns>A summarized report accross all libraries</returns>
        public CleanupReport CleanupLibraries(bool delete = false)
        {
            CleanupReport result = new CleanupReport();

            List<SteamLibrary> libraries = GetLibraries();
            foreach (var lib in libraries)
            {
                CleanupReport report = lib.Cleanup(delete: delete);

                result.AddReport(report);
            }

            return result;
        }
    }
}
