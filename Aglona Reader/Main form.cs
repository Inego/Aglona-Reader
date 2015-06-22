using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.IO;
using AglonaReader.Mp3Player;

[assembly: CLSCompliant(true)]
namespace AglonaReader
{
    
    public partial class MainForm : Form
    {

        private const int timerInterval = 100;
        private const int timerHalfInterval = timerInterval / 2;

        AudioPlayer player = new AudioPlayer();

        Timer playbackTimer = new Timer();
        long nextTime = -1;
        int nextPairToPlay = -1;
        
        bool newBook;

        AppSettings appSettings;

        byte opState;
        private bool startedXMoving = false;
        private bool startedYMoving = false;
        private bool justJumped = false;
        private int prevHorizontalDistance = 0;

        FindForm findForm;
        private bool CtrlPressed;
        private int currentAudioFileNumber;

        private bool dragging;
        private int mouseYBeforeDragging;

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.RButton | Keys.ShiftKey))
                return true;
            
            return base.ProcessDialogKey(keyData);
        }

        private void pTC_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                ProcessUpArrow(true);
            else if (e.Delta < 0)
                ProcessDownArrow(true);
            
        }

        public MainForm()
        {
            InitializeComponent();

            pTC.MouseWheel += new MouseEventHandler(pTC_MouseWheel);

            opState = 0;
            CtrlPressed = false;
            WindowState = FormWindowState.Maximized;

            playbackTimer.Interval = timerInterval;
            playbackTimer.Tick += playbackTimer_Tick;

            currentAudioFileNumber = -1;
            
        }

        void playbackTimer_Tick(object sender, EventArgs e)
        {
            uint currentTime = player.CurrentTime;

            if (currentTime >= nextTime || !player.Playing)
            {

                if (nextPairToPlay == -1)
                {
                    playbackTimer.Enabled = false;
                    StopPlayback();
                    return;
                }
                
                // Update selection

                if (pTC[nextPairToPlay].AudioFileNumber != currentAudioFileNumber)
                {
                    playbackTimer.Enabled = false;
                    pTC.HighlightedPair = nextPairToPlay - 1;
                    StartPlayback(true);
                    return;
                }

                pTC.HighlightedPair = nextPairToPlay;

                DetermineNextTime();

                if (pTC.NotFitOnScreen(pTC.HighlightedPair))
                {
                    pTC.CurrentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs(false);
                }

                pTC.UpdateFramesOnScreen(0);

                pTC.Render();

                UpdateStatusBar(true);
                
            }
        }

        private void DetermineNextTime()
        {

            nextTime = -1;
            nextPairToPlay = -1;
            
            for (int i = pTC.HighlightedPair + 1; i < pTC.Number; i++)
            {
                if (pTC[i].AudioFileNumber != 0
                    && pTC[i].AudioFileNumber != currentAudioFileNumber)
                {
                    if (pTC.hp.TimeEnd != 0)
                        nextTime = pTC.hp.TimeEnd;
                    nextPairToPlay = i;
                    break;
                }
                if (pTC[i].TimeBeg != 0)
                {
                    nextTime = pTC[i].TimeBeg - timerHalfInterval;
                    nextPairToPlay = i;
                    break;
                }
            }

            if (nextPairToPlay == -1 && pTC.hp.TimeEnd != 0)
                nextTime = pTC.hp.TimeEnd;

        }

        private void StartPlayback(bool fromNextPair)
        {
            if (playbackTimer.Enabled)
                StopPlayback();

            if (!pTC.WithAudio() || pTC.EditMode)
                return;

            int pairToStart = pTC.HighlightedPair + (fromNextPair ? 1 : 0);

            // Rewind forward until a pair with sound is found (if any)

            while (pairToStart < pTC.Number && pTC[pairToStart].AudioFileNumber == 0)
                pairToStart++;

            if (pairToStart >= pTC.Number || pTC[pairToStart].AudioFileNumber == 0)
                return;

            if (pTC.HighlightedPair != pairToStart)
                GotoPair(pairToStart, false, false, 0);
            
            TextPair hp = pTC.hp;

            if (SetFileToAudioplayer(hp.AudioFileNumber))
            {
                DetermineNextTime();
                player.PlayFromTo(hp.TimeBeg, 0);
                playbackTimer.Enabled = true;
            }

        }

        // Stops both single and countinuous playback
        private void StopPlayback()
        {
            playbackTimer.Enabled = false;
            player.Stop(true);
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


            

            String[] args = Environment.GetCommandLineArgs();

            bool outerLoad = (args.Length > 1);

            if (appSettings.FileUsages.Count > 0)
            {
                LoadSettingsFromFileUsageInfo(appSettings.FileUsages[0], !outerLoad);
                newBook = false;
            }
            else
            {
                pTC.SetSplitterPositionByRatio(0.5F);
                newBook = true;
                SetEditMode(true);
            }

            if (outerLoad)
                LoadFromFile(args[1]);
            
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
                currentAudioFileNumber = -1;
                UpdateWindowTitle();
                CtrlPressed = false;
            }

            if (f.SplitterRatio == 0)
                f.SplitterRatio = 0.5F;

            pTC.Reversed = f.Reversed;
            reverseToolStripMenuItem.Checked = pTC.Reversed;

            pTC.ReadingMode = f.ReadingMode;
            pTC.AlternatingColorScheme = f.AlternatingColorScheme;

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
                //Recompute();
            }

            ProcessEditModeChange(true);

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

            if (pTC.WithAudio())
                cpt += " ♪";

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

            if (dragging)
            {
                if (Math.Abs(mouseYBeforeDragging - e.Y) > pTC.lineHeight)
                {
                    var deltaY = mouseYBeforeDragging - e.Y;
                    var linesToScroll = deltaY / pTC.lineHeight;
                    var rest = deltaY % pTC.lineHeight;
                    mouseYBeforeDragging = e.Y - rest;

                    for (int i = 0; i < Math.Abs(linesToScroll); ++i)
                    {
                        if (linesToScroll > 0)
                            ProcessDownArrow(true);
                        else
                            ProcessUpArrow(true);
                    }
                }
            }
            else if (XonSplitter(pTC.LastMouseX) || opState == 1)
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

                        TextPair p = pTC.hp;

                        bool forward = (distance > prevHorizontalDistance);

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

                            pTC.UpdateFramesOnScreen(needToRender);
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

                        if (pTC.NotFitOnScreen(pTC.HighlightedPair))
                        {
                            pTC.CurrentPair = pTC.HighlightedPair;
                            pTC.PrepareScreen();
                            pTC.RenderPairs(false);
                            
                        }

                        pTC.FindFirstNaturalDividers();
                        pTC.UpdateFramesOnScreen(0);
                        pTC.ProcessMousePosition(true, true);

                        justJumped = true;

                    }


                    pTC.horizontalStartingPosition = e.X;
                    pTC.verticalStartingPosition = e.Y;

                    p = pTC.hp;

                    startedXMoving = false;
                    startedYMoving = false;

                }

                else
                {
                    // In view mode mouse down begins selection

                    if (CtrlPressed)
                    {

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
                    else
                    {
                        dragging = true;
                        pTC.Capture = true;
                        pTC.Cursor = Cursors.SizeNS;
                        mouseYBeforeDragging = e.Y;
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

            if (dragging)
            {
                dragging = false;
                pTC.Capture = false;
                pTC.Cursor = Cursors.Default;
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
                {
                    if (!pTC.SelectionFinished)
                    {
                        pTC.SelectionFinished = true;
                        CopyToClipboard();
                    }
                    else
                    {
                        if (pTC.WithAudio() && pTC.mouse_text_word != null)
                        {
                            // Move to this fragment
                            if (pTC.mouse_text_word.PairIndex != pTC.HighlightedPair)
                            {

                                if (pTC.NotFitOnScreen(pTC.mouse_text_word.PairIndex))
                                    GotoPair(pTC.mouse_text_word.PairIndex, false, false, 1);
                                else
                                {
                                    pTC.HighlightedPair = pTC.mouse_text_word.PairIndex;
                                    pTC.UpdateFramesOnScreen(0);
                                    pTC.Render();
                                    PlayCurrentPhrase();
                                }

                            }
                            else
                                PlayCurrentPhrase();

                        }
                    }
                }
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
            Recompute();
        }

        private void Recompute()
        {
            pTC.mouse_text_line = -1;
            pTC.MouseCurrentWord = null;
            pTC.ProcessLayoutChange(true);
        }

        private void pTC_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData & Keys.ControlKey) == Keys.ControlKey)
                CtrlPressed = true;

            if (e.KeyData == Keys.Down
                || e.KeyData == Keys.N)
                ProcessDownArrow(true);

            else if (e.KeyData == Keys.Up)
                ProcessUpArrow(true);

            else if (e.KeyData == (Keys.Control | Keys.Down)
                && !pTC.EditMode)
                StartPlayback(true);

            else if ((e.KeyData == (Keys.Control | Keys.Space)
                || e.KeyData == (Keys.Control | Keys.Enter))
                && !pTC.EditMode)
                StartPlayback(false);

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
            {
                if (pTC.EditMode)
                    ChangeNatural(0, true);
                else
                    ProcessDownArrow(false);
            }

            else if (e.KeyData == Keys.Left
                || e.KeyData == Keys.T
                || e.KeyData == Keys.G)
            {
                if (pTC.EditMode)
                    ChangeNatural(0, false);
                else
                    ProcessUpArrow(false);
            }

            else if (e.KeyData == (Keys.Control | Keys.End))
            {
                int n = pTC.Number;
                if (n == 0)
                    return;
                if (pTC.HighlightedPair != n - 1)
                    GotoPair(n - 1, false, false, 1);

            }

            else if (e.KeyData == (Keys.Control | Keys.Home))
            {
                int n = pTC.Number;
                if (n == 0)
                    return;

                GotoPair(0, false, true, 1);
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

            else if (e.KeyData == Keys.Enter
                || e.KeyData == Keys.Space)
            {
                if (playbackTimer.Enabled)
                    StopPlayback();
                else if (player.Playing)
                    player.Stop(true);
                else
                    PlayCurrentPhrase();
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

                if (pTC.CurrentPair > pTC.HighlightedPair)
                    pTC.CurrentPair = pTC.HighlightedPair;

                GotoChangedPair();
            }
        }

        private void GotoChangedPair()
        {

            if (pTC.HighlightedPair >= pTC.Number)
                pTC.HighlightedPair = pTC.Number - 1;

            pTC.PairChanged(pTC.HighlightedPair, true);
            GotoPair(pTC.HighlightedPair, false, true, 2);

            pTC.hp.UpdateTotalSize();
            pTC.PText.UpdateAggregates(pTC.HighlightedPair);
            UpdateStatusBar(true);
            pTC.Side1Set = false;
            pTC.Side2Set = false;
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

                while (pairIndex > 0 && pTC.NotFitOnScreen(pairIndex))
                    pairIndex--;

                pairIndex++;

                if (pairIndex > pTC.Number - 1)
                    pairIndex = pTC.Number - 1;

                GotoPair(pairIndex, false, false, 1);
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
                GotoPair(newCurrentPair, false, false, 1);

        }

        // playType
        //  0: Stop playing (if playing sound)
        //  1: play once
        //  2: keep playing
        //
        public void GotoPair(int newCurrentPair, bool setCurrentPair, bool forced, int playType)
        {
            
            if (pTC.HighlightedPair == newCurrentPair
                && !forced
                && !(setCurrentPair && pTC.CurrentPair != newCurrentPair))
                return;

            pTC.HighlightedPair = newCurrentPair;

            pTC.FindFirstNaturalDividers();

            if (setCurrentPair && pTC.CurrentPair != newCurrentPair
                || pTC.NotFitOnScreen(pTC.HighlightedPair))
            {
                pTC.CurrentPair = newCurrentPair;
                pTC.UpdateScreen();
            }
            else
            {
                pTC.UpdateFramesOnScreen(0);
                pTC.ProcessMousePosition(true, false);
                pTC.Render();
            }

            UpdateStatusBar(true);


            if (playType == 0)
                StopPlayback();
            else if (playType == 1)
                PlayCurrentPhrase();
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

                            s += "  " + ((long)((double)(pTC.Number - pTC.startingNumberOfFrags) * 3600000 / elapsed)).ToString() + " f/hr";

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
                vScrollBar.Value = pTC.HighlightedPair;
            }

        }

        private void PlayCurrentPhrase()
        {
            if (!pTC.EditMode && pTC.WithAudio())
            {
                if (playbackTimer.Enabled)
                    StopPlayback();

                TextPair tp = pTC.hp;

                uint audioFileNumber = tp.AudioFileNumber;

                if (audioFileNumber == 0)
                    return;

                uint beg = tp.TimeBeg;

                uint end = tp.TimeEnd;

                for (int _i = pTC.HighlightedPair + 1; end == 0 & _i < pTC.Number; _i++)
                {
                    TextPair _p = pTC[_i];
                    if (_p.AudioFileNumber == 0)
                        continue;
                    if (_p.AudioFileNumber != audioFileNumber)
                        break;
                    if (_p.TimeBeg >= beg)
                    {
                        end = _p.TimeBeg;
                        break;
                    }

                }

                if(SetFileToAudioplayer(audioFileNumber))
                    player.PlayFromTo(beg, end);
                
            }            
        }

        private bool SetFileToAudioplayer(uint audioFileNumber)
        {
            if (audioFileNumber != currentAudioFileNumber)
            {
                string folder = Path.GetDirectoryName(pTC.PText.FileName);
                string fName = folder + @"\" + audioFileNumber.ToString() + ".mp3";

                if (File.Exists(fName))
                    player.Open(fName);
                else
                    return false;

                currentAudioFileNumber = (int)audioFileNumber;

            }

            return true;
        }

        private void ProcessUpArrow(bool withPlayback)
        {
            if (pTC.HighlightedPair == 0)
                return;

            if (pTC.EditMode || pTC.WithAudio())
            {

                TextPair prev_p = pTC.hp;

                pTC.HighlightedPair--;

                if (pTC.EditMode)
                {
                    pTC.Side1Set = false;
                    pTC.Side2Set = false;
                    pTC.FindFirstNaturalDividers();
                }

                if (pTC.NotFitOnScreen(pTC.HighlightedPair))
                {
                    pTC.CurrentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs(false);
                }

                pTC.UpdateFramesOnScreen(0);
                
                pTC.ProcessMousePosition(true, false);
                
                pTC.Render();
                
                UpdateStatusBar(true);

                if (pTC.WithAudio())
                    if (withPlayback)
                        PlayCurrentPhrase();
                    else
                        StopPlayback();

            }
            else
            {
                // View mode (simple text)
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
                    GotoPair(newCurrentPair, false, false, 1);
            }
        }

        public void ProcessDownArrow(bool withPlayback)
        {
            if (pTC.HighlightedPair >= pTC.Number - 1)
            {
                if (pTC.CurrentPair != pTC.HighlightedPair)
                    GotoPair(pTC.HighlightedPair, true, false, (withPlayback ? 1 : 0));
                return;
            }

            if (pTC.EditMode || pTC.WithAudio())
            {
                pTC.HighlightedPair++;

                if (pTC.EditMode)
                {
                    pTC.Side1Set = false;
                    pTC.Side2Set = false;
                    pTC.FindFirstNaturalDividers();
                }

                if (pTC.NotFitOnScreen(pTC.HighlightedPair))
                {
                    pTC.CurrentPair = pTC.HighlightedPair;
                    pTC.PrepareScreen();
                    pTC.RenderPairs(false);
                }

                pTC.UpdateFramesOnScreen(0);

                pTC.ProcessMousePosition(true, false);

                pTC.Render();

                UpdateStatusBar(true);

                if (pTC.WithAudio())
                    if (withPlayback)
                        PlayCurrentPhrase();
                    else
                        StopPlayback();
    
            }
            else
            {
                // View mode (silent)
                int pairIndex = pTC.CurrentPair;

                while (pTC[pairIndex].Height == 0 && pairIndex < pTC.Number - 1)
                    pairIndex++;
                if (pairIndex < pTC.Number - 1)
                    pairIndex++;
                if (pairIndex != pTC.CurrentPair)
                    GotoPair(pairIndex, true, false, 2);
            }
        }

        private void ChangeNatural(byte screen_side, bool inc)
        {

            if (!pTC.EditMode)
                return;

            TextPair p = pTC.hp;

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
                    pTC.UpdateFramesOnScreen(1);
                    pTC.Side1Set = true;
                }

            if (side == 0 || side == 2)
                if (pTC.NextRecommended(2, inc))
                {
                    NeedToRender = true;
                    pTC.UpdateFramesOnScreen(2);
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

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Parallel Books (*.pbo,*.pbs)|*.pbo;*.pbs";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    LoadFromFile(openFileDialog.FileName);
            }
        }

        private bool LoadFromFile(string fileName)
        {
            pTC.PText = new ParallelText();
            pTC.mouse_text_line = -1;

            bool result = pTC.PText.Load(fileName);

            CtrlPressed = false;

            currentAudioFileNumber = -1;

            if (RetrieveToTheTop(fileName))
                LoadSettingsFromFileUsageInfo(appSettings.FileUsages[0], false);
            else
            {
                pTC.CurrentPair = 0;
                pTC.HighlightedPair = 0;
                SetEditMode(false);
            }

            ProcessEditModeChange(true);

            pTC.stopwatchStarted = false;

            newBook = false;

            pTC.Modified = false;

            pTC.FindFirstNaturalDividers();

            Recompute();

            UpdateWindowTitle();

            return result;
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
                f.AlternatingColorScheme = pTC.AlternatingColorScheme;
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
                ProcessDownArrow(true);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pTC.EditCurrentPair())
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

            player.Close();
            playbackTimer.Enabled = false;

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

            pTC.SetLayoutMode();

            pTC.ProcessLayoutChange(false);

            pTC.HighlightedFrame.SetVisibility(false);
            pTC.NippingFrame.SetVisibility(false);
            pTC.AudioSingleFrame.Visible = false;

            ProcessEditModeChange(false);

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
                    GotoPair(f.pairIndex, true, false, 0);
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

            ProcessEditModeChange(false);

        }

        private void ProcessEditModeChange(bool updateStatusBar)
        {
            if (pTC.EditMode)
            {
                pTC.SelectionSide = 0;
                pTC.SelectionFrame.Visible = false;
                pTC.HighlightedFrame.SetPen(pTC.HighlightedPen);
                player.Stop(true);
            }
            else
            {
                if (pTC.WithAudio())
                    pTC.HighlightedFrame.SetPen(pTC.AudioPen);
                else
                    pTC.HighlightedFrame.SetVisibility(false);
                pTC.NippingFrame.SetVisibility(false);
                pTC.MouseCurrentWord = null;
            }

            if (pTC.ReadingMode != FileUsageInfo.NormalMode)
                Recompute();
            else
                pTC.UpdateScreen();

            UpdateStatusBar(updateStatusBar);
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
                ProcessUpArrow(true);
            else if (e.Type == ScrollEventType.SmallIncrement)
                ProcessDownArrow(true);
            else if (e.Type == ScrollEventType.ThumbTrack)
            {
                GotoPair(e.NewValue, false, false, 0);
                statusStrip.Refresh();
            }
            else
                return;

            e.NewValue = pTC.HighlightedPair;

            
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

        private void exportLeftTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportText(true);
        }

        private void ExportText(bool leftSide)
        {
            // Ask for the file name
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = "Text files (*.txt)|*.txt";
                
                d.RestoreDirectory = true;
                
                DialogResult dialogResult = d.ShowDialog();

                if (dialogResult != DialogResult.OK)
                    return;

                pTC.ExportText(d.FileName, leftSide);

            }

        }

        private void exportRightTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportText(false);
        }

        private void pTC_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyData & Keys.ControlKey) == Keys.ControlKey)
                CtrlPressed = false;
        }

        private void reverseContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really reverse book contents?"
                + "\r\nThis will physically reverse the two texts in the file."
                + "\r\nPlease don't forget to change the file name as well.",
                "Reverse book contents",
                MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {

                return;
            }

            pTC.PText.ReverseContents();
            Recompute();
            UpdateWindowTitle();

        }

        private void aglonaReaderSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSite("https://sites.google.com/site/aglonareader/");
        }

        private void OpenSite(string p)
        {
            System.Diagnostics.Process.Start(p);
        }

        private void paraBooksMakerSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSite("https://sites.google.com/site/parabooksmaker/");
        }

        private void deletePairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeletePair(true);
        }

        private void DeletePair(bool ask)
        {
            if (!pTC.EditMode || pTC.Number == 0)
                return;

            if (ask)
            {
                if (MessageBox.Show("Really delete this pair?\nThis cannot be undone.", "Delete Pair", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            pTC.PText.DeletePair(pTC.HighlightedPair);
            pTC.Modified = true;
            GotoChangedPair();
        }

        private void insertPairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertPair(true);
        }

        private void InsertPair(bool insertAfter)
        {
            if (!pTC.EditMode)
                return;

            int newIndex;

            if (pTC.Number == 0)
                newIndex = 0;
            else if (insertAfter)
                newIndex = pTC.HighlightedPair + 1;
            else
                newIndex = pTC.HighlightedPair;

            TextPair newTp = new TextPair("", "", true, true);

            pTC.PText.TextPairs.Insert(newIndex, newTp);

            if (pTC.EditPair(newIndex, true))
            {
                GotoPair(newIndex, false, false, 2);
            }
            else
                pTC.PText.DeletePair(newIndex);
        }


        private void insertBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertPair(false);
        }


        private void normalModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.ChangeReadingMode(FileUsageInfo.NormalMode);
        }


        private void alternatingModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.ChangeReadingMode(FileUsageInfo.AlternatingMode);
        }


        private void advancedModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pTC.ChangeReadingMode(FileUsageInfo.AdvancedMode);
        }

    }
        
    
    public class FileUsageInfo
    {
        public const int NormalMode = 0;
        public const int AlternatingMode = 1;
        public const int AdvancedMode = 2;

        public const int AlternatingColorScheme_BlackGreen = 0;
        public const int AlternatingColorScheme_GreenBlack = 1;
        


        public string FileName { get; set; }
        public int Pair { get; set; }
        public int TopPair { get; set; }
        public bool Reversed { get; set; }
        public float SplitterRatio { get; set; }
        public bool EditMode { get; set; }
        public int ReadingMode { get; set; }
        public int AlternatingColorScheme { get; set; }
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
            Brightness = 0.974;
            FontName = "Arial";
            FontSize = 18.0F;
            FileUsages = new Collection<FileUsageInfo>();
        }

    }

}
