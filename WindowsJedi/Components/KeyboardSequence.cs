namespace WindowsJedi.Components
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;
    using WindowsJedi.WinApis;

    public class KeySequence
    {
        readonly List<KeyState> _keyQueue;

        public KeySequence()
        {
            _keyQueue = new List<KeyState>();
        }

        public KeySequence Repeat(int times, Keys key)
        {
            for (int i = 0; i < times; i++)
            {
                _keyQueue.Add(new KeyState { Key = key, State = WmKeyEvent.Down });
            }
            return this;
        }
        public KeySequence Up(Keys key)
        {
            _keyQueue.Add(new KeyState { Key = key, State = WmKeyEvent.Up });
            return this;
        }

        public void PlayToFocusedWindow()
        {
            var target = Win32.GetForegroundWindow();
            if (target == IntPtr.Zero) return;

            foreach (var keyState in _keyQueue.ToArray())
            {
                Win32.PostMessage(target, (UInt32)keyState.State, (int)keyState.Key, 0);
                Thread.Sleep(100);
            }
        }

    }

    public class KeyState
    {
        public Keys Key { get; set; }
        public WmKeyEvent State { get; set; }
    }

    public enum WmKeyEvent:uint
    {
        Up = Win32.WM_KEYUP,
        Down = Win32.WM_KEYDOWN
    }
}