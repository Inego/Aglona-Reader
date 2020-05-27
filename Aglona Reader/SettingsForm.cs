using System;
using System.Drawing;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class SettingsForm : Form
    {
        private string prevFont;
        private int prevSize;

        public ParallelTextControl pTc = null;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            highlightFirstWordsCheckBox.Checked = pTc.HighlightFirstWords;
            highlightFragmentsCheckBox.Checked = pTc.HighlightFragments;

            onSelectionCopyToClipboardRb.Checked = pTc.SelectionAction == SelectionAction.CopyToClipboard;
            onSelectionOpenInGtRb.Checked = pTc.SelectionAction == SelectionAction.OpenInGoogleTranslate;

            translationLanguage.Text = pTc.GoogleTranslateTargetLanguage;

            switch (pTc.ReadingMode)
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

            alternatingColorSchemeComboBox.SelectedIndex = pTc.AlternatingColorScheme;

            prevFont = pTc.textFont.Name;
            var currentFontSize = pTc.textFont.Size;

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
            brightnessBar.Value = (int)(pTc.Brightness * 1000);
        }

        private void brightnessBar_Scroll(object sender, EventArgs e)
        {
            pTc.Brightness = (float)brightnessBar.Value / 1000;
            pTc.RenderPairs(true);
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

            pTc.SetFont(
                new Font(ff.Name, (float)fontSizeTrackBar.Value * 16 / 1000 + 8));

            prevFont = ff.Name;
            prevSize = fontSizeTrackBar.Value;
        }

        private void fontSizeTrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateFont();
        }

        private void highlightFirstWordsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            pTc.HighlightFirstWords = highlightFirstWordsCheckBox.Checked;

            pTc.RenderPairs(true);
        }

        private void highlightFragmentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            pTc.HighlightFragments= highlightFragmentsCheckBox.Checked;

            pTc.RenderPairs(true);
        }

        private void SettingsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Close();
        }

        private void readingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pTc.ChangeReadingMode(readingModeComboBox.SelectedIndex);
        }

        private void alternatingColorSchemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (alternatingColorSchemeComboBox.SelectedIndex)
            {
                case 0:
                    pTc.AlternatingColorScheme = FileUsageInfo.AlternatingColorSchemeBlackGreen;
                    break;
                case 1:
                    pTc.AlternatingColorScheme = FileUsageInfo.AlternatingColorSchemeGreenBlack;
                    break;
            }

            pTc.RenderPairs(true);
        }

        private void onSelectionCopyToClipboardRb_Click(object sender, EventArgs e)
        {
            pTc.SelectionAction = SelectionAction.CopyToClipboard;
        }

        private void onSelectionOpenInGtRb_Click(object sender, EventArgs e)
        {
            pTc.SelectionAction = SelectionAction.OpenInGoogleTranslate;
        }

        private void translationLanguage_TextChanged(object sender, EventArgs e)
        {
            pTc.GoogleTranslateTargetLanguage = translationLanguage.Text;
        }
    }
}
