namespace WindowsJedi.UserInterface
{
    partial class FileRetypeChooser
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileRetypeChooser));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.chooseSource = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.goRetype = new System.Windows.Forms.Button();
            this.fileSelected = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chooseSource
            // 
            this.chooseSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseSource.Location = new System.Drawing.Point(12, 165);
            this.chooseSource.Name = "chooseSource";
            this.chooseSource.Size = new System.Drawing.Size(346, 23);
            this.chooseSource.TabIndex = 0;
            this.chooseSource.Text = "Choose Source";
            this.chooseSource.UseVisualStyleBackColor = true;
            this.chooseSource.Click += new System.EventHandler(this.chooseSource_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(346, 153);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // goRetype
            // 
            this.goRetype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.goRetype.Location = new System.Drawing.Point(12, 220);
            this.goRetype.Name = "goRetype";
            this.goRetype.Size = new System.Drawing.Size(346, 23);
            this.goRetype.TabIndex = 2;
            this.goRetype.Text = "Re-type File";
            this.goRetype.UseVisualStyleBackColor = true;
            this.goRetype.Click += new System.EventHandler(this.goRetype_Click);
            // 
            // fileSelected
            // 
            this.fileSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelected.Location = new System.Drawing.Point(12, 194);
            this.fileSelected.Name = "fileSelected";
            this.fileSelected.Size = new System.Drawing.Size(346, 20);
            this.fileSelected.TabIndex = 3;
            // 
            // FileRetypeChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(370, 255);
            this.Controls.Add(this.fileSelected);
            this.Controls.Add(this.goRetype);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chooseSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "FileRetypeChooser";
            this.Text = "File Retype";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button chooseSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button goRetype;
        private System.Windows.Forms.TextBox fileSelected;
    }
}