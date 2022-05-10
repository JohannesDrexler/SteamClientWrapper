namespace SteamClientWrapper.Types
{
    /// <summary>
    /// Represents the configured autoupdate-behaviour
    /// </summary>
    public enum AutoUpdateBehaviour
    {
        /// <summary>
        /// Keep the game updated
        /// </summary>
        KeepGameUpdated = 0,

        /// <summary>
        /// Only update the game when launching
        /// </summary>
        OnlyUpdateOnLaunch = 1,

        /// <summary>
        /// Update this game before everything else
        /// </summary>
        HighPriority = 2
    }
}
