using System;
using System.Drawing;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class SettingsForm : Form
    {
        private string prevFont;
        private int prevSize;

        public ParallelTextControl pTC = null;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            highlightFirstWordsCheckBox.Checked = pTC.HighlightFirstWords;
            highlightFragmentsCheckBox.Checked = pTC.HighlightFragments;

            switch (pTC.ReadingMode)
            {
                case FileUsageInfo.NormalMode:
                    readingModeComboBox.SelectedIndex = 0;
                    break;
                case FileUsageInfo.AlternatingMode:
                    readingModeComboBox.SelectedIndex = 1;
                    break;
                case FileUsageInfo.AdvancedMode:
                    readingModeComboBox.SelectedIndex = 2;
                    break;
            }

            alternatingColorSchemeComboBox.SelectedIndex = pTC.AlternatingColorScheme;

            prevFont = pTC.textFont.Name;
            var currentFontSize = pTC.textFont.Size;

            var newTrack = (int)((currentFontSize - 8) / 16 * 1000);

            fontSizeTrackBar.Value = newTrack;

            prevSize = newTrack;

            var idx = -1;
            var foundIdx = -1;
            foreach (var ff in FontFamily.Families)
            {
                idx++;
                fontsComboBox.Items.Add(ff);
                if (ff.Name == prevFont)
                    foundIdx = idx;
            }

            fontsComboBox.DisplayMember = "Name";
            fontsComboBox.ValueMember = "Name";

            if (foundIdx != -1)
                fontsComboBox.SelectedIndex = foundIdx;

            fontNameLabel.Text = prevFont;
            brightnessBar.Value = (int)(pTC.Brightness * 1000);

        }

        private void brightnessBar_Scroll(object sender, EventArgs e)
        {
            pTC.Brightness = (float)brightnessBar.Value / 1000;
            pTC.RenderPairs(true);
        }

        private void fontsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFont();
        }

        private void UpdateFont()
        {
            var ff = (FontFamily)fontsComboBox.Items[fontsComboBox.SelectedIndex];

            if (!ff.IsStyleAvailable(FontStyle.Regular))
                return;

            if (prevFont == ff.Name && prevSize == fontSizeTrackBar.Value)
                return;

            fontNameLabel.Text = ff.Name;

            pTC.SetFont(
                new Font(ff.Name, (float)fontSizeTrackBar.Value * 16 / 1000 + 8),
                new Font(ff.Name, (float)fontSizeTrackBar.Value * 16 / 1000 + 8, FontStyle.Italic));

            prevFont = ff.Name;
            prevSize = fontSizeTrackBar.Value;
        }

        private void fontSizeTrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateFont();
        }

        private void highlightFirstWordsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            pTC.HighlightFirstWords = highlightFirstWordsCheckBox.Checked;

            pTC.RenderPairs(true);
        }

        private void highlightFragmentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            pTC.HighlightFragments= highlightFragmentsCheckBox.Checked;

            pTC.RenderPairs(true);
        }

        private void SettingsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Close();
        }

        private void readingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pTC.ChangeReadingMode(readingModeComboBox.SelectedIndex);
        }

        private void alternatingColorSchemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (alternatingColorSchemeComboBox.SelectedIndex)
            {
                case 0:
                    pTC.AlternatingColorScheme = FileUsageInfo.AlternatingColorScheme_BlackGreen;
                    break;
                case 1:
                    pTC.AlternatingColorScheme = FileUsageInfo.AlternatingColorScheme_GreenBlack;
                    break;
            }

            pTC.RenderPairs(true);
        }

    }
}
