namespace WindowsJedi.Components
{
    using System;
    using WindowsJedi.WinApis;

    class WindowArranger : IDisposable
    {
        public void Dispose()
        {
        }

        /// <summary>
        /// Move the active window to fill the top-left quarter of its current screen.
        /// </summary>
        public void SetTopLeft()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var width = screenRect.Width / 2;
                var height = screenRect.Height / 2;
                w.SetBounds(screenRect.Left, screenRect.Top, width, height);
            }
        }

        public void SetTop()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var height = screenRect.Height / 2;
                w.SetBounds(screenRect.Left, screenRect.Top, screenRect.Width, height);
            }
        }

        public void SetTopRight()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var width = screenRect.Width / 2;
                var height = screenRect.Height / 2;
                w.SetBounds(screenRect.Left + width, screenRect.Top, width, height);
            }
        }

        public void SetLeft()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var width = screenRect.Width / 2;
                w.SetBounds(screenRect.Left, screenRect.Top, width, screenRect.Height);
            }
        }

        public void SetCentre()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var width = screenRect.Width / 3;
                var height = screenRect.Height / 3;
                var qtrW = width / 2;
                var qtrH = height / 2;
                w.SetBounds(screenRect.Left + qtrW, screenRect.Top + qtrH, width*2, height*2);
            }
        }

        public void SetRight()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var width = screenRect.Width / 2;
                w.SetBounds(screenRect.Left + width, screenRect.Top, width, screenRect.Height);
            }
        }

        public void SetBottomLeft()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var width = screenRect.Width / 2;
                var height = screenRect.Height / 2;
                w.SetBounds(screenRect.Left, screenRect.Top + height, width, height);
            }
        }

        public void SetBottom()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var height = screenRect.Height / 2;
                w.SetBounds(screenRect.Left, screenRect.Top + height, screenRect.Width, height);
            }
        }

        public void SetBottomRight()
        {
            using (var w = Window.ForegroundWindow())
            {
                var screenRect = w.ScreenRect();
                var width = screenRect.Width / 2;
                var height = screenRect.Height / 2;
                w.SetBounds(screenRect.Left + width, screenRect.Top + height, width, height);
            }
        }
    }
}