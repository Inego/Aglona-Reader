using System;
using System.Windows.Forms;
using AglonaReader.Properties;

namespace AglonaReader
{
    public partial class OpenRecentForm : Form
    {
        public AppSettings AppSettings { get; set; }

        public string Result { get; private set; }

        public OpenRecentForm()
        {
            InitializeComponent();
        }

        private void OpenRecentForm_Shown(object sender, EventArgs e)
        {
            foreach (var fileUsageInfo in AppSettings.FileUsages)
                listBox.Items.Add(fileUsageInfo.FileName);

            var selectedIndex = listBox.Items.Count - 1;
            if (selectedIndex > 1) selectedIndex = 1;
            
            listBox.SelectedIndex = selectedIndex;

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

            var r = MessageBox.Show(
                    Resources.RECENT_FILE_REMOVE_CONFIRM_TEXT, "Removing a recent file record", MessageBoxButtons.YesNoCancel);

            if (r != DialogResult.Yes) return;
            
            // 1. Remove the corresponding file usage info

            var prevIndex = listBox.SelectedIndex;

            var fileName = (string)listBox.Items[prevIndex];

            foreach (var fileUsageInfo in AppSettings.FileUsages)
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

        private void removeAllButton_Click(object sender, EventArgs e)
        {
            // Ask the user

            var r = MessageBox.Show(
                    "Really clear the list?", "Removing all records", MessageBoxButtons.YesNoCancel);

            if (r != DialogResult.Yes) return;
            
            AppSettings.FileUsages.Clear();
            listBox.Items.Clear();
        }
    }
}
