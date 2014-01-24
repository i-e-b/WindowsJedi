using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsJedi.UserInterface {
	class NotifyTrayApp {
		private readonly string _infoUrl;
		private readonly NotifyIcon _icon;

		public event EventHandler Exit;

		public NotifyTrayApp(string name, System.Drawing.Icon icon, string infoUrl) {
			_infoUrl = infoUrl;
			_icon = new NotifyIcon{Icon = icon};

			var items = new MenuItem[2];
			items[1] = new MenuItem("Exit");
			items[1].Click += Exit_Click;
			items[0] = new MenuItem("About "+ name);
			items[0].Click += About_Click;
			_icon.ContextMenu = new ContextMenu(items);
			_icon.Visible = true;
			_icon.Text = name;
		}

		public void AddMenuItem (string name, EventHandler response) {
			_icon.ContextMenu.MenuItems.Add(0, new MenuItem(name, response));
		}

		public void InvokeExit () {
			var handler = Exit;
			if (handler != null) handler(this, new EventArgs());
		}

		private void Exit_Click(object sender, EventArgs e) {
			_icon.Visible = false;
			_icon.Dispose();

			InvokeExit();

			Application.Exit();
		}

		private void About_Click(object sender, EventArgs e) {
			Process.Start(_infoUrl);
		}
	}
}
