using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


[assembly: CLSCompliant(true)]
namespace AglonaReader
{
    
    public partial class MainForm : Form
    {

        bool newBook;

        AppSettings appSettings;

        byte opState;
        
        private int mouse_text_line = -1;
        private int mouse_text_x = -1;
        

        public MainForm()
        {
            InitializeComponent();
            opState = 0;
            this.WindowState = FormWindowState.Maximized;

            
        }
        
        private void MainForm_Resize(object sender, EventArgs e)
        {
            pTC.SetSplitterPositionByRatio();
            Recompute();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            appSettings = Properties.Settings.Default.AppSettings;

            if (appSettings == null)
                appSettings = new AppSettings();


            pTC.HighlightFragments = appSettings.HighlightFragments;
            pTC.HighlightFirstWords = appSettings.HighlightFirstWords;
            pTC.Brightness = appSettings.Brightness;

            if (appSettings.FileUsages.Count > 0)
            {
                LoadSettingsFromFileUsageInfo(appSettings.FileUsages[0], true);
                newBook = false;
            }
            else
            {
                pTC.SetSplitterPositionByRatio(0.5F);
                newBook = true;                
            }

            highlightFramgentsToolStripMenuItem.Checked = appSettings.HighlightFragments;
            highlightFirstWordsToolStripMenuItem.Checked = appSettings.HighlightFirstWords;
        }

        private void LoadSettingsFromFileUsageInfo(FileUsageInfo f, bool load)
        {
            
            pTC.reversed = f.Reversed;
            reverseToolStripMenuItem.Checked = pTC.reversed;
            pTC.SetSplitterPositionByRatio(f.SplitterRatio);

            if (load)
                pTC.pText.Load(f.FileName);
            pTC.Modified = false;

            if (pTC.Number > 0)
            {
                if (f.Pair >= pTC.Number)
                    pTC.HighlightedPair = pTC.Number - 1;
                else
                    pTC.HighlightedPair = f.Pair;

                if (f.TopPair >= pTC.Number)
                    pTC.currentPair = pTC.Number - 1;
                else
                    pTC.currentPair = f.TopPair;

                pTC.FindNaturalDividers(0);
                Recompute();
            }
        }

        private bool XonSplitter(int x)
        {
            return (x >= pTC.SplitterPosition && x < pTC.SplitterPosition + pTC.SplitterWidth);
        }

        private void parallelTextControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X == pTC.lastMouseX && e.Y == pTC.lastMouseY)
                return;

            pTC.lastMouseX = e.X;
            pTC.lastMouseY = e.Y;

            if (XonSplitter(e.X) || opState == 1)
                Cursor = Cursors.VSplit;
            else
                Cursor = Cursors.Default;

            if (opState == 1)
            {
                // Move splitter

                int newSplitterPosition = e.X - pTC.splitterMoveOffset;

                if (newSplitterPosition != pTC.SplitterPosition)
                {
                    
                    pTC.SplitterPosition = newSplitterPosition;
                    pTC.SetSplitterRatioByPosition();
                    
                    Recompute();
                }
            }

