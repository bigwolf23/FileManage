using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManage.Common
{
    public static class CommonPro
    {
        public static string GetCurrentDir()
        {
            string currentDir = ConfigurationManager.AppSettings["FolderDir"];
            //如果配置文件里没有配置路径或者这个路径不存在的情况下
            if (currentDir == null || !Directory.Exists(currentDir))
            {
                currentDir = "C:\\FileDirectory";
                if (!Directory.Exists(currentDir))
                {
                    Directory.CreateDirectory(currentDir);
                }
                ConfigurationManager.AppSettings["FolderDir"] = currentDir;
            }
           
            return currentDir;
        }
    }
}
