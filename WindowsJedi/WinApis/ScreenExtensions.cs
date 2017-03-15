using System;
using System.Windows.Forms;

namespace WindowsJedi.WinApis
{
    public static class ScreenExtensions
    {
        /// <summary>
        /// Returns the scaling of the given screen. Defaults to 96dpi in the case of an error
        /// </summary>
        public static void GetDpi(this Screen screen, out uint dpiX, out uint dpiY)
        {
            var point = new Win32.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var hmonitor = Win32.MonitorFromPoint(point, Win32.MONITOR_DEFAULTTONEAREST);

            var result = Win32.GetDpiForMonitor(hmonitor, Win32.DpiType.Raw, out dpiX, out dpiY).ToInt32();

            if (result != 0)
            {
                dpiX = 96;
                dpiY = 96;
            }
        }

        /// <summary>
        /// Returns an effective scaling factor for the given screen. Defaults to 1.0 in the case of an error.
        /// </summary>
        public static double GetEffectiveScale(this Screen screen)
        {
            uint x, y;
            screen.GetDpi(out x, out y);
            return Math.Max(x, y) / 96.0d;
        }
    }
}