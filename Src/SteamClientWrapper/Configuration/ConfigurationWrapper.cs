using SteamClientWrapper.Resources;
using SteamClientWrapper.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SteamClientWrapper.Configuration
{
    public class ConfigurationWrapper
    {
        private readonly Dictionary<string, SteamManifest> configs = new Dictionary<string, SteamManifest>();
        public IReadOnlyDictionary<string, SteamManifest> Configs
        {
            get => configs;
        }

        private readonly Dictionary<long, SteamUser> users = new Dictionary<long, SteamUser>();
        public IReadOnlyDictionary<long, SteamUser> Users
        {
            get => users;
        }

        public string SteamDirectory { get; }

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

        public void Refresh()
        {
            Trace.TraceInformation(Messages.RefreshingConfigurationWrapper);

            //configs
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

            //users
            users.Clear();
            SteamManifest userConfig = Configs["loginusers.vdf"];
            List<SteamUser> usersFromConfig = ReadUsersFromConfig(userConfig);
            foreach (var user in usersFromConfig)
            {
                users.Add(user.UserId, user);
            }
        }

        private List<SteamUser> ReadUsersFromConfig(SteamManifest doc)
        {
            var result = new List<SteamUser>();
            SteamManifestNode usersNode = doc.GetNode("users");
            foreach (KeyValuePair<string, SteamManifestNode> userNodeKvp in usersNode.ChildNodes)
            {
                SteamManifestNode userNode = userNodeKvp.Value;
                long userId = long.Parse(userNode.Name);
                string accountName = userNode.Values["AccountName"];
                string personalName = userNode.Values["PersonaName"];
                var user = new SteamUser(userId, accountName, personalName);
                result.Add(user);
            }

            return result;
        }
    }
}
