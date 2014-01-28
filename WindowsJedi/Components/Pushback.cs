using System;
using WindowsJedi.WinApis;

namespace WindowsJedi.Components
{
    /// <summary>
    /// Pushes the frontmost window to the back of the window stack
    /// </summary>
    public class Pushback : IDisposable
    {
        public void Dispose() { }

        public void PushBackFrontWindow()
        {
            var handle = Win32.GetForegroundWindow();
            if (handle == IntPtr.Zero) return;

            Win32.SetWindowPos(handle, Win32.HWND_BOTTOM, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
        }
    }
}