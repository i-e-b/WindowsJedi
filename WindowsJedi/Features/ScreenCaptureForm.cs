namespace WindowsJedi.Features
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Windows.Forms;
    using WindowsJedi.Algorithms;
    using WindowsJedi.Components;
    using WindowsJedi.WinApis;

    public class ScreenCaptureForm : OverlayForm
    {
        GifWriter _outputFile;
        bool _disposed;
        readonly Timer _recordingTimer;
        const int FPS_20 = 50;//ms

        public ScreenCaptureForm()
        {
            _disposed = false;
            _recordingTimer = new Timer { Enabled = false, Interval = FPS_20 };
            _recordingTimer.Tick += _recordingTimer_Tick;

            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            SetBounds(20, 200, 320, 240);
            UpdateOverlay();
        }

        void _recordingTimer_Tick(object sender, EventArgs e)
        {
            if (_outputFile != null) SnapFrame();
        }


        /// <summary>
        /// Save a frame (handles scaling, but does not control checks)
        /// </summary>
        void SnapFrame()
        {
            using (var win = new Window(this))
            {
                var screen = win.PrimaryScreen();
                var s_scale = screen.GetEffectiveScale();
                var screenRel = win.ScreenRelativeRectangle;
                var offset = 11 * s_scale;
                var left = (screenRel.X * s_scale) + screen.Bounds.X;
                var top = (screenRel.Y * s_scale) + screen.Bounds.Y;
                _outputFile.WriteScreenFrame(new Point((int)(left + offset), (int)(top + offset)));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_disposed) return;
            _disposed = true;
            if (_outputFile != null)
            {
                _outputFile.Dispose();
            }
            base.OnClosed(e);
        }

        void ChooseFile()
        {
            var dlog = new SaveFileDialog();
            dlog.OverwritePrompt = true;
            dlog.DefaultExt = "gif";
            dlog.AddExtension = true;
            var result = dlog.ShowDialog();
            switch (result) {
                case DialogResult.OK:
                case DialogResult.Yes:
                    {
                        if (_outputFile != null)
                        {
                            _outputFile.Dispose();
                        }
                        _outputFile = new GifWriter(dlog.FileName, new Size(Width - 21, Height - 31), true);
                        UpdateOverlay();
                        break;
                    }
                default:
                    return;
            }
        }

        void AddFrame()
        {
            if (!_recordingTimer.Enabled) SnapFrame();
        }

        void ToggleRecording()
        {
            _recordingTimer.Enabled = !_recordingTimer.Enabled;
            UpdateOverlay();
        }

        protected override void OnActivated(EventArgs e)
        {
            UpdateOverlay();
            base.OnActivated(e);
        }

        void UpdateOverlay()
        {
            TopMost = true;
            TopLevel = true;
            BringToFront();
            using (var bitmap = DrawInner())
            {
                SetBitmap(bitmap);
            }
        }

        private double scale = 1.0;

        int s(float i) { return (int) (i * scale); }

        Bitmap DrawInner()
        {
            using (var win = new Window(this))
            {
                var screen = win.PrimaryScreen();
                scale = screen.GetEffectiveScale();
            }
            
            var b = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(b))
            {
                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                //Solid edge bars
                g.FillRectangle(Brushes.Beige, 0, 0, s(10), Height);
                g.FillRectangle(Brushes.Beige, Width - s(10), 0, Width, Height);
                g.FillRectangle(Brushes.Beige, 0, 0, Width, s(10));
                g.FillRectangle(Brushes.Beige, 0, Height - s(20), Width, Height);
                
                // outline of solid edge bars
                g.DrawRectangle(Pens.Black, 0,0,Width-1,Height-1);
                g.DrawRectangle(Pens.Black, s(10),s(10),Width-s(20),Height-s(30));

                // resize chevrons
                if (_outputFile == null)
                {
                    g.DrawLine(Pens.Black, Width, Height - s(18), Width - s(18), Height);
                    g.DrawLine(Pens.Black, Width, Height - s(14), Width - s(14), Height);
                    g.DrawLine(Pens.Black, Width, Height - s(10), Width - s(10), Height);
                    g.DrawLine(Pens.Black, Width, Height - s(6), Width - s(6), Height);
                }

                // close cross
                g.DrawLine(Pens.Black, s(2), s(2), s(9), s(9));
                g.DrawLine(Pens.Black, s(2), s(9), s(9), s(2));

                // Set file icon
                DrawPageIcon(g, s(10), Height - s(17));

                // record icon
                DrawRecordIcon(g, s(25), Height - s(17));

                // snap frame icon
                DrawSnapFrameIcon(g, s(40), Height - s(17));
            }
            return b;
        }

        void DrawSnapFrameIcon(Graphics g, int x, int y)
        {
            var p = Pens.Black;
            g.DrawRectangle(p, x, y, s(7), s(7));

            var s11 = s(11);
            var s9 = s(9);

            g.DrawLine(p, x+s11, y, x+s11, y+s11);
            g.DrawLine(p, x, y + s11, x + s11, y + s11);
            g.DrawLine(p, x + s9, y, x + s9, y + s9);
            g.DrawLine(p, x, y + s9, x + s9, y + s9);

            g.FillRectangle(Brushes.Black, x+s(2.5f), y, s(2), s(7));
            g.FillRectangle(Brushes.Black, x, y+s(2.5f), s(7), s(2));
        }

        void DrawRecordIcon(Graphics g, int x, int y)
        {
            g.DrawRectangle(Pens.Black, x, y, s(11), s(11));
            if (_recordingTimer.Enabled) g.FillRectangle(Brushes.Black, x+s(2), y+s(2), s(7), s(7));
            else g.FillEllipse(Brushes.Red, x+s(2), y+s(2), s(7), s(7));
        }

        void DrawPageIcon(Graphics g, int x, int y) {
            var p = Pens.Black;
            g.DrawLine(p, x    , y     , x + s(6), y);
            g.DrawLine(p, x + s(9), y + s(3) , x + s(9), y + s(11));
            g.DrawLine(p, x + s(9), y + s(11), x    , y + s(11));
            g.DrawLine(p, x    , y + s(11), x    , y);

            g.DrawLine(p, x + s(6), y  , x + s(6), y + s(3));
            g.DrawLine(p, x + s(6), y  , x + s(9), y + s(3));
            g.DrawLine(p, x + s(6), y+s(3), x + s(9), y + s(3));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateOverlay();
        }
        protected override void OnResize(EventArgs e)
        {
            UpdateOverlay();
        }

        static bool InRect(Point p, int l, int t, int r, int b)
        {
            return p.X >= l && p.X <= r && p.Y >= t && p.Y <= b;
        }

        protected override void WndProc(ref Message m)
        {
            if (_disposed) return;
            switch (m.Msg)
            {
                case Win32.WM_NCHITTEST: // screen-space coords
                    {
                        var c = PointToClient(GetPoint(m.LParam));
                        var sizeEnabled = _outputFile == null; // disable resize once we start a file

                        if (sizeEnabled && c.X > Width - s(10) && c.Y > Height - s(10)) m.Result = Win32.HTBOTTOMRIGHT;
                        else if (
                            InRect(c, 0, 0, s(10), s(10)) ||
                            InRect(c, s(10), Height - s(17), s(22), Height) ||
                            InRect(c, s(25), Height - s(17), s(37), Height) ||
                            InRect(c, s(40), Height - s(17), s(52), Height)
                            ) m.Result = Win32.HTCLIENT; // If you don't return HTCLIENT, you won't get the button events
                        else m.Result = Win32.HTCAPTION;
                        return;
                    }
                case Win32.WM_LBUTTONUP: // client-space coords
                    {
                        var c = GetPoint(m.LParam);
                        if (InRect(c,0,0,s(10),s(10))) { 
                            Close();
                            return;
                        }
                        
                        if (InRect(c, s(10), Height - s(17), s(22), Height)) { ChooseFile(); }
                        else if (InRect(c, s(25), Height - s(17), s(37), Height)) { ToggleRecording(); }
                        else if (InRect(c, s(40), Height - s(17), s(52), Height)) { AddFrame(); }

                        base.WndProc(ref m);
                        return;
                    }
                case Win32.WM_DPICHANGED:
                    {
                        UpdateOverlay();
                        return;
                    }
                default:
                    base.WndProc(ref m);
                    return;
            }
        }

        static Point GetPoint(IntPtr packed)
        {
            int x = (short)(packed.ToInt32() & 0x0000FFFF);
            int y = (short)((packed.ToInt32() & 0xFFFF0000) >> 16);
            return new Point(x, y);
        }
    }
}
