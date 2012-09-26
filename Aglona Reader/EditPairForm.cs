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

        //private string Text1;
        //private string Text2;
        //private byte level;

        public EditPairForm()
        {
            InitializeComponent();
        }

        private void PressOK()
        {
            Result = true;

            if (ParallelTextControl.Reversed)
            {
                TextPair.Text1 = textBox2.Text;
                TextPair.Text2 = textBox1.Text;
            }
            else
            {
                TextPair.Text1 = textBox1.Text;
                TextPair.Text2 = textBox2.Text;
            }

            TextPair.SB1 = null;
            TextPair.SB2 = null;

            if (level0.Checked)
                TextPair.StructureLevel = 0;
            else if (level1.Checked)
                TextPair.StructureLevel = 1;
            else if (level2.Checked)
                TextPair.StructureLevel = 2;
            else if (level3.Checked)
                TextPair.StructureLevel = 3;

            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            PressOK();
        }

        private void EditPairForm_Shown(object sender, EventArgs e)
        {
            TextPair = ParallelTextControl[PairIndex];

            if (ParallelTextControl.Reversed)
            {
                textBox1.Text = TextPair.SB2 == null ? TextPair.Text2 : TextPair.SB2.ToString();
                textBox2.Text = TextPair.SB1 == null ? TextPair.Text1 : TextPair.SB1.ToString();
            }
            else
            {
                textBox1.Text = TextPair.SB1 == null ? TextPair.Text1 : TextPair.SB1.ToString();
                textBox2.Text = TextPair.SB2 == null ? TextPair.Text2 : TextPair.SB2.ToString();
            }

            switch (TextPair.StructureLevel)
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
