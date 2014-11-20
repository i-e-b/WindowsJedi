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
                //using (var experimental = new Experimental())
                using (var hotKeys = new HotkeyCore())
                {
                    //hotKeys.Macro(new[] { Keys.LControlKey, Keys.Space }, new KeySequence().Up(Keys.LControlKey).Repeat(4, Keys.Space)); // for annoying text entry that does't do tabs 

                    hotKeys.Bind(new[] { Keys.LWin, Keys.Tab }, switcherForm.Toggle);
                    hotKeys.Bind(new[] { Keys.RShiftKey, Keys.F12 }, concentrationForm.Toggle);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Space }, popupWindows.ToggleVisibility);
                    hotKeys.Bind(new[] { Keys.LControlKey, Keys.LWin, Keys.Space }, popupWindows.ToggleFade);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Escape }, pushback.PushBackFrontWindow);

                    //hotKeys.Bind(new[] { Keys.LWin, Keys.A }, experimental.HideForegroundWindow);

                    TrayIcon = new NotifyTrayApp("Windows Jedi", Resources.JediIcon, "https://github.com/i-e-b/WindowsJedi");
                    TrayIcon.AddMenuItem("Settings", delegate { new Settings().ShowDialog(); });
                    TrayIcon.AddMenuItem("Re-type file", delegate { new FileRetypeChooser().Show(); });

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
