﻿using SteamClientWrapper.Resources;
using System;
using System.Diagnostics;
using System.IO;

namespace SteamClientWrapper.Types
{
    /// <summary>
    /// This describes a game installed in steam.
    /// This model is based on the app-manifest found in the steam-library
    /// </summary>
    [DebuggerDisplay("{Name} ({AppId}) (State: {State})")]
    public class SteamGame
    {
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

        /// <summary>
        /// Gets the diskspace occupied by the game
        /// </summary>
        public long SizeOnDisk
        {
            get
            {
                bool parsed = long.TryParse(Manifest.GetNodeValue("AppState", "SizeOnDisk"), out long space);
                return parsed ? space : 0;
            }
        }

        /// <summary>
        /// Returns the path where this game is installed. 
        /// If no library is associated the directory won't be full resolved.
        /// The value is in lower case
        /// </summary>
        public string InstallDir
        {
            get
            {
                string result = Manifest.GetNodeValue("AppState", "installdir");
                if (ParentLibrary != null)
                {
                    string libraryDir = ParentLibrary.LibDirectory;
                    return Path.Combine(libraryDir, "Common", result);
                }
                else
                {
                    return result;
                }
            }
        }


        /// <summary>
        /// Returns the State of the game
        /// </summary>
        public GameState State
        {
            get
            {
                bool parsed = Enum.TryParse(Manifest.GetNodeValue("AppState", "StateFlags"), out GameState result);
                return parsed ? result : GameState.Downloading;
            }
        }

        /// <summary>
        /// Returns the Autoupdatebehavior of the game
        /// </summary>
        public AutoUpdateBehaviour AutoUpdateBehaviour
        {
            get
            {
                bool parsed = Enum.TryParse(Manifest.GetNodeValue("AppState", "AutoUpdateBehavior"), out AutoUpdateBehaviour behaviour);
                return parsed ? behaviour : AutoUpdateBehaviour.KeepGameUpdated;
            }
        }

        /// <summary>
        /// Gets the underlying Manifest for this installed game
        /// </summary>
        public SteamManifest Manifest { get; private set; }

        /// <summary>
        /// Gets the associated library where this game is installed to
        /// </summary>
        public SteamLibrary ParentLibrary { get; private set; }

        /// <summary>
        /// Initializes a new SteamGame from the given Manifest-document
        /// </summary>
        /// <param name="manifest">The appManifest for the game. commonly found uner steam\steamapps\app_XXXX.acf</param>
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
        /// <param name="parentLibrary">Associated library instance</param>
        internal SteamGame(SteamManifest manifest, SteamLibrary parentLibrary)
            : this(manifest)
        {
            ParentLibrary = parentLibrary ?? throw new ArgumentNullException(nameof(parentLibrary));
        }

        /// <summary>
        /// Returns the icons file path of the game. This can only with a library associated
        /// </summary>
        /// <returns>Returns the path of the icon for this game</returns>
        public string GetIconFilePath()
        {
            string iconPath = Path.Combine(Path.Combine(ParentLibrary.ConfigurationWrapper.SteamDirectory, "appcache"), "librarycache");
            iconPath = Path.Combine(iconPath, $"{AppId}_icon.jpg");
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
