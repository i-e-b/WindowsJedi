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
                Press(key);
            }
            return this;
        }

        public KeySequence Up(Keys key)
        {
            _keyQueue.Add(new KeyState { Key = key, State = WmKeyEvent.Up });
            return this;
        }

        public KeySequence Down(Keys key)
        {
            _keyQueue.Add(new KeyState { Key = key, State = WmKeyEvent.Down });
            return this;
        }

        public KeySequence Press(Keys keys)
        {
            Down(keys); Up(keys);
            return this;
        }

        public void PlayToFocusedWindow()
        {
            var target = Win32.GetForegroundWindow();
            if (target == IntPtr.Zero) return;

            foreach (var keyState in _keyQueue.ToArray())
            {
                Win32.PostMessage(target, (UInt32)keyState.State, (int)keyState.Key, 0);
                Thread.Sleep(5);
            }
        }

        public void Clear()
        {
            _keyQueue.Clear();
        }

        public static void PlayStringToFocusedWindow(string str)
        {
            var ks = new KeySequence();
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    ks.Down((Keys)c);
                }
                else if (char.IsLower(c))
                {
                    ks.Down(Keys.ShiftKey | ((Keys)(char.ToUpper(c))));
                }
                ks.PlayToFocusedWindow();
                ks.Clear();
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