using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsJedi.Components {
	class NotifyTrayApp {
		private readonly string infoUrl;
		private readonly NotifyIcon icon;

		public event EventHandler Exit;

		public NotifyTrayApp(string Name, System.Drawing.Icon Icon, string InfoUrl) {
			infoUrl = InfoUrl;
			icon = new NotifyIcon{Icon = Icon};

			var items = new MenuItem[2];
			items[1] = new MenuItem("Exit");
			items[1].Click += Exit_Click;
			items[0] = new MenuItem("About "+ Name);
			items[0].Click += About_Click;
			icon.ContextMenu = new ContextMenu(items);
			icon.Visible = true;
			icon.Text = Name;
		}

		public void AddMenuItem (string Name, EventHandler response) {
			icon.ContextMenu.MenuItems.Add(0, new MenuItem(Name, response));
		}

		public void InvokeExit () {
			EventHandler handler = Exit;
			if (handler != null) handler(this, new EventArgs());
		}

		private void Exit_Click(object sender, EventArgs e) {
			icon.Visible = false;
			icon.Dispose();

			InvokeExit();

			Application.Exit();
			Environment.Exit(0);
		}

		private void About_Click(object sender, EventArgs e) {
			Process.Start(infoUrl);
		}
	}
}
