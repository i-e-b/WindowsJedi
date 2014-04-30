namespace WindowsJedi.Components {
    using System;
    using System.Windows.Forms;

    public partial class ShellEventsDelegateForm : Form {
        /// <summary> The task bar flash event. Fires for every draw of the flash </summary>
        readonly IntPtr HSHELL_FLASH = new IntPtr(0x8006);

        /*#define HSHELL_WINDOWCREATED        1
#define HSHELL_WINDOWDESTROYED      2
#define HSHELL_ACTIVATESHELLWINDOW  3

#define HSHELL_WINDOWACTIVATED      4
#define HSHELL_GETMINRECT           5
#define HSHELL_REDRAW               6
#define HSHELL_TASKMAN              7
#define HSHELL_LANGUAGE             8
#define HSHELL_SYSMENU              9
#define HSHELL_ENDTASK              10
#define HSHELL_ACCESSIBILITYSTATE   11
#define HSHELL_APPCOMMAND           12

#define HSHELL_WINDOWREPLACED       13
#define HSHELL_WINDOWREPLACING      14

#define HSHELL_HIGHBIT            0x8000
#define HSHELL_FLASH              (HSHELL_REDRAW|HSHELL_HIGHBIT)
#define HSHELL_RUDEAPPACTIVATED   (HSHELL_WINDOWACTIVATED|HSHELL_HIGHBIT)*/

        public ShellEventsDelegateForm () {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.WParam == HSHELL_FLASH)
            {
                //MessageBox.Show("FLASH!");
            }
            base.WndProc(ref m);
        }
	}
}
