using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamClientWrapper.Configuration;
using System.Diagnostics;
using System.Linq;

namespace SteamClientWrapper.Tests
{
    [TestClass]
    public class ConfigurationWrapperTests
    {
        ConfigurationWrapper instance;

        [TestInitialize]
        public void TestInitialize()
        {
            SteamInfo info = new SteamInfo();
            info.Refresh();
            instance = info.Configuration;
        }

        [TestMethod]
        public void ConfigurationWrapper_Users()
        {
            var users = instance.Users;
            Assert.IsTrue(users.Count > 0);

            int userCount = users.Count;
            Trace.TraceInformation($"Found {userCount} users");

            foreach (var user in users.Select(kvp => kvp.Value))
            {
                Trace.TraceInformation($"UserID: {user.UserId}");
                Trace.TraceInformation($"   AcountName: {user.AcountName}");
                Trace.TraceInformation($"   PersonalName: {user.PersonalName}");
            }
        }

        [TestMethod]
        public void ConfigurationWrapper_ConfigsProperty()
        {
            Assert.AreNotEqual(0, instance.Configs.Count);

            Trace.TraceInformation($"{instance.Configs.Count} configs loaded.");

            foreach (var configName in instance.Configs.Keys)
            {
                Trace.TraceInformation($"Loaded config '{configName}'");
            }
        }
    }
}
