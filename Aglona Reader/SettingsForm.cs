using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AglonaReader
{
    public partial class SettingsForm : Form
    {

        string prevFont;
        int prevSize;


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

            prevFont = pTC.textFont.Name;
            float currentFontSize = pTC.textFont.Size;

            int newTrack = (int)((currentFontSize - 8) / 16 * 1000);

            fontSizeTrackBar.Value = newTrack;

            prevSize = newTrack;

            int idx = -1;
            int foundidx = -1;
            foreach (FontFamily ff in FontFamily.Families)
            {
                idx++;
                fontsComboBox.Items.Add(ff);
                if (ff.Name == prevFont)
                    foundidx = idx;
            }

            fontsComboBox.DisplayMember = "Name";
            fontsComboBox.ValueMember = "Name";

            if (foundidx != -1)
                fontsComboBox.SelectedIndex = foundidx;

            fontNameLabel.Text = prevFont;
            brightnessBar.Value = (int)(pTC.Brightness * 1000);

        }

        private void brightnessBar_Scroll(object sender, EventArgs e)
        {
            pTC.Brightness = (float)(brightnessBar.Value) / 1000;
            pTC.RenderPairs();
            pTC.Render();
        }

        private void fontsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFont();
        }

        private void UpdateFont()
        {

            FontFamily ff = (FontFamily)fontsComboBox.Items[fontsComboBox.SelectedIndex];

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

            pTC.RenderPairs();
            pTC.Render();
        }

        private void highlightFragmentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            pTC.HighlightFragments= highlightFragmentsCheckBox.Checked;

            pTC.RenderPairs();
            pTC.Render();
        }

        private void SettingsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Close();
        }

        private void readingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (readingModeComboBox.SelectedIndex)
            {
                case 0:
                    pTC.ReadingMode = FileUsageInfo.NormalMode;
                    break;
                case 1:
                    pTC.ReadingMode = FileUsageInfo.AlternatingMode;
                    break;
                case 2:
                    pTC.ReadingMode = FileUsageInfo.AdvancedMode;
                    break;
            }

            pTC.SetLayoutMode();
            
            if (pTC.EditMode)
            {
                pTC.SelectionSide = 0;
                pTC.SelectionFrame.Visible = false;
            }
            else
            {
                pTC.HighlightedFrame.SetVisibility(false);
                pTC.NippingFrame.SetVisibility(false);
                pTC.MouseCurrentWord = null;
            }
            
            pTC.ProcessLayoutChange(true);

        }

    }
}
