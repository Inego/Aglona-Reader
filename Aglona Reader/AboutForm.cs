using System.Drawing;
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

        private void AboutForm_Paint(object sender, PaintEventArgs e)
        {
            return;

            var formGraphics = e.Graphics;
            Pen myPen;

            ColorRGB c;

            for (var i = 0; i < 1000; i++)
            {
                
                c = ColorRGB.HSL2RGB((double)i / 1000, 1, 0.5);
                myPen = new Pen(Color.FromArgb(c.Red, c.Green, c.Blue));
                formGraphics.DrawLine(myPen, 10 + i, 60, 10 + i, 80);
                myPen.Dispose();
            }

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
