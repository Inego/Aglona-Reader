using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;


[assembly: CLSCompliant(true)]
namespace AglonaReader
{
    
    public partial class MainForm : Form
    {

        bool newBook;

        AppSettings appSettings;

        byte opState;
        private bool startedXMoving = false;
        private bool startedYMoving = false;
        private bool justJumped = false;
        private int prevHorizontalDistance = 0;

        

        FindForm findForm;
        

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.RButton | Keys.ShiftKey))
                return true;
            
            return base.ProcessDialogKey(keyData);
        }

        private void pTC_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                ProcessKeyUp();
            else if (e.Delta < 0)
                ProcessKeyDown();
            
        }

        public MainForm()
        {
            InitializeComponent();

            this.pTC.MouseWheel += new MouseEventHandler(this.pTC_MouseWheel);

            opState = 0;
            this.WindowState = FormWindowState.Maximized;
            
        }
        
        private void MainForm_Resize(object sender, EventArgs e)
        {
            pTC.ResizeBufferedGraphic();
            pTC.SetSplitterPositionByRatio();
            Recompute();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UpdateRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateRequired = false;
            }

            appSettings = Properties.Settings.Default.AppSettings;

            if (appSettings == null)
                appSettings = new AppSettings();
            else
            {
                if (string.IsNullOrEmpty(appSettings.FontName))
                    appSettings.FontName = "Times New Roman";
                if (appSettings.FontSize == 0)
                    appSettings.FontSize = 18.0F;
            }

            pTC.HighlightFragments = appSettings.HighlightFragments;
            pTC.HighlightFirstWords = appSettings.HighlightFirstWords;
            pTC.Brightness = appSettings.Brightness;
            
            pTC.SetFont(
                new Font(appSettings.FontName, appSettings.FontSize),
                new Font(appSettings.FontName, appSettings.FontSize, FontStyle.Italic));

            if (appSettings.FileUsages.Count > 0)
            {
                LoadSettingsFromFileUsageInfo(appSettings.FileUsages[0], true);
                newBook = false;
            }
            else
            {
                pTC.SetSplitterPositionByRatio(0.5F);
                newBook = true;
                SetEditMode(true);
            }

        }

        private void SetEditMode(bool p)
        {
            pTC.EditMode = p;
            editModeToolStripMenuItem.Checked = p;
        }

        private void LoadSettingsFromFileUsageInfo(FileUsageInfo f, bool load)
        {

            if (load)
            {
                pTC.PText.Load(f.FileName);
                UpdateWindowTitle();
            }

            if (f.SplitterRatio == 0)
                f.SplitterRatio = 0.5F;

            pTC.Reversed = f.Reversed;
            reverseToolStripMenuItem.Checked = pTC.Reversed;

            pTC.ReadingMode = f.ReadingMode;

            SetEditMode(f.EditMode);

            pTC.SetLayoutMode();

            pTC.SetSplitterPositionByRatio(f.SplitterRatio);
    
            pTC.Modified = false;

            if (pTC.Number > 0)
            {
                if (f.Pair >= pTC.Number)
                    pTC.HighlightedPair = pTC.Number - 1;
                else
                    pTC.HighlightedPair = f.Pair;

                if (f.TopPair >= pTC.Number)
                    pTC.CurrentPair = pTC.Number - 1;
                else
                    pTC.CurrentPair = f.TopPair;

                pTC.FindFirstNaturalDividers();
                Recompute();
            }

            UpdateStatusBar(true);

        }

        bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }

        private void UpdateWindowTitle()
        {
            string cpt;

            if (IsNullOrWhiteSpace(pTC.PText.Title1))
                cpt = "<No title>";
            else
                cpt = pTC.PText.Title1;

            if (!IsNullOrWhiteSpace(pTC.PText.Lang1) && !IsNullOrWhiteSpace(pTC.PText.Lang2))
                cpt += " [" + pTC.PText.Lang1 + "-" + pTC.PText.Lang2 + "]";

            if (IsNullOrWhiteSpace(cpt))
                Text = "Aglona Reader";
            else
                Text = cpt + " - Aglona Reader";

        }

        private bool XonSplitter(int x)
        {
            return (pTC.LayoutMode == ParallelTextControl.LayoutMode_Normal
                && x >= pTC.SplitterPosition
                && x < pTC.SplitterPosition + pTC.SplitterWidth);
        }

        private void parallelTextControl_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.X == pTC.LastMouseX && e.Y == pTC.LastMouseY)
                return;

            pTC.LastMouseX = e.X;
            pTC.LastMouseY = e.Y;

            if (XonSplitter(pTC.LastMouseX) || opState == 1)
                Cursor = Cursors.VSplit;

            else if (pTC.MousePressed && pTC.EditMode)
            {

                if (!startedXMoving)
                {
                    int distanceY = pTC.verticalStartingPosition - e.Y;
                    if (distanceY >= ParallelTextControl.horizontalMouseStep)
                    {
                        startedYMoving = true;
                        Cursor = Cursors.PanNorth;
                    }
                    else
                    {
                        startedYMoving = false;
                        Cursor = Cursors.Default;
                    }
                }

                if (!startedYMoving)
                {
                    int distance = (e.X - pTC.horizontalStartingPosition) / ParallelTextControl.horizontalMouseStep;

                    if (distance != prevHorizontalDistance)
                    {

                        Cursor = Cursors.SizeWE;

                        startedXMoving = true;

                        TextPair p = pTC[pTC.HighlightedPair];

                        bool forward = (distance > prevHorizontalDistance);

                        //pTC.DebugString = distance.ToString() + " " + prevHorizontalDistance.ToString() + " " + forward;

                        prevHorizontalDistance = distance;

                        byte needToRender = 0;

                        bool MovedRec1 = pTC.NextRecommended(1, forward);
                        bool MovedRec2 = pTC.NextRecommended(2, forward);

                        if (MovedRec1)
                        {
                            needToRender += 1;
                            pTC.Side1Set = true;
                        }

                        if (MovedRec2)
                        {
                            needToRender += 2;
                            pTC.Side2Set = true;
                        }

                        if (needToRender != 0)
                        {
                            if (needToRender == 3)
                                needToRender = 0;

                            pTC.FindNaturalDividersScreen(needToRender);
                            pTC.Render();
                        }
                    }

                }
                
            }
                else
                Cursor = Cursors.Default;
            

            if (opState == 1)
            {
                // Move splitter

                int newSplitterPosition = pTC.LastMouseX - pTC.SplitterMoveOffset;

                if (newSplitterPosition != pTC.SplitterPosition)
                {
                    
                    pTC.SplitterPosition = newSplitterPosition;
                    pTC.SetSplitterRatioByPosition();
                    
                    Recompute();
                }
            }

            else if (opState == 0)
                pTC.ProcessMousePosition(false, true);
        }

        
        private void parallelTextControl_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;

            pTC.LastMouseX = -1;
            pTC.LastMouseY = -1;

            pTC.ProcessMousePosition(true, true);

        }

        private void parallelTextControl_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            pTC.MousePressed = true;

            if (opState == 0)
            {
                if (XonSplitter(e.X))
                {
                    opState = 1;
                    pTC.SplitterMoveOffset = e.X - pTC.SplitterPosition;
                }

                else if (pTC.EditMode)
                {

                    TextPair p;

                    if (pTC.MouseCurrentWord == null && pTC.mouse_text_word != null
                        && pTC.HighlightedPair != pTC.mouse_text_word.PairIndex)
                    {
                        // Set focus to this pair
                        pTC.HighlightedPair = pTC.mouse_text_word.PairIndex;

                        pTC.Side1Set = false;
                        pTC.Side2Set = false;

                        p = pTC[pTC.HighlightedPair];

                        if (pTC.NotFitOnScreen(p))
                        {
                            pTC.CurrentPair = pTC.HighlightedPair;
                            pTC.PrepareScreen();
                            pTC.RenderPairs();
                            
                        }

                        pTC.FindFirstNaturalDividers();
                        pTC.FindNaturalDividersScreen(0);
                        pTC.ProcessMousePosition(true, true);

                        justJumped = true;

                    }


                    pTC.horizontalStartingPosition = e.X;
                    pTC.verticalStartingPosition = e.Y;

                    p = pTC[pTC.HighlightedPair];

                    startedXMoving = false;
                    startedYMoving = false;

                }

                else
                {
                    // In view mode mouse down begins selection

                    if (pTC.mouse_text_word == null)
                        return;

                    if (pTC.SelectionFinished)
                    {
                        pTC.SelectionFinished = false;
                        pTC.SelectionSide = pTC.mouse_text_word.Side;
                        pTC.Selection1Pair = pTC.mouse_text_word.PairIndex;
                        pTC.Selection2Pair = pTC.mouse_text_word.PairIndex;
                        pTC.Selection1Position = pTC.mouse_text_word.Pos;
                        pTC.Selection2Position = pTC.mouse_text_word.Pos;
                        pTC.SelectionFrame.Visible = true;
                        pTC.SelectionFrame.Side = pTC.mouse_text_word.Side;
                        pTC.SelectionFrame.Line1 = pTC.mouse_text_word.Line;
                        pTC.SelectionFrame.Line2 = pTC.mouse_text_word.Line;
                        pTC.SelectionFrame.X1 = pTC.mouse_text_word.FX1;
                        pTC.SelectionFrame.X2 = pTC.mouse_text_word.FX2;
                        pTC.Render();
                    }

                }

            }
            
        }

        private void parallelTextControl_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (pTC.EditMode)
                    SeparateCurrentPair();
                else
                {
                    if (pTC.LayoutMode == ParallelTextControl.LayoutMode_Advanced)
                        pTC.SwitchAdvancedShowPopups();
                }

                return;
            }

            if (!pTC.MousePressed)
                return;

            pTC.MousePressed = false;


            if (opState == 1)
                opState = 0;
            else
            {
                if (pTC.EditMode)
                {
                    if (startedYMoving)
                        MergeWithPrevious();
                    else if (startedXMoving)
                    {
                        Cursor = Cursors.Default;
                        startedXMoving = false;
                        prevHorizontalDistance = 0;
                    }
                    else
                        MouseUpInEditMode();
                }

                else
                    pTC.SelectionFinished = true;
            }
        }

        private void MouseUpInEditMode()
        {

            bool needToRender = false;
            bool needToUpdateStatusBar = false;
            bool needToNip = false;

            if (pTC.MouseCurrentWord != null && pTC.MouseCurrentWord.Next != null)
            {

                if (pTC.MouseCurrentWord.Side == 1)
                {
                    if (pTC.NaturalDividerPosition1W == pTC.MouseCurrentWord.Next && !justJumped)
                        needToNip = true;
                    else
                    {
                        pTC.NaturalDividerPosition1W = pTC.MouseCurrentWord.Next;
                        pTC.NaturalDividerPosition1 = pTC.NaturalDividerPosition1W.Pos;
                        pTC.SetNippingFrameByScreenWord(1, pTC.NaturalDividerPosition1W);
                        (pTC.NippingFrame.F1 as Frame).FramePen = pTC.CorrectedPen;
                        pTC.Side1Set = true;
                    }
                }
                else
                {
                    if (pTC.NaturalDividerPosition2W == pTC.MouseCurrentWord.Next && !justJumped)
                        needToNip = true;
                    else
                    {
                        pTC.NaturalDividerPosition2W = pTC.MouseCurrentWord.Next;
                        pTC.NaturalDividerPosition2 = pTC.NaturalDividerPosition2W.Pos;
                        pTC.SetNippingFrameByScreenWord(2, pTC.NaturalDividerPosition2W);
                        (pTC.NippingFrame.F2 as Frame).FramePen = pTC.CorrectedPen;
                        pTC.Side2Set = true;
                    }
                }


                if (pTC.Side1Set && pTC.Side2Set)
                    needToNip = true;

                if (needToNip)
                {
                    pTC.NipHighlightedPair();
                    needToUpdateStatusBar = true;
                }
                else
                    needToRender = true;

            }

            if (needToUpdateStatusBar)
                UpdateStatusBar(true);

            if (needToRender)
                pTC.Render();

            justJumped = false;
        }

        private void reverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.Reversed = reverseToolStripMenuItem.Checked;
            pTC.SetSplitterPositionByRatio(1 - pTC.SplitterRatio);
            pTC.SetLayoutMode();
            //pTC.ComputeSideCoordinates();
            Recompute();
        }

        private void Recompute()
        {
            pTC.mouse_text_line = -1;
            pTC.MouseCurrentWord = null;
            pTC.ProcessLayoutChange();
        }

        private void pTC_KeyDown(object sender, KeyEventArgs e)
        {

            //pTC.DebugString = e.KeyData.ToString();
            //pTC.Render();

            if (e.KeyData == Keys.Down)
                ProcessKeyDown();

            else if (e.KeyData == Keys.Up)
                ProcessKeyUp();

            else if (e.KeyData == (Keys.Control | Keys.Up)
                || e.KeyData == Keys.Z)
                MergeWithPrevious();

            else if (e.KeyData == (Keys.Control | Keys.Right)
                || e.KeyData == Keys.E
                || e.KeyData == Keys.D)
                ChangeNatural(1, true);

            else if (e.KeyData == (Keys.Control | Keys.Left)
                || e.KeyData == Keys.W
                || e.KeyData == Keys.S)
                ChangeNatural(1, false);

            else if (e.KeyData == (Keys.Alt | Keys.Right)
                || e.KeyData == Keys.O
                || e.KeyData == Keys.L)
                ChangeNatural(2, true);

            else if (e.KeyData == (Keys.Alt | Keys.Left)
                || e.KeyData == Keys.I
                || e.KeyData == Keys.K)
                ChangeNatural(2, false);

            else if (e.KeyData == Keys.R
                || e.KeyData == Keys.F
                || e.KeyData == (Keys.Shift | Keys.Control | Keys.Right))
                StepOneWord(1, true);

            else if (e.KeyData == Keys.Q
                || e.KeyData == Keys.A
                || e.KeyData == (Keys.Shift | Keys.Control | Keys.Left))
                StepOneWord(1, false);

            else if (e.KeyData == Keys.P
                || e.KeyData == Keys.Oem1
                || e.KeyData == (Keys.Shift | Keys.Alt | Keys.Right))
                StepOneWord(2, true);

            else if (e.KeyData == Keys.U
                || e.KeyData == Keys.J
                || e.KeyData == (Keys.Shift | Keys.Alt | Keys.Left))
                StepOneWord(2, false);

            else if (e.KeyData == Keys.Right
                || e.KeyData == Keys.Y
                || e.KeyData == Keys.H)
                ChangeNatural(0, true);

            else if (e.KeyData == Keys.Left
                || e.KeyData == Keys.T
                || e.KeyData == Keys.G)
                ChangeNatural(0, false);

            else if (e.KeyData == (Keys.Control | Keys.End))
            {
                int n = pTC.Number;
                if (n == 0)
                    return;
                if (pTC.HighlightedPair != n - 1)
                    GotoPair(n - 1);

            }

            else if (e.KeyData == (Keys.Control | Keys.Home))
            {
                int n = pTC.Number;
                if (n == 0)
                    return;

                GotoPair(0);
            }

            else if (e.KeyData == Keys.PageUp)
                ProcessPageUp();

            else if (e.KeyData == Keys.PageDown)
                ProcessPageDown();

            else if (e.KeyData == Keys.Escape)
            {
                if (!pTC.EditMode && pTC.SelectionSide != 0)
                {
                    pTC.SelectionSide = 0;
                    pTC.UpdateSelectionFrame();
                    pTC.Render();
                }
            }

            else if (!pTC.EditMode && e.KeyData == (Keys.Control | Keys.C))
                CopyToClipboard();

            else if (e.KeyData == Keys.Tab)
            {
                editModeToolStripMenuItem.Checked = !pTC.EditMode;
                editModeToolStripMenuItem_Click(null, null);
            }
            
        }

        private void StepOneWord(byte screenSide, bool forward)
        {

            byte side = pTC.Reversed ? (byte)(3 - screenSide) : screenSide;

            ScreenWord current = side == 1 ? pTC.NaturalDividerPosition1W : pTC.NaturalDividerPosition2W;

            if (current == null)
            {

                if (forward)
                {
                    current = pTC.FindFirstScreenWord(pTC.HighlightedPair, side);
                    if (current == null)
                        return;
                }
                else
                {
                    current = pTC.FindLastScreenWord(pTC.HighlightedPair, side);
                    if (current == null)
                        return;
                    goto CurrentFound;
                }

            }

            if (forward)
                current = current.Next;
            else
            {
                current = current.Prev;
                if (current != null && current.Prev == null)
                    return;
            }

            

            if (current == null)
                return;

            CurrentFound:

            if (side == 1)
            {
                pTC.NaturalDividerPosition1W = current;
                pTC.NaturalDividerPosition1 = current.Pos;
                (pTC.NippingFrame.F1 as Frame).FramePen = pTC.CorrectedPen;
                pTC.Side1Set = true;
            }
            else
            {
                pTC.NaturalDividerPosition2W = current;
                pTC.NaturalDividerPosition2 = current.Pos;
                (pTC.NippingFrame.F2 as Frame).FramePen = pTC.CorrectedPen;
                pTC.Side2Set = true;
            }

            pTC.SetNippingFrameByScreenWord(side, current);

            pTC.Render();

        }


        private void MergeWithPrevious()
        {
            if (pTC.EditMode && pTC.HighlightedPair > 0)
            {
                pTC.MergePairs(pTC.HighlightedPair - 1);
                pTC.HighlightedPair--;
                
                if (pTC.CurrentPair >= pTC.HighlightedPair)
                {
                    pTC.PairChanged(pTC.HighlightedPair, false);
                    GotoPair(pTC.HighlightedPair, true);
                }
                else
                    pTC.PairChanged(pTC.HighlightedPair, true);

                pTC[pTC.HighlightedPair].UpdateTotalSize();
                pTC.PText.UpdateAggregates(pTC.HighlightedPair);
                UpdateStatusBar(true);
                pTC.Side1Set = false;
                pTC.Side2Set = false;
            }
        }

        private void CopyToClipboard()
        {
            if (pTC.SelectionSide != 0 && pTC.SelectionFinished)
            {
                //System.Windows.Forms.Clipboard.SetText("abc");

                StringBuilder selectedText = new StringBuilder();

                int Y1;
                int Y2;
                int X1;
                int X2;

                pTC.AssignProperSelectionOrder(out Y1, out Y2, out X1, out X2);


                if (Y1 == Y2)
                {
                    // The simplest case - text selected inside one pair

                    // If there is more than one word...
                    if (X1 < X2)
                        selectedText.Append(pTC[Y1].Substring(pTC.SelectionSide, X1, X2 - X1));
                    
                }
                else
                {
                    // The text spans across several pairs

                    int currentPair = Y1;

                    while (true)
                    {
                        if (currentPair > Y1)
                            // Insert space or linebreak, depending on Startparagraph
                            if (pTC[currentPair].StartParagraph(pTC.SelectionSide))
                                selectedText.Append("\r\n");
                            else if (pTC.WesternJoint(currentPair - 1, pTC.SelectionSide))
                                selectedText.Append(' ');

                        if (currentPair == Y1)
                            selectedText.Append(pTC[currentPair].Substring(pTC.SelectionSide, X1));
                        else if (currentPair == Y2)
                            selectedText.Append(pTC[currentPair].Substring(pTC.SelectionSide, 0, X2));
                        else
                            selectedText.Append(pTC[currentPair].GetText(pTC.SelectionSide));

                        currentPair++;

                        if (currentPair > Y2)
                            break;

                    }

                    

                }

                // Append the last word starting in Y2 from X2 
                selectedText.Append(ParallelTextControl.GetWord(pTC[Y2], pTC.SelectionSide, X2));


                Clipboard.SetText(selectedText.ToString());
                
                
            }
        }

        private void ProcessPageDown()
        {
            // Let's find the last pair that fully fits on normal lines
            // then rewind to the next of it
            if (pTC.LastRenderedPair != pTC.CurrentPair)
            {
                int pairIndex = pTC.LastRenderedPair;

                while (pairIndex > 0 && pTC.NotFitOnScreen(pTC[pairIndex]))
                    pairIndex--;

                pairIndex++;

                if (pairIndex > pTC.Number - 1)
                    pairIndex = pTC.Number - 1;

                GotoPair(pairIndex);
            }
        }

        private void ProcessPageUp()
        {

            int newCurrentPair = pTC.CurrentPair;
            int req = pTC.LastFullScreenLine;
            
            int accLines = 0; // Accumulated lines

            TextPair processedPair;

            while (newCurrentPair > 0)
            {
                newCurrentPair--;
                processedPair = pTC[newCurrentPair];

                if (!(processedPair.AllLinesComputed1 && processedPair.AllLinesComputed2))
                    pTC.PrepareScreen(newCurrentPair, -1);

                accLines += processedPair.Height;

                if (accLines > req)
                {
                    newCurrentPair++;
                    break;
                }
            }

            if (newCurrentPair == pTC.CurrentPair
                && newCurrentPair > 0)
                newCurrentPair--;

            if (newCurrentPair != pTC.CurrentPair)
                GotoPair(newCurrentPair);

        }

        public void GotoPair(int newCurrentPair)
        {
            GotoPair(newCurrentPair, false);
        }

        public void GotoPair(int newCurrentPair, bool forced)
        {
            if (pTC.CurrentPair == newCurrentPair && !forced)
                return;

            pTC.CurrentPair = newCurrentPair;
            pTC.HighlightedPair = newCurrentPair;

            pTC.FindFirstNaturalDividers();

            pTC.UpdateScreen();

            UpdateStatusBar(true);
        }

        private void UpdateStatusBar(bool updateScrollBar)
        {

            if (pTC.Number == 0)
            {
                ssPosition.Text = "0 / 0";
                ssPositionPercent.Text = "---";
                return;
            }


            ssPosition.Text = (pTC.HighlightedPair + 1).ToString() + " / " + pTC.Number;

            int totalVolume = pTC[pTC.Number - 1].aggregateSize;

            if (totalVolume == 0)
                ssPositionPercent.Text = "---";

            else
            {
                int prevVolume;

                if (pTC.HighlightedPair == 0)
                    prevVolume = 0;
                else
                    prevVolume = pTC[pTC.HighlightedPair - 1].aggregateSize;

                string s = ((float)prevVolume / totalVolume).ToString("P3");

                if (pTC.EditMode)
                {

                    s += "    ";

                    if (pTC.stopwatchStarted)
                    {
                        s += pTC.stopWatch.IsRunning ? "Aligning..." : "Aligning [PAUSED]";

                        if (pTC.Number > pTC.startingNumberOfFrags)
                        {

                            // Let's count speed
                            long elapsed = pTC.stopWatch.ElapsedMilliseconds;

                            s += "  " + ((pTC.Number - pTC.startingNumberOfFrags) * 3600000 / elapsed).ToString() + " f/hr";



                            // current top percent

                            float currentPercent;

                            if (pTC.Number > 1)
                                currentPercent = (float)pTC[pTC.Number - 2].aggregateSize / pTC[pTC.Number - 1].aggregateSize;
                            else
                                currentPercent = 0;

                            if (currentPercent > pTC.startingPercent)
                            {
                                float percentDiff = currentPercent - pTC.startingPercent;

                                s += "  Aligned " + percentDiff.ToString("P3");

                                float whatIsLeft = 1.0f - currentPercent;

                                //float percentSpeed = percentDiff / elapsed;

                                int secondsLeft = (int)(whatIsLeft / percentDiff * elapsed / 1000);

                                int hoursLeft = secondsLeft / 3600;

                                secondsLeft -= hoursLeft * 3600;

                                int minutesLeft = secondsLeft / 60;

                                secondsLeft -= minutesLeft * 60;

                                s += "  ETA " + string.Format("{0:00}:{1:D2}:{2:D2}",
                                                hoursLeft, minutesLeft, secondsLeft);

                            }

                        }

                    }

                    else
                        s += " Aligning not started";

                }


                ssPositionPercent.Text = s;

            }

            if (updateScrollBar)
            {
                vScrollBar.Maximum = (pTC.Number == 0 ? 0 : pTC.Number - 1);
                vScrollBar.Value = pTC.CurrentPair;
            }

        }

        private void ProcessKeyUp()
        {
            if (pTC.HighlightedPair == 0)
                return;

            if (pTC.EditMode)
            {

                pTC.Side1Set = false;
                pTC.Side2Set = false;

                TextPair prev_p = pTC[pTC.HighlightedPair];

                pTC.HighlightedPair--;
                pTC.FindFirstNaturalDividers();

                TextPair p = pTC[pTC.HighlightedPair];

                //if (prev_p.RenderedInfo1.Line1 == 0 && (prev_p.RenderedInfo1.X1 == 0 || p.Height > 0)
                //    || prev_p.RenderedInfo2.Line1 == 0 && (prev_p.RenderedInfo2.X1 == 0 || p.Height > 0))
                if (pTC.HighlightedPair < pTC.FirstRenderedPair
                    || p.Height == -1 || !p.RenderedInfo1.Valid || p.RenderedInfo1.Line1 == -1 || p.RenderedInfo1.Line2 == -1)
                {
                    pTC.CurrentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                }

                pTC.FindNaturalDividersScreen(0);
                pTC.ProcessMousePosition(true, false);
                pTC.Render();
                UpdateStatusBar(true);
            }
            else
            {
                // View mode
                int newCurrentPair = pTC.CurrentPair;
                
                TextPair processedPair;

                while (newCurrentPair > 0)
                {
                    newCurrentPair--;
                    processedPair = pTC[newCurrentPair];

                    if (!(processedPair.AllLinesComputed1 && processedPair.AllLinesComputed2))
                        pTC.PrepareScreen(newCurrentPair, -1);

                    if (processedPair.Height > 0)
                        break;
                }

                if (newCurrentPair != pTC.CurrentPair)
                    GotoPair(newCurrentPair);
            }
        }

        public void ProcessKeyDown()
        {
            if (pTC.HighlightedPair == pTC.Number - 1)
            {
                if (pTC.CurrentPair != pTC.HighlightedPair)
                    GotoPair(pTC.HighlightedPair);
                return;
            }

            if (pTC.EditMode)
            {

                pTC.Side1Set = false;
                pTC.Side2Set = false;

                pTC.HighlightedPair++;
                pTC.FindFirstNaturalDividers();

                TextPair p = pTC[pTC.HighlightedPair];

                if ((p.RenderedInfo1.Line2 == -1
                    || p.RenderedInfo1.Line2 >= pTC.LastFullScreenLine
                    || p.RenderedInfo2.Line2 == -1
                    || p.RenderedInfo2.Line2 >= pTC.LastFullScreenLine)
                    && !pTC[pTC.HighlightedPair].IsBig())
                {
                    pTC.CurrentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs();
                }

                pTC.FindNaturalDividersScreen(0);

                pTC.ProcessMousePosition(true, false);

                pTC.Render();

                UpdateStatusBar(true);
            }
            else
            {
                // View mode
                int pairIndex = pTC.CurrentPair;
                while (pTC[pairIndex].Height == 0 && pairIndex < pTC.Number - 1)
                    pairIndex++;
                if (pairIndex < pTC.Number - 1)
                    pairIndex++;
                if (pairIndex != pTC.CurrentPair)
                    GotoPair(pairIndex);
            }
        }

        private void ChangeNatural(byte screen_side, bool inc)
        {

            if (!pTC.EditMode)
                return;

            TextPair p = pTC[pTC.HighlightedPair];

            byte side;

            if (screen_side == 0)
                side = 0;
            else if (pTC.Reversed)
                side = (byte) (3 - screen_side);
            else
                side = screen_side;

            bool NeedToRender = false;

            if (side == 0 || side == 1)
                if (pTC.NextRecommended(1, inc))
                {
                    NeedToRender = true;
                    pTC.FindNaturalDividersScreen(1);
                    pTC.Side1Set = true;
                }

            if (side == 0 || side == 2)
                if (pTC.NextRecommended(2, inc))
                {
                    NeedToRender = true;
                    pTC.FindNaturalDividersScreen(2);
                    pTC.Side2Set = true;
                }

            if (NeedToRender)
                pTC.Render();

        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!pTC.EditMode)
                return;

            using (ImportTextForm importTextForm = new ImportTextForm())
            {
                importTextForm.PText = pTC.PText;
                importTextForm.ShowDialog();
            }

            pTC.FindFirstNaturalDividers();

            UpdateStatusBar(true);

            Recompute();

        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (BookInfoForm f = new BookInfoForm())
            {
                f.ParallelTC = pTC;
                f.ShowDialog();
                UpdateWindowTitle();
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
                    d.Filter = "Parallel Books (*.pbo)|*.pbo";
                    d.RestoreDirectory = true;
                    DialogResult dialogResult = d.ShowDialog();

                    if (dialogResult != DialogResult.OK)
                        return;

                    fileName = d.FileName;

                    RetrieveToTheTop(fileName);

                    newBook = false;
                }
            }
            else
                fileName = pTC.PText.FileName;

            pTC.PText.Save(fileName);
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

            if (AskToSaveModified(sender) == System.Windows.Forms.DialogResult.Cancel)
                return;

            string fileName;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Parallel Books (*.pbo)|*.pbo";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    fileName = LoadFromFile(openFileDialog.FileName);
            }
        }

        private string LoadFromFile(string fileName)
        {
            pTC.PText = new ParallelText();
            pTC.mouse_text_line = -1;

            pTC.PText.Load(fileName);

            if (RetrieveToTheTop(fileName))
                LoadSettingsFromFileUsageInfo(appSettings.FileUsages[0], false);
            else
            {
                pTC.CurrentPair = 0;
                pTC.HighlightedPair = 0;
                SetEditMode(false);
            }

            ProcessEditModeChange();

            pTC.stopwatchStarted = false;

            newBook = false;

            pTC.Modified = false;

            pTC.FindFirstNaturalDividers();

            Recompute();

            UpdateStatusBar(true);
            UpdateWindowTitle();

            return fileName;
        }

        private void SaveAppSettings()
        {
            if (appSettings.FileUsages.Count > 0 && !newBook)
            {
                FileUsageInfo f = appSettings.FileUsages[0];

                f.Pair = pTC.HighlightedPair;
                f.TopPair = pTC.CurrentPair;
                f.Reversed = pTC.Reversed;
                f.SplitterRatio = pTC.SplitterRatio;
                f.EditMode = pTC.EditMode;
                f.ReadingMode = pTC.ReadingMode;
            }

            appSettings.HighlightFragments = pTC.HighlightFragments;
            appSettings.HighlightFirstWords = pTC.HighlightFirstWords;
            
            appSettings.Brightness = pTC.Brightness;

            appSettings.FontName = pTC.textFont.Name;
            appSettings.FontSize = pTC.textFont.Size;

            Properties.Settings.Default.AppSettings = appSettings;
            Properties.Settings.Default.Save();
        }

        private void pTC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (pTC.EditMode)
            {
                switch (e.KeyChar)
                {
                    case ' ':
                        SeparateCurrentPair();
                        break;
                }
            }

            
        }

        private void SeparateCurrentPair()
        {
            if (pTC.NipHighlightedPair())
            {
                UpdateStatusBar(true);
                pTC.MouseCurrentWord = null;
            }
            else
                ProcessKeyDown();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.EditCurrentPair();
            UpdateStatusBar(false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveBook(true);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm f = new AboutForm())
                f.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AskToSaveModified(sender) == System.Windows.Forms.DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
        }

        DialogResult AskToSaveModified(Object sender)
        {
            SaveAppSettings();

            if (!pTC.Modified)
                return DialogResult.No;

            DialogResult r = MessageBox.Show(
                    "Save modified book?", "The book was modified", MessageBoxButtons.YesNoCancel);

            if (r == System.Windows.Forms.DialogResult.Yes)
                saveToolStripMenuItem_Click(sender, EventArgs.Empty);

            return r;

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AskToSaveModified(sender) == System.Windows.Forms.DialogResult.Cancel)
                return;

            pTC.CreateNewParallelBook();

            pTC.stopwatchStarted = false;

            UpdateStatusBar(true);

            UpdateWindowTitle();

            reverseToolStripMenuItem.Checked = false;
            pTC.ComputeSideCoordinates();
            editModeToolStripMenuItem.Checked = true;

            pTC.HighlightedFrame.SetInvisible();
            pTC.NippingFrame.SetInvisible();

            pTC.ProcessLayoutChange();

            newBook = true;
        }
                
        private void structureleftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowseBookStructure(1);
        }

        private void BrowseBookStructure(byte screenSide)
        {
            using (BookStructureForm f = new BookStructureForm())
            {
                f.parallelTextControl = pTC;
                f.screenSide = screenSide;
                f.ShowDialog();

                if (f.pairIndex != -1)
                    GotoPair(f.pairIndex);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void structurerightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowseBookStructure(2);
        }

        private void editModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            pTC.EditMode = editModeToolStripMenuItem.Checked;

            pTC.SetLayoutMode();

            ProcessEditModeChange();

            if (pTC.ReadingMode != FileUsageInfo.NormalMode)
                Recompute();
            else
                pTC.UpdateScreen();
            
        }

        private void ProcessEditModeChange()
        {
            if (pTC.EditMode)
            {
                pTC.SelectionSide = 0;
                pTC.SelectionFrame.Visible = false;
            }
            else
            {
                pTC.HighlightedFrame.SetInvisible();
                pTC.NippingFrame.SetInvisible();
                pTC.MouseCurrentWord = null;
            }

            UpdateStatusBar(false);
        }

        private void openRecentToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (AskToSaveModified(sender) == System.Windows.Forms.DialogResult.Cancel)
                return;

            using (OpenRecentForm f = new OpenRecentForm())
            {
                f.appSettings = appSettings;
                f.ShowDialog();
                if (!string.IsNullOrEmpty(f.result))
                    LoadFromFile(f.result);
            }

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (SettingsForm settingsForm = new SettingsForm())
            {
                settingsForm.pTC = pTC;
                settingsForm.ShowDialog();

            }

        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.LargeDecrement)
                ProcessPageUp();
            else if (e.Type == ScrollEventType.LargeIncrement)
                ProcessPageDown();
            else if (e.Type == ScrollEventType.SmallDecrement)
                ProcessKeyUp();
            else if (e.Type == ScrollEventType.SmallIncrement)
                ProcessKeyDown();
            else if (e.Type == ScrollEventType.ThumbTrack)
            {
                GotoPair(e.NewValue);
                statusStrip.Refresh();
            }
            else
                return;

            e.NewValue = pTC.CurrentPair;

            
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (findForm == null)
            {
                findForm = new FindForm();
                findForm.Left = (Width - findForm.Width) / 2;
                findForm.Top = (Height - findForm.Height) / 2;
                findForm.pTC = pTC;
                findForm.mainForm = this;
                
                
            }

            findForm.ShowDialog();

        }

        private void startpauseStopwatchToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (!pTC.EditMode)
                return;

            if (!pTC.stopwatchStarted)
                pTC.ResetStopwatch(1);
            else
                if (pTC.stopWatch.IsRunning)
                    pTC.stopWatch.Stop();
                else
                    pTC.stopWatch.Start();

            UpdateStatusBar(false);

        }

        private void resetStopwatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.ResetStopwatch(0);
            UpdateStatusBar(false);
        }

    }

    public class CommonWordInfo : WordInfo
    {
        public TextPair TextPair { get; set; }
        public byte side;

        public CommonWordInfo(TextPair textPair, string word, int line, int wordX, int wordX2, int pos, bool eastern, byte side)
            : base(word, line, wordX, wordX2, pos, eastern)
        {
            this.TextPair = textPair;
            this.side = side;
        }
            
    }

    
    public class FileUsageInfo
    {
        public const int NormalMode = 0;
        public const int AlternatingMode = 1;
        public const int AdvancedMode = 2;


        public string FileName { get; set; }
        public int Pair { get; set; }
        public int TopPair { get; set; }
        public bool Reversed { get; set; }
        public float SplitterRatio { get; set; }
        public bool EditMode { get; set; }
        public int ReadingMode { get; set; }
    }

    public class AppSettings
    {
        public bool HighlightFragments { get; set; }
        public bool HighlightFirstWords { get; set; }
        public double Brightness { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }

        public Collection<FileUsageInfo> FileUsages { get; set; }

        public AppSettings()
        {
            HighlightFragments = true;
            HighlightFirstWords = true;
            Brightness = 0.96;
            FontName = "Arial";
            FontSize = 18.0F;
            FileUsages = new Collection<FileUsageInfo>();
        }

    }

}
