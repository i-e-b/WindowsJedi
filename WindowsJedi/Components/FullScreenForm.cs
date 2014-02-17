namespace WindowsJedi.Components {
    using System.Drawing;
    using System.Windows.Forms;
    using WindowsJedi.WinApis;

    public class FullScreenForm : Form {
		public FullScreenForm() {
			SuspendLayout();
			BackColor = Color.Black;
			ShowInTaskbar = false;
			FormBorderStyle = FormBorderStyle.None;
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.Manual;
			Name = "";
			Opacity = 0;
			SetSize();
			ResumeLayout(false);
		}

        protected override CreateParams CreateParams {
			get {
				// Turn on WS_EX_TOOLWINDOW style bit -- hides from ALT-TAB
				var cp = base.CreateParams;
				cp.ExStyle |= Win32.WS_EX_TOOLWINDOW;
				return cp;
			}
		}

        /// <summary>
        /// Set the size of the form to the greatest extent of all screens
        /// </summary>
        private void SetSize () {
            var xMin = int.MaxValue;
            var yMin = int.MaxValue;
            var xMax = int.MinValue;
            var yMax = int.MinValue;
            foreach (var s in Screen.AllScreens) {
                if (s.Bounds.X < xMin)
                    xMin = s.Bounds.X;
                if (s.Bounds.Y < yMin)
                    yMin = s.Bounds.Y;
                if (s.Bounds.X + s.Bounds.Width > xMax)
                    xMax = s.Bounds.X + s.Bounds.Width;
                if (s.Bounds.Y + s.Bounds.Height > yMax)
                    yMax = s.Bounds.Y + s.Bounds.Height;
            }
            Size = new Size(xMax, yMax);
            Location = new Point(xMin, yMin);
        }
    }
}
