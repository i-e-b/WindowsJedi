﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsJedi.Components;
using WindowsJedi.Properties;
using WindowsJedi.UserInterface;

namespace WindowsJedi {
	static class Program {
        public static NotifyTrayApp Notify;
        public static DummyForm DummyForm;


		[STAThread]
		static void Main () {
			InitialiseWinForms();

            using (var switcherForm = new SwitcherForm())
            using (var concentrationForm = new ConcentrationForm())
            using (var hotKeys = new HotkeyCore())
            {
// ReSharper disable AccessToDisposedClosure
                hotKeys.Bind(new[] { Keys.LWin, Keys.Tab }, switcherForm.Toggle);
                hotKeys.Bind(new[] { Keys.RShiftKey, Keys.F12 }, concentrationForm.Toggle);
// ReSharper restore AccessToDisposedClosure

                Notify = new NotifyTrayApp("Windows Jedi", Resources.JediIcon, "http://snippetsfor.net/WindowsJedi");
                Notify.AddMenuItem("Settings", delegate { (new UserInterface.Settings()).ShowDialog(); });

                Application.ThreadException += Application_ThreadException;

                Application.Run();

            }
            DummyForm.Dispose();
		}

		static void Application_ThreadException (object sender, System.Threading.ThreadExceptionEventArgs e) {
		}

		private static void InitialiseWinForms() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			DummyForm = new DummyForm();
			DummyForm.Show();
			DummyForm.Hide();
		}

	}
}
