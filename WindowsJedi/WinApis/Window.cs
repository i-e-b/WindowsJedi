using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WindowsJedi.WinApis.Data;

namespace WindowsJedi.WinApis {
    /// <summary>
    /// A DWM wrapper around a Win32 windows handle
    /// </summary>
    public class Window : IDisposable, IEquatable<Window>
    {

        private string _title;
        private readonly IntPtr _handle;
        private IntPtr _dwmThumb;

		public Window(IntPtr handle) {
			_handle = handle;
		}

        public Window(IntPtr handle, string title): this(handle)
        {
            _title = title;
        }

        ~Window()
        {
            Dispose(false);
		}

        protected virtual void Dispose(bool disposing)
        {
			if (_dwmThumb != IntPtr.Zero)
				Win32.DwmUnregisterThumbnail(_dwmThumb);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public override int GetHashCode()
        {
            return _handle.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is Window && Equals((Window)obj);
        }

        public bool Equals(Window other)
        {
            return _handle == other._handle;
        }

        public override string ToString () {
			return Title;
		}

		public string Title {
			get {
			    if (_title != null) return _title;

			    var sb = new StringBuilder(1000);
			    Win32.GetWindowText(_handle, sb, sb.Capacity);
			    _title = sb.ToString();
			    return _title;
			}
		}

        /// <summary>
        /// Gives the size of a window in 'Restored' mode
        /// </summary>
        public Rectangle NormalRectangle
        {
            get
            {
                var placement = Win32.WindowPlacement.Default;
                if (!Win32.GetWindowPlacement(_handle, ref placement))
                {
                    Win32.Rect rect;
                    Win32.GetWindowRect(_handle, out rect);
                    return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                }
                return new Rectangle(
                    placement.NormalPosition.Left,
                    placement.NormalPosition.Top,
                    placement.NormalPosition.Right - placement.NormalPosition.Left,
                    placement.NormalPosition.Bottom - placement.NormalPosition.Top);
            }
        }

        /// <summary>
        /// Gets the best estimate of a window's presentation size
        /// </summary>
        public Rectangle SafeRectangle
        {
            get {
                var normal = NormalRectangle;
                var layout = LayoutRectangle;

                var nsize = normal.Width * normal.Height;
                var lsize = layout.Width * layout.Height;

                return (nsize > lsize) ? normal : layout;
            }
        }

        /// <summary>
        /// Gives the current layout rectangle of the window
        /// </summary>
		public Rectangle LayoutRectangle {
			get {
                Win32.Rect rect;
                Win32.GetWindowRect(_handle, out rect);
                return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
			}
		}

        /// <summary>
        /// Internal target for this window. Used for layout algorithms.
        /// </summary>
		public Rectangle? TargetRectangle { get; set; }

        /// <summary>
        /// Size of window as it would be if it was focused state
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                var s = DwmThumbAspect();

                if (s.IsEmpty) return SafeRectangle;

                return new Rectangle(0, 0, s.Width, s.Height);
            }
        }

        /// <summary>
        /// Returns true if this window is a pop-up style.
        /// </summary>
        public bool IsPopup
        {
            get
            {
                return ((ulong)Win32.GetWindowLongPtr(_handle, Win32.GWL_STYLE).ToInt64() & Win32.WS_POPUP) != 0;
            }
        }

