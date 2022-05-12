namespace SteamClientWrapper
{
    /// <summary>
    /// Defines what kind of type a line in a manifest can represent
    /// </summary>
    public enum SteamManifestLineType
    {
        /// <summary>
        /// Undefined or unrecognizable
        /// </summary>
        Undefined,

        /// <summary>
        /// Value setting
        /// </summary>
        Value,

        /// <summary>
        /// Level label
        /// </summary>
        LevelLabel,

        /// <summary>
        /// Start bracket of a new label label
        /// </summary>
        LevelStart,

        /// <summary>
        /// Ending bracket of a level label
        /// </summary>
        LevelEnd
    }
}
