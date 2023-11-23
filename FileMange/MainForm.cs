// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2012. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, PO Box 1504, 
//  Glen Waverley, Vic 3150, Australia and are supplied subject to licence terms.
// 
//  Version 4.6.0.0 	www.ComponentFactory.com
// *****************************************************************************

using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using System.IO;
using System.Configuration;
using FileManage.Common;
using System.Linq;
using FileManage.Controls;
using FileManage.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading;

namespace FileManage
{
    public partial class MainForm : KryptonForm
    {
        public string CurrentDirPath = string.Empty;
        private FolderNode rootFolderNode;
        private ContextMenuStrip folderContextMenuStrip;
        private ContextMenuStrip fileContextMenuStrip;

        public MainForm()
        {
            InitializeComponent();
            initialForm();
            InitializeContextMenu();
        }

        private void initialForm()
        {
            kryptonManager.GlobalPaletteMode = PaletteModeManager.Office2010Black;
            treeView_Dir.LabelEdit = true;
            treeView_Dir.AfterLabelEdit += treeView_Dir_AfterLabelEdit;
            treeView_Dir.AfterSelect += treeView_Dir_AfterSelect;
            treeView_Dir.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_Dir_NodeMouseClick);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Populate the different data table
            CurrentDirPath = CommonPro.GetCurrentDir();
            //LoadDirectoryStructure(CurrentDirPath); // 你可以替换为你想要展示的文件夹路径
            LoadTreeView(CurrentDirPath);
        }

        private void LoadTreeView(string rootPath)
        {
            treeView_Dir.BeginUpdate(); // 开始更新
            rootFolderNode = CreateFolderNode(rootPath);
            treeView_Dir.Nodes.Clear();
            //foreach (var item in treeView_Dir.Nodes)
            //{
            //    KryptonTreeNode temp = (KryptonTreeNode)item;
            //    if (temp.Parent == null)
            //    {
            //        continue;
            //    }
            //    treeView_Dir.Nodes.Remove((KryptonTreeNode)item);
            //}
            treeView_Dir.Nodes.Add(CreateTreeNode(rootFolderNode));
            treeView_Dir.EndUpdate(); // 结束更新
        }

        private KryptonTreeNode CreateTreeNode(FolderNode folderNode)
        {
            var treeNode = new KryptonTreeNode(folderNode.Name);
            treeNode.ImageIndex = 0;
            treeNode.SelectedImageIndex = treeNode.ImageIndex;
            treeNode.Tag = folderNode;
            foreach (var subFolder in folderNode.SubFolders)
            {
                treeNode.Nodes.Add(CreateTreeNode(subFolder));
            }

            foreach (var file in folderNode.SubFiles)
            {
                var fileNode = new TreeNode(file.Name);
                fileNode.Tag = file; // 将 FileNode 与 TreeNode 关联
                fileNode.ImageIndex = 1;
                fileNode.SelectedImageIndex = fileNode.ImageIndex;
                treeNode.Nodes.Add(fileNode);
            }

            return treeNode;
        }

        private FolderNode CreateFolderNode(string folderPath)
        {
            var folderNode = new FolderNode(Path.GetFileName(folderPath),folderPath);
            foreach (var subDirectory in Directory.GetDirectories(folderPath))
            {
                folderNode.SubFolders.Add(CreateFolderNode(subDirectory));
            }

            foreach (var filePath in Directory.GetFiles(folderPath))
            {
                folderNode.SubFiles.Add(new FileNode(Path.GetFileName(filePath), filePath));
            }

            return folderNode;
        }


        private void AddPage(string fileName,string filePath,string fileContent)
        {
            ReaderPage newPage = new ReaderPage();
            newPage.Text = fileName;
            bool findPages = false;
            for (int i = 0;i < pageContainer_Reader.Pages.Count;i++)
            {
                if (pageContainer_Reader.Pages[i].Text.Equals(fileName,StringComparison.OrdinalIgnoreCase) )
                {
                    findPages = true;
                    pageContainer_Reader.SelectedIndex = i;
                    break;
                }
            }

            if (findPages == false)
            {
                newPage.SetRichEditContent(fileContent);
                newPage.SetRichEditFilePath(filePath);
                pageContainer_Reader.Pages.Add(newPage);
                pageContainer_Reader.SelectedPage = newPage;
            }                     
        }

