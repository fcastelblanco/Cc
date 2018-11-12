using System.IO;

namespace Cc.Upt.Common.ExtensionMethods
{
    public class Path
    {
        public static void CreateDirectoryRecursively(string path)
        {
            var pathParts = path.Split('\\');

            for (var i = 0; i < pathParts.Length; i++)
            {
                if (i > 0)
                    pathParts[i] = pathParts[i - 1] + @"\" + pathParts[i];

                if (!Directory.Exists(pathParts[i])) Directory.CreateDirectory(pathParts[i]);
            }
        }
    }
}