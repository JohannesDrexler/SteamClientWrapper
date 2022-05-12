using System.Collections.Generic;

namespace SteamClientWrapper
{
    /// <summary>
    /// Describes a node in a steam manifest
    /// </summary>
    public class SteamManifestNode
    {
        /// <summary>
        /// Name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path of the node
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Values of the node
        /// </summary>
        public Dictionary<string, string> Values { get; set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Children of this node
        /// </summary>
        public Dictionary<string, SteamManifestNode> ChildNodes { get; set; }
            = new Dictionary<string, SteamManifestNode>();
    }
}
