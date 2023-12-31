﻿using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManage.Controls
{
    public partial class ReaderPage : KryptonPage
    {
        private string _filePath = string.Empty;
        private ContextMenuStrip contextMenuStrip1;
        public ReaderPage()
        {
            InitializeComponent();
            InitRichTextBoxContextMenu();
        }
        public void SetRichEditFilePath(string filePath)
        {
            if (_filePath != null)
            {
                _filePath = filePath;
            }
        }

        public void SetRichEditContent(string fileContent)
        {
            if (fileContent != null) 
            {
                richText_Reader.Text = fileContent;
            }
        }

        public string GetRichEditContent()
        {
            return richText_Reader?.Text;
        }

        private void richText_Reader_KeyDown(object sender, KeyEventArgs e)
        {
            // 处理 Ctrl+C（复制）
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyText();
            }

            // 处理 Ctrl+V（粘贴）
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteText();
            }

            if (e.Control && e.KeyCode == Keys.X)
            {
                CutText();
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveText();
            }
        }
        // 复制文本
        public void CopyText()
        {
            if (richText_Reader.SelectionLength > 0)
            {
                Clipboard.SetText(richText_Reader.SelectedText);
            }
        }

        // 粘贴文本
        public void PasteText()
        {
            if (Clipboard.ContainsText())
            {
                richText_Reader.SelectedText = Clipboard.GetText();
            }
        }

        public void CutText()
        {
            if (richText_Reader.SelectionLength > 0)
            {
                richText_Reader.Cut();
            }
        }

        public void SelectAllText()
        {
            richText_Reader.Focus();
            richText_Reader.SelectAll();
        }

        public void SaveText()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    // 使用 StreamWriter 将内容写入文件
                    using (StreamWriter writer = new StreamWriter(_filePath))
                    {
                        writer.Write(richText_Reader.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("文件存储失败：{0}", ex.Message);
                }
            }
        }


        private void InitRichTextBoxContextMenu()
        {
            contextMenuStrip1 = new ContextMenuStrip();

            // 添加菜单项
            ToolStripMenuItem cutMenuItem = new ToolStripMenuItem("剪切");
            cutMenuItem.Click += CutMenuItem_Click;
            contextMenuStrip1.Items.Add(cutMenuItem);

            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("复制");
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenuStrip1.Items.Add(copyMenuItem);

            ToolStripMenuItem pasteMenuItem = new ToolStripMenuItem("粘贴");
            pasteMenuItem.Click += PasteMenuItem_Click;
            contextMenuStrip1.Items.Add(pasteMenuItem);

            // 将上下文菜单与RichTextBox关联
            richText_Reader.ContextMenuStrip = contextMenuStrip1;           

        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            richText_Reader.Cut();
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            richText_Reader.Copy();
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            richText_Reader.Paste();
        }


    }
}
