using System;
using System.Windows.Forms;

namespace AglonaReader
{
    internal partial class FindForm : Form
    {

        public ParallelTextControl pTc;
        internal MainForm mainForm;

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

            var textToFind = textToFindBox.Text;

            if (string.IsNullOrEmpty(textToFind))
            {
                MessageBox.Show("Specify a text to find.", "Text not specified");
                return;
            }

            var checkLeft = leftTextRadioButton.Checked || bothTextsRadioButton.Checked;
            var checkRight = rightTextRadioButton.Checked || bothTextsRadioButton.Checked;

            if (pTc.Reversed)
            {
                var tmp = checkLeft;
                checkLeft = checkRight;
                checkRight = tmp;
            }

            var start = pTc.EditMode ? pTc.HighlightedPair : pTc.CurrentPair;

            var current = start;

            while (true)
            {

                current += 1;

                if (current == pTc.Number)
                {
                    var d = MessageBox.Show("Continue from the start?", "End of the document reached", MessageBoxButtons.YesNo);

                    if (d == DialogResult.No)
                        return;

                    current = 0;

                }

                if (current == start)
                {
                    MessageBox.Show("Nothing found.");
                    return;
                }

                var p = pTc[current];

                // ReSharper disable once InvertIf
                if (checkLeft && (p.Sb1 == null ? p.Text1 : p.Sb1.ToString()).Contains(textToFind)
                    || checkRight && (p.Sb2 == null ? p.Text2 : p.Sb2.ToString()).Contains(textToFind))
                {
                    if (!pTc.EditMode || current > pTc.LastRenderedPair || current < pTc.CurrentPair)
                        mainForm.GotoPair(current, false, false, 1);
                    else
                    {
                        pTc.HighlightedPair = current - 1;
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
