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
using OutlookMockup.Common;
using System.Linq;
using OutlookMockup.Controls;
using OutlookMockup.Data;

namespace OutlookMockup
{
    public partial class MainForm : KryptonForm
    {
        public string CurrentDirPath = string.Empty;
        private FolderNode rootFolderNode;
        public MainForm()
        {
            InitializeComponent();
            InitializeContextMenu();
            initialForm();
        }

        private void initialForm()
        {
            kryptonManager.GlobalPaletteMode = PaletteModeManager.ProfessionalSystem;
            treeView_Dir.LabelEdit = true;
            treeView_Dir.AfterLabelEdit += treeView_Dir_AfterLabelEdit;
            treeView_Dir.AfterSelect += treeView_Dir_AfterSelect;            
        }

        private void LoadTreeView(string rootPath)
        {
            rootFolderNode = CreateFolderNode(rootPath);
            treeView_Dir.Nodes.Clear();
            treeView_Dir.Nodes.Add(CreateTreeNode(rootFolderNode));
        }

        private KryptonTreeNode CreateTreeNode(FolderNode folderNode)
        {
            var treeNode = new KryptonTreeNode(folderNode.Name);
            treeNode.Tag = folderNode;
            foreach (var subFolder in folderNode.SubFolders)
            {
                treeNode.Nodes.Add(CreateTreeNode(subFolder));
            }

            foreach (var file in folderNode.Files)
            {
                var fileNode = new TreeNode(file.Name);
                fileNode.Tag = file; // 将 FileNode 与 TreeNode 关联
                treeNode.Nodes.Add(fileNode);
            }

            return treeNode;
        }

        private FolderNode CreateFolderNode(string folderPath)
        {
            var folderNode = new FolderNode(Path.GetFileName(folderPath));
            foreach (var subDirectory in Directory.GetDirectories(folderPath))
            {
                folderNode.SubFolders.Add(CreateFolderNode(subDirectory));
            }

            foreach (var filePath in Directory.GetFiles(folderPath))
            {
                folderNode.Files.Add(new FileNode(Path.GetFileName(filePath)));
            }

            return folderNode;
        }


        private void AddPage(string fileName,string fileContent)
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
                pageContainer_Reader.Pages.Add(newPage);
                pageContainer_Reader.SelectedPage = newPage;
            }                     
        }

        private void treeView_Dir_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 检查是否选择了一个文件节点
            if (File.Exists(GetFilePathFromNode(e.Node)))
            {
                // 读取文件内容并显示在 TextBox 中
                string filePath = GetFilePathFromNode(e.Node);
                try
                {
                    string fileContent = File.ReadAllText(filePath);
                    AddPage(e.Node.Text,fileContent);                    
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
        private void UpdateNodeName(FolderNode folderNode, string oldName, string newName)
        {
            if (folderNode.Name == oldName)
            {
                folderNode.UpdateName(newName);
            }

            foreach (var subFolder in folderNode.SubFolders)
            {
                UpdateNodeName(subFolder, oldName, newName);
            }

            foreach (var file in folderNode.Files)
            {
                if (file.Name == oldName)
                {
                    file.UpdateName(newName);
                }
            }
        }

        private void treeView_Dir_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.CancelEdit)
            {
                // 用户取消编辑
                return;
            }

            string oldName = e.Node.Text;
            string newName = e.Label;

            if (oldName.Equals(newName,StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            // 更新数据模型
            UpdateNodeName(rootFolderNode, oldName, newName);

            string nodePath = GetFilePathFromNode(e.Node);
            string newPath = Path.Combine(Path.GetDirectoryName(nodePath), newName);

            try
            {
                if (e.Node.Tag is FileNode)
                {
                    File.Move(nodePath, newPath);
                }
                else if (e.Node.Tag is FolderNode)
                {
                    Directory.Move(nodePath, newPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("更新错误的文件名: " + ex.Message);
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
            // 创建上下文菜单
            var contextMenu = new ContextMenuStrip();

            // 添加菜单项
            var importFilesMenuItem = new ToolStripMenuItem("导入多个文件");
            var deleteFilesMenuItem = new ToolStripMenuItem("删除多个文件");

            // 关联菜单项的点击事件处理程序
            importFilesMenuItem.Click += ImportFilesMenuItem_Click;
            deleteFilesMenuItem.Click += DeleteFilesMenuItem_Click;

            // 将菜单项添加到上下文菜单
            contextMenu.Items.Add(importFilesMenuItem);
            contextMenu.Items.Add(deleteFilesMenuItem);

            // 关联上下文菜单到 TreeView
            treeView_Dir.ContextMenuStrip = contextMenu;
        }
        private void ImportFilesMenuItem_Click(object sender, EventArgs e)
        {
            // 处理导入多个文件的操作
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择多个文件";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件路径数组
                    string[] selectedFiles = openFileDialog.FileNames;

                    // 处理选择的文件，可以根据需要进行操作
                    foreach (string file in selectedFiles)
                    {
                        MessageBox.Show("导入文件: " + file);
                    }
                }
            }
            MessageBox.Show("导入多个文件");
        }

        private void DeleteFilesMenuItem_Click(object sender, EventArgs e)
        {
            // 处理删除多个文件的操作
            // 可以弹出确认对话框等
            if (MessageBox.Show("你想删除这些文件吗？", "文件管理", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Populate the different data table
            CurrentDirPath = CommonPro.GetCurrentDir();
            //LoadDirectoryStructure(CurrentDirPath); // 你可以替换为你想要展示的文件夹路径
            LoadTreeView(CurrentDirPath);
        }
       

    }
}
