using System.Windows.Forms;

namespace AglonaReader
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
                Close();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void AboutForm_MouseMove(object sender, MouseEventArgs e)
        {
            var x = e.X - 10;

            if (x < 0 || x >= 1000)
                colorLabel.Text = "";
            else
                colorLabel.Text = x.ToString();
            
        }
    }
}
