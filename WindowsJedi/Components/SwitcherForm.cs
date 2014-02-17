using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsJedi.Algorithms;
using WindowsJedi.WinApis;

namespace WindowsJedi.Components {
    using System.Linq;
    using System.Threading;

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
		private volatile bool _showing;
		private readonly List<Window> _windows;
        readonly List<Form> _overlays;
		private readonly KeyHookManager _keyMgr;
		private readonly Queue<PassThroughKey> _passThroughKeys;
        bool _showingPopups;

        const bool ShowPopupsInitially = false;
        const string SelectorKeys = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        protected override void OnClick(EventArgs e)
        {
            foreach (var form in _overlays)
            {
                form.BringToFront();
            }
        }

		public SwitcherForm() {
			_keyMgr = new KeyHookManager();
			_keyMgr.KeyDown += keyMgr_KeyDown;
			_keyMgr.KeyUp += keyMgr_KeyUp;
			KeyPreview = true;
			_passThroughKeys = new Queue<PassThroughKey>();
			_windows = new List<Window>();
            _overlays = new List<Form>();
		}

        protected override void Dispose(bool disposing)
        {
			_keyMgr.Stop();
            _keyMgr.Dispose();
			HideSwitcher();
            base.Dispose(disposing);
        }

		/// <summary>
		/// Show or hide the switcher window.
		/// </summary>
		public void Toggle() {
			if (_showing) HideSwitcher();
			else ShowSwitcher();
		}

		public void ShowSwitcher() {
			Opacity = 1;
            _showingPopups = ShowPopupsInitially;
            GetAndPackWindows(_showingPopups);
			ShowDwmThumbs();
			Show();
			TopMost = true;
			Win32.SetForegroundWindow(Handle);
			_showing = true;

            ShowOverlays();
		}

        void ShowOverlays()
        {
            HideOverlays();

            try
            {
                foreach (var window in _windows)
                {
                    var target = window.TargetRectangle;
                    if (!target.HasValue) continue;
                    var icon = window.GetAppIcon();
                    if (icon == null) continue;

                    var lay = new NonHitOverlayForm { TopMost = true, TopLevel = true, Width = 64, Height = 64, Top = target.Value.Bottom - 32, Left = target.Value.Right - 32 };
                    _overlays.Add(lay);
                    lay.SetBitmap(icon.ToBitmap());
                    lay.Show();
                    lay.BringToFront();
                }
            }
            catch
            {
                HideOverlays();
                throw;
            }
        }

        void HideOverlays()
        {
            foreach (var form in _overlays)
            {
                form.Close();
                form.Dispose();
            }
            _overlays.Clear();
        }


        public void HideSwitcher()
        {			_showing = false;
            HideOverlays();
			HideDwmThumbs();
			TopMost = false;
			Hide();
		}

		#region Input handling & lock-down
		/// <summary>
		/// Switch to a window if user clicks on it's tile
		/// </summary>
		protected override void OnMouseClick (MouseEventArgs e) {
			if (!_showing) return;
			foreach (var window in _windows) {
				if (window.TargetRectangle == null) continue;

				if (window.TargetRectangle.Value.Contains(e.Location)) {
					window.Focus();
					HideSwitcher();
				}
			}
		}

		void keyMgr_KeyUp (object sender, KeyEventArgs e) {
			if (_showing) {
				GenerateKey(Keys.F24, true);
			}
		}

		private void keyMgr_KeyDown (object sender, KeyEventArgs e) {
			if (_showing) {
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
			if (_passThroughKeys.Count > 0) {
				PassThroughKey key2 = _passThroughKeys.Peek();
				if ((key2.Up == keyUp) && (key2.Key == key)) {
					_passThroughKeys.Dequeue();
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
			_passThroughKeys.Enqueue(item);

			var none = Win32.KeyboardInputFlags.None;
			if (((int)key & 0x800000) == 0x800000) {
				none |= Win32.KeyboardInputFlags.None | Win32.KeyboardInputFlags.ExtendedKey;
			}
			if (up) {
				none |= Win32.KeyboardInputFlags.KeyUp;
			}
			try {
				Win32.keybd_event((byte)(key & Keys.KeyCode), 0, none, IntPtr.Zero);
			} catch {
                Ignore();
			}
		}

        /// <summary>
        /// Marks exceptions as explicitly dropped
        /// </summary>
	    private void Ignore() { }

	    #endregion
		/// <summary>
		/// Switch to a window if user presses it's quick-key
		/// </summary>
		protected bool HandleKeyDown (Keys k) {
			if (!_showing) return false;

			if (k == Keys.Escape) {
				HideSwitcher();
				return true;
			}

            if (k == Keys.Tab)
            {
                TogglePopups();
                return true;
            }

		    var converter = new KeysConverter();
            var ch = (converter.ConvertToString(k) ?? "").ToUpper();
			var idx = SelectorKeys.IndexOf(ch, StringComparison.Ordinal);
			if (idx < 0) return false;
			if (idx >= _windows.Count) return true;
			_windows[idx].Focus();
			HideSwitcher();
			return true;
		}

        void TogglePopups()
        {
            _showingPopups = !_showingPopups;
            HideDwmThumbs();
            GetAndPackWindows(_showingPopups);
            ShowDwmThumbs();
            Invalidate();
        }

        protected override void OnPaint (PaintEventArgs e) {
			if (!_showing) return;
			// TODO: some kind of overlay form for the titles and shortcuts -- DWM is overwriting this...

			using (var g = e.Graphics) {
				var bold = new Font("Arial", 8.0f, FontStyle.Bold);
				var regular = new Font("Arial", 8.0f, FontStyle.Regular);
				var wb = new SolidBrush(Color.White);
				int i = 0;
				foreach (var window in _windows) {
					if (window.TargetRectangle == null) continue;
                    var rect = window.TargetRectangle.Value;

					var title = window.Title;
                    var left = rect.X;
                    var bottomOfTile = (rect.Y + rect.Height) - 18;
                    var width = Math.Max(rect.Width, 20);

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
		private void GetAndPackWindows(bool showPopups) {
			_windows.Clear();
			_windows.AddRange(WindowEnumeration.GetCurrentWindows().Where(w=> (! w.IsPopup) || showPopups));
			var scale = 0.7;
			if (_windows.Count > 15) scale = 0.5;
			while (!TryPacking(_windows, scale) && scale > 0.1) {
				scale *= 0.8;
			}
		}

		private bool TryPacking (IEnumerable<Window> windows, double scale) {
			var packer = new CygonRectanglePacker(Width, Height);
			foreach (var window in windows) {
				try {
				    var rect = window.Rectangle;

				    var ts = (rect.Width * rect.Height < 10000) ? 1.0 : scale; // show tiny windows at full size
					window.TargetRectangle = new Rectangle(
						packer.Pack((int)(rect.Width * ts), (int)(rect.Height * ts) + 20),
						new Size((int)(rect.Width * ts), (int)(rect.Height * ts) + 20));
				} catch {
					return false;
				}
			}
			return true;
		}

		private void ShowDwmThumbs () {


			foreach (var window in _windows) {
				if (window.TargetRectangle != null)
					window.ShowDwmThumb(this, window.TargetRectangle.Value);
			}
		}

		private void HideDwmThumbs () {
			foreach (var window in _windows) {
				window.ReleaseDwmThumb();
			}
		}
		#endregion
	}
}
