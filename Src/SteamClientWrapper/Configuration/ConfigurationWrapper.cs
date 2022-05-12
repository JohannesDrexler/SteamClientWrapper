using SteamClientWrapper.Resources;
using SteamClientWrapper.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SteamClientWrapper.Configuration
{
    /// <summary>
    /// This class accesses the clients configuration
    /// </summary>
    public class ConfigurationWrapper
    {
        private readonly Dictionary<string, SteamManifest> configs = new Dictionary<string, SteamManifest>();

        /// <summary>
        /// List of configuration files
        /// </summary>
        public IReadOnlyDictionary<string, SteamManifest> Configs
        {
            get => configs;
        }

        private readonly Dictionary<long, SteamUser> users = new Dictionary<long, SteamUser>();

        /// <summary>
        /// List of users using the client installation
        /// </summary>
        public IReadOnlyDictionary<long, SteamUser> Users
        {
            get => users;
        }

        /// <summary>
        /// Directory of the steam directory
        /// </summary>
        public string SteamDirectory { get; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="steamDirectory">Directory of the steam installation, must not be null or empty, directory must be accessable</param>
        public ConfigurationWrapper(string steamDirectory)
        {
            if (string.IsNullOrEmpty(steamDirectory))
            {
                throw new ArgumentNullException(nameof(steamDirectory));
            }
            if (!Directory.Exists(steamDirectory))
            {
                throw new DirectoryNotFoundException(string.Format(Messages.DirectoryNotFound, steamDirectory));
            }

            SteamDirectory = steamDirectory;
            Refresh();
        }

        /// <summary>
        /// Refreshes this instances caches and rereads data from configuration files
        /// </summary>
        public void Refresh()
        {
            Trace.TraceInformation(Messages.RefreshingConfigurationWrapper);

            RefreshConfigs();
            RefreshUsers();
        }

        /// <summary>
        /// Refreshes the configcache
        /// </summary>
        private void RefreshConfigs()
        {
            configs.Clear();

            string configFolder = Path.Combine(SteamDirectory, "config");
            string[] configFiles = Directory.GetFiles(configFolder, "*.vdf");

            foreach (var configPath in configFiles)
            {
                try
                {
                    using (var stream = new FileStream(configPath, FileMode.Open, FileAccess.Read))
                    {
                        SteamManifest configDocument = new SteamManifest();
                        configDocument.Load(stream);

                        //some of the configfiles are binary or sometimes corrupt (depending on how steam was shutdown)
                        if (configDocument.PartiallyUndefinedContent)
                        {
                            continue;
                        }

                        string configName = Path.GetFileName(configPath);
                        configs.Add(configName, configDocument);
                    }
                }
                catch (FileIsBinaryException fileIsBinaryEx)
                {
                    Trace.TraceWarning(string.Format(Messages.FailedToReadFileDueToBinary, configPath));

                    if (configPath.Contains("loginusers.vdf"))
                    {
                        throw new Exception(string.Format(Messages.FileIsCorrupt, configPath), fileIsBinaryEx);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(Messages.FailedToReadFile, configPath), ex);
                }
            }
        }

        /// <summary>
        /// Refreshes the user cache. Requires config cache to be initialized
        /// </summary>
        private void RefreshUsers()
        {
            users.Clear();

            string userConfigFule = "loginusers.vdf";

            if (Configs.TryGetValue(userConfigFule, out SteamManifest userConfig))
            {
                List<SteamUser> usersFromConfig = ReadUsersFromConfig(userConfig);

                foreach (var user in usersFromConfig)
                {
                    users.Add(user.UserId, user);
                }
            }
        }

        /// <summary>
        /// Reads the loginusers.vdf and returns a list of users
        /// </summary>
        /// <param name="doc">Document to read, must not be null</param>
        /// <returns>Returns a list of users</returns>
        private List<SteamUser> ReadUsersFromConfig(SteamManifest doc)
        {
            var result = new List<SteamUser>();

            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            SteamManifestNode usersNode = doc.GetNode("users");

            if (usersNode != null)
            {
                foreach (KeyValuePair<string, SteamManifestNode> userNodeKvp in usersNode.ChildNodes)
                {
                    SteamManifestNode userNode = userNodeKvp.Value;
                    long userId = long.Parse(userNode.Name);
                    string accountName = userNode.Values["AccountName"];
                    string personalName = userNode.Values["PersonaName"];
                    var user = new SteamUser(userId, accountName, personalName);
                    result.Add(user);
                }
            }

            return result;
        }
    }
}
