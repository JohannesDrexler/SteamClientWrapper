namespace SteamClientWrapper
{
    /// <summary>
    /// Interface describing what a steamcontroller should be able to do
    /// </summary>
    public interface ISteamController
    {
        /// <summary>
        /// Returns the associated steaminfo instance
        /// </summary>
        SteamInfo Info { get; }

        /// <summary>
        /// Returns true if steam is running
        /// </summary>
        /// <returns></returns>
        /// 
        bool IsRunning();

        /// <summary>
        /// Starts steam
        /// </summary>
        void StartSteam();

        /// <summary>
        /// Starts steam in big picture mode
        /// </summary>
        void StartSteamInBigPictureMode();

        /// <summary>
        /// Stops steam
        /// </summary>
        void StopSteam();

        /// <summary>
        /// Starts a game
        /// </summary>
        /// <param name="appId">Id of the game to start</param>
        void StartGame(int appId);
    }
}
