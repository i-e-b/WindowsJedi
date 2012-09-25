using System;

namespace WindowsJedi.Components {
	class WindowHookManager {
		private readonly IntPtr windowsEventsHook;

		public event EventHandler<WindowFocusChangedEventArgs> WindowFocusChanged;

		public void InvokeWindowFocusChanged(IntPtr windowHandle) {
			EventHandler<WindowFocusChangedEventArgs> handler = WindowFocusChanged;
			if (handler != null) handler(this, new WindowFocusChangedEventArgs(windowHandle));
		}

		public WindowHookManager () {
			windowsEventsHook = Win32.SetWinEventHook(Win32.EVENT_SYSTEM_FOREGROUND,
						  Win32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
						  WinEventProc, 0, 0, Win32.WINEVENT_OUTOFCONTEXT);
		}
		~WindowHookManager () {
			Win32.UnhookWinEvent(windowsEventsHook);
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
		private readonly IntPtr windowHandle;

		public WindowFocusChangedEventArgs(IntPtr windowHandle) {
			this.windowHandle = windowHandle;
		}

		public IntPtr WindowHandle {
			get { return windowHandle; }
		}
	}
}
