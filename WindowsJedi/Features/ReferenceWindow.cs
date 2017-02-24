namespace WindowsJedi.Features
{
    using System;
    using WindowsJedi.Components;
    using WindowsJedi.WinApis;
    using WindowsJedi.WinApis.ManagedEvents;

    /// <summary>
    /// Current experiment - 'reference windows'
    /// a window marked reference always stays on the opposite monitor from the currently focused window
    /// on single screen set-up we do nothing. If the 'reference' window is the current focus, we do nothing.
    ///
    /// There is only one 'reference window' active at once.
    /// </summary>
    public class ReferenceWindow : IDisposable
    {
        public void Dispose() {
            _winHook.WindowFocusChanged -= _winHook_WindowFocusChanged;
        }

        private readonly WindowHookManager _winHook;
        IntPtr _currentRefWindow;
        IntPtr _lastSwitched;

        public ReferenceWindow(WindowHookManager winHook)
        {
            _winHook = winHook;
            _winHook.WindowFocusChanged += _winHook_WindowFocusChanged;
            _currentRefWindow = IntPtr.Zero;
            _lastSwitched = IntPtr.Zero;
        }

        /// <summary>
        /// Toggles the current front-most window to be the reference window.
        /// </summary>
        public void SetReferenceWindow()
        {
            var newRef = Win32.GetForegroundWindow();
            _currentRefWindow = (_currentRefWindow == newRef)
                ? (IntPtr.Zero)
                : (Win32.GetForegroundWindow());
        }

        void _winHook_WindowFocusChanged(object sender, WindowHandleEventArgs e)
        {
            if (_currentRefWindow == IntPtr.Zero) return;
            if (_currentRefWindow == e.WindowHandle) return;
            if (_lastSwitched == e.WindowHandle) return;

            using (var targw = Window.ForegroundWindow())
            using (var refw = new Window(_currentRefWindow))
            {
                _lastSwitched = targw.Handle;
                Direction[] tryOrder = { Direction.Left, Direction.Right, Direction.Down, Direction.Up };
                foreach (var dir in tryOrder)
                {
                    if (WindowArranger.MoveWindowRelativeScreenDirection(refw, targw, dir))
                    {
                        break;
                    }
                }

                refw.SendToFront();
                targw.SendToFront();
            }
        }
    }
}