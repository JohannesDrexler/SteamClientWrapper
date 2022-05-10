using SteamWrapper.Configuration;
using SteamWrapper.Resources;
using System;
using System.Diagnostics;
using System.IO;

namespace SteamWrapper.Types
{
    /// <summary>
    /// This describes a game installed in steam.
    /// This model is based on the app-manifest found in the steam-library
    /// </summary>
    [DebuggerDisplay("{Name} ({AppId}) (State: {State})")]
    public class SteamGame
    {
        #region fields and properties

        /// <summary>
        /// Gets the unique identifier for this game.
        /// This Id is used by Valve in steam and everythere else
        /// </summary>
        public int AppId { get; private set; }

        /// <summary>
        /// Gets Name of the Game
        /// </summary>
        public string Name
        {
            get { return Manifest.GetNodeValue("AppState", "name"); }
        }

        public long SizeOnDisk
        {
            get { return long.Parse(Manifest.GetNodeValue("AppState", "SizeOnDisk")); }
        }

        public string InstallDir
        {
            get
            {
                string result = Manifest.GetNodeValue("AppState", "installdir");
                string libraryDir = ParentLibary.LibDirectory;
                return Path.Combine(libraryDir, "Common", result);
            }
        }

        public int StateFlags
        {
            get { return int.Parse(Manifest.GetNodeValue("AppState", "StateFlags")); }
        }

        public GameState State
        {
            get { return (GameState)Enum.Parse(typeof(GameState), StateFlags.ToString()); }
        }

        public int AutoUpdateBehaviourFlags
        {
            get { return int.Parse(Manifest.GetNodeValue("AppState", "AutoUpdateBehavior")); }
        }

        public AutoUpdateBehaviour AutoUpdateBehaviour
        {
            get { return (AutoUpdateBehaviour)Enum.Parse(typeof(AutoUpdateBehaviour), AutoUpdateBehaviourFlags.ToString()); }
        }

        public SteamUser LastOwner { get; private set; }

        /// <summary>
        /// Gets the underlying Manifest for this installed game
        /// </summary>
        public SteamManifest Manifest { get; private set; }

        public SteamLibrary ParentLibary { get; private set; }

        #endregion

        public SteamGame(SteamManifest manifest)
        {
            Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));

            string appIdValue = manifest.GetNodeValue("AppState", "appid");
            if (string.IsNullOrEmpty(appIdValue) || !int.TryParse(appIdValue, out int appID))
            {
                throw new Exception(Messages.InvalidManifest + "" + Messages.InvalidAppId);
            }

            AppId = appID;
        }

        /// <summary>
        /// Initializes a new SteamGame from the given Manifest-document
        /// </summary>
        /// <param name="manifest">The appManifest for the game. commonly found uner steam\steamapps\app_XXXX.acf</param>
        public SteamGame(SteamManifest manifest, ConfigurationWrapper cfgWrapper, SteamLibrary parentLibrary)
            : this(manifest)
        {
            ParentLibary = parentLibrary ?? throw new ArgumentNullException(nameof(parentLibrary));

            if (cfgWrapper == null)
            {
                throw new ArgumentNullException(nameof(cfgWrapper));
            }

            string lastOwnerId = manifest.GetNodeValue("AppState", "LastOwner");
            if ((!string.IsNullOrEmpty(lastOwnerId)) && (long.TryParse(lastOwnerId, out long ownerId)))
            {
                if (cfgWrapper.Users.TryGetValue(ownerId, out SteamUser user))
                {
                    LastOwner = user;
                }
            }
        }

        private string GetIconFileName()
        {
            string fileName = $"{AppId}_icon.jpg";
            return fileName;
        }

        public string GetIconFilePath()
        {
            string iconPath = Path.Combine(Path.Combine(this.ParentLibary.ConfigurationWrapper.SteamDirectory, "appcache"), "librarycache");
            iconPath = Path.Combine(iconPath, this.GetIconFileName());
            return iconPath;
        }

        /// <summary>
        /// Overrides the ToString() function to return the name of the game beeing represented here
        /// </summary>
        /// <returns>Returns the name of the game. If reading the games name fails the name of this type is returned</returns>
        public override string ToString()
        {
            string ret = base.ToString();

            try
            {
                ret = this.Name;
            }
            catch
            {
                //don't care for exception in this case
            }
            return ret;
        }
    }
}
