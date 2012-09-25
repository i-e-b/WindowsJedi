using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsJedi {
	public class KeyHookManager {
		private readonly HashSet<Keys> keyboardState;

		public KeyHookManager () {
			keyboardState = new HashSet<Keys>();
			Start();
		}

		~KeyHookManager () {
			Stop();
		}

		public event KeyEventHandler KeyDown;
		public event KeyPressEventHandler KeyPress;
		public event KeyEventHandler KeyUp;

		static int hKeyboardHook;
		Win32.HookProc KeyboardHookProcedure;

		[StructLayout(LayoutKind.Sequential)]
		public class KeyboardHookStruct {
			public int vkCode;	//Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
			public int scanCode; // Specifies a hardware scan code for the key. 
			public int flags;  // Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
			public int time; // Specifies the time stamp for this message.
			public int dwExtraInfo; // Specifies extra information associated with the message. 
		}

		public void Start () {
			KeyboardHookProcedure = new Win32.HookProc(KeyboardHookProc);
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule) {
				hKeyboardHook = Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, KeyboardHookProcedure,
					Win32.GetModuleHandle(curModule.ModuleName), 0);
			}
			if (hKeyboardHook == 0) {
				Stop();
				throw new Exception("SetWindowsHookEx startup failed.");
			}
		}

		public void Stop () {
			if (hKeyboardHook == 0) return;
			Win32.UnhookWindowsHookEx(hKeyboardHook);
			hKeyboardHook = 0;
		}

		public static bool IsKeyHeld (Keys key) {
			short s = Win32.GetAsyncKeyState((int)key);
			if (s == -32767 || s == -32768) return true;
			return false;
		}

		public bool IsKeyComboPressed(Keys[] combo) {
			return keyboardState.SetEquals(combo);
		}

		private int KeyboardHookProc (int nCode, Int32 wParam, IntPtr lParam) {
			bool suppressKeyPress = false;
			if ((nCode >= 0) && (KeyDown != null || KeyUp != null || KeyPress != null)) {
				var MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
				if (wParam == Win32.WM_KEYDOWN || wParam == Win32.WM_SYSKEYDOWN) {
					var keyData = (Keys)MyKeyboardHookStruct.vkCode;
					keyboardState.Add(keyData);

					if (KeyDown != null) {
						var e = new KeyEventArgs(keyData);
						KeyDown(this, e);
						suppressKeyPress |= e.SuppressKeyPress;
					}
				}

				if (KeyPress != null && (wParam == Win32.WM_KEYDOWN|| wParam == Win32.WM_SYSKEYDOWN)) {
					var keyState = new byte[256];
					Win32.GetKeyboardState(keyState);

					var inBuffer = new byte[2];
					if (Win32.ToAscii(MyKeyboardHookStruct.vkCode,
							MyKeyboardHookStruct.scanCode,
							keyState,
							inBuffer,
							MyKeyboardHookStruct.flags) == 1) {
						var e = new KeyPressEventArgs((char)inBuffer[0]);
						KeyPress(this, e);
						suppressKeyPress |= e.Handled;
					}
				}

				if (wParam == Win32.WM_KEYUP || wParam == Win32.WM_SYSKEYUP) {
					var keyData = (Keys)MyKeyboardHookStruct.vkCode;
					keyboardState.Remove(keyData);

					if (KeyUp != null) {
						var e = new KeyEventArgs(keyData);
						KeyUp(this, e);
						suppressKeyPress |= e.SuppressKeyPress;
					}
				}

			}
			if (suppressKeyPress) return 1;
			return Win32.CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
		}



	}
}
