using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;



namespace AglonaReader
{
    
    public partial class MainForm : Form
    {

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

            if (appSettings != null && appSettings.fileUsages.Count > 0)
            {
                FileUsageInfo f = appSettings.fileUsages[0];
                pTC.reversed = f.reversed;
                pTC.SetSplitterPositionByRatio(f.splitterRatio);
                pTC.HighlightedPair = f.pair;
                pTC.currentPair = f.topPair;
                pTC.pText.Load(f.fileName);
                pTC.FindNaturalDividers(0);
                Recompute();
            }
            else
                pTC.SetSplitterPositionByRatio(0.5F);


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
                // Let's check whether the cursor points to a word
                
                // Compute current line

                int line = (e.Y - pTC.vMargin) / pTC.lineHeight;

                // Let's see what we've got on this line

                int word_x = -1;

                ScreenWord found_word = pTC.WordAfterCursor(line, e.X);

                if (found_word != null)
                {
                    word_x = found_word.x;
                }

                if (mouse_text_line != line || mouse_text_x != word_x)
                {

                    

                    if (found_word == null
                        || pTC.HighlightedPair != -1 && found_word.pair != pTC.pText.textPairs[pTC.HighlightedPair])
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
                        pTC.naturalDividerPosition1 = pTC.naturalDividerPosition1_w.pos;
                        pTC.SetNippingFrameByScreenWord(1, pTC.naturalDividerPosition1_w);
                        (pTC.nippingFrame.f1 as Frame).pen = pTC.correctedPen;
                        pTC.side1Set = true;
                    }
                    else
                    {
                        pTC.naturalDividerPosition2_w = pTC.mouse_text_currentword.next;
                        pTC.naturalDividerPosition2 = pTC.naturalDividerPosition2_w.pos;
                        pTC.SetNippingFrameByScreenWord(2, pTC.naturalDividerPosition2_w);
                        (pTC.nippingFrame.f2 as Frame).pen = pTC.correctedPen;
                        pTC.side2Set = true;
                    }

                    if (pTC.side1Set && pTC.side2Set)
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
            pTC.Recompute();
        }

        private void pTC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {

                if (pTC.HighlightedPair == pTC.pText.Number() - 1)
                {
                    if (pTC.currentPair != pTC.HighlightedPair)
                    {
                        pTC.currentPair = pTC.HighlightedPair;
                        pTC.nippedPair = null;
                        pTC.PrepareScreen();
                        pTC.RenderPairs();
                        pTC.FindNaturalDividersOnScreen(0);
                        pTC.Render();
                    }
                    return;
                }

                pTC.HighlightedPair++;

                pTC.FindNaturalDividers(0);
                pTC.nippedPair = null;

                TextPair p = pTC.pText.textPairs[pTC.HighlightedPair];

                if (p.renderedInfo1.line2 == -1
                    || p.renderedInfo1.line2 >= pTC.linesOnScreen - 1
                    || p.renderedInfo2.line2 == -1
                    || p.renderedInfo2.line2 >= pTC.linesOnScreen - 1)
                {
                    pTC.currentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                }

                pTC.FindNaturalDividersOnScreen(0);

                pTC.Render();
            }

            else if (e.KeyData == Keys.Up)
            {
                if (pTC.HighlightedPair == 0)
                    return;

                TextPair prev_p = pTC.pText.textPairs[pTC.HighlightedPair];

                pTC.HighlightedPair--;

                pTC.FindNaturalDividers(0);
                pTC.nippedPair = null;

                TextPair p = pTC.pText.textPairs[pTC.HighlightedPair];

                if (prev_p.renderedInfo1.line1 == 0 && (prev_p.renderedInfo1.x1 == 0 || p.height > 0)
                    || prev_p.renderedInfo2.line1 == 0 && (prev_p.renderedInfo2.x1 == 0 || p.height > 0))
                {
                    pTC.currentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                }

                pTC.FindNaturalDividersOnScreen(0);
                pTC.Render();
            }

