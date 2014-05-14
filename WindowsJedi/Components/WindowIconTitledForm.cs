namespace WindowsJedi.Components
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Threading;
    using WindowsJedi.WinApis;

    /// <summary>
    /// A non-interactive overlay that displays:
    /// <para>A window's icon</para>
    /// <para>A title in large text</para>
    /// <para>A subtitle in smaller text</para>
    /// </summary>
    public class WindowIconTitledForm : NonHitOverlayForm
    {
        readonly string _title;
        readonly string _subtitle;
        readonly Rectangle _rect;
        Icon _icon;

        /// <summary>
        /// Create a new titled overlay
        /// </summary>
        /// <param name="targetWindow">Window whose icon will be displayed</param>
        /// <param name="title">Large font title</param>
        /// <param name="subtitle">Small font subtitle</param>
        /// <param name="targetRectangle">target size</param>
        public WindowIconTitledForm(Window targetWindow, string title, string subtitle, Rectangle targetRectangle)
        {
            _title = title;
            _subtitle = subtitle;
            _rect = targetRectangle;

            new Thread(() =>
            {
                _icon = targetWindow.GetAppIcon();
                try { Invoke(new UpdateDelegate(UpdateOverlay)); }
                catch { Ignore(); } // If the icon is found quickly, the underlying window won't be ready

            }) { IsBackground = true }.Start();
        }

        protected override void OnActivated(EventArgs e)
        {
            UpdateOverlay();
            base.OnActivated(e);
        }

        static void Ignore() { } 

        public delegate void UpdateDelegate();

        void UpdateOverlay()
        {
            TopMost = true;
            TopLevel = true;
            BringToFront();
            SetBounds(_rect.X, _rect.Y, _rect.Width, _rect.Height);
            using (var bitmap = DrawInner())
            {
                SetBitmap(bitmap);
            }
        }

        Bitmap DrawInner()
        {
            var b = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(b))
            {
                g.Clear(Color.FromArgb(128,0,0,0));
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias; // clear type can look weird

                var accelFont = new Font("Arial", 18.0f, FontStyle.Bold);
                var titleFont = new Font("Arial", 10.0f, FontStyle.Regular);
                var wb = new SolidBrush(Color.White);
                var rect = Bounds;

                var left = 5;
                if (_icon != null)
                {
                    g.DrawIcon(_icon, 0, 0);
                    left += _icon.Width;
                }

                var midline = (rect.Height / 2.0f) - 6;
                var width = Math.Max(rect.Width, 20);

                g.DrawString(_title, accelFont, wb, new RectangleF(left, midline - 8, width, 25));
                left += 20;
                g.DrawString(_subtitle, titleFont, wb, new RectangleF(left, midline, width, 20));

            }
            return b;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            UpdateOverlay();
        }

        protected override void Dispose(bool disposing)
        {
            if (_icon != null)
            {
                _icon.Dispose();
                _icon = null;
            }
            base.Dispose(disposing);
        }

    }
}