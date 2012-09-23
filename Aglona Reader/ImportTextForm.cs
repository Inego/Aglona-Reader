using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AglonaReader
{
    public partial class ImportTextForm : Form
    {

        public bool Result { get; set; }

        public ImportTextForm()
        {
            InitializeComponent();
        }

        public ParallelText pText;

        private void selectFile1Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName1.Text = openFileDialog.FileName;
            }
        }

        private void selectFile2Button_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName2.Text = openFileDialog.FileName;
            }

        }

        private void importButton_Click(object sender, EventArgs e)
        {
            string t1 = File.ReadAllText(fileName1.Text);
            string t2 = File.ReadAllText(fileName2.Text);

            pText.AddPair(t1, t2);

            Close();
        }

        private void ImportTextForm_Shown(object sender, EventArgs e)
        {
            Result = false;   
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
