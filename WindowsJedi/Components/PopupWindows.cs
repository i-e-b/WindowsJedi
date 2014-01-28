using System;
using System.Collections.Generic;
using System.Linq;
using WindowsJedi.WinApis;

namespace WindowsJedi.Components
{
    public class PopupWindows : IDisposable
    {
        readonly List<Window> _hiddenWindows;
        static readonly object Lock = new object();

        public PopupWindows()
        {
            _hiddenWindows = new List<Window>();
        }

        public void Toggle()
        {
            lock (Lock)
            {
                if (_hiddenWindows.Any())
                {
                    foreach (var window in _hiddenWindows)
                    {
                        window.Show();
                        window.Dispose();
                    }
                    _hiddenWindows.Clear();
                }
                else
                {
                    foreach (var window in WindowEnumeration.GetCurrentWindows())
                    {
                        if (!window.IsPopup || !window.IsVisible)
                        {
                            window.Dispose();
                            continue;
                        }
                        window.Hide();
                        _hiddenWindows.Add(window);
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (Lock)
            {
                if (!_hiddenWindows.Any()) return;

                foreach (var window in _hiddenWindows)
                {
                    window.Show();
                    window.Dispose();
                }
                _hiddenWindows.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}