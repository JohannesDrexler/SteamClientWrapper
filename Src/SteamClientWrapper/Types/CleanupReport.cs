namespace SteamWrapper.Types
{
    /// <summary>
    /// Describes what can be cleaned up or was cleaned up
    /// </summary>
    public class CleanupReport
    {
        /// <summary>
        /// Diskspace in bytes
        /// </summary>
        public long Space { get; internal set; }

        /// <summary>
        /// Number of files
        /// </summary>
        public int Files { get; internal set; }

        /// <summary>
        /// Number of directories
        /// </summary>
        public int Directories { get; internal set; }

        /// <summary>
        /// Returns a boolean indicating if there is anything to delete
        /// </summary>
        public bool AnythingToDelete()
        {
            return (Space > 0) || (Files > 0) || (Directories > 0);
        }
    }
}