        /// <summary>
        /// Returns true if this window is currently visible, false if hidden.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return WindowIsVisible(_handle);
            }
        }
        /// <summary>
        /// Returns true if the window is still present (regardless of if it's visible)
        /// <para>Returns false if window has been closed / disposed</para>
        /// </summary>
        public bool Exists {
            get
            {
                return Win32.IsWindow(_handle);
            }
        }

        /// <summary>
        /// Target window's handle
        /// </summary>
        public IntPtr Handle { get { return _handle; } }

        /// <summary>
        /// Return approximate aspect ratio of the thumbnail DWM would show.
        /// </summary>
        public Size DwmThumbAspect()
        {
            IntPtr dwmThumb = IntPtr.Zero;
            try
            {
                int i = Win32.DwmRegisterThumbnail(Program.ShellEventsDelegateForm.Handle, _handle, out dwmThumb);
                if (i != 0) return new Size(SafeRectangle.Width, SafeRectangle.Height);
                
                Win32.PSIZE size;
                Win32.DwmQueryThumbnailSourceSize(dwmThumb, out size);

                return new Size(size.x, size.y);
            } 
            finally
            {
                if (dwmThumb != IntPtr.Zero) Win32.DwmUnregisterThumbnail(dwmThumb);
            }
        }

		public void ShowDwmThumb(Form host, Rectangle destination) {
			ReleaseDwmThumb();

			int i = Win32.DwmRegisterThumbnail(host.Handle, _handle, out _dwmThumb);
			if (i != 0) return;

			Win32.PSIZE size;
			Win32.DwmQueryThumbnailSourceSize(_dwmThumb, out size);

			var props = new Win32.DWM_THUMBNAIL_PROPERTIES
			            {
			            	fVisible = true,
			            	opacity = 255,
			            	rcDestination = new Win32.Rect(destination.Left, destination.Top, destination.Right, destination.Bottom),
			            	dwFlags = Win32.DWM_TNP_VISIBLE | Win32.DWM_TNP_RECTDESTINATION | Win32.DWM_TNP_OPACITY
			            };

			if (size.x < destination.Width)
				props.rcDestination.Right = props.rcDestination.Left + size.x;

			if (size.y < destination.Height)
				props.rcDestination.Bottom = props.rcDestination.Top + size.y;

			Win32.DwmUpdateThumbnailProperties(_dwmThumb, ref props);
		}

		public void ReleaseDwmThumb() {
			if (_dwmThumb != IntPtr.Zero) {
				Win32.DwmUnregisterThumbnail(_dwmThumb);
				_dwmThumb = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Make this window visible and bring it to the front with keyboard focus
		/// </summary>
		public void Focus () {
			var placement = Win32.WindowPlacement.Default;
			if (Win32.GetWindowPlacement(_handle, ref placement))
				if (placement.ShowCmd == Win32.ShowWindowCommand.ShowMinimized)
					Win32.ShowWindow(_handle, Win32.ShowWindowCommand.Restore);

            Win32.SetActiveWindow(_handle);
			Win32.BringWindowToTop(_handle);
			Win32.SetForegroundWindow(_handle);
		}

        /// <summary>
        /// Set a window to layered and transparent[0..255]opaque
        /// </summary>
        public void SetTranslucent(byte translucency)
        {// Set WS_EX_LAYERED on this window 
            Win32.SetWindowLongPtr(_handle, Win32.GWL_EXSTYLE, new IntPtr(Win32.GetWindowLongPtr(_handle, Win32.GWL_EXSTYLE).ToInt32() | Win32.WS_EX_LAYERED));

            // Make this window 70% alpha
            Win32.SetLayeredWindowAttributes(_handle, 0, translucency, LayeredWindowFlags.UseAlpha);

            // Ask the window and its children to repaint
            Win32.RedrawWindow(_handle, IntPtr.Zero, IntPtr.Zero, Win32.RDW_ERASE | Win32.RDW_INVALIDATE | Win32.RDW_FRAME | Win32.RDW_ALLCHILDREN);
        }


        /// <summary>
        /// Set a window to non-layered and opaque
        /// </summary>
        public void SetOpaque()
        {
            // Remove WS_EX_LAYERED from this window styles
            Win32.SetWindowLongPtr(_handle, Win32.GWL_EXSTYLE, new IntPtr(Win32.GetWindowLongPtr(_handle, Win32.GWL_EXSTYLE).ToInt32() & ~Win32.WS_EX_LAYERED));

            // Ask the window and its children to repaint
            Win32.RedrawWindow(_handle, IntPtr.Zero, IntPtr.Zero, Win32.RDW_ERASE | Win32.RDW_INVALIDATE | Win32.RDW_FRAME | Win32.RDW_ALLCHILDREN);
        }

        /// <summary>
        /// Hide this window
        /// </summary>
        public void Hide()
        {
            Win32.ShowWindow(_handle, Win32.ShowWindowCommand.Hide);
        }

        /// <summary>
        /// Show this window
        /// </summary>
        public void Show()
        {
            Win32.ShowWindow(_handle, Win32.ShowWindowCommand.Show);
        }

        public static Window ForegroundWindow()
        {
            var target = Win32.GetForegroundWindow();
            return target == IntPtr.Zero ? null : new Window(target);
        }

        public static Window ActiveWindow()
        {
            var target = Win32.GetActiveWindow();
            return target == IntPtr.Zero ? null : new Window(target);
        }

        /// <summary>
        /// Returns the next visible window below this one, or null if it's at the bottom
        /// </summary>
        public Window NextVisibleBelow()
        {
            if (_handle == IntPtr.Zero) return null;
            var found = Win32.GetWindow(_handle, WindowStack.BelowTarget);

            while (found != IntPtr.Zero)
            {
                if (WindowIsVisible(found)) return new Window(found);
                found = Win32.GetWindow(found, WindowStack.BelowTarget);
            }
            return null;
        }

        /// <summary>
        /// Try to return the Exe icon for a given window. Tries to return largest first
        /// <para>Return null if one can't be found</para>
        /// </summary>
        public Icon GetAppIcon()
        {
            var iconHandle = Win32.SendMessage(_handle, Win32.WM_GETICON, Win32.ICON_BIG, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = Win32.SendMessage(_handle, Win32.WM_GETICON, Win32.ICON_SMALL, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = Win32.SendMessage(_handle, Win32.WM_GETICON, Win32.ICON_SMALL2, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(_handle, Win32.GCL_HICON);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(_handle, Win32.GCL_HICONSM);

            return iconHandle == IntPtr.Zero ? null : Icon.FromHandle(iconHandle);
        }

        static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4) return Win32.GetClassLongPtr64(hWnd, nIndex);
            return new IntPtr(Win32.GetClassLongPtr32(hWnd, nIndex));
        }

        private static bool WindowIsVisible(IntPtr target)
        {
            return ((ulong)Win32.GetWindowLongPtr(target, Win32.GWL_STYLE).ToInt64() & Win32.WS_VISIBLE) != 0;
        }

        /// <summary>
        ///  Get process id for a window by it's window handle
        /// </summary>
        public static uint WindowProcess(IntPtr hWnd)
        {
            uint processid;
            Win32.GetWindowThreadProcessId(hWnd, out processid);
            return processid;
        }

        /// <summary>
        /// Leave window visible, but send it to the back of the z-order stack
        /// </summary>
        public void SendToBack()
        {
            Win32.SetWindowPos(_handle, Win32.HWND_BOTTOM, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
        }

        /// <summary>
        /// Send this Window to the front of the z-order stack
        /// </summary>
        public void SendToFront()
        {
            //Win32.SetWindowPos(_handle, Win32.HWND_TOP, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE);
            Win32.BringWindowToTop(_handle);
        }

        /// <summary>
        /// Move this window to directly under the target
        /// </summary>
        public void PutUnder(IntPtr targetHwnd)
        {
            Win32.SetWindowPos(_handle, targetHwnd, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
        }

        /// <summary>
        /// Send a close message to a window. This is not a force close, so it can be ignored or cause more windows to show up.
        /// </summary>
        public void Close()
        {
            Win32.SendMessage(_handle, Win32.WM_CLOSE, 0, 0);
        }

        /// <summary>
        /// Return the bounds of the screen that contains the most of this window.
        /// Gives the working area, which excludes the taskbar
        /// </summary>
        public Rectangle ScreenRect()
        {
            return Screen.FromRectangle(NormalRectangle).WorkingArea;
        }

        public void SetBounds(int left, int top, int width, int height)
        {
            Win32.SetWindowPos(_handle, IntPtr.Zero, left, top, width, height, Win32.SWP_NOZORDER);
        }

        /// <summary>
        /// Returns the screen that contains the most of this window
        /// </summary>
        public Screen PrimaryScreen()
        {
            return Screen.FromRectangle(NormalRectangle);
        }

        /// <summary>
        /// Move window to a new position without changing size or Z-order
        /// </summary>
        public void MoveTo(Point newOffset)
        {
            Win32.SetWindowPos(_handle, IntPtr.Zero, newOffset.X, newOffset.Y, 0, 0, Win32.SWP_NOZORDER | Win32.SWP_NOSIZE);
        }
    }
}