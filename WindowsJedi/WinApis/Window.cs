using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsJedi.WinApis {
    /// <summary>
    /// A DWM wrapper around a Win32 windows handle
    /// </summary>
	public class Window :IDisposable {
		private string _title;
        private readonly IntPtr _handle;
        private IntPtr _dwmThumb;

		public Window(IntPtr handle) {
			_handle = handle;
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

        public override string ToString () {
			return Title;
		}

		public string Title {
			get {
				if (_title == null) {
					var sb = new StringBuilder(100);
					Win32.GetWindowText(_handle, sb, sb.Capacity);
					_title = sb.ToString();
				}
				return _title;
			}
		}

		public Rectangle Rectangle {
			get {
				var placement = Win32.WindowPlacement.Default;
				if (!Win32.GetWindowPlacement(_handle, ref placement)) {
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

		public Rectangle? TargetRectangle { get; set; }

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

			Win32.BringWindowToTop(_handle);
			Win32.SetForegroundWindow(_handle);
		}
	}
}