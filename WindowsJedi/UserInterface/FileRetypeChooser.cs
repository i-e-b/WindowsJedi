namespace WindowsJedi.UserInterface
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    public partial class FileRetypeChooser : Form
    {
        public FileRetypeChooser()
        {
            InitializeComponent();
            openFileDialog.FileOk += openFileDialog_FileOk;
        }

        void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            fileSelected.Text = openFileDialog.FileName;
        }

        private void chooseSource_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void goRetype_Click(object sender, EventArgs e)
        {
            var text = File.ReadAllText(fileSelected.Text);
            goRetype.Enabled = false;

            var buttonTitle = goRetype.Text;
            for (int i = 10; i > 0; i--)
            {
                goRetype.Text = i.ToString(CultureInfo.InvariantCulture);
                Refresh();
                Application.DoEvents();
                Thread.Sleep(1000);
            }
            goRetype.Text = "Sending";
            SendInChunks(text);

            goRetype.Text = buttonTitle;
            goRetype.Enabled = true;
        }

        static void SendInChunks(string text)
        {
            var sb = new StringBuilder();
            var cr = false;

            Action sendNow = () => { SendKeys.SendWait(sb.ToString()); sb.Clear(); SendKeys.Flush(); };

            foreach (char c in text)
            {
                switch (c)
                {
                    case '+': sb.Append("{+}"); break;
                    case '^': sb.Append("{^}"); break;
                    case '%': sb.Append("{%}"); break;
                    case '~': sb.Append("{~}"); break;
                    case '(': sb.Append("{(}"); break;
                    case ')': sb.Append("{)}"); break;
                    case '[': sb.Append("{[}"); break;
                    case ']': sb.Append("{]}"); break;
                    case '{': sb.Append("{{}"); break;
                    case '}': sb.Append("{}}"); break;

                    case '"': sb.Append("\"{ESC}"); break; // for stupid VersionOne bugs

                    case '\r': if (!cr) { sb.Append("{ENTER}"); } break;
                    case '\n': if (!cr) { sb.Append("{ENTER}"); } break;

                    default: sb.Append(c); break;
                }
                cr = (c == '\r' || c == '\n') && (!cr);
                if (cr || c == '"') sendNow();
                else if (sb.Length > 20) sendNow();
            }
            sendNow();
        }
    }
}
