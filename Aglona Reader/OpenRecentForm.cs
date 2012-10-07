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
    }
}
