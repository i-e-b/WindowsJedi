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


            // TESTING:
            /*Console.WriteLine(DateTime.Now.ToString());
            using (var gf = new GifWriter(@"W:\work\tmp.gif", new Size(320,240)))
            {
                for (var x = 0; x < 100; x++)
                {
                    gf.WriteScreenFrame(new Point(10+x, 10));
                }
            }
            Console.WriteLine(DateTime.Now.ToString());// manages about 30fps, almost regardless of capture size (up to a limit)
            */
            // END TEST

            // Hook into system wide windowing events
            using (var winHook = new WindowHookManager())
            {
                
                using (var switcherForm = new SwitcherForm(winHook))
                using (var concentrationForm = new ConcentrationForm(winHook))
                using (var popupWindows = new PopupWindows())
                using (var pushback = new Pushback())
                using (var arranger = new WindowArranger())
                using (var experimental = new ReferenceWindow(winHook))
                using (var hotKeys = new HotkeyCore())
                {
                    // General features
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Tab }, switcherForm.Toggle);
                    hotKeys.Bind(new[] { Keys.LShiftKey, Keys.F12 }, concentrationForm.Toggle);
                    hotKeys.Bind(new[] { Keys.RShiftKey, Keys.F12 }, concentrationForm.Toggle);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Space }, popupWindows.ToggleVisibility);
                    hotKeys.Bind(new[] { Keys.LControlKey, Keys.LWin, Keys.Space }, popupWindows.ToggleFade);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Escape }, pushback.PushBackFrontWindow);

                    // re-arrangement of top-window that breaks less things than the Windows built-in
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D1 }, arranger.SetTopLeft);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D2 }, arranger.SetTop);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D3 }, arranger.SetTopRight);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D4 }, arranger.SetLeft);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D5 }, arranger.SetCentre);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D6 }, arranger.SetRight);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D7 }, arranger.SetBottomLeft);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D8 }, arranger.SetBottom);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.LControlKey, Keys.D9 }, arranger.SetBottomRight);

                    // win-arrow to move screen
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Left }, arranger.MoveScreenLeft);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Up }, arranger.MoveScreenUp);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Right }, arranger.MoveScreenRight);
                    hotKeys.Bind(new[] { Keys.LWin, Keys.Down }, arranger.MoveScreenDown);

                    // Reference window
                    hotKeys.Bind(new[] { Keys.LWin, Keys.OemMinus}, experimental.SetReferenceWindow);

                    TrayIcon = new NotifyTrayApp("Windows Jedi", Resources.JediIcon, "https://github.com/i-e-b/WindowsJedi");
                    TrayIcon.AddMenuItem("Settings", delegate { new Settings().ShowDialog(); });
                    TrayIcon.AddMenuItem("Re-type file", delegate { new FileRetypeChooser().Show(); });
                    TrayIcon.AddMenuItem("Screen Capture to GIF", delegate { new ScreenCaptureForm().Show(); });

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
