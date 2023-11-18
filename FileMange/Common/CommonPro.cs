using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlookMockup.Common
{
    public static class CommonPro
    {
        public static string GetCurrentDir()
        {
            string currentDir = ConfigurationManager.AppSettings["FolderDir"];
            if (currentDir == null)
            {
                currentDir = "C:\\FileDirectory";
                if (!Directory.Exists(currentDir))
                {
                    Directory.CreateDirectory(currentDir);
                }
            }
            return currentDir;
        }
    }
}
