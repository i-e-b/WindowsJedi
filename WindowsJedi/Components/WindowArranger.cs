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

        /// <summary>
        /// Move a given window to a different screen by direction.
        /// Does not wrap. Returns false if there is no screen available in that direction,
        /// returns true if the window was moved.
        /// </summary>
        public static bool MoveWindowRelativeScreenDirection(Window toMove, Window relative, Direction dd)
        {
            var targetScreenRect = relative.ScreenRect();
            var hw = targetScreenRect.Width / 2;
            var hh = targetScreenRect.Height / 2;
            var nextPoint = targetScreenRect.Location;
            switch (dd)
            {
                case Direction.Left:
                    nextPoint.Offset(-1, hh);
                    break;
                case Direction.Up:
                    nextPoint.Offset(hw, -1);
                    break;
                case Direction.Right:
                    nextPoint.Offset(targetScreenRect.Width + 1, hh);
                    break;
                case Direction.Down:
                    nextPoint.Offset(hw, targetScreenRect.Height + 1);
                    break;
            }

            var newScreen = Screen.FromPoint(nextPoint);
            if (newScreen.Equals(relative.PrimaryScreen()))
            {
                return false; // no screen in this direction
            }

            var currentScreenRect = toMove.ScreenRect();
            var newOffset = RepositionPoint(toMove.NormalRectangle.Location, currentScreenRect.Location, newScreen.WorkingArea.Location);

            toMove.MoveTo(newOffset);
            return true; // yes, we moved the screen
        }

        public void MoveScreen(Direction dd)
        {
            using (var w = Window.ForegroundWindow())
            {
                MoveWindowRelativeScreenDirection(w, w, dd);
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