namespace WindowsJedi.UserInterface {
	partial class Settings {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
            this.WindowSwitcher = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FocusSwitcher = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ConcentrationMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // WindowSwitcher
            // 
            this.WindowSwitcher.AutoSize = true;
            this.WindowSwitcher.Location = new System.Drawing.Point(13, 13);
            this.WindowSwitcher.Name = "WindowSwitcher";
            this.WindowSwitcher.Size = new System.Drawing.Size(109, 17);
            this.WindowSwitcher.TabIndex = 0;
            this.WindowSwitcher.Text = "&Window Switcher";
            this.WindowSwitcher.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(30, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(374, 37);
            this.label1.TabIndex = 1;
            this.label1.Text = "Switch windows using a set of preview tiles. Allows selection by key-stroke.\r\nWin" +
    "dows-Tab to activate";
            // 
            // FocusSwitcher
            // 
            this.FocusSwitcher.AutoSize = true;
            this.FocusSwitcher.Location = new System.Drawing.Point(13, 85);
            this.FocusSwitcher.Name = "FocusSwitcher";
            this.FocusSwitcher.Size = new System.Drawing.Size(99, 17);
            this.FocusSwitcher.TabIndex = 2;
            this.FocusSwitcher.Text = "&Focus Switcher";
            this.FocusSwitcher.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(30, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(374, 37);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pick focus element within the current front-most window with keystrokes";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(29, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(374, 68);
            this.label3.TabIndex = 5;
            this.label3.Text = "Lock a window to the foreground, dim out other windows and prevent other windows " +
    "from coming to the front. Concentration is as strict as Windows allows!\r\nRightSh" +
    "ift-F12 to activate";
            // 
            // ConcentrationMode
            // 
            this.ConcentrationMode.AutoSize = true;
            this.ConcentrationMode.Location = new System.Drawing.Point(12, 145);
            this.ConcentrationMode.Name = "ConcentrationMode";
            this.ConcentrationMode.Size = new System.Drawing.Size(122, 17);
            this.ConcentrationMode.TabIndex = 4;
            this.ConcentrationMode.Text = "&Concentration Mode";
            this.ConcentrationMode.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 370);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ConcentrationMode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FocusSwitcher);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.WindowSwitcher);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.Text = "WindowsJedi Settings";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox WindowSwitcher;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox FocusSwitcher;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox ConcentrationMode;
	}
}