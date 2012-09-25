using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsJedi {
	class HotkeyCore {
		private readonly KeyHookManager keyMgr;
		private readonly Dictionary<Keys[], Action> bindings;


		public HotkeyCore() {
			bindings = new Dictionary<Keys[], Action>();
			keyMgr = new KeyHookManager();
			keyMgr.KeyDown += keyMgr_KeyDown;
			keyMgr.KeyPress += keyMgr_KeyPress;
			keyMgr.KeyUp += keyMgr_KeyUp;
		}

		~HotkeyCore () {
			keyMgr.Stop();
		}

		/// <summary>
		/// Add a key-combination and an action to fire in response.
		/// </summary>
		public void Bind(Keys[] KeyPress, Action Response) {
			bindings.Add(KeyPress, Response);
		}

		private bool ShouldHandle (Keys keyCode) {
			return bindings.Any(b => keyMgr.IsKeyComboPressed(b.Key));
		}

		void keyMgr_KeyUp (object sender, KeyEventArgs e) {
			if (ShouldHandle(e.KeyCode)) {
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		void keyMgr_KeyPress (object sender, KeyPressEventArgs e) {
			Keys keyCode;
			Enum.TryParse(e.KeyChar.ToString(), out keyCode);
			if (ShouldHandle(keyCode)) {
				e.Handled = true;
			}
		}

		void keyMgr_KeyDown (object sender, KeyEventArgs e) {
			if (ShouldHandle(e.KeyCode)) {
				e.Handled = true;
				e.SuppressKeyPress = true;

				foreach (var binding in bindings) {
					if (keyMgr.IsKeyComboPressed(binding.Key)) {
						binding.Value.Invoke();
					}
				}
			}
		}
	}
}
