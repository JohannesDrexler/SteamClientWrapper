using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamClientWrapper.Types;
using System.Diagnostics;
using System.Linq;

namespace SteamClientWrapper.Tests
{
    [TestClass]
    public class SteamLibraryTests
    {
        private SteamLibrary library;

        [TestInitialize]
        public void Initialize()
        {
            var steamInfo = new SteamInfo();
            library = steamInfo.GetLibraries().First();
        }

        [TestMethod]
        public void SteamLibrary_GetEstimatedLibrarySize()
        {
            long estSize = library.GetEstimatedLibSize();
            Assert.IsTrue(estSize > 0);
            Trace.TraceInformation($"Library '{library.LibDirectory}' has an estimated size of '{estSize}'b");
        }
    }
}
