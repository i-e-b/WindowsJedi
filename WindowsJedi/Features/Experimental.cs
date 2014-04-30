namespace WindowsJedi.Features
{
    using System;
    using WindowsJedi.WinApis;

    public class Experimental : IDisposable
    {
        public void Dispose()
        {
            if (_prev != null)
            {
                _prev.Show();
            }
        }


        Window _prev = null;
        public void HideForegroundWindow()
        {
            if (_prev != null)
            {
                _prev.Show();
                _prev.Dispose();
                _prev = null;
                return;
            }
            _prev = Window.ForegroundWindow();
            if (_prev == null) return;

            var next = _prev.NextVisibleBelow();
            _prev.Hide();

            if (next != null) next.Focus();
        }
    }
}