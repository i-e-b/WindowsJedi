namespace WindowsJedi.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using WindowsJedi.WinApis;

    public class PopupWindows : IDisposable
    {
        readonly List<Window> _hiddenWindows, _fadedWindows;
        static readonly object Lock = new object();

        public PopupWindows()
        {
            _hiddenWindows = new List<Window>();
            _fadedWindows = new List<Window>();
        }

        public void ToggleVisibility()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                lock (Lock)
                {
                    if (_hiddenWindows.All(Closed))
                    {
                        foreach (var window in _hiddenWindows)
                        {
                            window.Dispose();
                        }
                        _hiddenWindows.Clear();
                    }

                    if (_hiddenWindows.Any())
                    {
                        ShowHiddenWindows();
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
            });
        }

        void ShowHiddenWindows()
        {
            foreach (var window in _hiddenWindows)
            {
                window.Show();
                window.Dispose();
            }
            _hiddenWindows.Clear();
        }

        static bool Closed(Window window)
        {
            return !window.Exists;
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (Lock)
            {
                ShowHiddenWindows();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// This doesn't work very well on WPF windows, as they have weird container windows that retain opacity.
        /// </summary>
        public void ToggleFade()
        {
            lock (Lock)
            {
                if (_fadedWindows.All(Closed))
                {
                    _fadedWindows.Clear();
                    foreach (var window in _fadedWindows)
                    {
                        window.Dispose();
                    }
                    _fadedWindows.Clear();
                }
                if (_fadedWindows.Any())
                {
                    foreach (var window in _fadedWindows)
                    {
                        window.SetOpaque();
                        window.Dispose();
                    }
                    _fadedWindows.Clear();
                }
                else
                {
                    foreach (var window in WindowEnumeration.GetCurrentWindows())
                    {
                        // Not filtered to popups until it's working properly
                        /*if (!window.IsPopup || !window.IsVisible)
                        {
                            window.Dispose();
                            continue;
                        }*/
                        
                        window.SetTranslucent(70);

                        _fadedWindows.Add(window);
                    }
                }
            }
        }
    }
}