        private void treeView_Dir_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 检查是否选择了一个文件节点
            if (!(e.Node.Tag is FileNode))
            {
                return;
            }
            FileNode fileNode = (FileNode)e.Node.Tag;
            if (File.Exists(fileNode.Path))
            {
                // 读取文件内容并显示在 TextBox 中
                try
                {
                    string fileContent = File.ReadAllText(fileNode.Path);
                    AddPage(e.Node.Text, fileNode.Path, fileContent);                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取文件错误: " + ex.Message);
                }
            }
            else
            {
                // 如果选择的是目录节点，清空 TextBox
                //MessageBox.Show("请选择文件，而非目录。" );
            }
        }
        private void RenameFolderNode(TreeNode treeNode, string newName)
        {
            // 获取文件夹节点的数据模型
            FolderNode folderNode = (FolderNode)treeNode.Tag;

            // 获取文件夹的旧路径
            string oldPath = folderNode.Path;

            // 构建新的文件夹路径
            string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName);

            // 更新本地文件系统中的文件夹名称
            
            Directory.Move(oldPath, newPath);
            Thread.Sleep(500);
            // 更新文件夹节点的名称和路径
            folderNode.Name = newName;
            folderNode.Path = newPath;

            // 更新TreeView节点的文本
            treeNode.Text = newName;
            
            // 更新子文件夹和文件的路径和名称
            UpdatePathAndNameRecursively(treeNode, oldPath, newPath, newName);
        }

        private void RenameFileNode(TreeNode treeNode, string newName)
        {
            // 获取文件夹节点的数据模型
            FileNode fileNode = (FileNode)treeNode.Tag;

            // 获取文件夹的旧路径
            string oldPath = fileNode.Path;

            // 构建新的文件夹路径
            string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName);

            // 更新文件夹节点的名称和路径
            fileNode.Name = newName;
            fileNode.Path = newPath;

            // 更新TreeView节点的文本
            treeNode.Text = newName;
            // 更新本地文件系统中的文件夹名称

            File.Move(oldPath, newPath);
        }


        private void UpdatePathAndNameRecursively(TreeNode treeNode, string oldPath, string newPath, string newFolderName)
        {
            if (treeNode.Tag == null || !(treeNode.Tag is FolderNode))
            {
                return;
            }
            // 更新当前节点的路径
            //FolderNode folderNode = ((FolderNode)treeNode.Tag);
            //oldPath = folderNode.Path;
            //((FolderNode)treeNode.Tag).Path = newPath;

            // 更新子文件夹路径和名称
            foreach (FolderNode subFolder in ((FolderNode)treeNode.Tag).SubFolders)
            {
                string subFolderPath = subFolder.Path.Replace(oldPath, newPath);
                subFolder.Path = subFolderPath;
                subFolder.Name = Path.GetFileName(subFolderPath);
            }

            // 更新子文件路径和名称
            foreach (FileNode subFile in ((FolderNode)treeNode.Tag).SubFiles)
            {
                string subFilePath = subFile.Path.Replace(subFile.Path, newPath);
                subFile.Path = subFilePath;
                subFile.Name = Path.GetFileName(subFilePath);
            }

            // 递归更新子节点的路径和名称
            foreach (TreeNode childNode in treeNode.Nodes)
            {
                UpdatePathAndNameRecursively(childNode, oldPath, newPath, newFolderName);
            }
        }

        private void treeView_Dir_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.CancelEdit )
            {
                // 用户取消编辑
                return;
            }
            if (string.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            string oldName = e.Node.Text;
            string newName = e.Label;

            if (oldName.Equals(newName,StringComparison.OrdinalIgnoreCase))
            {
                e.CancelEdit = true;
                return;
            }
            // 更新数据模型
            TreeNode selectedNode = e.Node;//treeView_Dir.SelectedNode;
            if (selectedNode == null)
            {
                e.CancelEdit = true;
                return;
            }
            else
            {
                if (selectedNode.Parent == null)
                {
                    MessageBox.Show("这个文件夹节点是根节点，无法修改名字。 " );
                    e.CancelEdit = true;
                    return;
                }
            }

            try
            {
                if (selectedNode.Tag is FolderNode)
                {
                    RenameFolderNode(selectedNode, newName);
                }
                else if (selectedNode.Tag is FileNode)
                {
                    RenameFileNode(selectedNode, newName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("更新错误的文件名: " + ex.Message);
                e.Node.Text = oldName;
                // 恢复节点文本
                e.CancelEdit = true;
                return;
            }
            e.Node.EndEdit(false);
        }

        private string GetFilePathFromNode(TreeNode node)
        {
            List<string> pathSegments = new List<string>();
            while (node != null)
            {
                pathSegments.Insert(0, node.Text);
                node = node.Parent;
            }

            if (pathSegments.Count > 0)
            {
                pathSegments.RemoveAt(0);
            }

            return Path.Combine(CurrentDirPath, Path.Combine(pathSegments.ToArray()));
        }


        private void InitializeContextMenu()
        {
            folderContextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem importFilesMenuItem = new ToolStripMenuItem("导入文件");
            ToolStripMenuItem addFolderMenuItem = new ToolStripMenuItem("新建文件夹");
            ToolStripMenuItem deleteFolderMenuItem = new ToolStripMenuItem("删除文件夹");

            importFilesMenuItem.Click += ImportFilesMenuItem_Click;
            addFolderMenuItem.Click += AddFolderMenuItem_Click;
            deleteFolderMenuItem.Click += DeleteFolderMenuItem_Click;

            folderContextMenuStrip.Items.Add(importFilesMenuItem);
            folderContextMenuStrip.Items.Add(addFolderMenuItem);
            folderContextMenuStrip.Items.Add(deleteFolderMenuItem);

            fileContextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem deleteFileMenuItem = new ToolStripMenuItem("删除文件");
            deleteFileMenuItem.Click += DeleteFileMenuItem_Click;
            fileContextMenuStrip.Items.Add(deleteFileMenuItem);

            //treeView_Dir.ContextMenuStrip = folderContextMenuStrip;
        }

        private void treeView_Dir_NodeMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView_Dir.SelectedNode = treeView_Dir.GetNodeAt(e.X, e.Y);

                if (treeView_Dir.SelectedNode != null)
                {
                    if (treeView_Dir.SelectedNode.Tag is FolderNode)
                    {
                        treeView_Dir.ContextMenuStrip = folderContextMenuStrip;
                    }
                    else if (treeView_Dir.SelectedNode.Tag is FileNode)
                    {
                        treeView_Dir.ContextMenuStrip = fileContextMenuStrip;
                    }

                    treeView_Dir.ContextMenuStrip.Show(treeView_Dir, e.Location);
                }
            }
        }

        private bool PreCheckFileDuplicate(string[] selectedFiles, TreeNode destinationFolder)
        {
            FolderNode folder = (FolderNode)destinationFolder.Tag;
            if (folder == null)
            {
                MessageBox.Show("拷贝的文件路径不是文件夹。");
                return false;
            }
            foreach (string file in selectedFiles)
            {
                string destFile = Path.Combine(folder.Path, Path.GetFileName(file));
                if (File.Exists(destFile))
                {
                    return false;
                }
            }

            return true;
        }
        private void ImportFiles(string sourceFilerPath, TreeNode destinationFolder)
        {
            try
            {                
                if (!(destinationFolder.Tag is FolderNode))
                {
                    return;
                }
                FolderNode folder = (FolderNode)destinationFolder.Tag;

                //将文件拷贝到目标目录
                string destFile = Path.Combine(folder.Path, Path.GetFileName(sourceFilerPath));
                
                bool CreateNode = true;
                if (File.Exists(destFile))
                {
                    CreateNode = false;
                }

                File.Copy(sourceFilerPath, destFile);

                if (CreateNode == false)
                {
                    return;
                }
                //将数据结构更新
                string fileName = Path.GetFileName(sourceFilerPath);
                FileNode newFile = new FileNode(
                                fileName, 
                                Path.Combine(folder.Path, fileName));
                folder.SubFiles.Add(newFile);

                //更新Tree结构
                var fileNode = new TreeNode(fileName);
                fileNode.Tag = newFile; // 将 FileNode 与 TreeNode 关联
                fileNode.ImageIndex = 1;
                fileNode.SelectedImageIndex = fileNode.ImageIndex;
                destinationFolder.Nodes.Add(fileNode);
            }
            catch (Exception ex)
            {
                // 处理异常，例如显示错误消息
                //Console.WriteLine($"Error importing folder: {ex.Message}");
            }
        }

        private void ImportFilesMenuItem_Click(object sender, EventArgs e)
        {
            // 处理导入多个文件的操作
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择多个文件";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv";
                openFileDialog.Multiselect = true;

                TreeNode treeNode = treeView_Dir.SelectedNode;
                if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;                   
                }
                // 获取用户选择的文件路径数组
                string[] selectedFiles = openFileDialog.FileNames;
                bool bCopyFlag = PreCheckFileDuplicate(selectedFiles, treeNode);
                if (bCopyFlag == false)
                {
                    if (MessageBox.Show("文件夹下有重复的文件，你想覆盖吗？", "文件管理", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        bCopyFlag = true;
                    }
                }

                // 处理选择的文件，可以根据需要进行操作
                if (bCopyFlag)
                {
                    foreach (string file in selectedFiles)
                    {
                        ImportFiles(file, treeNode);
                    }
                }
            }
        }

        private void DeleteFolderFromTreeView(TreeNode folderTreeNode)
        {
            // 获取父节点的node
            FolderNode parentNode = GetParentFolderNode(folderTreeNode);
            // 获取要删除的文件夹节点的数据模型
            FolderNode folderData = (FolderNode)folderTreeNode.Tag;
            try
            {
                // 删除本地文件系统中的文件夹及其内容
                Directory.Delete(folderData.Path, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除文件夹失败: " + ex.Message);
                return;
            }
            
            // 从数据模型中删除文件夹节点
            RemoveFolderFromDataModel(folderData, parentNode);

            // 从TreeView中移除节点
            folderTreeNode.Remove();
        }

        private void RemoveFolderFromDataModel(FolderNode folderNode, FolderNode parentNode)
        {
            // 递归删除文件夹及其子文件夹和文件
            //foreach (FolderNode subFolder in folderNode.SubFolders)
            //{
            //    RemoveFolderFromDataModel(subFolder,folderNode);
            //}

            // 从父文件夹的子文件夹列表中移除当前文件夹
            if (parentNode != null)
            {
                parentNode.SubFolders.Remove(folderNode);
            }
            else
            {
                // 如果没有父节点，说明当前节点是根节点
                // 在这个简化示例中，可以在根节点列表中移除
                //rootFolderNodes.Remove(folderNode);
            }
        }

        private FolderNode GetParentFolderNode(TreeNode node)
        {
            // 获取父节点
            TreeNode parentNode = node.Parent;

            // 获取文件夹节点的数据模型
            return parentNode?.Tag as FolderNode;
        }

        private void DeleteFolderMenuItem_Click(object sender, EventArgs e)
        {
            // 假设选中的节点是文件夹节点
            TreeNode selectedNode = treeView_Dir.SelectedNode;
            if (selectedNode.Parent == null)
            {
                MessageBox.Show("这是根文件目录，无法删除");
                return;
            }
            // 处理删除多个文件的操作
            // 可以弹出确认对话框等
            if (MessageBox.Show("你想删除这个文件夹和文件夹下的文件吗？", "文件管理", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {               
                if (selectedNode != null && selectedNode.Tag is FolderNode)
                {
                    DeleteFolderFromTreeView(selectedNode);
                }
            }
            
        }
        private void DeleteFileFromTreeView(TreeNode fileNode)
        {
            // 获取要删除的文件夹节点的数据模型
            FileNode fileData = (FileNode)fileNode.Tag;
            if (fileData == null)
            {
                MessageBox.Show("请选择一个文件。" );
                return;
            }

            try
            {
                // 删除本地文件系统中的文件夹及其内容
                File.Delete(fileData.Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除文件夹失败: " + ex.Message);
                return;
            }

            // 从数据模型中删除文件夹节点            
            RemoveFileFromDataModel(fileData, GetParentFolderNode(fileNode));

            // 从TreeView中移除节点
            fileNode.Remove();
        }

        private void RemoveFileFromDataModel(FileNode fileNode, FolderNode parentNode)
        {
            // 递归删除文件夹及其子文件夹和文件
            parentNode.DeleteFile(fileNode);            
        }
        private void DeleteFileMenuItem_Click(object sender, EventArgs e)
        {
            // 处理删除多个文件的操作
            // 可以弹出确认对话框等
            if (MessageBox.Show("你想删除这个文件吗？", "文件管理", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            // 假设选中的节点是文件夹节点
            TreeNode selectedNode = treeView_Dir.SelectedNode;

            if (selectedNode != null && selectedNode.Tag is FileNode)
            {
                DeleteFileFromTreeView(selectedNode);
            }
        }
        private void AddFolderToTreeView(TreeNode parentNode, string folderName)
        {
            string parentFolderPath = GetFilePathFromNode(parentNode);

            // 构建新文件夹的初始名称
            string newFolderName = folderName;

            // 获取新文件夹的完整路径
            string newFolderPath = Path.Combine(parentFolderPath, folderName);

            int suffix = 1;
            while (Directory.Exists(newFolderPath))
            {
                newFolderName = $"{folderName}_{suffix}";
                newFolderPath = Path.Combine(parentFolderPath, newFolderName);
                suffix++;
            }

            // 检查文件夹是否已经存在
            if (Directory.Exists(newFolderPath))
            {
                MessageBox.Show("文件夹已经存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 在本地文件系统中创建对应的文件夹
            Directory.CreateDirectory(newFolderPath);

            FolderNode parentFolderNode = (FolderNode)parentNode.Tag;
            // 创建TreeView中的节点
            FolderNode newFolderNode = new FolderNode(newFolderName, newFolderPath);
            parentFolderNode.SubFolders.Add(newFolderNode);

            // 创建TreeView中的节点
            TreeNode folderNode = new TreeNode(newFolderName);
            folderNode.Tag = newFolderNode;

            // 将节点添加到TreeView中
            parentNode.Nodes.Add(folderNode);

            //RefreshTreeView();
        }

        private void AddFolderMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView_Dir.SelectedNode;

            if (selectedNode != null && selectedNode.Tag is FolderNode)
            {
                string newFolderName = "NewFolder"; // 替换为你想要的文件夹名称
                AddFolderToTreeView(selectedNode, newFolderName);
            }
        }

        private void RefreshTreeView()
        {
            //ClearTreeView();
            ClearFolderStructure();
            LoadTreeView(CurrentDirPath);
        }

        private void ClearTreeView()
        {
            treeView_Dir.Nodes.Clear();
        }

        private void ClearFolderStructure()
        {
            rootFolderNode.SubFolders.Clear();
            rootFolderNode.SubFiles.Clear();
        }
        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            RefreshTreeView();            
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.CutText();
            }            
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.CopyText();
            }
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.PasteText();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.CutText();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.CopyText();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.PasteText();
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.SelectAllText();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReaderPage page = (ReaderPage)pageContainer_Reader.SelectedPage;
            if (page != null)
            {
                page.SaveText();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
