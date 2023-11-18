namespace OutlookMockup.Controls
{
    partial class ReaderPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_Reader = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.richText_Reader = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Reader)).BeginInit();
            this.panel_Reader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Reader
            // 
            this.panel_Reader.Controls.Add(this.richText_Reader);
            this.panel_Reader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Reader.Location = new System.Drawing.Point(0, 0);
            this.panel_Reader.Name = "panel_Reader";
            this.panel_Reader.Size = new System.Drawing.Size(796, 571);
            this.panel_Reader.TabIndex = 0;
            // 
            // richText_Reader
            // 
            this.richText_Reader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richText_Reader.Location = new System.Drawing.Point(0, 0);
            this.richText_Reader.Name = "richText_Reader";
            this.richText_Reader.Size = new System.Drawing.Size(796, 571);
            this.richText_Reader.TabIndex = 0;
            this.richText_Reader.Text = "kryptonRichTextBox1";
            // 
            // ReaderPage
            // 
            this.Controls.Add(this.panel_Reader);
            this.Name = "ReaderPage";
            this.Size = new System.Drawing.Size(796, 571);
            ((System.ComponentModel.ISupportInitialize)(this.panel_Reader)).EndInit();
            this.panel_Reader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel panel_Reader;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox richText_Reader;
    }
}
