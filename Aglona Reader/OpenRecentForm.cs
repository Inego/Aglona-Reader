using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class OpenRecentForm : Form
    {

        private AppSettings appSettings;
        private string result;

        public AppSettings AppSettings
        {
            get
            {
                return appSettings;
            }

            set
            {
                appSettings = value;
            }
        }

        public string Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        public OpenRecentForm()
        {
            InitializeComponent();
        }

        private void OpenRecentForm_Shown(object sender, EventArgs e)
        {

            foreach (FileUsageInfo fileUsageInfo in AppSettings.FileUsages)
                listBox.Items.Add(fileUsageInfo.FileName);

            if (listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;

            Result = "";

        }

        private void formOKButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
                Result = (string) listBox.Items[listBox.SelectedIndex];

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
                    Properties.Resources.RECENT_FILE_REMOVE_CONFIRM_TEXT, "Removing a recent file record", MessageBoxButtons.YesNoCancel);

            if (r == System.Windows.Forms.DialogResult.Yes)
            {
                // 1. Remove the corresponding fileusage info

                int prevIndex = listBox.SelectedIndex;

                string fileName = (string)listBox.Items[prevIndex];

                foreach (FileUsageInfo fileUsageInfo in AppSettings.FileUsages)
                    if (fileUsageInfo.FileName == fileName)
                    {
                        AppSettings.FileUsages.Remove(fileUsageInfo);
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
                AppSettings.FileUsages.Clear();
                listBox.Items.Clear();
            }

        }
    }
}
