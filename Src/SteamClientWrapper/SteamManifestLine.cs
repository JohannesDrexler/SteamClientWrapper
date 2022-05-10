namespace SteamWrapper
{
    internal class SteamManifestLine
    {
        public SteamManifestLineType Type { get; }

        public string Name { get; }

        public string Value { get; }

        public SteamManifestLine(string content)
        {
            content = content.Trim();

            if (content.Contains("\""))
            {
                string[] splits = content.Split('"');

                if (splits.Length == 3)
                {
                    Type = SteamManifestLineType.LevelLabel;
                    Name = splits[1].Trim();
                }

                if (splits.Length == 5)
                {
                    Type = SteamManifestLineType.Value;
                    Name = splits[1];
                    Value = splits[3];
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
