using System;
using System.Windows.Forms;

namespace AglonaReader
{
    internal partial class FindForm : Form
    {

        public ParallelTextControl pTC;
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

            if (pTC.Reversed)
            {
                var tmp = checkLeft;
                checkLeft = checkRight;
                checkRight = tmp;
            }

            var start = pTC.EditMode ? pTC.HighlightedPair : pTC.CurrentPair;

            var current = start;

            while (true)
            {

                current += 1;

                if (current == pTC.Number)
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

                var p = pTC[current];

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
