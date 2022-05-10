using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SteamClientWrapper.Tests
{
    [TestClass]
    public class SteamInfoTests
    {
        SteamInfo info;

        [TestInitialize]
        public void Initialize()
        {
            info = new SteamInfo();
            Assert.IsTrue(info.SteamInstalled);

            if (info.SteamInstalled)
            {
                Assert.IsFalse(string.IsNullOrEmpty(info.SteamDirectory));

                //This code is only reached if assert-call above didn't throw any exception
                Trace.TraceInformation($"Steam is installed into direcory '{info.SteamDirectory}'");
            }
            else
            {
                Trace.TraceError($"SteamInfo doesn't recognice a steam installation");
            }
        }

        [TestMethod]
        public void SteamInfo_SteamInstalled()
        {
            //nothing, this is just to have the init-function wrapped up as single test
        }

        [TestMethod]
        public void SteamInfo_GetAllGames()
        {
            var games = info.GetAllGames();

            Assert.IsNotNull(games);
            Assert.IsTrue(games.Count > 0);
            games = games.OrderBy(g => g.Name).ToList();

            foreach (var game in games)
            {
                Trace.TraceInformation($"Game: '{game.Name}' State: '{game.State}' (ID: {game.AppId})");
            }
        }

        [TestMethod]
        public void SteamInfo_GetLibraries()
        {
            var libs = info.GetLibraries();

            Assert.IsNotNull(libs);
            Assert.IsTrue(libs.Count != 0);

            //log libraries
            foreach (var lib in libs)
            {
                Trace.TraceInformation($"Librarys location : '{lib.LibDirectory}'");
                foreach (var game in lib)
                {
                    Trace.TraceInformation($"    Contains game '{game.Name}' (ID: {game.AppId})");
                }
            }
        }

        [TestMethod]
        public void SteamInfo_GetLibraryPaths()
        {
            var paths = info.GetLibraryPaths();

            Assert.IsNotNull(paths);
            foreach (string path in paths)
            {
                Trace.TraceInformation($"Library found -> {path}");
                Assert.IsTrue(Directory.Exists(path));
            }

            Assert.IsTrue(paths.Count > 0);
        }

        [TestMethod]
        public void SteamInfo_GetLibraryPathsFromDocument()
        {
            var libraryConfig = EnvironmentHelper.GetSampleManifest("libraryfolders.vdf");
            var paths = info.GetLibraryPathsFromDocument(libraryConfig);

            Assert.IsNotNull(paths);
            Assert.AreEqual(2, paths.Count);

            foreach (string path in paths)
            {
                Trace.TraceInformation($"Library found -> {path}");
            }

            Assert.AreEqual(2, paths.Count);
        }
    }
}
