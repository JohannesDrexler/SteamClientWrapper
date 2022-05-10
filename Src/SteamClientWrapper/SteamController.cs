using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SteamClientWrapper
{
    public class SteamController : ISteamController
    {
        public SteamInfo Info { get; private set; }

        public SteamController()
        {
            Info = new SteamInfo();
        }

        public SteamController(SteamInfo info)
        {
            this.Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public bool IsRunning()
        {
            bool steamRunning = false;

            Process[] processes = Process.GetProcesses();
            steamRunning = processes.Where(p => p.ProcessName.ToLower() == "steam").Any();
            foreach (var p in processes)
            {
                p.Dispose();
            }

            return steamRunning;
        }

        public void StartSteam()
        {
            if (!IsRunning())
            {
                if (!Info.SteamInstalled)
                {
                    //cannot start steam when steam is not foud yet
                    return;
                }

                CallSteam(null);
            }
        }

        private Process CreateProcess()
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Info.SteamDirectory, "steam.exe")
                }
            };
            return process;
        }

        public void StartGame(int appId)
        {
            CallSteam($"-applaunch {appId}");
        }

        public void StopSteam()
        {
            if (IsRunning())
            {
                using (Process process = CreateProcess())
                {
                    process.StartInfo.Arguments = "-shutdown";
                    process.Start();
                }
            }
        }

        private void CallSteam(string args = null)
        {
            using (Process process = CreateProcess())
            {
                if (!string.IsNullOrEmpty(args))
                {
                    process.StartInfo.Arguments = args;
                }
                process.Start();
            }
        }

        public void StartSteamInBigPictureMode()
        {
            if (!IsRunning())
            {
                CallSteam("-tenfoot");
            }
        }
    }
}
