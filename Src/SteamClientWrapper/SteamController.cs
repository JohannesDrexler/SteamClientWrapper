using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SteamClientWrapper
{
    /// <summary>
    /// Steamcontroller implementation
    /// </summary>
    public class SteamController : ISteamController
    {
        /// <summary>
        /// SteamInfo instance
        /// </summary>
        public SteamInfo Info { get; private set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public SteamController()
        {
            Info = new SteamInfo();
        }

        /// <summary>
        /// Returns true if steam is running
        /// </summary>
        /// <returns>True if it is running, otherwise false</returns>
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

        /// <summary>
        /// Starts steam
        /// </summary>
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

        /// <summary>
        /// Create a process instance 
        /// </summary>
        /// <returns>Returns a process instance handling steam.exe</returns>
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

        /// <summary>
        /// Starts a game
        /// </summary>
        /// <param name="appId">Id of the game to start</param>
        public void StartGame(int appId)
        {
            if (appId > 0)
            {
                CallSteam($"-applaunch {appId}");
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(appId));
            }
        }

        /// <summary>
        /// Stops steam
        /// </summary>
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

        /// <summary>
        /// Calls steam with the specified arguments
        /// </summary>
        /// <param name="args">Arguments</param>
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

        /// <summary>
        /// Starts steam in big picture mode
        /// </summary>
        public void StartSteamInBigPictureMode()
        {
            if (!IsRunning())
            {
                CallSteam("-tenfoot");
            }
        }
    }
}
