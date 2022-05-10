namespace SteamWrapper
{
    public interface ISteamController
    {
        SteamInfo Info { get; }

        bool IsRunning();
        void StartSteam();
        void StartSteamInBigPictureMode();
        void StopSteam();

        void StartGame(int appId);
    }
}
