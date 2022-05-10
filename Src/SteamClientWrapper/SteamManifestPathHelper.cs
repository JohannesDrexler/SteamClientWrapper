using System.Collections;
using System.Text;

namespace SteamWrapper
{
    internal class SteamManifestPathHelper
    {
        private ArrayList levels = new ArrayList();

        public void AddLevel(string lvl)
        {
            levels.Add(lvl);
        }

        public void RemoveLastLevel()
        {
            if (levels.Count != 0)
            {
                levels.RemoveAt(levels.Count - 1);
            }
        }

        public string GetCurrentPath()
        {
            if (levels.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                StringBuilder builder = new StringBuilder();

                bool firstLvl = true;
                foreach (object lvl in levels)
                {
                    if (firstLvl)
                    {
                        firstLvl = false;
                    }
                    else
                    {
                        builder.Append("/");
                    }

                    builder.Append(lvl.ToString());
                }

                return builder.ToString();
            }
        }

        public string GetParentPath()
        {
            string currentPath = GetCurrentPath();
            int lastIndex = currentPath.LastIndexOf('/');
            string parentPath = currentPath.Substring(0, lastIndex);
            return parentPath;
        }
    }
}
