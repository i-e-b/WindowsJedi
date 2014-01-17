using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using WindowsJedi.Components;

namespace WindowsJedi {
	public class OverlayForm : Form {
		/// <summary>
		/// Set overlay bitmap
		/// </summary>
		public void SetBitmap (Bitmap bitmap) {
			SetBitmap(bitmap, 255);
		}

		/// <summary>
		/// Set overlay bitmap with a custom opacity level.
		/// </summary>
		public void SetBitmap (Bitmap bitmap, byte opacity) {
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

			IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
			IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
			IntPtr hBitmap = IntPtr.Zero;
			IntPtr oldBitmap = IntPtr.Zero;

			try {
				hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
				oldBitmap = Win32.SelectObject(memDc, hBitmap);

				var size = new Win32.Size(bitmap.Width, bitmap.Height);
				var pointSource = new Win32.Point(0, 0);
				var topPos = new Win32.Point(Left, Top);
				var blend = new Win32.BLENDFUNCTION{
					BlendOp = Win32.AC_SRC_OVER,
					BlendFlags = 0,
					SourceConstantAlpha = opacity,
					AlphaFormat = Win32.AC_SRC_ALPHA
				};

				Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
			} finally {
				Win32.ReleaseDC(IntPtr.Zero, screenDc);
				if (hBitmap != IntPtr.Zero) {
					Win32.SelectObject(memDc, oldBitmap);
					Win32.DeleteObject(hBitmap);
				}
				Win32.DeleteDC(memDc);
			}
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= Win32.WS_EX_LAYERED; // This form has to have the WS_EX_LAYERED extended style
				return cp;
			}
		}
	}

	public class DraggableOverlayForm : OverlayForm {
		protected override void WndProc (ref Message m) {
			if (m.Msg == Win32.WM_NCHITTEST) {
				m.Result = (IntPtr)2;	// pass to HTCLIENT
				return;
			}
			base.WndProc(ref m);
		}
	}
}
