using System;
using System.Threading;
using System.Windows.Forms;
using WindowsJedi.Components;
using WindowsJedi.Properties;
using WindowsJedi.UserInterface;

namespace WindowsJedi {
    using WindowsJedi.WinApis;

    static class Program {
        public static NotifyTrayApp Notify;
        public static ShellEventsDelegateForm ShellEventsDelegateForm;


		[STAThread]
		static void Main () {
			InitialiseWinForms();

            // Hook 'shell' events into the capturing form.
            Win32.RegisterShellHookWindow(ShellEventsDelegateForm.Handle);

            using (var switcherForm = new SwitcherForm())
            using (var concentrationForm = new ConcentrationForm())
            using (var popupWindows = new PopupWindows())
            using (var pushback = new Pushback())
            using (var hotKeys = new HotkeyCore())
            {
// ReSharper disable AccessToDisposedClosure
                hotKeys.Bind(new[] { Keys.LWin, Keys.Tab }, switcherForm.Toggle);
                hotKeys.Bind(new[] { Keys.RShiftKey, Keys.F12 }, concentrationForm.Toggle);
                hotKeys.Bind(new[] { Keys.LWin, Keys.Space }, popupWindows.Toggle);
                hotKeys.Bind(new[] { Keys.LWin, Keys.Escape }, pushback.PushBackFrontWindow);
// ReSharper restore AccessToDisposedClosure

                Notify = new NotifyTrayApp("Windows Jedi", Resources.JediIcon, "http://snippetsfor.net/WindowsJedi");
                Notify.AddMenuItem("Settings", delegate { (new UserInterface.Settings()).ShowDialog(); });

                Application.ThreadException += Application_ThreadException;

                Application.Run();

            }
            ShellEventsDelegateForm.Dispose();
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
