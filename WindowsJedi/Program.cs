using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsJedi.Components;
using WindowsJedi.Properties;

namespace WindowsJedi {
	static class Program {
		public static HotkeyCore hotKeys;
		private static NotifyTrayApp notify;
		private static DummyForm dummyForm;

		private static ConcentrationForm concentrationForm;
		private static SwitcherForm switcherForm;

		[STAThread]
		static void Main () {
			InitialiseWinForms();

			concentrationForm = new ConcentrationForm();
			switcherForm = new SwitcherForm();

			hotKeys = new HotkeyCore();
            Action toggleSwitcher = () => switcherForm.Toggle();
            Action toggleConcentration = () => concentrationForm.Toggle();

            var pin1 = GCHandle.Alloc(toggleSwitcher);
            var pin2 = GCHandle.Alloc(toggleConcentration);

            hotKeys.Bind(new[] { Keys.LWin, Keys.Tab }, toggleSwitcher);
            hotKeys.Bind(new[] { Keys.RShiftKey, Keys.F12 }, toggleConcentration);

			notify = new NotifyTrayApp("Windows Jedi", Resources.JediIcon, "http://snippetsfor.net/WindowsJedi");
			notify.AddMenuItem("Settings", delegate { (new UserInterface.Settings()).ShowDialog(); });

			Application.ThreadException += Application_ThreadException;

			Application.Run();

            pin1.Free();
            pin2.Free();
		}

		static void Application_ThreadException (object sender, System.Threading.ThreadExceptionEventArgs e) {
		}

		private static void InitialiseWinForms() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			dummyForm = new DummyForm();
			dummyForm.Show();
			dummyForm.Hide();
		}

	}
}
