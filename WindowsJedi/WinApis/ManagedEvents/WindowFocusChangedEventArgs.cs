namespace WindowsJedi.WinApis.ManagedEvents
{
    using System;

    public class WindowHandleEventArgs : EventArgs {
        private readonly IntPtr _windowHandle;

        public WindowHandleEventArgs(IntPtr windowHandle) {
            _windowHandle = windowHandle;
        }

        public IntPtr WindowHandle {
            get { return _windowHandle; }
        }
    }
}