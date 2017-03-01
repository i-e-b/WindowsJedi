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
            if (_outputFile != null)
            {
                _outputFile.WriteScreenFrame(new Point(Left + 11, Top + 11));
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
            if (_recordingTimer.Enabled) return;
            _outputFile.WriteScreenFrame(new Point(Left + 11, Top + 11));
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

        Bitmap DrawInner()
        {
            var b = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(b))
            {
                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                //Solid edge bars
                g.FillRectangle(Brushes.Beige, 0, 0, 10, Height);
                g.FillRectangle(Brushes.Beige, Width - 10, 0, Width, Height);
                g.FillRectangle(Brushes.Beige, 0, 0, Width, 10);
                g.FillRectangle(Brushes.Beige, 0, Height - 20, Width, Height);
                
                // outline of solid edge bars
                g.DrawRectangle(Pens.Black, 0,0,Width-1,Height-1);
                g.DrawRectangle(Pens.Black, 10,10,Width-20,Height-30);

                // resize chevrons
                if (_outputFile == null)
                {
                    g.DrawLine(Pens.Black, Width, Height - 18, Width - 18, Height);
                    g.DrawLine(Pens.Black, Width, Height - 14, Width - 14, Height);
                    g.DrawLine(Pens.Black, Width, Height - 10, Width - 10, Height);
                    g.DrawLine(Pens.Black, Width, Height - 6, Width - 6, Height);
                }

                // close cross
                g.DrawLine(Pens.Black, 2, 2, 9, 9);
                g.DrawLine(Pens.Black, 2, 9, 9, 2);

                // Set file icon
                DrawPageIcon(g, 10, Height - 17);

                // record icon
                DrawRecordIcon(g, 25, Height - 17);

                // snap frame icon
                DrawSnapFrameIcon(g, 40, Height - 17);
            }
            return b;
        }

        static void DrawSnapFrameIcon(Graphics g, int x, int y)
        {
            var p = Pens.Black;
            g.DrawRectangle(p, x, y, 7, 7);
            
            g.DrawLine(p, x+11, y, x+11, y+11);
            g.DrawLine(p, x, y + 11, x + 11, y + 11);
            g.DrawLine(p, x + 9, y, x + 9, y + 9);
            g.DrawLine(p, x, y + 9, x + 9, y + 9);

            g.FillRectangle(Brushes.Black, x+2.5f, y, 2, 7);
            g.FillRectangle(Brushes.Black, x, y+2.5f, 7, 2);
        }

        void DrawRecordIcon(Graphics g, int x, int y)
        {
            g.DrawRectangle(Pens.Black, x, y, 11, 11);
            if (_recordingTimer.Enabled) g.FillRectangle(Brushes.Black, x+2, y+2, 7, 7);
            else g.FillEllipse(Brushes.Red, x+2, y+2, 7, 7);
        }

        static void DrawPageIcon(Graphics g, int x, int y) {
            var p = Pens.Black;
            g.DrawLine(p, x    , y     , x + 6, y);
            g.DrawLine(p, x + 9, y + 3 , x + 9, y + 11);
            g.DrawLine(p, x + 9, y + 11, x    , y + 11);
            g.DrawLine(p, x    , y + 11, x    , y);

            g.DrawLine(p, x + 6, y  , x + 6, y + 3);
            g.DrawLine(p, x + 6, y  , x + 9, y + 3);
            g.DrawLine(p, x + 6, y+3, x + 9, y + 3);
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

                        if (sizeEnabled && c.X > Width - 10 && c.Y > Height - 10) m.Result = Win32.HTBOTTOMRIGHT;
                        else if (
                            InRect(c, 0, 0, 10, 10) ||
                            InRect(c, 10, Height - 17, 22, Height) ||
                            InRect(c, 25, Height - 17, 37, Height) ||
                            InRect(c, 40, Height - 17, 52, Height)
                            ) m.Result = Win32.HTCLIENT; // If you don't return HTCLIENT, you won't get the button events
                        else m.Result = Win32.HTCAPTION;
                        return;
                    }
                case Win32.WM_LBUTTONUP: // client-space coords
                    {
                        var c = GetPoint(m.LParam);
                        if (InRect(c,0,0,10,10)) { 
                            Close();
                            return;
                        }
                        
                        if (InRect(c, 10, Height - 17, 22, Height)) { ChooseFile(); }
                        else if (InRect(c, 25, Height - 17, 37, Height)) { ToggleRecording(); }
                        else if (InRect(c, 40, Height - 17, 52, Height)) { AddFrame(); }

                        base.WndProc(ref m);
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
