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

        public bool Result { get; set; }

        public ParallelTextControl ParallelTextControl { get; set; }

        private TextPair TextPair { get; set; }

        public int PairIndex { get; set; }

        //private string text1;
        //private string text2;
        //private byte level;

        public EditPairForm()
        {
            InitializeComponent();
        }

        private void PressOK()
        {
            Result = true;

            if (ParallelTextControl.reversed)
            {
                TextPair.text1 = textBox2.Text;
                TextPair.text2 = textBox1.Text;
            }
            else
            {
                TextPair.text1 = textBox1.Text;
                TextPair.text2 = textBox2.Text;
            }

            TextPair.sb1 = null;
            TextPair.sb2 = null;

            if (level0.Checked)
                TextPair.structureLevel = 0;
            else if (level1.Checked)
                TextPair.structureLevel = 1;
            else if (level2.Checked)
                TextPair.structureLevel = 2;
            else if (level3.Checked)
                TextPair.structureLevel = 3;

            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            PressOK();
        }

        private void EditPairForm_Shown(object sender, EventArgs e)
        {
            TextPair = ParallelTextControl[PairIndex];

            if (ParallelTextControl.reversed)
            {
                textBox1.Text = TextPair.sb2 == null ? TextPair.text2 : TextPair.sb2.ToString();
                textBox2.Text = TextPair.sb1 == null ? TextPair.text1 : TextPair.sb1.ToString();
            }
            else
            {
                textBox1.Text = TextPair.sb1 == null ? TextPair.text1 : TextPair.sb1.ToString();
                textBox2.Text = TextPair.sb2 == null ? TextPair.text2 : TextPair.sb2.ToString();
            }

            switch (TextPair.structureLevel)
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


            Result = false;
            
        }
    }
}
