﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsJedi {
	/// <summary>
	/// A big pile of Win32 low level calls, constants and data types
	/// </summary>
	internal class Win32 {

		#region PInvoke Types
		public enum Bool {
			False = 0,
			True
		};

		[Flags]
		public enum KeyboardInputFlags : uint {
			ExtendedKey = 1,
			KeyUp = 2,
			None = 0,
			Scancode = 8,
			Unicode = 4
		}

		internal enum ShowWindowCommand {
			/// <summary>
			/// Hides the window and activates another window.
			/// </summary>
			Hide = 0,
			/// <summary>
			/// Activates and displays a window. If the window is minimized or 
			/// maximized, the system restores it to its original size and position.
			/// An application should specify this flag when displaying the window 
			/// for the first time.
			/// </summary>
			Normal = 1,
			/// <summary>
			/// Activates the window and displays it as a minimized window.
			/// </summary>
			ShowMinimized = 2,
			/// <summary>
			/// Maximizes the specified window.
			/// </summary>
			Maximize = 3, // is this the right value?
			/// <summary>
			/// Activates the window and displays it as a maximized window.
			/// </summary>       
			ShowMaximized = 3,
			/// <summary>
			/// Displays a window in its most recent size and position. This value 
			/// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
			/// the window is not activated.
			/// </summary>
			ShowNoActivate = 4,
			/// <summary>
			/// Activates the window and displays it in its current size and position. 
			/// </summary>
			Show = 5,
			/// <summary>
			/// Minimizes the specified window and activates the next top-level 
			/// window in the Z order.
			/// </summary>
			Minimize = 6,
			/// <summary>
			/// Displays the window as a minimized window. This value is similar to
			/// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
			/// window is not activated.
			/// </summary>
			ShowMinNoActive = 7,
			/// <summary>
			/// Displays the window in its current size and position. This value is 
			/// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
			/// window is not activated.
			/// </summary>
			ShowNA = 8,
			/// <summary>
			/// Activates and displays the window. If the window is minimized or 
			/// maximized, the system restores it to its original size and position. 
			/// An application should specify this flag when restoring a minimized window.
			/// </summary>
			Restore = 9,
			/// <summary>
			/// Sets the show state based on the SW_* value specified in the 
			/// STARTUPINFO structure passed to the CreateProcess function by the 
			/// program that started the application.
			/// </summary>
			ShowDefault = 10,
			/// <summary>
			///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
			/// that owns the window is not responding. This flag should only be 
			/// used when minimizing windows from a different thread.
			/// </summary>
			ForceMinimize = 11
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		internal struct WindowPlacement {
			public int Length;
			public int Flags;
			public ShowWindowCommand ShowCmd;
			public Point MinPosition;
			public Point MaxPosition;
			public Rect NormalPosition;
			public static WindowPlacement Default {
				get {
					var result = new WindowPlacement();
					result.Length = Marshal.SizeOf(result);
					return result;
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct DWM_THUMBNAIL_PROPERTIES {
			public int dwFlags;
			public Rect rcDestination;
			public Rect rcSource;
			public byte opacity;
			public bool fVisible;
			public bool fSourceClientAreaOnly;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct Rect {
			internal Rect (int left, int top, int right, int bottom) {
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct PSIZE {
			public int x;
			public int y;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct Point {
			public Int32 x;
			public Int32 y;

			public Point (Int32 x, Int32 y) { this.x = x; this.y = y; }
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Size {
			public Int32 cx;
			public Int32 cy;

			public Size (Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
		}

		/*[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct ARGB {
			public byte Blue;
			public byte Green;
			public byte Red;
			public byte Alpha;
		}*/

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION {
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}
		#endregion
		#region Constants
		#region Window Manager
		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_SYSKEYDOWN = 0x104;
		public const int WM_SYSKEYUP = 0x105;
		public const int WM_NCHITTEST = 0x0084;
		public const int WM_ACTIVATEAPP = 0x001C;
		#endregion

		#region Windows Hooks
		public const int WH_KEYBOARD_LL = 13;
		public const int WH_CBT = 5;
		#endregion

		#region Window Style Flags
		public const ulong WS_VISIBLE = 0x10000000L;
		public const ulong WS_BORDER = 0x00800000L;
		public const ulong WS_CHILD = 0x40000000L;
		public const ulong WS_OVERLAPPED = 0;
		public const ulong WS_POPUP = 0x80000000;

		public const int WS_EX_TOPMOST = 0x00000008;
		public const int WS_EX_APPWINDOW = 0x00040000;
		public const int WS_EX_LAYERED = 0x00080000;
		public const int WS_EX_TOOLWINDOW = 0x80;
		#endregion

		#region Graphic Composition Flags
		public const Int32 ULW_COLORKEY = 0x00000001;
		public const Int32 ULW_ALPHA = 0x00000002;
		public const Int32 ULW_OPAQUE = 0x00000004;

		public const byte AC_SRC_OVER = 0x00;
		public const byte AC_SRC_ALPHA = 0x01;
		#endregion

		#region Windows Events Flags
		public const int LSFW_LOCK = 1;
		public const int LSFW_UNLOCK = 2;
		public const int HCBT_SETFOCUS = 9;

		public const uint WINEVENT_OUTOFCONTEXT = 0;
		public const uint EVENT_SYSTEM_FOREGROUND = 3;
		#endregion

		#region DWM flags
		public const int GWL_STYLE = -16;
		public const int DWM_TNP_VISIBLE = 0x8;
		public const int DWM_TNP_OPACITY = 0x4;
		public const int DWM_TNP_RECTDESTINATION = 0x1;
		#endregion
		#endregion

		#region Kernel 32
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle (string lpModuleName);
		#endregion
		#region GDI 32
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC (IntPtr hDC);

		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern Bool DeleteDC (IntPtr hdc);

		[DllImport("gdi32.dll", ExactSpelling = true)]
		public static extern IntPtr SelectObject (IntPtr hDC, IntPtr hObject);

		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern Bool DeleteObject (IntPtr hObject);
		#endregion
		#region User 32
		[DllImport("user32.dll")]
		public static extern void keybd_event (byte key, byte scan, KeyboardInputFlags flags, IntPtr extraInfo);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow (string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx (IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnableWindow (IntPtr hWnd, bool bEnable);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow (IntPtr hWnd, ShowWindowCommand nCmdShow);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowPlacement (IntPtr hWnd, ref WindowPlacement lpwndpl);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPlacement (IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect (IntPtr hwnd, out Rect lpRect);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow ();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow (IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool LockSetForegroundWindow (uint uLockCode);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern Bool UpdateLayeredWindow (IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetDC (IntPtr hWnd);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern int ReleaseDC (IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll", CharSet = CharSet.Auto,
			CallingConvention = CallingConvention.StdCall)]
		public static extern int SetWindowsHookEx (int idHook, HookProc lpfn,
			IntPtr hInstance, int threadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto,
			 CallingConvention = CallingConvention.StdCall)]
		public static extern bool UnhookWindowsHookEx (int idHook);

		[DllImport("user32.dll", CharSet = CharSet.Auto,
			 CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx (int idHook, int nCode,
			Int32 wParam, IntPtr lParam);


		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState (int vKey);


		[DllImport("user32.dll")]
		public static extern int ToAscii (int uVirtKey, //[in] Specifies the virtual-key code to be translated. 
										 int uScanCode, // [in] Specifies the hardware scan code of the key to be translated. The high-order bit of this value is set if the key is up (not pressed). 
										 byte[] lpbKeyState, // [in] Pointer to a 256-byte array that contains the current keyboard state. Each element (byte) in the array contains the state of one key. If the high-order bit of a byte is set, the key is down (pressed). The low bit, if set, indicates that the key is toggled on. In this function, only the toggle bit of the CAPS LOCK key is relevant. The toggle state of the NUM LOCK and SCROLL LOCK keys is ignored.
										 byte[] lpwTransKey, // [out] Pointer to the buffer that receives the translated character or characters. 
										 int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 

		[DllImport("user32.dll")]
		public static extern int GetKeyboardState (byte[] pbKeyState);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow ();

		[DllImport("user32.dll")]
		public static extern bool BringWindowToTop (IntPtr hWnd);


		[DllImport("user32.dll")]
		public static extern ulong GetWindowLongA (IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int EnumWindows (EnumWindowsCallback lpEnumFunc, int lParam);

		[DllImport("user32.dll")]
		public static extern bool UnhookWinEvent (IntPtr hWinEventHook);
		[DllImport("user32.dll")]
		public static extern IntPtr SetWinEventHook (uint eventMin,
			uint eventMax, IntPtr hmodWinEventProc,
			WinEventDelegate lpfnWinEventProc, uint idProcess,
			uint idThread, uint dwFlags);
		[DllImport("user32.dll")]
		public static extern int GetWindowText (IntPtr hWnd,
			StringBuilder lpString, int nMaxCount);
		#endregion

		#region DWM api
		[DllImport("dwmapi.dll")]
		public static extern int DwmRegisterThumbnail (IntPtr dest, IntPtr src, out IntPtr thumb);

		[DllImport("dwmapi.dll")]
		public static extern int DwmUnregisterThumbnail (IntPtr thumb);

		[DllImport("dwmapi.dll")]
		public static extern int DwmQueryThumbnailSourceSize (IntPtr thumb, out PSIZE size);

		[DllImport("dwmapi.dll")]
		public static extern int DwmUpdateThumbnailProperties (IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);
		#endregion

		#region Delegates
		public delegate bool EnumWindowsCallback (IntPtr hwnd, int lParam);
		
		public delegate void WinEventDelegate (IntPtr hWinEventHook,
		 uint eventType, IntPtr hwnd, int idObject,
		 int idChild, uint dwEventThread, uint dwmsEventTime);

		public delegate int HookProc (int nCode, Int32 wParam, IntPtr lParam);
		#endregion
	}

}
