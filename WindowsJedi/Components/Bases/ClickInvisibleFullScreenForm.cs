namespace WindowsJedi.Components
{
    using System;
    using System.Windows.Forms;

    public class ClickInvisibleFullScreenForm : FullScreenForm
    {
        protected  const int WM_MOUSEACTIVATE = 0x0021, MA_NOACTIVATE = 0x0003;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }
    }
}