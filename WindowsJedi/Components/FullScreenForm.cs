using System.Drawing;
using System.Windows.Forms;
using WindowsJedi.WinApis;

namespace WindowsJedi.Components {
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

		/// <summary>
		/// Set the size of the form to the greatest extent of all screens
		/// </summary>
		private void SetSize () {
			int xMin = int.MaxValue;
			int yMin = int.MaxValue;
			int xMax = int.MinValue;
			int yMax = int.MinValue;
			foreach (Screen s in Screen.AllScreens) {
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

		protected override CreateParams CreateParams {
			get {
				// Turn on WS_EX_TOOLWINDOW style bit -- hides from ALT-TAB
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= Win32.WS_EX_TOOLWINDOW;
				return cp;
			}
		}
	}
}
