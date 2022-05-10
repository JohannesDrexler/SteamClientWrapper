using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamClientWrapper.Types;

namespace SteamClientWrapper.Tests
{
    [TestClass]
    public class SteamGameTests
    {
        const string manifestDestiny2 = "appmanifest_1085660_Destiny2.acf";
        const string manifestDoomEternal = "appmanifest_782330_DoomEternal.acf";
        const string manifestHowToSurvive = "appmanifest_250400_howToSurvive.acf";

        [TestMethod]
        public void AutoUpdateBehaviourProperty()
        {
            SteamGame destiny2Manifest = EnvironmentHelper.GetSampleGame(manifestDestiny2);
            Assert.AreEqual(AutoUpdateBehaviour.KeepGameUpdated, destiny2Manifest.AutoUpdateBehaviour);

            SteamGame doomEternal = EnvironmentHelper.GetSampleGame(manifestDoomEternal);
            Assert.AreEqual(AutoUpdateBehaviour.HighPriority, doomEternal.AutoUpdateBehaviour);

            SteamGame howToSurvive = EnvironmentHelper.GetSampleGame(manifestHowToSurvive);
            Assert.AreEqual(AutoUpdateBehaviour.OnlyUpdateOnLaunch, howToSurvive.AutoUpdateBehaviour);
        }
    }
}
