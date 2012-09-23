using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class ColorTableForm : Form
    {
        public ColorTableForm()
        {
            InitializeComponent();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            var hue = (double)trackBar1.Value / 1000;
            label1.Text = hue.ToString();
            this.BackColor = ColorRGB.Hsl2Rgb(hue, 1, 0.5);
        }
    }
}
