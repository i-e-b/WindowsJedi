namespace WindowsJedi.Features
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Windows.Forms;
    using WindowsJedi.Components;
    using WindowsJedi.WinApis;

    public class ScreenCaptureForm : OverlayForm
    {
        //readonly Rectangle _rect;

        public ScreenCaptureForm()
        {
            // TODO: show a file save dialog to pick the target
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            SetBounds(20, 200, 320, 240);
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
                g.DrawLine(Pens.Black, Width, Height - 18, Width - 18, Height);
                g.DrawLine(Pens.Black, Width, Height - 14, Width - 14, Height);
                g.DrawLine(Pens.Black, Width, Height - 10, Width - 10, Height);
                g.DrawLine(Pens.Black, Width, Height - 6, Width - 6, Height);

                // close cross
                g.DrawLine(Pens.Black, 0, 0, 10, 10);
                g.DrawLine(Pens.Black, 0, 10, 10, 0);

                // TODO: buttons to toggle recording and to snap one frame
            }
            return b;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateOverlay();
        }
        protected override void OnResize(EventArgs e)
        {
            UpdateOverlay();
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_NCHITTEST: // screen-space coords
                    {
                        var c = PointToClient(GetPoint(m.LParam));
                        if (c.X > Width - 10 && c.Y > Height - 10) m.Result = Win32.HTBOTTOMRIGHT;
                        else if (c.X < 10 && c.Y < 10) m.Result = Win32.HTCLIENT;
                        else m.Result = Win32.HTCAPTION;
                        return;
                    }
                case Win32.WM_LBUTTONUP: // client-space coords
                    {
                        var c = GetPoint(m.LParam);
                        if (c.X < 10 && c.Y < 10) Close();
                        return;
                    }
                /*case Win32.WM_MOUSEMOVE:
                    {
                        var p = GetPoint(m.LParam);
                        var c = PointToClient(GetPoint(m.LParam));

                        if (c.X > Width - 10 && c.Y > Height - 10)
                        {

                        }
                        else
                        {
                            SetDesktopLocation(p.X, p.Y);
                        }
                        return;
                    }*/
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
