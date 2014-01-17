using System;
using System.Threading;

namespace WindowsJedi.Components {
	class WindowHookManager {
		private readonly IntPtr _windowsEventsHook;
        private readonly Win32.WinEventDelegate _hookDelegate;
        volatile bool _running;

	    public event EventHandler<WindowFocusChangedEventArgs> WindowFocusChanged;

		public void InvokeWindowFocusChanged(IntPtr windowHandle) {
			EventHandler<WindowFocusChangedEventArgs> handler = WindowFocusChanged;
			if (handler != null) handler(this, new WindowFocusChangedEventArgs(windowHandle));
		}

		public WindowHookManager () {
            _running = true;
            _hookDelegate = WinEventProc;
			_windowsEventsHook = Win32.SetWinEventHook(Win32.EVENT_SYSTEM_FOREGROUND,
						  Win32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
						  WinEventProc, 0, 0, Win32.WINEVENT_OUTOFCONTEXT);

            var t = new Thread(() => {
                while (_running) { Thread.Sleep(10000); } 
            

            GC.KeepAlive(WindowFocusChanged);
            GC.KeepAlive(_windowsEventsHook);
            GC.KeepAlive(_hookDelegate);
            }) {IsBackground = true};
		    t.Start();
		}
        ~WindowHookManager()
        {
			Win32.UnhookWinEvent(_windowsEventsHook);
            _running = false;
		}

		private void WinEventProc (IntPtr hWinEventHook, uint eventType,
		   IntPtr hwnd, int idObject, int idChild,
		   uint dwEventThread, uint dwmsEventTime) {
			if (eventType == Win32.EVENT_SYSTEM_FOREGROUND) {
				InvokeWindowFocusChanged(hwnd);
			}
		}

	}

	internal class WindowFocusChangedEventArgs : EventArgs {
		private readonly IntPtr _windowHandle;

		public WindowFocusChangedEventArgs(IntPtr windowHandle) {
			_windowHandle = windowHandle;
		}

		public IntPtr WindowHandle {
			get { return _windowHandle; }
		}
	}
}
