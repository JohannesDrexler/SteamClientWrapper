using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SteamClientWrapper
{
    public class SteamManifest
    {
        public bool PartiallyUndefinedContent { get; private set; }

        private Dictionary<string, SteamManifestNode> Nodes { get; }
            = new Dictionary<string, SteamManifestNode>();

        /// <summary>
        /// Tries to load a steamManifest from stream
        /// </summary>
        /// <exception cref="SteamWrapper.Exceptions.FileIsBinaryException">Thrown when filecontent is binary</exception>
        /// <param name="stream">Stream containing the document</param>
        public void Load(Stream stream)
        {
            var lines = new List<string>();
            using (var streamReader = new StreamReader(stream))
            {
                while (!streamReader.EndOfStream)
                {
                    var nextLine = streamReader.ReadLine();

                    if (nextLine.Contains("\0"))
                    {
                        throw new FileIsBinaryException();
                    }

                    lines.Add(nextLine);
                }
            }

            var linesParsed = new List<SteamManifestLine>();
            foreach (var line in lines)
            {
                var parsedLine = new SteamManifestLine(line);
                if (parsedLine.Type != SteamManifestLineType.Undefined)
                {
                    linesParsed.Add(parsedLine);
                }
                else
                {
                    PartiallyUndefinedContent = true;
                }
            }

            SteamManifestPathHelper pathHelper = new SteamManifestPathHelper();

            for (int i = 0; i < linesParsed.Count; i++)
            {
                SteamManifestLine ml = linesParsed[i];

                switch (ml.Type)
                {
                    case SteamManifestLineType.LevelLabel:
                        pathHelper.AddLevel(ml.Name);
                        var node = new SteamManifestNode()
                        {
                            Name = ml.Name,
                            Path = pathHelper.GetCurrentPath()
                        };
                        if (!node.Path.Contains('/'))
                        {
                            Nodes.Add(node.Name, node);
                        }
                        else
                        {
                            string parentNodePath = pathHelper.GetParentPath();
                            if (!string.IsNullOrEmpty(parentNodePath))
                            {
                                SteamManifestNode parentNode = GetNode(parentNodePath);
                                parentNode.ChildNodes.Add(node.Name, node);
                            }
                            else
                            {
                                //happens when config is corrupted
                                PartiallyUndefinedContent = true;
                            }
                        }
                        break;

                    case SteamManifestLineType.LevelStart:
                        //nothing to to here
                        break;

                    case SteamManifestLineType.LevelEnd:
                        pathHelper.RemoveLastLevel();
                        break;

                    case SteamManifestLineType.Value:
                        string currentPath = pathHelper.GetCurrentPath();
                        if (!string.IsNullOrEmpty(currentPath))
                        {
                            SteamManifestNode currentNode = GetNode(currentPath);
                            currentNode.Values.Add(ml.Name, ml.Value);
                        }
                        else
                        {
                            //happens when config is corrupted
                            PartiallyUndefinedContent = true;
                        }

                        break;
                }
            }
        }

        public string GetNodeValue(string nodePath, string keyName)
        {
            SteamManifestNode node = GetNode(nodePath);
            if (node.Values.TryGetValue(keyName, out string result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        public SteamManifestNode GetNode(string nodePath)
        {
            if (string.IsNullOrEmpty(nodePath))
            {
                throw new ArgumentNullException(nameof(nodePath));
            }

            string[] pathSegments = nodePath.Split('/');
            SteamManifestNode node = Nodes[pathSegments[0]];
            pathSegments = pathSegments.Skip(1).ToArray();
            foreach (var segment in pathSegments)
            {
                node = node.ChildNodes[segment];
            }
            return node;
        }
    }
}
