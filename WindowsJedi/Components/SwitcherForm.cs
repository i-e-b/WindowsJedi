using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsJedi.Algorithms;
using WindowsJedi.WinApis;

namespace WindowsJedi.Components {
	/// <summary>
	/// Uses DWM composition to show an 'Expose' like alt-tab alternative.
	/// Adds a keyboard shortcut to each window for quick selection
	/// </summary>
	public class SwitcherForm : FullScreenForm {
		[StructLayout(LayoutKind.Sequential)]
		private struct PassThroughKey {
			public Keys Key;
			public bool Up;
		}
		private volatile bool showing;
		private readonly List<Window> windows;
		private readonly KeyHookManager keyMgr;
		private readonly Queue<PassThroughKey> passThroughKeys;
		private const string SelectorKeys = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";


		public SwitcherForm() {
			keyMgr = new KeyHookManager();
			keyMgr.KeyDown += keyMgr_KeyDown;
			keyMgr.KeyUp += keyMgr_KeyUp;
			KeyPreview = true;
			passThroughKeys = new Queue<PassThroughKey>();
			windows = new List<Window>();
		}

        protected override void Dispose(bool disposing)
        {
			keyMgr.Stop();
            keyMgr.Dispose();
			HideSwitcher();
            base.Dispose(disposing);
        }

		/// <summary>
		/// Show or hide the switcher window.
		/// </summary>
		public void Toggle() {
			if (showing) HideSwitcher();
			else ShowSwitcher();
		}

		public void ShowSwitcher() {
			Opacity = 1;
			GetAndPackWindows();
			ShowDwmThumbs();
			Show();
			TopMost = true;
			Win32.SetForegroundWindow(Handle);
			showing = true;
		}

		public void HideSwitcher () {
			showing = false;
			HideDwmThumbs();
			TopMost = false;
			Hide();
		}

		#region Input handling & lock-down
		/// <summary>
		/// Switch to a window if user clicks on it's tile
		/// </summary>
		protected override void OnMouseClick (MouseEventArgs e) {
			if (!showing) return;
			foreach (var window in windows) {
				if (window.TargetRectangle == null) continue;

				if (window.TargetRectangle.Value.Contains(e.Location)) {
					window.Focus();
					HideSwitcher();
				}
			}
		}

		void keyMgr_KeyUp (object sender, KeyEventArgs e) {
			if (showing) {
				GenerateKey(Keys.F24, true);
			}
		}

		private void keyMgr_KeyDown (object sender, KeyEventArgs e) {
			if (showing) {
				if (HandleKeyDown(e.KeyData)) {
					e.SuppressKeyPress = true;
				} else {
					if (e.KeyCode == Keys.Tab) e.SuppressKeyPress = true;

					if (!ShouldIgnoreKey(e.KeyCode, false))
						if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin || e.KeyCode == Keys.Alt) {
							e.SuppressKeyPress = true;
							GenerateKey(e.KeyCode, false);
							GenerateKey(Keys.F24, false);
							GenerateKey(Keys.F24, true);
						}
				}
			}
		}

		private bool ShouldIgnoreKey (Keys key, bool keyUp) {
			if (passThroughKeys.Count > 0) {
				PassThroughKey key2 = passThroughKeys.Peek();
				if ((key2.Up == keyUp) && (key2.Key == key)) {
					passThroughKeys.Dequeue();
					return true;
				}
			}
			return false;
		}
		private void GenerateKey (Keys key, bool up) {
			var item = new PassThroughKey
			{
				Key = key,
				Up = up
			};
			passThroughKeys.Enqueue(item);

			Win32.KeyboardInputFlags none = Win32.KeyboardInputFlags.None;
			if (((int)key & 0x800000) == 0x800000) {
				none |= Win32.KeyboardInputFlags.None | Win32.KeyboardInputFlags.ExtendedKey;
			}
			if (up) {
				none |= Win32.KeyboardInputFlags.KeyUp;
			}
			try {
				Win32.keybd_event((byte)(key & Keys.KeyCode), 0, none, IntPtr.Zero);
			} catch {
				return;
			}
		}
		#endregion
		/// <summary>
		/// Switch to a window if user presses it's quick-key
		/// </summary>
		protected bool HandleKeyDown (Keys k) {
			if (!showing) return false;

			if (k == Keys.Escape) {
				HideSwitcher();
				return true;
			}

			var converter = new KeysConverter();
			string ch = (converter.ConvertToString(k)??"").ToUpper();
			var idx = SelectorKeys.IndexOf(ch);
			if (idx < 0) return false;
			if (idx >= windows.Count) return true;
			windows[idx].Focus();
			HideSwitcher();
			return true;
		}

		protected override void OnPaint (PaintEventArgs e) {
			if (!showing) return;
			// TODO: some kind of overlay form for the titles and shortcuts -- DWM is overwriting this...
			

			using (var g = e.Graphics) {
				var bold = new Font("Arial", 8.0f, FontStyle.Bold);
				var regular = new Font("Arial", 8.0f, FontStyle.Regular);
				var wb = new SolidBrush(Color.White);
				int i = 0;
				foreach (var window in windows) {
					if (window.TargetRectangle == null) continue;
					var title = window.Title;
                    var left = window.TargetRectangle.Value.X;
                    var bottomOfTile = (window.TargetRectangle.Value.Y + window.TargetRectangle.Value.Height) - 18;
                    var width = Math.Max(window.TargetRectangle.Value.Width, 20);


                    if (i < SelectorKeys.Length)
                    {
                        g.DrawString(" " + SelectorKeys[i] + " ", bold, wb, new RectangleF(left, bottomOfTile, width, 20));
                        left += 15;
                    }
                    g.DrawString(title, regular, wb, new RectangleF(left, bottomOfTile, width, 20));

					i++;
				}
			}
		}

		#region Thumbnails
		private void GetAndPackWindows() {
			windows.Clear();
			windows.AddRange(WindowEnumeration.GetCurrentWindows());
			var scale = 0.7;
			if (windows.Count > 15) scale = 0.5;
			while (!TryPacking(windows, scale) && scale > 0.1) {
				scale *= 0.8;
			}
		}

		private bool TryPacking (IEnumerable<Window> windows, double scale) {
			var packer = new CygonRectanglePacker(Width, Height);
			foreach (var window in windows) {
				try {
					var ts = (window.Rectangle.Width * window.Rectangle.Height < 10000) ? 1.0 : scale;
					window.TargetRectangle = new Rectangle(
						packer.Pack((int)(window.Rectangle.Width * ts), (int)(window.Rectangle.Height * ts) + 20),
						new Size((int)(window.Rectangle.Width * ts), (int)(window.Rectangle.Height * ts) + 20));
				} catch {
					return false;
				}
			}
			return true;
		}

		private void ShowDwmThumbs () {
			foreach (var window in windows) {
				if (window.TargetRectangle != null)
					window.ShowDwmThumb(this, window.TargetRectangle.Value);
			}
		}

		private void HideDwmThumbs () {
			foreach (var window in windows) {
				window.ReleaseDwmThumb();
			}
		}
		#endregion
	}
}
