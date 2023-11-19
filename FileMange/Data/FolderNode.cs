using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlookMockup.Data
{
    public class FolderNode
    {
        public string Name { get; set; }
        public List<FolderNode> SubFolders { get; set; } = new List<FolderNode>();
        public List<FileNode> Files { get; set; } = new List<FileNode>();

        public FolderNode(string name)
        {
            Name = name;
        }

        public void UpdateName(string newName)
        {
            Name = newName;
        }

        public void AddFile(string fileName)
        {
            Files.Add(new FileNode(fileName));
        }
    }
}
