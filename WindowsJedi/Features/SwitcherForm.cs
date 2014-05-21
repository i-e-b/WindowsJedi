namespace WindowsJedi.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using WindowsJedi.Algorithms;
    using WindowsJedi.Components;
    using WindowsJedi.WinApis;

    /// <summary>
    /// Uses DWM composition to show an 'Expose' like alt-tab alternative.
    /// Adds a keyboard shortcut to each window for quick selection
    /// </summary>
    public class SwitcherForm : ClickInvisibleFullScreenForm
    {
        readonly WindowHookManager _winHook;

        [StructLayout(LayoutKind.Sequential)]
        private struct PassThroughKey
        {
            public Keys Key;
            public bool Up;
        }

        private static readonly object SwitchLock = new object();

        private volatile bool _showing;
        private readonly List<Window> _windows;
        readonly List<Form> _overlays;
        private readonly KeyHookManager _keyMgr;
        private readonly Queue<PassThroughKey> _passThroughKeys;
        bool _showingPopups;
        bool _closeMode;

        const bool ShowPopupsInitially = false;
        const string SelectorKeys = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        public SwitcherForm(WindowHookManager winHook)
        {
            _winHook = winHook;
            _keyMgr = new KeyHookManager();
            _keyMgr.KeyDown += keyMgr_KeyDown;
            _keyMgr.KeyUp += keyMgr_KeyUp;
            KeyPreview = true;
            _closeMode = false;
            _passThroughKeys = new Queue<PassThroughKey>();
            _windows = new List<Window>();
            _overlays = new List<Form>();

            _winHook.WindowCreated += _winHook_WindowSetChanged;
            _winHook.WindowDestroyed += _winHook_WindowSetChanged;
        }

        void _winHook_WindowSetChanged(object sender, WinApis.ManagedEvents.WindowHandleEventArgs e)
        {
            if (_showing && WindowSetChanged())
            {
                Debug.WriteLine("refresh");
                Redisplay(true);
            }
        }

        bool WindowSetChanged()
        {
            var current = new HashSet<Window>(_windows);
            var updated = new HashSet<Window>(FilteredWindowList());
            return ! current.SetEquals(updated);
        }

        protected override void Dispose(bool disposing)
        {
            _winHook.WindowCreated -= _winHook_WindowSetChanged;
            _winHook.WindowDestroyed -= _winHook_WindowSetChanged;
            _keyMgr.Stop();
            _keyMgr.Dispose();
            HideSwitcher();
            HideOverlays();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Show or hide the switcher window.
        /// </summary>
        public void Toggle()
        {
            lock (SwitchLock)
            {
                if (_showing) HideSwitcher();
                else ShowSwitcher();
            }
        }

        public void ShowSwitcher()
        {
            lock (SwitchLock)
            {
                _closeMode = false;
                HideOverlays();
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
        }

        void ShowOverlays()
        {
            HideOverlays();

            try
            {
                int i = 0;
                foreach (var window in _windows)
                {
                    if (!window.TargetRectangle.HasValue) continue;
                    var target = window.TargetRectangle.Value;
                    target.Height = 32;

                    var lay = new WindowIconTitledForm(window, SelectorKeys[i].ToString(CultureInfo.InvariantCulture), window.Title, target);

                    _overlays.Add(lay);
                    lay.Show();
                    i++;
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
            var local = _overlays.ToArray();
            _overlays.Clear();

            foreach (var form in local)
            {
                form.Close();
            }
            foreach (var form in _overlays)
            {
                form.Dispose();
            }
        }

        public void HideSwitcher()
        {
            lock (SwitchLock)
            {
                _showing = false;
                HideOverlays();
                HideDwmThumbs();
                TopMost = false;
                Hide();
            }
        }

        #region Input handling & lock-down
        /// <summary>
        /// Switch to a window if user clicks on it's tile
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!_showing) return;
            foreach (var window in _windows)
            {
                if (window.TargetRectangle == null) continue;

                if (window.TargetRectangle.Value.Contains(e.Location))
                {
                    window.Focus();
                    HideSwitcher();
                }
            }
        }

        void keyMgr_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKeyUp(e.KeyData);
            if (_showing)
            {
                GenerateKey(Keys.F24, true);
            }
        }

        private void keyMgr_KeyDown(object sender, KeyEventArgs e)
        {
            if (_showing)
            {
                if (HandleKeyDown(e.KeyData))
                {
                    e.SuppressKeyPress = true;
                }
                else
                {
                    if (e.KeyCode == Keys.Tab) e.SuppressKeyPress = true;

                    if (!ShouldIgnoreKey(e.KeyCode, false))
                        if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin || e.KeyCode == Keys.Alt)
                        {
                            e.SuppressKeyPress = true;
                            GenerateKey(e.KeyCode, false);
                            GenerateKey(Keys.F24, false);
                            GenerateKey(Keys.F24, true);
                        }
                }
            }
        }

        private bool ShouldIgnoreKey(Keys key, bool keyUp)
        {
            if (_passThroughKeys.Count > 0)
            {
                PassThroughKey key2 = _passThroughKeys.Peek();
                if ((key2.Up == keyUp) && (key2.Key == key))
                {
                    _passThroughKeys.Dequeue();
                    return true;
                }
            }
            return false;
        }
        private void GenerateKey(Keys key, bool up)
        {
            var item = new PassThroughKey
            {
                Key = key,
                Up = up
            };
            _passThroughKeys.Enqueue(item);

            var none = Win32.KeyboardInputFlags.None;
            if (((int)key & 0x800000) == 0x800000)
            {
                none |= Win32.KeyboardInputFlags.None | Win32.KeyboardInputFlags.ExtendedKey;
            }
            if (up)
            {
                none |= Win32.KeyboardInputFlags.KeyUp;
            }
            try
            {
                Win32.keybd_event((byte)(key & Keys.KeyCode), 0, none, IntPtr.Zero);
            }
            catch
            {
                Ignore();
            }
        }

        /// <summary>
        /// Marks exceptions as explicitly dropped
        /// </summary>
        private static void Ignore() { }

        #endregion

        void HandleKeyUp(Keys k)
        {
            if (k == Keys.LShiftKey || k == Keys.RShiftKey)
            {
                _closeMode = false;
            }
        }

        /// <summary>
        /// Switch to a window if user presses it's quick-key
        /// </summary>
        protected bool HandleKeyDown(Keys k)
        {
            if (!_showing) return false;

            if (k == Keys.Escape)
            {
                HideSwitcher();
                return true;
            }

            if (k == Keys.Tab)
            {
                TogglePopups();
                return true;
            }

            if (k == Keys.LShiftKey || k == Keys.RShiftKey)
            {
                _closeMode = true;
            }


            var converter = new KeysConverter();
            var ch = (converter.ConvertToString(k) ?? "").ToUpper();
            var idx = SelectorKeys.IndexOf(ch, StringComparison.Ordinal);
            if (idx < 0) return false;
            if (idx >= _windows.Count) return true;


            if (_closeMode)
            {
                // close window, stay in the switcher
                CloseAndRedisplay(_windows[idx]);
            }
            else
            {
                // Select and leave the switcher
                _windows[idx].Focus();
                HideSwitcher();
            }
            return true;
        }

        void CloseAndRedisplay(Window w)
        {
            w.Close();
            Redisplay(false);
        }

        void TogglePopups()
        {
            _showingPopups = !_showingPopups;
            Redisplay(true);
        }

        void Redisplay(bool Repack)
        {
            lock (SwitchLock)
            {
                HideOverlays();
                HideDwmThumbs();
                if (Repack) GetAndPackWindows(_showingPopups);
                ShowDwmThumbs();
                ShowOverlays();
                Invalidate();
            }
        }

        IEnumerable<Window> FilteredWindowList()
        {
            return WindowEnumeration.GetCurrentWindows()
                .Where(w => (!w.IsPopup) || _showingPopups)
                .Where(NotInIgnoreList);
        }

        #region Thumbnails
        private void GetAndPackWindows(bool showPopups)
        {
            _windows.Clear();
            _windows.AddRange(FilteredWindowList());

            var scale = 0.7;
            if (_windows.Count > 15) scale = 0.5;
            while (!TryPacking(_windows, scale) && scale > 0.1)
            {
                scale *= 0.8;
            }
        }

        static bool NotInIgnoreList(Window arg)
        {
            return !(
                arg.Title.StartsWith("ASP.NET Developer Server")
                );
        }

        private bool TryPacking(IEnumerable<Window> windows, double scale)
        {
            var packer = new CygonRectanglePacker(Width, Height);
            foreach (var window in windows)
            {
                try
                {
                    var rect = window.Rectangle;

                    var ts = (rect.Width * rect.Height < 10000) ? 1.0 : scale; // show tiny windows at full size
                    window.TargetRectangle = new Rectangle(
                        packer.Pack((int)(rect.Width * ts), (int)(rect.Height * ts)),
                        new Size((int)(rect.Width * ts), (int)(rect.Height * ts)));
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private void ShowDwmThumbs()
        {


            foreach (var window in _windows)
            {
                if (window.TargetRectangle != null)
                    window.ShowDwmThumb(this, window.TargetRectangle.Value);
            }
        }

        private void HideDwmThumbs()
        {
            foreach (var window in _windows)
            {
                window.ReleaseDwmThumb();
            }
        }
        #endregion
    }
}