            else if (opState == 0)
            {
                // Let'word check whether the cursor points to a word
                
                // Compute current line

                int line = (e.Y - pTC.vMargin) / pTC.lineHeight;

                // Let'word see what we've got on this line

                int word_x = -1;

                ScreenWord found_word = pTC.WordAfterCursor(line, e.X);

                if (found_word != null)
                {
                    word_x = found_word.x;
                }

                if (mouse_text_line != line || mouse_text_x != word_x)
                {

                    

                    if (found_word == null
                        || pTC.HighlightedPair != -1 && found_word.pair != pTC[pTC.HighlightedPair])
                    {
                        pTC.mouse_text_currentword = null;
                    }
                    else
                    {

                        pTC.mouse_text_currentword = found_word;
                    }

                    pTC.Render();

                    mouse_text_line = line;
                    mouse_text_x = word_x;

                }

            }

        }

        private void parallelTextControl_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            if (pTC.mouse_text_currentword != null)
            {
                pTC.mouse_text_currentword = null;
                pTC.Render();
            }
        }

        private void parallelTextControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (opState == 0)
            {
                if (XonSplitter(e.X))
                {
                    opState = 1;
                    pTC.splitterMoveOffset = e.X - pTC.SplitterPosition;
                }

            }
        }

        private void parallelTextControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (opState == 1)
                opState = 0;
            else
            {
                if (pTC.mouse_text_currentword != null && pTC.mouse_text_currentword.next != null)
                {

                    if (pTC.mouse_text_currentword.side == 1)
                    {
                        pTC.naturalDividerPosition1_w = pTC.mouse_text_currentword.next;
                        pTC.NaturalDividerPosition1 = pTC.naturalDividerPosition1_w.pos;
                        pTC.SetNippingFrameByScreenWord(1, pTC.naturalDividerPosition1_w);
                        (pTC.nippingFrame.F1 as Frame).pen = pTC.correctedPen;
                        pTC.Side1Set = true;
                    }
                    else
                    {
                        pTC.naturalDividerPosition2_w = pTC.mouse_text_currentword.next;
                        pTC.NaturalDividerPosition2 = pTC.naturalDividerPosition2_w.pos;
                        pTC.SetNippingFrameByScreenWord(2, pTC.naturalDividerPosition2_w);
                        (pTC.nippingFrame.F2 as Frame).pen = pTC.correctedPen;
                        pTC.Side2Set = true;
                    }

                    if (pTC.Side1Set && pTC.Side2Set)
                        pTC.NipHighlightedPair();
                    else
                    {
                        pTC.Render();
                        pTC.HighlightWord(pTC.mouse_text_currentword, Color.LightSkyBlue);
                    }

                }

            }
        }

        private void reverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.reversed = reverseToolStripMenuItem.Checked;
            pTC.ComputeSideCoordinates();
            Recompute();
        }

        private void Recompute()
        {
            mouse_text_line = -1;
            pTC.mouse_text_currentword = null;
            pTC.ProcessLayoutChange();
        }

        private void pTC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
                ProcessKeyDown();

            else if (e.KeyData == Keys.Up)
                ProcessKeyUp();

            else if (e.KeyData == (Keys.Control | Keys.Up))
            {
                if (pTC.HighlightedPair > 0)
                {
                    if (pTC.currentPair == pTC.HighlightedPair)
                        pTC.currentPair--;

                    pTC.MergePairs(pTC.HighlightedPair - 1);
                    pTC.HighlightedPair--;
                    pTC.PairChanged(pTC.HighlightedPair);
                }
            }

            else if (e.KeyData == (Keys.Control | Keys.Right))
                ChangeNatural(1, true);

            else if (e.KeyData == (Keys.Control | Keys.Left))
                ChangeNatural(1, false);

            else if (e.KeyData == (Keys.Alt| Keys.Right))
                ChangeNatural(2, true);

            else if (e.KeyData == (Keys.Alt| Keys.Left))
                ChangeNatural(2, false);

            else if (e.KeyData == Keys.Right)
                ChangeNatural(0, true);

            else if (e.KeyData == Keys.Left)
                ChangeNatural(0, false);

            else if (e.KeyData == (Keys.Control | Keys.End))
            {
                int n = pTC.Number;
                if (n == 0)
                    return;
                if (pTC.HighlightedPair != n - 1)
                {
                    pTC.HighlightedPair = n - 1;
                    pTC.currentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                    pTC.FindNaturalDividers(0);
                    pTC.FindNaturalDividersScreen(0);
                    pTC.Render();
                }

            }

            else if (e.KeyData == (Keys.Control | Keys.Home))
            {
                int n = pTC.Number;
                if (n == 0)
                    return;
                
                    pTC.HighlightedPair = 0;
                    pTC.currentPair = 0;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                    pTC.FindNaturalDividers(0);
                    pTC.FindNaturalDividersScreen(0);
                    pTC.Render();

            }

            else if (e.KeyData == (Keys.Control | Keys.OemOpenBrackets))
            {
                if (pTC.Brightness > 0.6)
                {
                    pTC.Brightness -= 0.005;
                    pTC.RenderPairs();
                    pTC.Render();
                }
            }

            else if (e.KeyData == (Keys.Control | Keys.OemCloseBrackets))
            {
                if (pTC.Brightness < 0.995)
                {
                    pTC.Brightness += 0.005;
                    pTC.RenderPairs();
                    pTC.Render();
                }
            }
            
        }

        private void ProcessKeyUp()
        {
            if (pTC.HighlightedPair == 0)
                return;

            TextPair prev_p = pTC[pTC.HighlightedPair];

            pTC.HighlightedPair--;

            pTC.FindNaturalDividers(0);

            TextPair p = pTC[pTC.HighlightedPair];

            if (prev_p.renderedInfo1.line1 == 0 && (prev_p.renderedInfo1.x1 == 0 || p.height > 0)
                || prev_p.renderedInfo2.line1 == 0 && (prev_p.renderedInfo2.x1 == 0 || p.height > 0))
            {
                pTC.currentPair = pTC.HighlightedPair;
                pTC.PrepareScreen();
                pTC.RenderPairs();
            }

            pTC.FindNaturalDividersScreen(0);
            pTC.Render();
        }

        private void ProcessKeyDown()
        {
            if (pTC.HighlightedPair == pTC.Number - 1)
            {
                if (pTC.currentPair != pTC.HighlightedPair)
                {
                    pTC.currentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                    pTC.FindNaturalDividersScreen(0);
                    pTC.Render();
                }
                return;
            }

            pTC.HighlightedPair++;

            pTC.FindNaturalDividers(0);

            TextPair p = pTC[pTC.HighlightedPair];

            if ((p.renderedInfo1.line2 == -1
                || p.renderedInfo1.line2 >= pTC.NumberOfScreenLines - 1
                || p.renderedInfo2.line2 == -1
                || p.renderedInfo2.line2 >= pTC.NumberOfScreenLines - 1)
                && !pTC[pTC.HighlightedPair].IsBig())
            {
                pTC.currentPair = pTC.HighlightedPair;
                pTC.PrepareScreen();
                pTC.RenderPairs();
            }

            pTC.FindNaturalDividersScreen(0);

            pTC.Render();
        }

        private void ChangeNatural(byte screen_side, bool inc)
        {
            TextPair p = pTC[pTC.HighlightedPair];

            byte side;

            if (screen_side == 0)
                side = 0;
            else if (pTC.reversed)
                side = (byte) (3 - screen_side);
            else
                side = screen_side;

            if (side == 0 || side == 1)
                if (inc || p.GetRecommendedNatural(1) > 0)
                {
                    if (inc)
                        p.IncRecommendedNatural(1);
                    else
                        p.DecRecommendedNatural(1);
                    pTC.FindNaturalDividers(1);
                    pTC.FindNaturalDividersScreen(1);
                }

            if (side == 0 || side == 2)
                if (inc || p.GetRecommendedNatural(2) > 0)
                {
                    if (inc)
                        p.IncRecommendedNatural(2);
                    else
                        p.DecRecommendedNatural(2);
                    pTC.FindNaturalDividers(2);
                    pTC.FindNaturalDividersScreen(2);
                }

            pTC.Render();

        }

        private void EditCurrentPair()
        {
            pTC.EditCurrentPair();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportTextForm importTextForm = new ImportTextForm())
            {
                importTextForm.pText = pTC.pText;
                importTextForm.ShowDialog();
                importTextForm.Dispose();
            }

            pTC.FindNaturalDividers(0);
            Recompute();

        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (BookInfoForm f = new BookInfoForm())
            {
                f.ParallelTC = pTC;
                f.ShowDialog();
                f.Dispose();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveBook(false);
        }

        private void SaveBook(bool askForFileName)
        {
            string fileName;

            if (newBook || askForFileName)
            {
                // Ask for the file name
                using (SaveFileDialog d = new SaveFileDialog())
                {
                    d.Filter = "XML files (*.xml)|*.xml";
                    d.RestoreDirectory = true;
                    DialogResult dialogResult = d.ShowDialog();

                    if (dialogResult != DialogResult.OK)
                    {
                        d.Dispose();
                        return;
                    }

                    fileName = d.FileName;

                    RetrieveToTheTop(fileName);

                    newBook = false;

                    d.Dispose();
                }
            }
            else
                fileName = pTC.pText.fileName;

            pTC.pText.Save(fileName);
            pTC.Modified = false;
        }

        private bool RetrieveToTheTop(string fileName)
        {
            // Let's check whether there exists this file in the list

            for (int index = 0; index < appSettings.FileUsages.Count; index++)
            {
                if (appSettings.FileUsages[index].FileName == fileName)
                {
                    if (index != 0)
                    {
                        FileUsageInfo toMove = appSettings.FileUsages[index];
                        appSettings.FileUsages.Remove(toMove);
                        appSettings.FileUsages.Insert(0, toMove);
                    }
                    return true;
                }
            }

            FileUsageInfo fileUsageInfo = new FileUsageInfo();

            fileUsageInfo.FileName = fileName;

            appSettings.FileUsages.Insert(0, fileUsageInfo);

            return false;

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (pTC.Modified)
                if (AskToSaveModified(sender) == System.Windows.Forms.DialogResult.Cancel)
                    return;

            string fileName;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML files (*.xml)|*.xml";
                openFileDialog.RestoreDirectory = true;
                //openFileDialog.AutoUpgradeEnabled = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                    pTC.pText = new ParallelText();
                    mouse_text_line = -1;
                    
                    pTC.pText.Load(fileName);

                    if (RetrieveToTheTop(fileName))
                        LoadSettingsFromFileUsageInfo(appSettings.FileUsages[0], false);
                    else
                    {
                        pTC.currentPair = 0;
                        pTC.HighlightedPair = 0;
                    }

                    newBook = false;

                    pTC.Modified = false;
                    
                    pTC.FindNaturalDividers(0);
                    
                    Recompute();

                }

                openFileDialog.Dispose();
            }
        }

        private void SaveAppSettings()
        {
            if (appSettings.FileUsages.Count > 0 && !newBook)
            {
                FileUsageInfo f = appSettings.FileUsages[0];

                f.Pair = pTC.HighlightedPair;
                f.TopPair = pTC.currentPair;
                f.Reversed = pTC.reversed;
                f.SplitterRatio = pTC.SplitterRatio;
            }

            appSettings.HighlightFragments = pTC.HighlightFragments;
            appSettings.HighlightFirstWords = pTC.HighlightFirstWords;
            
            appSettings.Brightness = pTC.Brightness;

            Properties.Settings.Default.AppSettings = appSettings;
            Properties.Settings.Default.Save();
        }

        private void pTC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                if (pTC.NipHighlightedPair())
                    pTC.mouse_text_currentword = null;
                else
                    ProcessKeyDown();

            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCurrentPair();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveBook(true);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorTableForm f = new ColorTableForm())
                f.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pTC.Modified)
            {
                if (AskToSaveModified(sender) == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            SaveAppSettings();
        }

        DialogResult AskToSaveModified(Object sender)
        {
            DialogResult r = MessageBox.Show(
                    "Save modified book?", "The book was modified", MessageBoxButtons.YesNoCancel);

            if (r == System.Windows.Forms.DialogResult.Yes)
            {
                saveToolStripMenuItem_Click(sender, EventArgs.Empty);
                SaveAppSettings();
            }

            return r;

        }


        private void highlightFramgentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.HighlightFragments = highlightFramgentsToolStripMenuItem.Checked;
            pTC.RenderPairs();
            pTC.Render();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pTC.Modified)
                if (AskToSaveModified(sender) == System.Windows.Forms.DialogResult.Cancel)
                    return;

            pTC.CreateNewParallelBook();

            pTC.highlightedFrame.SetInvisible();
            pTC.nippingFrame.SetInvisible();

            pTC.ProcessLayoutChange();

            newBook = true;
        }

        private void highlightFirstWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.HighlightFirstWords= highlightFirstWordsToolStripMenuItem.Checked;
            pTC.RenderPairs();
            pTC.Render();
        }

    }

    public class CommonWordInfo : WordInfo
    {
        public TextPair TextPair { get; set; }

        public CommonWordInfo(TextPair textPair, string word, int line, int wordX, int wordX2, int pos)
            : base(word, line, wordX, wordX2, pos)
        {
            this.TextPair = textPair;

        }
            
    }

    
    public class FileUsageInfo
    {
        public string FileName { get; set; }
        public int Pair { get; set; }
        public int TopPair { get; set; }
        public bool Reversed { get; set; }
        public float SplitterRatio { get; set; }
        public bool EditMode { get; set; }
    }

    public class AppSettings
    {
        public bool HighlightFragments { get; set; }
        public bool HighlightFirstWords { get; set; }
        public double Brightness { get; set; }

        public Collection<FileUsageInfo> FileUsages { get; set; }

        public AppSettings()
        {
            HighlightFragments = true;
            Brightness = 0.96;
            FileUsages = new Collection<FileUsageInfo>();
        }

    }

    


}
