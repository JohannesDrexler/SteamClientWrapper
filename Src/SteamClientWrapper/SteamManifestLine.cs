using System;
using System.Diagnostics;

namespace SteamClientWrapper
{
    /// <summary>
    /// Represents a line of a steam manifest
    /// </summary>
    [DebuggerDisplay("{Type} {Name}")]
    public class SteamManifestLine
    {
        /// <summary>
        /// Type of line
        /// </summary>
        public SteamManifestLineType Type { get; }

        /// <summary>
        /// Name of the line
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value (if this is a valuetype line)
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new line from the given string
        /// </summary>
        /// <param name="content">Line content that needs to be parsed</param>
        public SteamManifestLine(string content)
        {
            //NOTE: string.isnullorempty is intentionally not used here as a manifest can contain emtpy lines
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            content = content.Trim();

            if (content.Contains("\""))
            {
                string[] splits = content.Split('"');

                if (splits.Length == 3)
                {
                    Type = SteamManifestLineType.LevelLabel;
                    Name = splits[1].Trim().ToLowerInvariant();
                }

                if (splits.Length == 5)
                {
                    Type = SteamManifestLineType.Value;
                    Name = splits[1].ToLowerInvariant();
                    Value = splits[3].ToLowerInvariant();
                }
            }
            else
            {
                if (content == "{")
                {
                    Type = SteamManifestLineType.LevelStart;
                }

                if (content == "}")
                {
                    Type = SteamManifestLineType.LevelEnd;
                }
            }
        }
    }
}
