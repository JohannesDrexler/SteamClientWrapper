using SteamClientWrapper.Configuration;
using SteamClientWrapper.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SteamClientWrapper.Types
{
    /// <summary>
    /// This describes a library
    /// </summary>
    public class SteamLibrary : List<SteamGame>
    {
        /// <summary>
        /// ConfigurationWrapper to access steams configuration
        /// </summary>
        public ConfigurationWrapper ConfigurationWrapper { get; }

        /// <summary>
        /// Directory of the library
        /// </summary>
        public string LibDirectory { get; }

        private SteamLibrary()
        {
            //
        }

        /// <summary>
        /// Creates a new instance of a library
        /// </summary>
        /// <param name="directoryPath">Path to the library</param>
        /// <param name="configurationWrapper">ConfigurationWrapper to access shared configuration</param>
        internal SteamLibrary(string directoryPath, ConfigurationWrapper configurationWrapper)
                : this()
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            ConfigurationWrapper = configurationWrapper ?? throw new ArgumentNullException(nameof(configurationWrapper));

            LibDirectory = directoryPath;
        }

        /// <summary>
        /// Clears this collection and reloads all games in the directory
        /// </summary>
        public void Refresh()
        {
            if (Count != 0)
            {
                Clear();
            }

            string[] manifestFiles = Directory.GetFiles(LibDirectory, "*.acf", SearchOption.TopDirectoryOnly);

            List<SteamGame> games = new List<SteamGame>();
            foreach (string manifestPath in manifestFiles)
            {
                using (var stream = new FileStream(manifestPath, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        SteamManifest mf = new SteamManifest();
                        mf.Load(stream);

                        SteamGame game = new SteamGame(mf, this);
                        games.Add(game);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(Messages.ProcessingManifestFailed, manifestPath), ex);
                    }
                }
            }

            if (games.Count > 0)
            {
                var sortedGames = games.OrderBy(sg => sg.Name).ToArray();
                AddRange(sortedGames);
            }
        }

        /// <summary>
        /// Returns a SteamGame from this library by its appId
        /// </summary>
        /// <param name="appId">AppId of the game to retrieve</param>
        /// <returns>Returns the steamGame if possible, returns null if game can't be found.</returns>
        public SteamGame GetGameByAppId(int appId)
        {
            SteamGame game = null;

            foreach (SteamGame steamGame in this)
            {
                if (steamGame.AppId == appId)
                {
                    game = steamGame;
                    break;
                }
            }

            return game;
        }

        /// <summary>
        /// Counts and optionally deletes everything from the library that isn't supposed to be there
        /// </summary>
        /// <param name="delete">Set this to true to delete, otherwise this will only count</param>
        /// <returns>Returns a report of how much space can (or was cleaned up from the library)</returns>
        public CleanupReport Cleanup(bool delete = false)
        {
            var result = new CleanupReport();

            HashSet<string> expected = new HashSet<string>();
            foreach (SteamGame game in this)
            {
                expected.Add(Path.GetFileName(game.InstallDir));
            }

            string commonPath = Path.Combine(LibDirectory, "Common");
            if (Directory.Exists(commonPath))
            {
                string[] foundInCommon = Directory.GetDirectories(commonPath);
                foreach (var foundDir in foundInCommon)
                {
                    string dirName = Path.GetFileName(foundDir);
                    if (!expected.Contains(dirName))
                    {
                        ProcessDirectoryCleanup(result, foundDir, delete: delete);
                    }
                }
            }

            string workshopPath = Path.Combine(LibDirectory, "workshop");
            if (Directory.Exists(workshopPath))
            {
                string workshopContent = Path.Combine(workshopPath, "content");
                if (Directory.Exists(workshopContent))
                {
                    string[] foundInWorkshop = Directory.GetDirectories(workshopContent);
                    if (foundInWorkshop.Length > 0)
                    {
                        foreach (var workshopSubfolder in foundInWorkshop)
                        {
                            string appIdString = Path.GetFileName(workshopSubfolder);
                            if (int.TryParse(appIdString, out int appId))
                            {
                                SteamGame matchingGame = GetGameByAppId(appId);
                                if (matchingGame == null)
                                {
                                    ProcessDirectoryCleanup(result, workshopSubfolder, delete: delete);
                                }
                            }
                        }
                    }
                }

                string[] workshopManifests = Directory.GetFiles(workshopPath, "*.acf", SearchOption.TopDirectoryOnly);
                foreach (var workshopFile in workshopManifests)
                {
                    SteamManifest doc = new SteamManifest();
                    using (var docStream = new FileStream(workshopFile, FileMode.Open, FileAccess.Read))
                    {
                        doc.Load(docStream);
                    }

                    //skip this file if it can't be read
                    if (doc.PartiallyUndefinedContent)
                    {
                        continue;
                    }

                    string appIdFromDoc = doc.GetNodeValue("AppWorkshop", "appid");
                    if (int.TryParse(appIdFromDoc, out int appId))
                    {
                        SteamGame matchingGame = GetGameByAppId(appId);
                        if (matchingGame == null)
                        {
                            FileInfo fi = new FileInfo(workshopFile);
                            result.Files++;
                            result.Space += fi.Length;

                            if (delete)
                            {
                                fi.Delete();
                            }
                        }
                    }
                }
            }

            return result;
        }

        private void ProcessDirectoryCleanup(CleanupReport result, string foundDir, bool delete = false)
        {
            string[] files = Directory.GetFiles(foundDir, "*", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                result.Files += files.Length;
                foreach (var fil in files)
                {
                    FileInfo fi = new FileInfo(fil);
                    result.Space += fi.Length;

                    if (delete)
                    {
                        fi.Delete();
                    }
                }
            }

            string[] subDirectories = Directory.GetDirectories(foundDir, "*", SearchOption.AllDirectories);
            if (subDirectories.Length > 0)
            {
                result.Directories += subDirectories.Length;

                foreach (var subDir in subDirectories)
                {
                    if (delete && Directory.Exists(subDir))
                    {
                        Directory.Delete(subDir, true);
                    }
                }
            }

            result.Directories++;
            if (delete)
            {
                Directory.Delete(foundDir, true);
            }
        }

        /// <summary>
        /// Estimates how big a libary could be based on the game manifests of the library
        /// </summary>
        /// <returns>Returns an estimated size</returns>
        public long GetEstimatedLibSize()
        {
            long result = 0;

            foreach (var game in this)
            {
                result += game.SizeOnDisk;
            }

            return result;
        }
    }
}
