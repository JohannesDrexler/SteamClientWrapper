using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SteamClientWrapper
{
    /// <summary>
    /// This class can be used to read steams configuration and manifest files
    /// </summary>
    public class SteamManifest
    {
        /// <summary>
        /// Indicates if the vdf file is readably or partially binary
        /// </summary>
        public bool PartiallyUndefinedContent { get; private set; }

        /// <summary>
        /// Map of the documents nodes
        /// </summary>
        private Dictionary<string, SteamManifestNode> Nodes { get; }
            = new Dictionary<string, SteamManifestNode>();

        /// <summary>
        /// Tries to load a steamManifest from stream
        /// </summary>
        /// <exception cref="FileIsBinaryException">Thrown when filecontent is binary</exception>
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

        /// <summary>
        /// Returns a nodes value
        /// </summary>
        /// <param name="nodePath">Path of the node</param>
        /// <param name="keyName">Name of the value</param>
        /// <returns>Returns either a resolved name of null</returns>
        public string GetNodeValue(string nodePath, string keyName)
        {
            SteamManifestNode node = GetNode(nodePath);

            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }

            if (node != null && node.Values.TryGetValue(keyName.ToLower(), out string result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a node
        /// </summary>
        /// <param name="nodePath">Path of the node, f.ex.: "AppState/UserConfig"</param>
        /// <returns>Returns the specified node if found, otherwise null</returns>
        public SteamManifestNode GetNode(string nodePath)
        {
            if (string.IsNullOrEmpty(nodePath))
            {
                throw new ArgumentNullException(nameof(nodePath));
            }

            //Split into path segments and first first node
            string[] pathSegments = nodePath.ToLower().Split('/');
            if (Nodes.TryGetValue(pathSegments[0], out SteamManifestNode node))
            {
                //Check if the nodepath has sublevels
                if (nodePath.Contains('/'))
                {
                    pathSegments = pathSegments.Skip(1).ToArray();

                    foreach (var segment in pathSegments)
                    {
                        if (node.ChildNodes.TryGetValue(segment, out SteamManifestNode subNode))
                        {
                            node = subNode;
                        }
                        else
                        {
                            node = null;
                            break;
                        }
                    }
                }

                return node;
            }

            //Fallback
            return null;
        }
    }
}
