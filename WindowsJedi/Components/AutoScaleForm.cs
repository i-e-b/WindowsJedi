using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsJedi.WinApis;

namespace WindowsJedi.Components
{
    /// <summary>
    /// A WinForms window that automatically rescales controls and fonts
    /// </summary>
    public class AutoScaleForm:Form
    {
        public short Dpi = 96;
        protected short OldDpi = 96;
        protected float LastScale = 1.0f;

        public AutoScaleForm()
        {
            //AutoScaleMode = AutoScaleMode.Dpi;
            RescaleScreen();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_NCCREATE)
            {
                Win32.EnableNonClientDpiScaling(Handle);
            }
           /* if (m.Msg == Win32.WM_MOVING)
            {
                new Task(() => {
                    Invoke(new RescaleDelegate(RescaleScreen));
                }).Start();
                RescaleScreen();
            }*/
            if (m.Msg == Win32.WM_DPICHANGED)
            {
                /*new Task(() => {
                    Invoke(new RescaleDelegate(RescaleScreen));
                }).Start();*/
                RescaleScreen();
            }
            base.WndProc(ref m);
        }
        public delegate void RescaleDelegate();

        public void RescaleScreen()
        {
            using (var win = new Window(this))
            {
                var screen = win.PrimaryScreen();
                uint dx, dy;
                screen.GetDpi(out dx, out dy);
                Dpi = (short)dx;
                if (Dpi != OldDpi)
                {
                    var scale = Dpi / (float)OldDpi;
                    if (Math.Abs(scale - LastScale) > 0.01)
                    {
                        LastScale = scale;
                        /* foreach (Control ctrl in Controls)
                         {
                             if (ctrl.Anchor.HasFlag(AnchorStyles.Top)
                                 && ctrl.Anchor.HasFlag(AnchorStyles.Bottom))
                             {
                                 // Should fill space?
                                 //ctrl.Height = (int)(ctrl.Height / scale);
                             }
                             else
                             {
                                 //ctrl.Height = (int)(ctrl.Height * scale);
                                 //ctrl.Scale(new SizeF(scale, scale));
                             }
                             //ctrl.Width = (int)(ctrl.Width * scale);
                         }*/
                        var newWidth = (int)(Width * scale);
                        var newHeight = (int)(Height * scale);
                        SuspendLayout();
                        Scale(new SizeF(scale, scale));
                        Font = new Font(Font.FontFamily, Font.Size * scale, Font.Style);
                        ResumeLayout();
                        win.SetBounds(Left, Top, newWidth, newHeight);
                    }
                    OldDpi = Dpi;
                }
            }
        }

    }
}
