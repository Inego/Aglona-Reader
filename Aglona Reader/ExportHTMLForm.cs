using System;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class ExportHtmlForm : Form
    {

        private readonly ParallelTextControl pTc;

        public ExportHtmlForm(ParallelTextControl pTc)
        {
            InitializeComponent();
            this.pTc = pTc;
        }

        private void selectExportFileButton_Click(object sender, EventArgs e)
        {
            // Ask for the file name
            using (var d = new SaveFileDialog())
            {
                d.Filter = "HTML files|*.html";

                d.RestoreDirectory = true;

                var dialogResult = d.ShowDialog();

                if (dialogResult == DialogResult.OK)
                    exportFileName.Text = d.FileName;

            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (exportFileName.Text.Length == 0)
            {
                MessageBox.Show("File name not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var exportedSuccessfully = false;
            try
            {
                pTc.PText.ExportHtml(exportFileName.Text);
                exportedSuccessfully = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export error!" + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();

            if (!exportedSuccessfully) return;
            
            try
            {
                System.Diagnostics.Process.Start(exportFileName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error while opening exported file!" + Environment.NewLine + Environment.NewLine + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
