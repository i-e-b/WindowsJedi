using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace WindowsJedi.WinApis {
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using WindowsJedi.WinApis.ManagedEvents;

    /// <summary>
    /// Provides a set of events that hook into the Win32 window manager
    /// </summary>
    public class WindowHookManager : CriticalFinalizerObject, IDisposable
    {
		private readonly IntPtr _windowsEventsHook;
        private GCHandle _focusedChangedEventPin, _hookDelegatePin;
        int _ownProcessId;

        #region Events
        public event EventHandler<WindowHandleEventArgs> WindowFocusChanged;
        public event EventHandler<WindowHandleEventArgs> WindowCreated;
        public event EventHandler<WindowHandleEventArgs> WindowDestroyed;

        /// <summary>
        /// Trigger the WindowFocusChanged event for a given window handle
        /// </summary>
		public void InvokeWindowFocusChanged(IntPtr windowHandle) {
			var handler = WindowFocusChanged;
			if (handler != null) handler(this, new WindowHandleEventArgs(windowHandle));
		}

        /// <summary>
        /// Trigger the WindowFocusChanged event for a given window handle
        /// </summary>
        public void InvokeWindowCreated(IntPtr windowHandle)
        {
            var handler = WindowCreated;
            if (handler != null) handler(this, new WindowHandleEventArgs(windowHandle));
        }

        /// <summary>
        /// Trigger the WindowFocusChanged event for a given window handle
        /// </summary>
        public void InvokeWindowDestroyed(IntPtr windowHandle)
        {
            var handler = WindowDestroyed;
            if (handler != null) handler(this, new WindowHandleEventArgs(windowHandle));
        }
        #endregion

        /// <summary>
        /// Create a window hook manager and start listening for events
        /// </summary>
		public WindowHookManager () {
            Win32.WinEventDelegate hookDelegate = WinEventProc;

            _focusedChangedEventPin = GCHandle.Alloc(WindowFocusChanged);
            _hookDelegatePin = GCHandle.Alloc(hookDelegate);

            _ownProcessId = Process.GetCurrentProcess().Id;

			_windowsEventsHook = Win32.SetWinEventHook(
                Win32.EVENT_MIN,Win32.EVENT_MAX, // give me all the events. This may cause slow-down...
                IntPtr.Zero,
                hookDelegate, 
                0, 0, // all processes and threads
                Win32.WINEVENT_OUTOFCONTEXT | Win32.WINEVENT_SKIPOWNPROCESS); // only other processes
		}
        
        ~WindowHookManager()
        {
            Win32.UnhookWinEvent(_windowsEventsHook);
            if (_focusedChangedEventPin.IsAllocated) _focusedChangedEventPin.Free();
            if (_hookDelegatePin.IsAllocated) _hookDelegatePin.Free();
		}

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            Win32.UnhookWinEvent(_windowsEventsHook);
            if (_focusedChangedEventPin.IsAllocated) _focusedChangedEventPin.Free();
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
        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
                                  IntPtr hwnd, int idObject, int idChild,
                                  uint dwEventThread, uint dwmsEventTime)
        {
            if (OwnProcessThread(hwnd)) return;

            switch (eventType) {
                case Win32.EVENT_SYSTEM_FOREGROUND:
                    InvokeWindowFocusChanged(hwnd);
                    return;

                case Win32.EVENT_OBJECT_CREATE:
                    DispatchObjectCreation(hwnd, idObject, idChild);
                    return;

                case Win32.EVENT_OBJECT_DESTROY:
                    DispatchObjectDestruction(hwnd, idObject, idChild);
                    return;

                default: // an event we don't have a C# handler for
                    return;
            }
        }

        bool OwnProcessThread(IntPtr hwnd)
        {
            var procid = Window.WindowProcess(hwnd);
            return procid == _ownProcessId || procid == 0;
        }

        void DispatchObjectDestruction(IntPtr hwnd, int idObject, int idChild)
        {
            // Only handle created windows for now
            if (idObject != Win32.OBJID_WINDOW) return;
            if (idChild != 0) return;

            InvokeWindowCreated(hwnd);
        }

        void DispatchObjectCreation(IntPtr hwnd, int idObject, int idChild)
        {
            // Only handle created windows for now
            if (idObject != Win32.OBJID_WINDOW) return;
            if (idChild != 0) return;
            if (OwnProcessThread(hwnd)) return;

            InvokeWindowDestroyed(hwnd);
        }
    }
}
