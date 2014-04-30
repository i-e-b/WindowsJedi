namespace WindowsJedi.Components
{
    using System;
    using System.Windows.Forms;

    public class UnclickableFullScreenForm : FullScreenForm
    {
        protected const int WM_MOUSEACTIVATE = 0x0021, MA_NOACTIVATEANDEAT = 0x0004;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATEANDEAT;
                return;
            }
            base.WndProc(ref m);
        }
    }
}