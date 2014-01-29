namespace WindowsJedi.Components
{
    using System;
    using WindowsJedi.WinApis;

    /// <summary>
    /// Pushes the frontmost window to the back of the window stack
    /// </summary>
    public class Pushback : IDisposable
    {
        public void Dispose() { }

        public void PushBackFrontWindow()
        {
            var target = Window.ForegroundWindow();
            if (target == null) return;

            var next = target.NextVisibleBelow();
            target.SendToBack();

            if (next != null) next.Focus();
        }
    }
}