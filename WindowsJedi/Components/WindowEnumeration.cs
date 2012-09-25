﻿using System;
using System.Collections.Generic;

// TODO:
// Seems like the DWM only wants to do the compositing itself.
// Need to change this to a form, and register several thumbnails at once.

namespace WindowsJedi.Components {
	public static class WindowEnumeration {
		private static readonly List<Window> windows = new List<Window>();

		public static List<Window> GetCurrentWindows() {
			windows.Clear();
			Win32.EnumWindows(Callback, 0);
			return windows;
		}

		private static bool Callback (IntPtr hwnd, int lParam) {
			if ((Win32.GetWindowLongA(hwnd, Win32.GWL_STYLE) & (Win32.WS_BORDER | Win32.WS_CHILD | Win32.WS_VISIBLE))
				== (Win32.WS_BORDER | Win32.WS_VISIBLE)
				) {
				windows.Add(new Window(hwnd));
			}
			return true; // continue enumeration
		}

	}
}
