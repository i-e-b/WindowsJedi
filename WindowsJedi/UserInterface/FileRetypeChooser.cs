namespace WindowsJedi.UserInterface
{
    using System.Globalization;
    using System.IO;
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

        private void chooseSource_Click(object sender, System.EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void goRetype_Click(object sender, System.EventArgs e)
        {
            var text = File.ReadAllText(fileSelected.Text);
            goRetype.Enabled = false;

            var buttonTitle = goRetype.Text;
            for (int i = 5; i > 0; i--)
            {
                goRetype.Text = i.ToString(CultureInfo.InvariantCulture);
                Refresh();
                Application.DoEvents();
                Thread.Sleep(1000);
            }
            goRetype.Text = "Sending";
            SendKeys.SendWait(text);

            goRetype.Text = buttonTitle;
            goRetype.Enabled = true;
        }
    }
}
