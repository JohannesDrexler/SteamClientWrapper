using System.Collections.Generic;

namespace SteamWrapper
{
    public class SteamManifestNode
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public Dictionary<string, string> Values { get; set; }
            = new Dictionary<string, string>();

        public Dictionary<string, SteamManifestNode> ChildNodes { get; set; }
            = new Dictionary<string, SteamManifestNode>();
    }
}
