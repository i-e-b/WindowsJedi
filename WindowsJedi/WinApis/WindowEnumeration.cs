using System;
using System.Collections.Generic;

namespace WindowsJedi.WinApis {
	public static class WindowEnumeration {
		private static readonly List<Window> Windows = new List<Window>();

		public static List<Window> GetCurrentWindows() {
			Windows.Clear();
			Win32.EnumWindows(Callback, 0);
			return Windows;
		}

		private static bool Callback (IntPtr hwnd, int lParam) {
			if ((Win32.GetWindowLongA(hwnd, Win32.GWL_STYLE) & (Win32.WS_BORDER | Win32.WS_CHILD | Win32.WS_VISIBLE))
				== (Win32.WS_BORDER | Win32.WS_VISIBLE)
				) {
				Windows.Add(new Window(hwnd));
			}
			return true; // continue enumeration
		}

	}
}
