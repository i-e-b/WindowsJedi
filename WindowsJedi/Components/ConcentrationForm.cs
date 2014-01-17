using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsJedi.Components {
	/// <summary>
	/// A form which covers all screens and places itself behind another window
	/// </summary>
	public class ConcentrationForm : FullScreenForm {
		private IntPtr _currentWindow;
		private IntPtr _previousWindow;
		private volatile bool _locking;
		private readonly WindowHookManager _winHook;

		public ConcentrationForm () {
			Name = "WindowsJedi Concentration Veil";
			_winHook = new WindowHookManager();
		}
		~ConcentrationForm () {
			UnConcentrate();
		}

		/// <summary>
		/// Toggle focus lock on or off
		/// </summary>
		public void Toggle() {
			if (_locking) UnConcentrate();
			else Concentrate();
		}

		/// <summary>
		/// Lock focus on current frontmost window
		/// </summary>
		public void Concentrate() {
			if (_locking) return;
			_locking = true;
			_currentWindow = Win32.GetForegroundWindow();
			_previousWindow = _currentWindow;
			if (InvokeRequired) {
				Invoke(new MethodInvoker(ConcentrateFadeIn));
			} else {
				ConcentrateFadeIn();
			}
		}

		/// <summary>
		/// Unlock a previously locked focus
		/// </summary>
		public void UnConcentrate () {
			if (!_locking) return;
			_locking = false;
			_winHook.WindowFocusChanged -= winHook_WindowFocusChanged;
			_currentWindow = _previousWindow;
			if (InvokeRequired) {
				Invoke(new MethodInvoker(ConcentrateFadeOut));
			} else {
				ConcentrateFadeOut();
			}
		}

		protected override void WndProc (ref Message m) {
			if (m.Msg >= 0x201 && m.Msg <= 0x209) { // We've been clicked on
				UnConcentrate();
				m.Result = new IntPtr(1); // capture the event to prevent propagation
			} else {
				base.WndProc(ref m);
			}
		}

		void winHook_WindowFocusChanged (object sender, WindowFocusChangedEventArgs e) {
			if (!Win32.LockSetForegroundWindow(Win32.LSFW_UNLOCK)) {}
			if (_locking && (e.WindowHandle != this.Handle) && (e.WindowHandle != _currentWindow)) {
				SuspendLayout();
				TopMost = true;
				Win32.BringWindowToTop(this.Handle);
				Win32.SetForegroundWindow(this.Handle);
				TopMost = false;
				Win32.BringWindowToTop(_currentWindow);
				Win32.SetForegroundWindow(_currentWindow);
				ResumeLayout(false);
			}
			if (!Win32.LockSetForegroundWindow(Win32.LSFW_LOCK)) {}
		}

		private void ConcentrateFadeIn () {
			Opacity = 0;
			Show();
			Win32.BringWindowToTop(_currentWindow);
			if (!Win32.LockSetForegroundWindow(Win32.LSFW_LOCK)) {}
			
			while (Opacity < .70 && _locking) {
				Application.DoEvents();
				Thread.Sleep(5);
				Opacity += .04;
			}
			
			_winHook.WindowFocusChanged += winHook_WindowFocusChanged;
		}

		private void ConcentrateFadeOut () {
			//Show();
			Win32.BringWindowToTop(_currentWindow);
			if (!Win32.LockSetForegroundWindow(Win32.LSFW_UNLOCK)) {}

			while (Opacity > 0 && !_locking) {
				Application.DoEvents();
				Thread.Sleep(5);
				Opacity -= .04;
			}
			Hide();
			_currentWindow = IntPtr.Zero;
		}
	}
}
