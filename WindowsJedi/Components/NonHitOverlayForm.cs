namespace WindowsJedi.Components
{
    using System.Windows.Forms;
    using WindowsJedi.WinApis;

    /// <summary>
    /// Overlay that is not interactive
    /// </summary>
    public class NonHitOverlayForm : OverlayForm
    {
        public NonHitOverlayForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= Win32.WS_EX_LAYERED; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_NCHITTEST)
            {
                m.Result = Win32.HTNOWHERE;	// pass to HTCLIENT
                return;
            }
            base.WndProc(ref m);
        }
    }
}