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
    public partial class EditPairForm : Form
    {

        public bool result;

        public ParallelTextControl pTC;

        private TextPair p;

        public int pairIndex;

        //private string text1;
        //private string text2;
        //private byte level;

        public EditPairForm()
        {
            InitializeComponent();
        }

        private void PressOK()
        {
            result = true;

            if (pTC.reversed)
            {
                p.text1 = textBox2.Text;
                p.text2 = textBox1.Text;
            }
            else
            {
                p.text1 = textBox1.Text;
                p.text2 = textBox2.Text;
            }

            p.sb1 = null;
            p.sb2 = null;

            if (level0.Checked)
                p.structureLevel = 0;
            else if (level1.Checked)
                p.structureLevel = 1;
            else if (level2.Checked)
                p.structureLevel = 2;
            else if (level3.Checked)
                p.structureLevel = 3;

            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            PressOK();
        }

        private void EditPairForm_Shown(object sender, EventArgs e)
        {
            p = pTC.pText.textPairs[pairIndex];

            if (pTC.reversed)
            {
                textBox1.Text = p.sb2 == null ? p.text2 : p.sb2.ToString();
                textBox2.Text = p.sb1 == null ? p.text1 : p.sb1.ToString();
            }
            else
            {
                textBox1.Text = p.sb1 == null ? p.text1 : p.sb1.ToString();
                textBox2.Text = p.sb2 == null ? p.text2 : p.sb2.ToString();
            }

            switch (p.structureLevel)
            {
                case 0:
                    level0.Checked = true;
                    break;
                case 1:
                    level1.Checked = true;
                    break;
                case 2:
                    level2.Checked = true;
                    break;
                case 3:
                    level3.Checked = true;
                    break;
            }


            result = false;
            
        }
    }
}
