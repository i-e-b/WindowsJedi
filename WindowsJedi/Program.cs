namespace WindowsJedi {
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using WindowsJedi.Components;
    using WindowsJedi.Features;
    using WindowsJedi.Properties;
    using WindowsJedi.UserInterface;
    using WindowsJedi.WinApis;
    using Settings = WindowsJedi.UserInterface.Settings;

    static class Program {
        public static NotifyTrayApp TrayIcon;
        public static ShellEventsDelegateForm ShellEventsDelegateForm;

		[STAThread]
		static void Main () {
			InitialiseWinForms();

            // Hook 'shell' events into the capturing form.
            Win32.RegisterShellHookWindow(ShellEventsDelegateForm.Handle);

            // Hook into system wide windowing events
            using (var winHook = new WindowHookManager())
            {
                
                using (var switcherForm = new SwitcherForm(winHook))
                using (var concentrationForm = new ConcentrationForm(winHook))
                using (var popupWindows = new PopupWindows())
                using (var pushback = new Pushback())
                using (var arranger = new WindowArranger())
                //using (var experimental = new Experimental())
                using (var hotKeys = new HotkeyCore())
                {
                    //hotKeys.Macro(new[] { Keys.LControlKey, Keys.Space }, new KeySequence().Up(Keys.LControlKey).Repeat(4, Keys.Space)); // for annoying text entry that does't do tabs 

                    hotKeys.Bind(new[] { Keys.LWin, Keys.Tab }, switcherForm.Toggle);
                    hotKeys.Bind(new[] { Keys.LShiftKey, Keys.F12 }, concentrationForm.Toggle);
                    hotKeys.Bind(new[] { Keys.RShiftKey, Keys.F12 }, concentrationForm.Toggle);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Space }, popupWindows.ToggleVisibility);
                    hotKeys.Bind(new[] { Keys.LControlKey, Keys.LWin, Keys.Space }, popupWindows.ToggleFade);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Escape }, pushback.PushBackFrontWindow);

                    // re-arrangement of top-window that breaks less things than the Windows built-in
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D1 }, arranger.SetTopLeft);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D2 }, arranger.SetTop);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D3 }, arranger.SetTopRight);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D4 }, arranger.SetLeft);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D5 }, arranger.SetCentre);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D6 }, arranger.SetRight);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D7 }, arranger.SetBottomLeft);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D8 }, arranger.SetBottom);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LMenu /*alt*/, Keys.D9 }, arranger.SetBottomRight);

                    //hotKeys.Bind(new[] { Keys.LWin, Keys.A }, experimental.HideForegroundWindow);

                    TrayIcon = new NotifyTrayApp("Windows Jedi", Resources.JediIcon, "https://github.com/i-e-b/WindowsJedi");
                    TrayIcon.AddMenuItem("Settings", delegate { new Settings().ShowDialog(); });

                    Application.ThreadException += Application_ThreadException;

                    Application.Run();

                }
                ShellEventsDelegateForm.Dispose();
            }
		}

		static void Application_ThreadException (object sender, ThreadExceptionEventArgs e) {
		}

		private static void InitialiseWinForms() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			ShellEventsDelegateForm = new ShellEventsDelegateForm();
			ShellEventsDelegateForm.Show();
			ShellEventsDelegateForm.Hide();
		}

	}
}
