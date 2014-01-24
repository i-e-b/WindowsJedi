using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace WindowsJedi.WinApis {
 
    /// <summary>
    /// Provides a set of events that hook into the Win32 window manager
    /// </summary>
    public class WindowHookManager : CriticalFinalizerObject, IDisposable
    {
		private readonly IntPtr _windowsEventsHook;
        private readonly Win32.WinEventDelegate _hookDelegate;
	    private GCHandle _focusedChangedEventPin, _windowsEventHookPin, _hookDelegatePin;

	    public event EventHandler<WindowFocusChangedEventArgs> WindowFocusChanged;

        /// <summary>
        /// Trigger the WindowFocusChanged event for a given window handle
        /// </summary>
		public void InvokeWindowFocusChanged(IntPtr windowHandle) {
			var handler = WindowFocusChanged;
			if (handler != null) handler(this, new WindowFocusChangedEventArgs(windowHandle));
		}

        /// <summary>
        /// Create a window hook manager and start listening for events
        /// </summary>
		public WindowHookManager () {
            _hookDelegate = WinEventProc;

            _focusedChangedEventPin = GCHandle.Alloc(WindowFocusChanged);
            _windowsEventHookPin = GCHandle.Alloc(_windowsEventsHook);
            _hookDelegatePin = GCHandle.Alloc(_hookDelegate);

			_windowsEventsHook = Win32.SetWinEventHook(Win32.EVENT_SYSTEM_FOREGROUND,
						  Win32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                          _hookDelegate, 0, 0, Win32.WINEVENT_OUTOFCONTEXT);

		}
        
        ~WindowHookManager()
        {
            Win32.UnhookWinEvent(_windowsEventsHook);
            if (_focusedChangedEventPin.IsAllocated) _focusedChangedEventPin.Free();
            if (_windowsEventHookPin.IsAllocated) _windowsEventHookPin.Free();
            if (_hookDelegatePin.IsAllocated) _hookDelegatePin.Free();
		}

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            Win32.UnhookWinEvent(_windowsEventsHook);
            if (_focusedChangedEventPin.IsAllocated) _focusedChangedEventPin.Free();
            if (_windowsEventHookPin.IsAllocated) _windowsEventHookPin.Free();
            if (_hookDelegatePin.IsAllocated) _hookDelegatePin.Free();
        }

        /// <summary>
        /// Dispose of hooks and GC pins.
        /// </summary>
	    public void Dispose()
	    {
            Dispose(true);
            GC.SuppressFinalize(this);
	    }

        /// <summary>
        /// Bridge from Win32 callback to .Net event
        /// </summary>
	    private void WinEventProc (IntPtr hWinEventHook, uint eventType,
		   IntPtr hwnd, int idObject, int idChild,
		   uint dwEventThread, uint dwmsEventTime) {
			if (eventType == Win32.EVENT_SYSTEM_FOREGROUND) {
				InvokeWindowFocusChanged(hwnd);
			}
		}

	}

    public class WindowFocusChangedEventArgs : EventArgs {
		private readonly IntPtr _windowHandle;

		public WindowFocusChangedEventArgs(IntPtr windowHandle) {
			_windowHandle = windowHandle;
		}

		public IntPtr WindowHandle {
			get { return _windowHandle; }
		}
	}
}
