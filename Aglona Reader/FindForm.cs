using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class FindForm : Form
    {

        public ParallelTextControl pTC;
        public MainForm mainForm;

        public FindForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.Escape)
                Close();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {

            textToFindBox.SelectAll();

            String textToFind = textToFindBox.Text;

            if (String.IsNullOrEmpty(textToFind))
            {
                MessageBox.Show("Specify a text to find.", "Text not specified");
                return;
            }

            bool checkLeft = (leftTextRadioButton.Checked || bothTextsRadioButton.Checked);
            bool checkRight = (rightTextRadioButton.Checked || bothTextsRadioButton.Checked);

            if (pTC.Reversed)
            {
                bool tmp = checkLeft;
                checkLeft = checkRight;
                checkRight = tmp;
            }

            int start;
            
            if (pTC.EditMode)
                start = pTC.HighlightedPair;
            else
                start = pTC.CurrentPair;

            int current = start;

            TextPair p;
            bool found;

            
            while (true)
            {

                current = current + 1;

                if (current == pTC.Number)
                {
                    DialogResult d = MessageBox.Show("Continue from the start?", "End of the document reached", MessageBoxButtons.YesNo);

                    if (d == System.Windows.Forms.DialogResult.No)
                        return;

                    current = 0;

                }

                if (current == start)
                {
                    MessageBox.Show("Nothing found.");
                    return;
                }

                p = pTC[current];

                found = false;

                if (checkLeft && (p.SB1 == null ? p.Text1 : p.SB1.ToString()).Contains(textToFind)
                    || checkRight && (p.SB2 == null ? p.Text2 : p.SB2.ToString()).Contains(textToFind))
                {

                    if (!pTC.EditMode || current > pTC.LastRenderedPair || current < pTC.CurrentPair)
                        mainForm.GotoPair(current, false, false, 1);
                    else
                    {
                        pTC.HighlightedPair = current - 1;
                        mainForm.ProcessDownArrow(true);
                    }
                    
                    return;

                }

            }

        }

        private void FindForm_Load(object sender, EventArgs e)
        {
            bothTextsRadioButton.Checked = true;
        }


        private void FindForm_Activated(object sender, EventArgs e)
        {
            textToFindBox.SelectAll();
            textToFindBox.Focus();
        }

    }
}
