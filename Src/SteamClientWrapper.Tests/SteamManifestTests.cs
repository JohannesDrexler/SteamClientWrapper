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
        public void SteamManifest_GetNodeValue_ValueNotFound()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            string nodeValue = csGoManifest.GetNodeValue("AppState/UserConfig", "keyWillNotBeFound");
            Assert.IsTrue(string.IsNullOrEmpty(nodeValue));
        }

        [TestMethod]
        public void SteamManifest_GetNode()
        {
            SteamManifest csGoManifest = GetCsGoManifest();

            SteamManifestNode node = csGoManifest.GetNode("AppState/UserConfig");
            Assert.IsNotNull(node);
            Assert.AreEqual("AppState/UserConfig", node.Path);
            Assert.AreEqual("UserConfig", node.Name);
            Assert.IsTrue(node.Values.Count == 1);
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
