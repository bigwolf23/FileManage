using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManage.Data
{
    public class FolderNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<FolderNode> SubFolders { get; set; } = new List<FolderNode>();
        public List<FileNode> SubFiles { get; set; } = new List<FileNode>();

        public FolderNode(string name,string path)
        {
            Name = name;
            Path = path;
        }

        public void DeleteFolder(FolderNode folderNode)
        {
            SubFolders.Remove(folderNode);
        }

        public void UpdateName(string newName, string path)
        {
            Name = newName;
            Path = path;
        }

        public void AddFile(string fileName, string path)
        {
            SubFiles.Add(new FileNode(fileName,path));
        }

        public void DeleteFile(FileNode fileNode)
        {
            SubFiles.Remove(fileNode);
        }
    }
}
