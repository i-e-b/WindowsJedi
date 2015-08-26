namespace WindowsJedi.Components
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using WindowsJedi.WinApis;

    class WindowArranger : IDisposable
    {
        public void Dispose() { }


        public void MoveScreenLeft() { MoveScreen(Direction.Left); }
        public void MoveScreenUp() { MoveScreen(Direction.Up); }
        public void MoveScreenRight() { MoveScreen(Direction.Right); }
        public void MoveScreenDown() { MoveScreen(Direction.Down); }

        public void MoveScreen(Direction dd)
        {
            using (var w = Window.ForegroundWindow())
            {
                var currentScreenRect = w.ScreenRect();
                var hw = currentScreenRect.Width / 2;
                var hh = currentScreenRect.Height / 2;
                var nextPoint = currentScreenRect.Location;
                switch (dd)
                {
                    case Direction.Left:
                        nextPoint.Offset(-1, hh);
                        break;
                    case Direction.Up:
                        nextPoint.Offset(hw, -1);
                        break;
                    case Direction.Right:
                        nextPoint.Offset(currentScreenRect.Width + 1, hh);
                        break;
                    case Direction.Down:
                        nextPoint.Offset(hw, currentScreenRect.Height + 1);
                        break;
                }

                var newScreen = Screen.FromPoint(nextPoint);

                var newOffset = RepositionPoint(w.NormalRectangle.Location, currentScreenRect.Location, newScreen.WorkingArea.Location);

                w.MoveTo(newOffset);
            }
        }

        static Point RepositionPoint(Point oldLocation, Point oldOrigin, Point newOrigin)
        {
            var dx = newOrigin.X - oldOrigin.X;
            var dy = newOrigin.Y - oldOrigin.Y;
            return new Point(oldLocation.X + dx, oldLocation.Y + dy);
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