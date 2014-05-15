namespace WindowsJedi.WinApis {
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    /// Wrapper around Win32 desktops api
    /// </summary>
	public class Desktop : IDisposable, ICloneable {
        public const long AccessRights = DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK | DESKTOP_CREATEWINDOW | DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP | DESKTOP_CREATEMENU | DESKTOP_HOOKCONTROL | DESKTOP_READOBJECTS;
        public const long DESKTOP_CREATEMENU = 0x0004L;
        public const long DESKTOP_CREATEWINDOW = 0x0002L;
        public const long DESKTOP_ENUMERATE = 0x0040L;
        public const long DESKTOP_HOOKCONTROL = 0x0008L;
        public const long DESKTOP_JOURNALPLAYBACK = 0x0020L;
        public const long DESKTOP_JOURNALRECORD = 0x0010L;
        public const long DESKTOP_READOBJECTS = 0x0001L;
        public const long DESKTOP_SWITCHDESKTOP = 0x0100L;
        public const long DESKTOP_WRITEOBJECTS = 0x0080L;
        /// <summary>
        /// Size of buffer used when retrieving window names.
        /// </summary>
        public const int MaxWindowNameLength = 100;
        public const int NORMAL_PRIORITY_CLASS = 0x00000020;
        public const int STARTF_USEPOSITION = 0x00000004;
        public const int STARTF_USESHOWWINDOW = 0x00000001;
        public const int STARTF_USESTDHANDLES = 0x00000100;
        public const short SW_HIDE = 0;
        public const short SW_NORMAL = 1;
        public const int UOI_NAME = 2;
        /// <summary>
        /// Opens the default desktop.
        /// </summary>
        public static readonly Desktop Default = OpenDefaultDesktop();

        /// <summary>
        /// Opens the desktop the user if viewing.
        /// </summary>
        public static readonly Desktop Input = OpenInputDesktop();
        private readonly List<IntPtr> _windows;
        private static StringCollection mSc;
        private IntPtr _desktop;
        private bool _disposed;

        /// <summary>
        /// Creates a new Desktop object.
        /// </summary>
        public Desktop()
        {
            // init variables.
            _desktop = IntPtr.Zero;
            DesktopName = String.Empty;
            _windows = new List<IntPtr>();
            _disposed = false;
        }

        /// <summary>
        ///  constructor is private to prevent invalid handles being passed to it.
        /// </summary>
        private Desktop(IntPtr desktop)
        {
            // init variables.
            _desktop = desktop;
            DesktopName = GetDesktopName(desktop);
            _windows = new List<IntPtr>();
            _disposed = false;
        }

        ~Desktop()
        {
            Dispose(false);
        }

        private delegate bool EnumDesktopProc (string lpszDesktop, IntPtr lParam);
        private delegate bool EnumDesktopWindowsProc (IntPtr desktopHandle, IntPtr lParam);

        /// <summary>
        /// Gets a handle to the desktop, IntPtr.Zero if no desktop open.
        /// </summary>
        public IntPtr DesktopHandle {
            get {
                return _desktop;
            }
        }
        /// <summary>
        /// Gets the name of the desktop, returns null if no desktop is open.
        /// </summary>
        public string DesktopName { get; private set; }
        /// <summary>
        /// Gets if a desktop is open.
        /// </summary>
        public bool IsOpen {
            get {
                return (_desktop != IntPtr.Zero);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION {
            // ReSharper disable FieldCanBeMadeReadOnly.Local, MemberCanBePrivate.Local
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
            // ReSharper restore FieldCanBeMadeReadOnly.Local, MemberCanBePrivate.Local
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STARTUPINFO {
            // ReSharper disable FieldCanBeMadeReadOnly.Local, MemberCanBePrivate.Local
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
            // ReSharper restore FieldCanBeMadeReadOnly.Local, MemberCanBePrivate.Local
        }

        /// <summary>
        /// Dispose Object.
        /// </summary>
        public void Dispose()
        {
            // dispose
            Dispose(true);

            // suppress finalisation
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose Object.
        /// </summary>
        /// <param name="disposing">True to dispose managed resources.</param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // dispose of managed resources,
                // close handles
                Close();
            }

            _disposed = true;
        }

        [DllImport("kernel32.dll")]
		public static extern int GetThreadId (IntPtr thread);

		[DllImport("kernel32.dll")]
        public static extern int GetProcessId(IntPtr process);

		//
		// Imported winAPI functions.
		//
		[DllImport("user32.dll")]
		private static extern IntPtr CreateDesktop (string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags, long dwDesiredAccess, IntPtr lpsa);

		[DllImport("user32.dll")]
		private static extern bool CloseDesktop (IntPtr hDesktop);

		[DllImport("user32.dll")]
		private static extern IntPtr OpenDesktop (string lpszDesktop, int dwFlags, bool fInherit, long dwDesiredAccess);

		[DllImport("user32.dll")]
		private static extern IntPtr OpenInputDesktop (int dwFlags, bool fInherit, long dwDesiredAccess);

		[DllImport("user32.dll")]
		private static extern bool SwitchDesktop (IntPtr hDesktop);

		[DllImport("user32.dll")]
		private static extern bool EnumDesktops (IntPtr hwinsta, EnumDesktopProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern IntPtr GetProcessWindowStation ();

		[DllImport("user32.dll")]
		private static extern bool EnumDesktopWindows (IntPtr hDesktop, EnumDesktopWindowsProc lpfn, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool SetThreadDesktop (IntPtr hDesktop);

		[DllImport("user32.dll")]
		private static extern IntPtr GetThreadDesktop (int dwThreadId);

		[DllImport("user32.dll")]
		private static extern bool GetUserObjectInformation (IntPtr hObj, int nIndex, IntPtr pvInfo, int nLength, ref int lpnLengthNeeded);

		[DllImport("kernel32.dll")]
		private static extern bool CreateProcess (
			string lpApplicationName,
			string lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			bool bInheritHandles,
			int dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			ref STARTUPINFO lpStartupInfo,
			ref PROCESS_INFORMATION lpProcessInformation
			);

		[DllImport("user32.dll")]
		private static extern int GetWindowText (IntPtr hWnd, IntPtr lpString, int nMaxCount);

        /// <summary>
		/// Creates a new desktop.  If a handle is open, it will be closed.
		/// </summary>
		/// <param name="name">The name of the new desktop.  Must be unique, and is case sensitive.</param>
		/// <returns>True if desktop was successfully created, otherwise false.</returns>
		public bool Create (string name) {
			// make sure object isnt disposed.

		    // close the open desktop.
			if (_desktop != IntPtr.Zero) {
				// attempt to close the desktop.
				if (!Close()) return false;
			}

			// make sure desktop doesnt already exist.
			if (Exists(name)) {
				// it exists, so open it.
				return Open(name);
			}

			// attempt to create desktop.
			_desktop = CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, AccessRights, IntPtr.Zero);

			DesktopName = name;

			// something went wrong.
			if (_desktop == IntPtr.Zero) return false;

			return true;
		}

		/// <summary>
		/// Closes the handle to a desktop.
		/// </summary>
		/// <returns>True if an open handle was successfully closed.</returns>
		public bool Close () {
			// make sure object isnt disposed.

		    // check there is a desktop open.
			if (_desktop != IntPtr.Zero) {
				// close the desktop.
				var result = CloseDesktop(_desktop);

				if (result) {
					_desktop = IntPtr.Zero;

					DesktopName = String.Empty;
				}

				return result;
			}

			// no desktop was open, so desktop is closed.
			return true;
		}

		/// <summary>
		/// Opens a desktop.
		/// </summary>
		/// <param name="name">The name of the desktop to open.</param>
		/// <returns>True if the desktop was successfully opened.</returns>
		public bool Open (string name) {
			// make sure object isnt disposed.

		    // close the open desktop.
			if (_desktop != IntPtr.Zero) {
				// attempt to close the desktop.
				if (!Close()) return false;
			}

			// open the desktop.
			_desktop = OpenDesktop(name, 0, true, AccessRights);

			// something went wrong.
			if (_desktop == IntPtr.Zero) return false;

			DesktopName = name;

			return true;
		}

		/// <summary>
		/// Opens the current input desktop.
		/// </summary>
		/// <returns>True if the desktop was succesfully opened.</returns>
		public bool OpenInput () {
			// make sure object isnt disposed.

		    // close the open desktop.
			if (_desktop != IntPtr.Zero) {
				// attempt to close the desktop.
				if (!Close()) return false;
			}

			// open the desktop.
			_desktop = OpenInputDesktop(0, true, AccessRights);

			// something went wrong.
			if (_desktop == IntPtr.Zero) return false;

			// get the desktop name.
			DesktopName = GetDesktopName(_desktop);

			return true;
		}

		/// <summary>
		/// Switches input to the currently opened desktop.
		/// </summary>
		/// <returns>True if desktops were successfully switched.</returns>
		public bool Show () {
			// make sure object isnt disposed.

		    // make sure there is a desktop to open.
			if (_desktop == IntPtr.Zero) return false;

			// attempt to switch desktops.
			var result = SwitchDesktop(_desktop);

			return result;
		}

		/// <summary>
		/// Enumerates the windows on a desktop.
		/// </summary>
		/// <returns>A window colleciton if successful, otherwise null.</returns>
        public List<Window> GetWindows()
        {
			// make sure object isnt disposed.

		    // make sure a desktop is open.
			if (!IsOpen) return null;

			// init the arraylist.
			_windows.Clear();

		    // get windows.
			var result = EnumDesktopWindows(_desktop, DesktopWindowsProc, IntPtr.Zero);

			// check for error.
			if (!result) return null;

			// get window names.
			var windows = new List<Window>();

			var ptr = Marshal.AllocHGlobal(MaxWindowNameLength);

			foreach (var wnd in _windows) {
				GetWindowText(wnd, ptr, MaxWindowNameLength);
				windows.Add(new Window(wnd, Marshal.PtrToStringAnsi(ptr)));
			}

			Marshal.FreeHGlobal(ptr);

			return windows;
		}

		private bool DesktopWindowsProc (IntPtr wndHandle, IntPtr lParam) {
			// add window handle to colleciton.
			_windows.Add(wndHandle);

			return true;
		}

		/// <summary>
		/// Creates a new process in a desktop.
		/// </summary>
		/// <param name="path">Path to application.</param>
		/// <returns>The process object for the newly created process.</returns>
		public Process CreateProcess (string path) {
			// make sure object isnt disposed.

		    // make sure a desktop is open.
			if (!IsOpen) return null;

			// set startup parameters.
			var si = new STARTUPINFO();
			si.cb = Marshal.SizeOf(si);
			si.lpDesktop = DesktopName;

			var pi = new PROCESS_INFORMATION();

			// start the process.
			var result = CreateProcess(null, path, IntPtr.Zero, IntPtr.Zero, true, NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref si, ref pi);

			// error?
			if (!result) return null;

			// Get the process.
			return Process.GetProcessById(pi.dwProcessId);
		}

		/// <summary>
		/// Prepares a desktop for use.  For use only on newly created desktops, call straight after CreateDesktop.
		/// </summary>
		public void Prepare () {
			// make sure object isnt disposed.

		    // make sure a desktop is open.
			if (IsOpen) {
				// load explorer.
				CreateProcess("explorer.exe");
			}
		}

        /// <summary>
		/// Enumerates all of the desktops.
		/// </summary>
		public static string[] GetDesktops () {
			// attempt to enum desktops.
			var windowStation = GetProcessWindowStation();

			// check we got a valid handle.
			if (windowStation == IntPtr.Zero) return new string[0];

			string[] desktops;

			// lock the object. thread safety and all.
			lock (mSc = new StringCollection()) {
				var result = EnumDesktops(windowStation, DesktopProc, IntPtr.Zero);

				// something went wrong.
				if (!result) return new string[0];

				//	// turn the collection into an array.
				desktops = new string[mSc.Count];
				for (var i = 0; i < desktops.Length; i++) desktops[i] = mSc[i];
			}

			return desktops;
		}

		private static bool DesktopProc (string lpszDesktop, IntPtr lParam) {
			// add the desktop to the collection.
			mSc.Add(lpszDesktop);

			return true;
		}

		/// <summary>
		/// Switches to the specified desktop.
		/// </summary>
		/// <param name="name">Name of desktop to switch input to.</param>
		/// <returns>True if desktops were successfully switched.</returns>
		public static bool Show (string name) {
			// attmempt to open desktop.
			bool result;

			using (var d = new Desktop()) {
				result = d.Open(name);

				// something went wrong.
				if (!result) return false;

				// attempt to switch desktops.
				result = d.Show();
			}

			return result;
		}

		/// <summary>
		/// Gets the desktop of the calling thread.
		/// </summary>
		/// <returns>Returns a Desktop object for the valling thread.</returns>
		public static Desktop GetCurrent () {
			// get the desktop.
			return new Desktop(GetThreadDesktop(Thread.CurrentThread.ManagedThreadId));
		}

		/// <summary>
		/// Sets the desktop of the calling thread.
		/// NOTE: Function will fail if thread has hooks or windows in the current desktop.
		/// </summary>
		/// <param name="desktop">Desktop to put the thread in.</param>
		/// <returns>True if the threads desktop was successfully changed.</returns>
		public static bool SetCurrent (Desktop desktop) {
			// set threads desktop.
			if (!desktop.IsOpen) return false;

			return SetThreadDesktop(desktop.DesktopHandle);
		}

		/// <summary>
		/// Opens a desktop.
		/// </summary>
		/// <param name="name">The name of the desktop to open.</param>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop OpenDesktop (string name) {
			// open the desktop.
			var desktop = new Desktop();
			var result = desktop.Open(name);

			// somethng went wrong.
			if (!result) return null;

			return desktop;
		}

		/// <summary>
		/// Opens the current input desktop.
		/// </summary>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop OpenInputDesktop () {
			// open the desktop.
			var desktop = new Desktop();
			var result = desktop.OpenInput();

			// somethng went wrong.
			if (!result) return null;

			return desktop;
		}

		/// <summary>
		/// Opens the default desktop.
		/// </summary>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop OpenDefaultDesktop () {
			// opens the default desktop.
			return OpenDesktop("Default");
		}

		/// <summary>
		/// Creates a new desktop.
		/// </summary>
		/// <param name="name">The name of the desktop to create.  Names are case sensitive.</param>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop CreateDesktop (string name) {
			// open the desktop.
			var desktop = new Desktop();
			var result = desktop.Create(name);

			// something went wrong.
			if (!result) return null;

			return desktop;
		}

		/// <summary>
		/// Gets the name of a given desktop.
		/// </summary>
		/// <param name="desktop">Desktop object whos name is to be found.</param>
		/// <returns>If successful, the desktop name, otherwise, null.</returns>
		public static string GetDesktopName (Desktop desktop) {
			// get name.
			if (desktop.IsOpen) return null;

			return GetDesktopName(desktop.DesktopHandle);
		}

		/// <summary>
		/// Gets the name of a desktop from a desktop handle.
		/// </summary>
		/// <param name="desktopHandle"></param>
		/// <returns>If successful, the desktop name, otherwise, null.</returns>
		public static string GetDesktopName (IntPtr desktopHandle) {
			// check its not a null pointer.
			// null pointers wont work.
			if (desktopHandle == IntPtr.Zero) return null;

			// get the length of the name.
			var needed = 0;
		    GetUserObjectInformation(desktopHandle, UOI_NAME, IntPtr.Zero, 0, ref needed);

			// get the name.
			var ptr = Marshal.AllocHGlobal(needed);
			var result = GetUserObjectInformation(desktopHandle, UOI_NAME, ptr, needed, ref needed);
			var name = Marshal.PtrToStringAnsi(ptr);
			Marshal.FreeHGlobal(ptr);

			// something went wrong.
			if (!result) return null;

			return name;
		}

		/// <summary>
		/// Checks if the specified desktop exists (using a case sensitive search).
		/// </summary>
		/// <param name="name">The name of the desktop.</param>
		/// <returns>True if the desktop exists, otherwise false.</returns>
		public static bool Exists (string name) {
			return Exists(name, false);
		}

		/// <summary>
		/// Checks if the specified desktop exists.
		/// </summary>
		/// <param name="name">The name of the desktop.</param>
		/// <param name="caseInsensitive">If the search is case INsensitive.</param>
		/// <returns>True if the desktop exists, otherwise false.</returns>
		public static bool Exists (string name, bool caseInsensitive) {
			// enumerate desktops.
			var desktops = GetDesktops();

			// return true if desktop exists.
			foreach (var desktop in desktops) {
				if (caseInsensitive) {
					// case insensitive, compare all in lower case.
					if (desktop.ToLower() == name.ToLower()) return true;
				} else {
					if (desktop == name) return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Creates a new process on the specified desktop.
		/// </summary>
		/// <param name="path">Path to application.</param>
		/// <param name="desktop">Desktop name.</param>
		/// <returns>A Process object for the newly created process, otherwise, null.</returns>
		public static Process CreateProcess (string path, string desktop) {
			if (!Exists(desktop)) return null;

			// create the process.
			var d = OpenDesktop(desktop);
			return d.CreateProcess(path);
		}

		/// <summary>
		/// Gets an array of all the processes running on the Input desktop.
		/// </summary>
		/// <returns>An array of the processes.</returns>
		public static Process[] GetInputProcesses () {
			// get all processes.
			var processes = Process.GetProcesses();

		    // get the current desktop name.
			var currentDesktop = GetDesktopName(Input.DesktopHandle);

			// cycle through the processes.
		    var m_procs = processes.Where(process => 
                process.Threads.Cast<ProcessThread>().Any(pt => GetDesktopName(GetThreadDesktop(pt.Id)) == currentDesktop)
                ).ToList();

		    // put ArrayList into array.
			var procs = new Process[m_procs.Count];

			for (var i = 0; i < procs.Length; i++) procs[i] = m_procs[i];

			return procs;
		}

        /// <summary>
		/// Creates a new Desktop object with the same desktop open.
		/// </summary>
		/// <returns>Cloned desktop object.</returns>
		public object Clone () {
			// make sure object isnt disposed.

		    var desktop = new Desktop();

			// if a desktop is open, make the clone open it.
			if (IsOpen) desktop.Open(DesktopName);

			return desktop;
		}

        /// <summary>
		/// Gets the desktop name.
		/// </summary>
		/// <returns>The desktop name, or a blank string if no desktop open.</returns>
		public override string ToString () {
			// return the desktop name.
			return DesktopName;
		}
	}
}