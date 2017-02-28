namespace WindowsJedi.Components
{
    using System.Windows.Forms;
    using WindowsJedi.WinApis;

    /// <summary>
    /// Overlay that can be dragged and repositioned by user
    /// </summary>
    public class DraggableOverlayForm : OverlayForm {
        protected override void WndProc (ref Message m) {
            if (m.Msg == Win32.WM_NCHITTEST) {
                m.Result = Win32.HTCAPTION;	// report title bar, OS will handle dragging
                return;
            }
            base.WndProc(ref m);
        }
    }
}