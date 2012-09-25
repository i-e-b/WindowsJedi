using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsJedi.Components {
	public class Window {
		private string title;
		public IntPtr Handle;
		public IntPtr DwmThumb;

		public Window(IntPtr handle) {
			Handle = handle;
		}

		~Window () {
			if (DwmThumb != IntPtr.Zero)
				Win32.DwmUnregisterThumbnail(DwmThumb);
		}

		public override string ToString () {
			return Title;
		}

		public string Title {
			get {
				if (title == null) {
					var sb = new StringBuilder(100);
					Win32.GetWindowText(Handle, sb, sb.Capacity);
					title = sb.ToString();
				}
				return title;
			}
		}

		public Rectangle Rectangle {
			get {
				var placement = Win32.WindowPlacement.Default;
				if (!Win32.GetWindowPlacement(Handle, ref placement)) {
					Win32.Rect rect;
					Win32.GetWindowRect(Handle, out rect);
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

			int i = Win32.DwmRegisterThumbnail(host.Handle, Handle, out DwmThumb);
			if (i != 0) return;

			Win32.PSIZE size;
			Win32.DwmQueryThumbnailSourceSize(DwmThumb, out size);

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

			Win32.DwmUpdateThumbnailProperties(DwmThumb, ref props);
		}

		public void ReleaseDwmThumb() {
			if (DwmThumb != IntPtr.Zero) {
				Win32.DwmUnregisterThumbnail(DwmThumb);
				DwmThumb = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Make this window visible and bring it to the front with keyboard focus
		/// </summary>
		public void Focus () {
			var placement = Win32.WindowPlacement.Default;
			if (Win32.GetWindowPlacement(Handle, ref placement))
				if (placement.ShowCmd == Win32.ShowWindowCommand.ShowMinimized)
					Win32.ShowWindow(Handle, Win32.ShowWindowCommand.Restore);

			Win32.BringWindowToTop(Handle);
			Win32.SetForegroundWindow(Handle);
		}
	}
}