using System;
using System.Windows.Forms;
using System.IO;

namespace AglonaReader
{
    public partial class ImportTextForm : Form
    {
        public ImportTextForm()
        {
            InitializeComponent();
        }

        public ParallelText PText { get; set; }

        private void selectFile1Button_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    fileName1.Text = openFileDialog.FileName;
            }
        }

        private void selectFile2Button_Click(object sender, EventArgs e)
        {

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    fileName2.Text = openFileDialog.FileName;
            }

        }

        private void importButton_Click(object sender, EventArgs e)
        {
            string t1;
            try
            {
                t1 = File.ReadAllText(fileName1.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Source file cannot be read!" + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string t2;
            try
            {
                t2 = File.ReadAllText(fileName2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Translation file cannot be read!" + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PText.AddPair(t1, t2);
            PText[PText.Number() - 1].UpdateTotalSize();
            PText.UpdateAggregates(PText.Number() - 1);
            

            Close();
        }

        private void ImportTextForm_Shown(object sender, EventArgs e)
        {
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
