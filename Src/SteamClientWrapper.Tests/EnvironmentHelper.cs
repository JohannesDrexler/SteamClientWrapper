using SteamClientWrapper.Types;
using System.IO;
using System.Reflection;

namespace SteamClientWrapper.Tests
{
    public class EnvironmentHelper
    {
        public static Stream GetSample(string sampleName)
        {
            string resourceName = $"SteamClientWrapper.Tests.Samples.{sampleName}";
            var resourceStream = Assembly.GetAssembly(typeof(EnvironmentHelper)).GetManifestResourceStream(resourceName);
            if (resourceStream == null)
            {
                throw new FileNotFoundException($"Requested sample '{resourceName}' is not embedded in this assembly");
            }
            return resourceStream;
        }

        public static SteamManifest GetSampleManifest(string sampleName)
        {
            SteamManifest manifest = new SteamManifest();
            using (Stream sampleStream = GetSample(sampleName))
            {
                manifest.Load(sampleStream);
            }
            return manifest;
        }

        public static SteamGame GetSampleGame(string sampleName)
        {
            SteamManifest manifest = GetSampleManifest(sampleName);
            SteamGame game = new SteamGame(manifest);
            return game;
        }
    }
}
