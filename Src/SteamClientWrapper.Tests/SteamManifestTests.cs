using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SteamClientWrapper.Tests
{
    [TestClass]
    public class SteamManifestTests
    {
        private SteamManifest GetCsGoManifest()
        {
            SteamManifest csGoManifest = new SteamManifest();
            var sample = EnvironmentHelper.GetSample("appmanifest_730_CSGO.acf");
            csGoManifest.Load(sample);
            return csGoManifest;
        }

        [TestMethod]
        public void SteamManifest_GetNodeValue()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            string nodeValue = csGoManifest.GetNodeValue("AppState/UserConfig", "language");
            Assert.IsFalse(string.IsNullOrEmpty(nodeValue));
        }

        [TestMethod]
        public void SteamManifest_GetNodeValue_CanHandleCaseInsensitivity()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            string nodeValue = csGoManifest.GetNodeValue("appstate/userConfig", "language");
            Assert.IsFalse(string.IsNullOrEmpty(nodeValue));
        }

        [TestMethod]
        public void SteamManifest_GetNodeValue_ValueNotFound()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            string nodeValue = csGoManifest.GetNodeValue("AppState/UserConfig", "keyWillNotBeFound");
            Assert.IsTrue(string.IsNullOrEmpty(nodeValue));

            string nodeValue2 = csGoManifest.GetNodeValue("AppState/ThisNodeDoesNotExist", "this key does not exist either");
            Assert.IsTrue(string.IsNullOrEmpty(nodeValue2));
        }

        [TestMethod]
        public void SteamManifest_GetNode()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            SteamManifestNode node = csGoManifest.GetNode("AppState/UserConfig");
            Assert.IsNotNull(node);
            Assert.IsTrue(node.Values.Count == 1);
        }

        [TestMethod]
        public void SteamManifest_GetNode_CanHandleCaseInsensitivity()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            SteamManifestNode node = csGoManifest.GetNode("appstate/userConfig");
            Assert.IsNotNull(node);
            Assert.IsTrue(node.Values.Count == 1);
        }

        [TestMethod]
        public void SteamManifest_GetNode_NotFound()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            SteamManifestNode node = csGoManifest.GetNode("thisNodeDoesNotExist");
            Assert.IsNull(node);

            SteamManifestNode subNode = csGoManifest.GetNode("AppState/ThisSubNodeDoesNotExistEither");
            Assert.IsNull(subNode);

            SteamManifestNode subNodeLevel3 = csGoManifest.GetNode("AppState/InstalledDepots/subLevelTestToTestRecursiveFunction");
            Assert.IsNull(subNodeLevel3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SteamManifest_GetNodeValue_ArgumentExceptionNodePath()
        {
            SteamManifest csGoManifest = GetCsGoManifest();
            csGoManifest.GetNodeValue(null, "installdir");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SteamManifest_GetNodeValue_ArgumentExceptionValueName()
        {
            SteamManifest csGoManifest = GetCsGoManifest();
            csGoManifest.GetNodeValue("AppState", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SteamManifest_GetNode_ArgumentException()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            SteamManifestNode node = csGoManifest.GetNode(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FileIsBinaryException))]
        public void SteamManifest_ThrowFileIsBinaryException()
        {
            SteamManifest manifest = new SteamManifest();
            manifest.Load(EnvironmentHelper.GetSample("coplay_76561198058922330.vdf"));
        }
    }
}
