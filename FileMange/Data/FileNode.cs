using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManage.Data
{
    public class FileNode
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public FileNode(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public void UpdateName(string newName, string path)
        {
            Name = newName;
            Path = path;
        }
    }
}
