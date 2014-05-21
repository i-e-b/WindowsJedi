using System;
using System.Runtime.InteropServices;
using System.Text;
using WindowsJedi.WinApis.Data;
// ReSharper disable InconsistentNaming

namespace WindowsJedi.WinApis {
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

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION {
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}
		#endregion
		#region Constants
		#region Window Manager events
		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_SYSKEYDOWN = 0x104;
		public const int WM_SYSKEYUP = 0x105;
		public const int WM_NCHITTEST = 0x0084;
        public const int WM_ACTIVATEAPP = 0x001C;
        public const int WM_GETICON = 0x7F;
        public const int WM_CLOSE = 0x0010;
		#endregion

        #region Hwnd positions
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        #endregion

        #region Window hit detection
        public static IntPtr HTCLIENT = new IntPtr(2);
        public static IntPtr HTNOWHERE = new IntPtr(0);
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
        public const ulong WS_DLGFRAME = 0x00400000L;

		public const int WS_EX_TOPMOST = 0x00000008;
		public const int WS_EX_APPWINDOW = 0x00040000;
		public const int WS_EX_LAYERED = 0x00080000;
		public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_NOACTIVATE = 0x08000000;
		#endregion

        #region Window positioning flags
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
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
        public const uint WINEVENT_SKIPOWNPROCESS = 2;

        public const uint EVENT_SYSTEM_FOREGROUND = 3;
        public const uint EVENT_OBJECT_CREATE = 0x8000;
        public const uint EVENT_OBJECT_DESTROY = 0x8001;

        public const uint EVENT_MIN = 0x00000001;
        public const uint EVENT_MAX = 0x7FFFFFFF;

        public const uint OBJID_WINDOW = 0x00000000;
        #endregion

        #region Window redraw flags
        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_INTERNALPAINT = 0x0002;
        public const int RDW_ERASE = 0x0004;

        public const int RDW_VALIDATE = 0x0008;
        public const int RDW_NOINTERNALPAINT = 0x0010;
        public const int RDW_NOERASE = 0x0020;

        public const int RDW_NOCHILDREN = 0x0040;
        public const int RDW_ALLCHILDREN = 0x0080;

        public const int RDW_UPDATENOW = 0x0100;
        public const int RDW_ERASENOW = 0x0200;

        public const int RDW_FRAME = 0x0400;
        public const int RDW_NOFRAME = 0x0800;
        #endregion

        #region DWM flags
        public const int GWL_EXSTYLE = -20;
		public const int GWL_STYLE = -16;
		public const int DWM_TNP_VISIBLE = 0x8;
		public const int DWM_TNP_OPACITY = 0x4;
		public const int DWM_TNP_RECTDESTINATION = 0x1;
		#endregion

        #region Icon styles
        public const int GCL_HICONSM = -34;
        public const int GCL_HICON = -14;
        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;
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

        /// <summary>
        /// Returns next window in requested z-order, or IntPtr.Zero if at the end.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr targetWindow, WindowStack direction);

        /// <summary>
        /// Returns previous active window
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr targetWindow);


        /// <summary>
        /// Returns true if handle is currently valid. It may not be your original window, use caution!
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr targetWindow);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Set layered window. You must set the window layered first, using `SetWindowLong`
        /// </summary>
        /// <param name="windowHandle">Window handle</param>
        /// <param name="crKey">color key (0x00RRGGBB)</param>
        /// <param name="bAlpha">[0..255] alpha, 255 is opaque, 0 is transparent</param>
        /// <param name="dwFlags">Use alpha or color key?</param>
        /// <example><![CDATA[
        /// // Set WS_EX_LAYERED on this window 
        /// SetWindowLong(hwnd, 
        ///               GWL_EXSTYLE, 
        ///                GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_LAYERED);
        ///
        /// // Make this window 70% alpha
        /// SetLayeredWindowAttributes(hwnd, 0, (255 * 70) / 100, LWA_ALPHA);
        /// ]]></example>
        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr windowHandle, UInt32 crKey, byte bAlpha, LayeredWindowFlags dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLongPtr(IntPtr windowHandle, int nIndex, IntPtr newSettings);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLongPtr(IntPtr windowHandle, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr windowHandle, IntPtr insertAfterWindowHandle, int x, int y, int cx, int cy, uint flags);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr windowHandle, /*[In] ref Rect*/ IntPtr updateRect, IntPtr updateRegion, uint flags);

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

        [DllImport("user32.dll")]
        public static extern bool RegisterShellHookWindow(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
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
