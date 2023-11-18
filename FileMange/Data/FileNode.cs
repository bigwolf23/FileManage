using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlookMockup.Data
{
    public class FileNode
    {
        public string Name { get; set; }

        public FileNode(string name)
        {
            Name = name;
        }

        public void UpdateName(string newName)
        {
            Name = newName;
        }
    }
}