            else if (e.KeyData == (Keys.Control | Keys.Up))
            {
                if (pTC.HighlightedPair > 0)
                {
                    if (pTC.currentPair == pTC.HighlightedPair)
                        pTC.currentPair--;

                    pTC.MergePairs(pTC.HighlightedPair - 1);
                    pTC.HighlightedPair--;
                    pTC.nippedPair = null;
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
                int n = pTC.pText.Number();
                if (n == 0)
                    return;
                if (pTC.HighlightedPair != n - 1)
                {
                    pTC.HighlightedPair = n - 1;
                    pTC.currentPair = pTC.HighlightedPair;
                    pTC.nippedPair = null;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                    pTC.FindNaturalDividers(0);
                    pTC.FindNaturalDividersOnScreen(0);
                    pTC.Render();
                }


            }
            
        }

        private void ChangeNatural(byte screen_side, bool inc)
        {
            TextPair p = pTC.pText.textPairs[pTC.HighlightedPair];

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
                    pTC.FindNaturalDividersOnScreen(1);
                }

            if (side == 0 || side == 2)
                if (inc || p.GetRecommendedNatural(2) > 0)
                {
                    if (inc)
                        p.IncRecommendedNatural(2);
                    else
                        p.DecRecommendedNatural(2);
                    pTC.FindNaturalDividers(2);
                    pTC.FindNaturalDividersOnScreen(2);
                }

            pTC.Render();

        }

        private void EditCurrentPair()
        {
            pTC.EditCurrentPair();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTextForm iF = new ImportTextForm();
            iF.pText = pTC.pText;
            iF.ShowDialog();

            pTC.FindNaturalDividers(0);
            Recompute();

        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BookInfoForm f = new BookInfoForm();
            f.pTC = pTC;
            f.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveBook(false);
        }

        private void SaveBook(bool askForFileName)
        {
            string fileName;

            if (pTC.pText.fileName == "" || askForFileName)
            {
                // Ask for the file name
                SaveFileDialog d = new SaveFileDialog();
                d.Filter = "XML files (*.xml)|*.xml";
                d.RestoreDirectory = true;

                if (d.ShowDialog() != DialogResult.OK)
                    return;
                fileName = d.FileName;
            }
            else
                fileName = pTC.pText.fileName;

            pTC.pText.Save(fileName);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string fileName;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.AutoUpgradeEnabled = true;
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                pTC.pText = new ParallelText();
                mouse_text_line = -1;
                pTC.currentPair = 0;
                pTC.HighlightedPair = 0;
                pTC.pText.Load(fileName);
                pTC.FindNaturalDividers(0);
                Recompute();

                // Add a new FileUsage

                FileUsageInfo f = new FileUsageInfo();
                f.fileName = fileName;

                appSettings.fileUsages.Insert(0, f);

            }
        }

        private void SaveAppSettings()
        {
            if (appSettings.fileUsages.Count > 0)
            {
                FileUsageInfo f = appSettings.fileUsages[0];
                f.pair = pTC.HighlightedPair;
                f.topPair = pTC.currentPair;
                f.reversed = pTC.reversed;
                f.splitterRatio = pTC.SplitterRatio;
            }

            Properties.Settings.Default.AppSettings = appSettings;
            Properties.Settings.Default.Save();
        }

        private void pTC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                pTC.NipHighlightedPair();
                pTC.mouse_text_currentword = null;

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

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAppSettings();
        }

    }

    public class DB_Common_Row : DB_Row
    {
        public TextPair textPair;

        public DB_Common_Row(TextPair _textPair, string _w, int _l, int _x, int _x2, int _pos)
            : base(_w, _l, _x, _x2, _pos)
        {
            textPair = _textPair;

        }
            
    }

    
    public class FileUsageInfo
    {
        public string fileName;
        public int pair;
        public int topPair;
        public bool reversed;
        public float splitterRatio;
        public bool editMode;
    }

    public class AppSettings
    {
        public List<FileUsageInfo> fileUsages;

        public AppSettings()
        {
            fileUsages = new List<FileUsageInfo>();
        }
    }

    


}
