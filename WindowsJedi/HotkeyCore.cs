using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsJedi.WinApis;

namespace WindowsJedi {
    class HotkeyCore : IDisposable
    {
		private readonly KeyHookManager _keyMgr;
		private readonly Dictionary<Keys[], Action> _bindings;


		public HotkeyCore() {
			_bindings = new Dictionary<Keys[], Action>();
			_keyMgr = new KeyHookManager();
			_keyMgr.KeyDown += keyMgr_KeyDown;
			_keyMgr.KeyPress += keyMgr_KeyPress;
			_keyMgr.KeyUp += keyMgr_KeyUp;
		}

        ~HotkeyCore()
        {
            _keyMgr.Stop();
            _keyMgr.Dispose();
		}

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;
            _keyMgr.Stop();
            _keyMgr.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
		/// Add a key-combination and an action to fire in response.
		/// </summary>
		public void Bind(Keys[] keyPress, Action response) {
			_bindings.Add(keyPress, response);
		}

		private bool ShouldHandle () {
			return _bindings.Any(b => _keyMgr.IsKeyComboPressed(b.Key));
		}

		void keyMgr_KeyUp (object sender, KeyEventArgs e) {
		    if (!ShouldHandle()) return;

		    e.Handled = true;
		    e.SuppressKeyPress = true;
		}

		void keyMgr_KeyPress (object sender, KeyPressEventArgs e) {
			Keys keyCode;
			Enum.TryParse(e.KeyChar.ToString(CultureInfo.InvariantCulture), out keyCode);

			if (ShouldHandle()) {
				e.Handled = true;
			}
		}

		void keyMgr_KeyDown (object sender, KeyEventArgs e) {
		    if (!ShouldHandle()) return;

		    e.Handled = true;
		    e.SuppressKeyPress = true;

		    foreach (var binding in _bindings.Where(binding => _keyMgr.IsKeyComboPressed(binding.Key)))
		    {
		        binding.Value();
		    }
		}
	}
}
