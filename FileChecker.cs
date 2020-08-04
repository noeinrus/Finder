using System;
using System.IO;

namespace Finder
{
    class FileChecker
    {
        public static Boolean CheckFile(string path, string subString)
        {
            using (StreamReader sr = File.OpenText(path))
            {
                string s = sr.ReadToEnd();
                if (s.Contains(subString))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
