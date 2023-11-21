using ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManage.Controls
{
    public partial class ReaderPage : KryptonPage
    {
        public ReaderPage()
        {
            InitializeComponent();
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

    }
}
