using System;
using System.Runtime.InteropServices;

namespace WindowsJedi.Components {
	class WindowHookManager {
		private readonly IntPtr _windowsEventsHook;
        private readonly Win32.WinEventDelegate _hookDelegate;
	    private GCHandle _pin1, _pin2, _pin3;

	    public event EventHandler<WindowFocusChangedEventArgs> WindowFocusChanged;

		public void InvokeWindowFocusChanged(IntPtr windowHandle) {
			EventHandler<WindowFocusChangedEventArgs> handler = WindowFocusChanged;
			if (handler != null) handler(this, new WindowFocusChangedEventArgs(windowHandle));
		}

		public WindowHookManager () {
            _hookDelegate = WinEventProc;
			_windowsEventsHook = Win32.SetWinEventHook(Win32.EVENT_SYSTEM_FOREGROUND,
						  Win32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
						  WinEventProc, 0, 0, Win32.WINEVENT_OUTOFCONTEXT);

            _pin1 = GCHandle.Alloc(WindowFocusChanged);
            _pin2 = GCHandle.Alloc(_windowsEventsHook);
            _pin3 = GCHandle.Alloc(_hookDelegate);
		}
        ~WindowHookManager()
        {
			Win32.UnhookWinEvent(_windowsEventsHook);
            _pin1.Free();
            _pin2.Free();
            _pin3.Free();
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
