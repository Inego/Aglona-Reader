using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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

            System.Drawing.Graphics formGraphics = e.Graphics;
            System.Drawing.Pen myPen;

            ColorRGB c;

            for (int i = 0; i < 1000; i++)
            {
                
                c = ColorRGB.HSL2RGB((double)i / 1000, 1, 0.5);
                myPen = new System.Drawing.Pen(Color.FromArgb(c.Red, c.Green, c.Blue));
                formGraphics.DrawLine(myPen, 10 + i, 60, 10 + i, 80);
                myPen.Dispose();
            }

        }

        private void AboutForm_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X - 10;

            if (x < 0 || x >= 1000)
                colorLabel.Text = "";
            else
                colorLabel.Text = x.ToString();
            
        }
    }
}
