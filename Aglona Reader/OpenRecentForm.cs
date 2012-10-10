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
    public partial class OpenRecentForm : Form
    {

        public AppSettings appSettings;
        public string result;

        public OpenRecentForm()
        {
            InitializeComponent();
        }

        private void OpenRecentForm_Shown(object sender, EventArgs e)
        {

            foreach (FileUsageInfo fileUsageInfo in appSettings.FileUsages)
                listBox.Items.Add(fileUsageInfo.FileName);

            if (listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;

            result = "";

        }

        private void formOKButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
                result = (string) listBox.Items[listBox.SelectedIndex];

            Close();
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            formOKButton_Click(null, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (listBox.SelectedIndex == -1)
                return;

            // Ask the user

            DialogResult r = MessageBox.Show(
                    "Really remove this file from the list?", "Removing a recent file record", MessageBoxButtons.YesNoCancel);

            if (r == System.Windows.Forms.DialogResult.Yes)
            {
                // 1. Remove the corresponding fileusage info

                int prevIndex = listBox.SelectedIndex;

                string fileName = (string)listBox.Items[prevIndex];

                foreach (FileUsageInfo fileUsageInfo in appSettings.FileUsages)
                    if (fileUsageInfo.FileName == fileName)
                    {
                        appSettings.FileUsages.Remove(fileUsageInfo);
                        break;
                    }

                listBox.Items.RemoveAt(prevIndex);

                if (listBox.Items.Count == prevIndex)
                    prevIndex--;

                if (prevIndex != -1)
                    listBox.SelectedIndex = prevIndex;

            }
                
        }

        private void removeAllButton_Click(object sender, EventArgs e)
        {

            // Ask the user

            DialogResult r = MessageBox.Show(
                    "Really clear the list?", "Removing all records", MessageBoxButtons.YesNoCancel);

            if (r == System.Windows.Forms.DialogResult.Yes)
            {
                appSettings.FileUsages.Clear();
                listBox.Items.Clear();
            }

        }
    }
}
