namespace SteamClientWrapper.Types
{
    /// <summary>
    /// Represents the status of a game
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game is installed
        /// </summary>
        Installed = 4,

        /// <summary>
        /// An update for the game is queued
        /// </summary>
        UpdateQueued = 38,

        /// <summary>
        /// The game is not installed
        /// </summary>
        Uninstalled = 68,

        /// <summary>
        /// The games installation is queued
        /// </summary>
        Queued = 1026,

        /// <summary>
        /// The game is currently updating
        /// </summary>
        Updating = 1030,

        /// <summary>
        /// The game is currently downloading
        /// </summary>
        Downloading = 1042
    }
}
