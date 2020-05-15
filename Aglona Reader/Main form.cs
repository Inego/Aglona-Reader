using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AglonaReader.Mp3Player;
using System.Text.RegularExpressions;

namespace AglonaReader
{
    
    internal partial class MainForm : Form
    {
        private const string DefaultTranslationSourceLanguage = "auto";
        private const string DefaultTranslationDestinationLanguage = "en";

        private const int TimerInterval = 100;
        private const int TimerHalfInterval = TimerInterval / 2;

        private readonly AudioPlayer player = new AudioPlayer();

        private readonly Timer playbackTimer = new Timer();
        private long nextTime = -1;
        private int nextPairToPlay = -1;

        private bool newBook;

        private AppSettings appSettings;

        private byte opState;
        private bool startedXMoving;
        private bool startedYMoving;
        private bool justJumped;
        private int prevHorizontalDistance;

        private FindForm findForm;
        private bool ctrlPressed;
        private int currentAudioFileNumber;

        private bool dragging;
        private int mouseYBeforeDragging;

        // Set to true if at least text was dragged at least for one line
        private bool draggingPerformed;

        private bool googleTranslatorEnabled;

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.RButton | Keys.ShiftKey))
                return true;
            
            return base.ProcessDialogKey(keyData);
        }

        private void pTC_MouseWheel(object sender, MouseEventArgs e)
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
            ctrlPressed = false;
            WindowState = FormWindowState.Maximized;

            playbackTimer.Interval = TimerInterval;
            playbackTimer.Tick += playbackTimer_Tick;

            currentAudioFileNumber = -1;
            splitContainer.Panel2Collapsed = true;
        }

        private void playbackTimer_Tick(object sender, EventArgs e)
        {
            var currentTime = player.CurrentTime;

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
            
            for (var i = pTC.HighlightedPair + 1; i < pTC.Number; i++)
            {
                if (pTC[i].AudioFileNumber != 0
                    && pTC[i].AudioFileNumber != currentAudioFileNumber)
                {
                    if (pTC.Hp.TimeEnd != 0)
                        nextTime = pTC.Hp.TimeEnd;
                    nextPairToPlay = i;
                    break;
                }
                if (pTC[i].TimeBeg != 0)
                {
                    nextTime = pTC[i].TimeBeg - TimerHalfInterval;
                    nextPairToPlay = i;
                    break;
                }
            }

            if (nextPairToPlay == -1 && pTC.Hp.TimeEnd != 0)
                nextTime = pTC.Hp.TimeEnd;

        }

        private void StartPlayback(bool fromNextPair)
        {
            if (playbackTimer.Enabled)
                StopPlayback();

            if (!pTC.WithAudio() || pTC.EditMode)
                return;

            var pairToStart = pTC.HighlightedPair + (fromNextPair ? 1 : 0);

            // Rewind forward until a pair with sound is found (if any)

            while (pairToStart < pTC.Number && pTC[pairToStart].AudioFileNumber == 0)
                pairToStart++;

            if (pairToStart >= pTC.Number || pTC[pairToStart].AudioFileNumber == 0)
                return;

            if (pTC.HighlightedPair != pairToStart)
                GotoPair(pairToStart, false, false, 0);
            
            var hp = pTC.Hp;

            if (SetFileToAudioPlayer(hp.AudioFileNumber))
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

            splitScreenVerticallyToolStripMenuItem.Checked = appSettings.SplitScreenVertically;
            splitContainer.Orientation = appSettings.SplitScreenVertically ? Orientation.Vertical : Orientation.Horizontal;

            var splitSideLength = appSettings.SplitScreenVertically ? splitContainer.Width : splitContainer.Height;
            splitContainer.SplitterDistance = (int)(splitSideLength * appSettings.WindowSplitterDistance);

            pTC.HighlightFragments = appSettings.HighlightFragments;
            pTC.HighlightFirstWords = appSettings.HighlightFirstWords;
            pTC.Brightness = appSettings.Brightness;
            
            pTC.SetFont(
                new Font(appSettings.FontName, appSettings.FontSize),
                new Font(appSettings.FontName, appSettings.FontSize, FontStyle.Italic));

            var args = Environment.GetCommandLineArgs();

            var outerLoad = args.Length > 1;

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

        private void SetEditMode(bool editMode)
        {
            pTC.EditMode = editMode;
            showGoogleTranslatorToolStripMenuItem.Enabled = !editMode;
            SetGoogleTranslatorEnabled(!editMode && showGoogleTranslatorToolStripMenuItem.Checked);
            editModeToolStripMenuItem.Checked = editMode;
        }

        private void LoadSettingsFromFileUsageInfo(FileUsageInfo f, bool load)
        {

            if (load)
            {
                pTC.PText.Load(f.FileName);
                currentAudioFileNumber = -1;
                UpdateWindowTitle();
                ctrlPressed = false;
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

            SetGoogleTranslatorEnabled(f.ShowGoogleTranslator);
            showGoogleTranslatorToolStripMenuItem.Checked = f.ShowGoogleTranslator;
        }

        private static bool IsNullOrWhiteSpace(string value)
        {
            return value == null || value.All(char.IsWhiteSpace);
        }

        private void UpdateWindowTitle()
        {
            var cpt = IsNullOrWhiteSpace(pTC.PText.Title1) ? "<No title>" : pTC.PText.Title1;

            if (!IsNullOrWhiteSpace(pTC.PText.Lang1) && !IsNullOrWhiteSpace(pTC.PText.Lang2))
                cpt += " [" + pTC.PText.Lang1 + "-" + pTC.PText.Lang2 + "]";

            if (pTC.WithAudio())
                cpt += " ♪";

            if (IsNullOrWhiteSpace(cpt))
                Text = "Aglona Reader";
            else
                Text = cpt + " - " + "Aglona Reader";

        }

        private bool XonSplitter(int x)
        {
            return pTC.layoutMode == ParallelTextControl.LayoutMode_Normal
                   && x >= pTC.SplitterPosition
                   && x < pTC.SplitterPosition + pTC.SplitterWidth;
        }

        private void parallelTextControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X == pTC.LastMouseX && e.Y == pTC.LastMouseY)
                return;

            pTC.LastMouseX = e.X;
            pTC.LastMouseY = e.Y;

            if (dragging)
            {
                if (pTC.Cursor != Cursors.SizeNS)
                {
                    pTC.Cursor = Cursors.SizeNS;
                }

                if (Math.Abs(mouseYBeforeDragging - e.Y) > pTC.lineHeight)
                {
                    var deltaY = mouseYBeforeDragging - e.Y;
                    var linesToScroll = deltaY / pTC.lineHeight;
                    var rest = deltaY % pTC.lineHeight;
                    mouseYBeforeDragging = e.Y - rest;

                    for (var i = 0; i < Math.Abs(linesToScroll); ++i)
                    {
                        if (linesToScroll > 0)
                            ProcessDownArrow(true);
                        else
                            ProcessUpArrow(true);
                    }

                    draggingPerformed = true;
                }
            }
            else if (XonSplitter(pTC.LastMouseX) || opState == 1)
                Cursor = Cursors.VSplit;
            else if (pTC.MousePressed && pTC.EditMode)
            {

                if (!startedXMoving)
                {
                    var distanceY = pTC.verticalStartingPosition - e.Y;
                    if (distanceY >= ParallelTextControl.HorizontalMouseStep)
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
                    var distance = (e.X - pTC.horizontalStartingPosition) / ParallelTextControl.HorizontalMouseStep;

                    if (distance != prevHorizontalDistance)
                    {

                        Cursor = Cursors.SizeWE;

                        startedXMoving = true;

                        var p = pTC.Hp;

                        var forward = distance > prevHorizontalDistance;

                        prevHorizontalDistance = distance;

                        byte needToRender = 0;

                        var movedRec1 = pTC.NextRecommended(1, forward);
                        var movedRec2 = pTC.NextRecommended(2, forward);

                        if (movedRec1)
                        {
                            needToRender += 1;
                            pTC.Side1Set = true;
                        }

                        if (movedRec2)
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
                
                var newSplitterPosition = pTC.LastMouseX - pTC.SplitterMoveOffset;

                if (newSplitterPosition == pTC.SplitterPosition) return;
                
                pTC.SplitterPosition = newSplitterPosition;
                pTC.SetSplitterRatioByPosition();
                    
                Recompute();
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
            if (e.Button != MouseButtons.Left)
                return;

            pTC.MousePressed = true;

            if (opState != 0) return;
            
            if (XonSplitter(e.X))
            {
                opState = 1;
                pTC.SplitterMoveOffset = e.X - pTC.SplitterPosition;
            }

            else if (pTC.EditMode)
            {
                if (pTC.MouseCurrentWord == null && pTC.mouseTextWord != null
                                                 && pTC.HighlightedPair != pTC.mouseTextWord.PairIndex)
                {
                    // Set focus to this pair
                    pTC.HighlightedPair = pTC.mouseTextWord.PairIndex;

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

                var p = pTC.Hp;

                startedXMoving = false;
                startedYMoving = false;
            }
            else
            {
                // In view mode mouse down begins selection

                if (ctrlPressed)
                {
                    if (pTC.mouseTextWord == null)
                        return;

                    if (!pTC.SelectionFinished) return;
                        
                    pTC.SelectionFinished = false;
                    pTC.SelectionSide = pTC.mouseTextWord.Side;
                    pTC.Selection1Pair = pTC.mouseTextWord.PairIndex;
                    pTC.Selection2Pair = pTC.mouseTextWord.PairIndex;
                    pTC.Selection1Position = pTC.mouseTextWord.Pos;
                    pTC.Selection2Position = pTC.mouseTextWord.Pos;
                    pTC.SelectionFrame.Visible = true;
                    pTC.SelectionFrame.Side = pTC.mouseTextWord.Side;
                    pTC.SelectionFrame.Line1 = pTC.mouseTextWord.Line;
                    pTC.SelectionFrame.Line2 = pTC.mouseTextWord.Line;
                    pTC.SelectionFrame.X1 = pTC.mouseTextWord.FX1;
                    pTC.SelectionFrame.X2 = pTC.mouseTextWord.FX2;
                    pTC.Render();
                }
                else
                {
                    dragging = true;
                    draggingPerformed = false;
                    pTC.Capture = true;
                    mouseYBeforeDragging = e.Y;
                }
            }
        }

        private void parallelTextControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (pTC.EditMode)
                    SeparateCurrentPair();
                else
                {
                    if (pTC.layoutMode == ParallelTextControl.LayoutMode_Advanced)
                        pTC.SwitchAdvancedShowPopups();
                }

                return;
            }

            if (!pTC.MousePressed)
                return;

            pTC.MousePressed = false;

            if (dragging)
            {
                dragging = false;
                pTC.Capture = false;
                pTC.Cursor = Cursors.Default;

                if (draggingPerformed || !googleTranslatorEnabled) return;
                
                var word = pTC.GetWordAtPosition(e.X, e.Y);

                if (word != null)
                {
                    TranslateText(word.Word, word.Side);
                }

                return;
            }

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

                        var selectedText = GetSelectedText();
                        
                        if (string.IsNullOrEmpty(selectedText)) return;
                        
                        Clipboard.SetText(selectedText);

                        if (googleTranslatorEnabled)
                        {
                            TranslateText(selectedText, pTC.SelectionSide);
                        }
                    }
                    else
                    {
                        if (!pTC.WithAudio() || pTC.mouseTextWord == null) return;
                        // Move to this fragment
                        if (pTC.mouseTextWord.PairIndex != pTC.HighlightedPair)
                        {

                            if (pTC.NotFitOnScreen(pTC.mouseTextWord.PairIndex))
                                GotoPair(pTC.mouseTextWord.PairIndex, false, false, 1);
                            else
                            {
                                pTC.HighlightedPair = pTC.mouseTextWord.PairIndex;
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

        private void TranslateText(string text, int textSide)
        {
            var srcLang = DefaultTranslationSourceLanguage;
            var destLang = DefaultTranslationDestinationLanguage;

            if (pTC.PText != null)
            {
                if (!string.IsNullOrEmpty(pTC.PText.Lang1))
                {
                    if (textSide == 1)
                        srcLang = pTC.PText.Lang1;
                    else
                        destLang = pTC.PText.Lang1;
                }

                if (!string.IsNullOrEmpty(pTC.PText.Lang2))
                {
                    if (textSide == 2)
                        srcLang = pTC.PText.Lang2;
                    else
                        destLang = pTC.PText.Lang2;
                }
            }

            var urlString = webBrowser.Url != null ? webBrowser.Url.ToString() : string.Empty;
            var match = Regex.Match(urlString, getGoogleTranslateUrl() + "#([a-z0-9]+?)/([a-z0-9]+?)/.*");
            
            var currentDestLang = destLang;

            if (match.Groups.Count == 3)
            {
                currentDestLang = match.Groups[2].Value;
            }

            // preserve current destination language if it differs from source language
            // Use case: the user is reading a book in English and German, but is actually a Russian native speaker,
            // so the Russian language should be the permanent destination language
            if (srcLang != currentDestLang)
            {
                destLang = currentDestLang;
            }

            webBrowser.Navigate(string.Format(getGoogleTranslateUrl() + @"#{0}/{1}/{2}", srcLang, destLang, text));
        }

        private void MouseUpInEditMode()
        {
            var needToRender = false;
            var needToUpdateStatusBar = false;
            var needToNip = false;

            if (pTC.MouseCurrentWord?.Next != null)
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
                        ((Frame) pTC.NippingFrame.F1).FramePen = pTC.CorrectedPen;
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
                        ((Frame) pTC.NippingFrame.F2).FramePen = pTC.CorrectedPen;
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
            pTC.mouseTextLine = -1;
            pTC.MouseCurrentWord = null;
            pTC.ProcessLayoutChange(true);
        }

        private void pTC_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData & Keys.ControlKey) == Keys.ControlKey)
                ctrlPressed = true;

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
                var n = pTC.Number;
                if (n == 0)
                    return;
                if (pTC.HighlightedPair != n - 1)
                    GotoPair(n - 1, false, false, 1);

            }

            else if (e.KeyData == (Keys.Control | Keys.Home))
            {
                var n = pTC.Number;
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
            var side = pTC.Reversed ? (byte)(3 - screenSide) : screenSide;

            var current = side == 1 ? pTC.NaturalDividerPosition1W : pTC.NaturalDividerPosition2W;

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
                ((Frame) pTC.NippingFrame.F1).FramePen = pTC.CorrectedPen;
                pTC.Side1Set = true;
            }
            else
            {
                pTC.NaturalDividerPosition2W = current;
                pTC.NaturalDividerPosition2 = current.Pos;
                ((Frame) pTC.NippingFrame.F2).FramePen = pTC.CorrectedPen;
                pTC.Side2Set = true;
            }

            pTC.SetNippingFrameByScreenWord(side, current);

            pTC.Render();

        }


        private void MergeWithPrevious()
        {
            if (!pTC.EditMode || pTC.HighlightedPair <= 0) return;
            
            pTC.MergePairs(pTC.HighlightedPair - 1);
            pTC.HighlightedPair--;

            if (pTC.CurrentPair > pTC.HighlightedPair)
                pTC.CurrentPair = pTC.HighlightedPair;

            GotoChangedPair();
        }

        private void GotoChangedPair()
        {

            if (pTC.HighlightedPair >= pTC.Number)
                pTC.HighlightedPair = pTC.Number - 1;

            pTC.PairChanged(pTC.HighlightedPair, true);
            GotoPair(pTC.HighlightedPair, false, true, 2);

            pTC.Hp.UpdateTotalSize();
            pTC.PText.UpdateAggregates(pTC.HighlightedPair);
            UpdateStatusBar(true);
            pTC.Side1Set = false;
            pTC.Side2Set = false;
        }

        private string GetSelectedText()
        {
            if (pTC.SelectionSide == 0 || !pTC.SelectionFinished) return null;
            
            var selectedText = new StringBuilder();

            pTC.AssignProperSelectionOrder(out var y1, out var y2, out var x1, out var x2);


            if (y1 == y2)
            {
                // The simplest case - text selected inside one pair

                // If there is more than one word...
                if (x1 < x2)
                    selectedText.Append(pTC[y1].Substring(pTC.SelectionSide, x1, x2 - x1));
                    
            }
            else
            {
                // The text spans across several pairs

                var currentPair = y1;

                while (true)
                {
                    if (currentPair > y1)
                        // Insert space or linebreak, depending on Startparagraph
                        if (pTC[currentPair].StartParagraph(pTC.SelectionSide))
                            selectedText.Append("\r\n");
                        else if (pTC.WesternJoint(currentPair - 1, pTC.SelectionSide))
                            selectedText.Append(' ');

                    if (currentPair == y1)
                        selectedText.Append(pTC[currentPair].Substring(pTC.SelectionSide, x1));
                    else if (currentPair == y2)
                        selectedText.Append(pTC[currentPair].Substring(pTC.SelectionSide, 0, x2));
                    else
                        selectedText.Append(pTC[currentPair].GetText(pTC.SelectionSide));

                    currentPair++;

                    if (currentPair > y2)
                        break;
                }
            }

            // Append the last word starting in Y2 from X2 
            selectedText.Append(ParallelTextControl.GetWord(pTC[y2], pTC.SelectionSide, x2));

            return selectedText.ToString();
        }

        private void CopyToClipboard()
        {
            var selectedText = GetSelectedText();

            if (!string.IsNullOrEmpty(selectedText)) 
                Clipboard.SetText(selectedText);
        }

        private void ProcessPageDown()
        {
            // Let's find the last pair that fully fits on normal lines
            // then rewind to the next of it
            if (pTC.LastRenderedPair == pTC.CurrentPair) return;
            
            var pairIndex = pTC.LastRenderedPair;

            while (pairIndex > 0 && pTC.NotFitOnScreen(pairIndex))
                pairIndex--;

            pairIndex++;

            if (pairIndex > pTC.Number - 1)
                pairIndex = pTC.Number - 1;

            GotoPair(pairIndex, false, false, 1);
        }

        private void ProcessPageUp()
        {
            var newCurrentPair = pTC.CurrentPair;
            var req = pTC.LastFullScreenLine;
            
            var accLines = 0; // Accumulated lines

            while (newCurrentPair > 0)
            {
                newCurrentPair--;
                var processedPair = pTC[newCurrentPair];

                if (!(processedPair.AllLinesComputed1 && processedPair.AllLinesComputed2))
                    pTC.PrepareScreen(newCurrentPair, -1);

                accLines += processedPair.Height;

                if (accLines <= req) continue;
                
                newCurrentPair++;
                break;
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

            switch (playType)
            {
                case 0:
                    StopPlayback();
                    break;
                case 1:
                    PlayCurrentPhrase();
                    break;
            }
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

            var totalVolume = pTC[pTC.Number - 1].aggregateSize;

            if (totalVolume == 0)
                ssPositionPercent.Text = "---";

            else
            {
                int prevVolume;

                prevVolume = pTC.HighlightedPair == 0 ? 0 : pTC[pTC.HighlightedPair - 1].aggregateSize;

                var s = ((float)prevVolume / totalVolume).ToString("P3");

                if (pTC.EditMode)
                {
                    s += "    ";

                    if (pTC.stopwatchStarted)
                    {
                        s += pTC.stopWatch.IsRunning ? "Aligning..." : "Aligning [PAUSED]";

                        if (pTC.Number > pTC.startingNumberOfFrags)
                        {
                            // Let's count speed
                            var elapsed = pTC.stopWatch.ElapsedMilliseconds;

                            s += "  " + ((long)((double)(pTC.Number - pTC.startingNumberOfFrags) * 3600000 / elapsed)).ToString() + " f/hr";

                            // current top percent

                            float currentPercent;

                            if (pTC.Number > 1)
                                currentPercent = (float)pTC[pTC.Number - 2].aggregateSize / pTC[pTC.Number - 1].aggregateSize;
                            else
                                currentPercent = 0;

                            if (currentPercent > pTC.startingPercent)
                            {
                                var percentDiff = currentPercent - pTC.startingPercent;

                                s += "  Aligned " + percentDiff.ToString("P3");

                                var whatIsLeft = 1.0f - currentPercent;

                                //float percentSpeed = percentDiff / elapsed;

                                var secondsLeft = (int)(whatIsLeft / percentDiff * elapsed / 1000);

                                var hoursLeft = secondsLeft / 3600;

                                secondsLeft -= hoursLeft * 3600;

                                var minutesLeft = secondsLeft / 60;

                                secondsLeft -= minutesLeft * 60;

                                s += "  ETA " + $"{hoursLeft:00}:{minutesLeft:D2}:{secondsLeft:D2}";
                            }
                        }
                    }

                    else
                        s += " Aligning not started";
                }

                ssPositionPercent.Text = s;
            }

            if (!updateScrollBar) return;
            
            vScrollBar.Maximum = pTC.Number == 0 ? 0 : pTC.Number - 1;
            vScrollBar.Value = pTC.HighlightedPair;
        }

        private void PlayCurrentPhrase()
        {
            if (pTC.EditMode || !pTC.WithAudio()) return;
            
            if (playbackTimer.Enabled)
                StopPlayback();

            var tp = pTC.Hp;

            var audioFileNumber = tp.AudioFileNumber;

            if (audioFileNumber == 0)
                return;

            var beg = tp.TimeBeg;

            var end = tp.TimeEnd;

            for (var i = pTC.HighlightedPair + 1; end == 0 & i < pTC.Number; i++)
            {
                var p = pTC[i];
                if (p.AudioFileNumber == 0)
                    continue;
                if (p.AudioFileNumber != audioFileNumber)
                    break;
                if (p.TimeBeg < beg) continue;
                
                end = p.TimeBeg;
                break;
            }

            if (SetFileToAudioPlayer(audioFileNumber))
                player.PlayFromTo(beg, end);
        }

        private bool SetFileToAudioPlayer(uint audioFileNumber)
        {
            if (audioFileNumber == currentAudioFileNumber) return true;
            
            var folder = Path.GetDirectoryName(pTC.PText.FileName);
            var fName = $@"{folder}\{audioFileNumber}.mp3";

            if (File.Exists(fName))
                player.Open(fName);
            else
                return false;

            currentAudioFileNumber = (int)audioFileNumber;

            return true;
        }

        private void ProcessUpArrow(bool withPlayback)
        {
            if (pTC.HighlightedPair == 0)
                return;

            if (pTC.EditMode || pTC.WithAudio())
            {
                var prevP = pTC.Hp;

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

                if (!pTC.WithAudio()) return;
                
                if (withPlayback)
                    PlayCurrentPhrase();
                else
                    StopPlayback();

            }
            else
            {
                // View mode (simple text)
                var newCurrentPair = pTC.CurrentPair;

                while (newCurrentPair > 0)
                {
                    newCurrentPair--;
                    var processedPair = pTC[newCurrentPair];

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
                    GotoPair(pTC.HighlightedPair, true, false, withPlayback ? 1 : 0);
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
                var pairIndex = pTC.CurrentPair;

                while (pTC[pairIndex].Height == 0 && pairIndex < pTC.Number - 1)
                    pairIndex++;
                if (pairIndex < pTC.Number - 1)
                    pairIndex++;
                if (pairIndex != pTC.CurrentPair)
                    GotoPair(pairIndex, true, false, 2);
            }
        }

        private void ChangeNatural(byte screenSide, bool inc)
        {
            if (!pTC.EditMode)
                return;

            var p = pTC.Hp;

            byte side;

            if (screenSide == 0)
                side = 0;
            else if (pTC.Reversed)
                side = (byte) (3 - screenSide);
            else
                side = screenSide;

            var needToRender = false;

            if (side == 0 || side == 1)
                if (pTC.NextRecommended(1, inc))
                {
                    needToRender = true;
                    pTC.UpdateFramesOnScreen(1);
                    pTC.Side1Set = true;
                }

            if (side == 0 || side == 2)
                if (pTC.NextRecommended(2, inc))
                {
                    needToRender = true;
                    pTC.UpdateFramesOnScreen(2);
                    pTC.Side2Set = true;
                }

            if (needToRender)
                pTC.Render();

        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!pTC.EditMode)
                return;

            using (var importTextForm = new ImportTextForm())
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
            using (var f = new BookInfoForm())
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
                using (var d = new SaveFileDialog())
                {
                    d.Filter = "Parallel Books (*.pbo)|*.pbo";
                    d.RestoreDirectory = true;
                    var dialogResult = d.ShowDialog();

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
            // Let's check whether this file exists in the list

            for (var index = 0; index < appSettings.FileUsages.Count; index++)
            {
                if (appSettings.FileUsages[index].FileName != fileName) continue;
                
                if (index == 0) return true;

                var toMove = appSettings.FileUsages[index];
                appSettings.FileUsages.Remove(toMove);
                appSettings.FileUsages.Insert(0, toMove);
                
                return true;
            }

            var fileUsageInfo = new FileUsageInfo {FileName = fileName};

            appSettings.FileUsages.Insert(0, fileUsageInfo);

            return false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (AskToSaveModified(sender) == DialogResult.Cancel)
                return;

            using (var openFileDialog = new OpenFileDialog())
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
            pTC.mouseTextLine = -1;

            var result = pTC.PText.Load(fileName);

            ctrlPressed = false;

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

            if (webBrowser.Url != null)
            {
                webBrowser.Navigate(getGoogleTranslateUrl());
                webBrowser.Refresh();
            }

            return result;
        }

        private void SaveAppSettings()
        {
            if (appSettings.FileUsages.Count > 0 && !newBook)
            {
                var f = appSettings.FileUsages[0];

                f.Pair = pTC.HighlightedPair;
                f.TopPair = pTC.CurrentPair;
                f.Reversed = pTC.Reversed;
                f.SplitterRatio = pTC.SplitterRatio;
                f.EditMode = pTC.EditMode;
                f.ReadingMode = pTC.ReadingMode;
                f.AlternatingColorScheme = pTC.AlternatingColorScheme;
                f.ShowGoogleTranslator = googleTranslatorEnabled;
            }

            appSettings.HighlightFragments = pTC.HighlightFragments;
            appSettings.HighlightFirstWords = pTC.HighlightFirstWords;
            
            appSettings.Brightness = pTC.Brightness;

            appSettings.FontName = pTC.textFont.Name;
            appSettings.FontSize = pTC.textFont.Size;

            appSettings.SplitScreenVertically = splitScreenVerticallyToolStripMenuItem.Checked;
            
            var splitSideLength = appSettings.SplitScreenVertically ? splitContainer.Width : splitContainer.Height;
            appSettings.WindowSplitterDistance = splitContainer.SplitterDistance / (float)splitSideLength;

            Properties.Settings.Default.AppSettings = appSettings;
            Properties.Settings.Default.Save();
        }

        private void pTC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!pTC.EditMode) return;
            
            switch (e.KeyChar)
            {
                case ' ':
                    SeparateCurrentPair();
                    break;
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
            using (var f = new AboutForm())
                f.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            player.Close();
            playbackTimer.Enabled = false;

            if (AskToSaveModified(sender) != DialogResult.Cancel) return;
            
            e.Cancel = true;
        }

        private DialogResult AskToSaveModified(object sender)
        {
            SaveAppSettings();

            if (!pTC.Modified)
                return DialogResult.No;

            var r = MessageBox.Show(
                    "Save modified book?", "The book was modified", MessageBoxButtons.YesNoCancel);

            if (r == DialogResult.Yes)
                saveToolStripMenuItem_Click(sender, EventArgs.Empty);

            return r;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AskToSaveModified(sender) == DialogResult.Cancel)
                return;

            pTC.CreateNewParallelBook();

            pTC.stopwatchStarted = false;

            UpdateStatusBar(true);

            UpdateWindowTitle();

            reverseToolStripMenuItem.Checked = false;
            pTC.ComputeSideCoordinates();

            pTC.SetLayoutMode();

            pTC.ProcessLayoutChange(false);

            pTC.HighlightedFrame.SetVisibility(false);
            pTC.NippingFrame.SetVisibility(false);
            pTC.AudioSingleFrame.Visible = false;

            SetEditMode(true);
            ProcessEditModeChange(false);

            showGoogleTranslatorToolStripMenuItem.Checked = false;
            SetGoogleTranslatorEnabled(false);

            newBook = true;
        }
                
        private void structureleftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BrowseBookStructure(1);
        }

        private void BrowseBookStructure(byte screenSide)
        {
            using (var f = new BookStructureForm())
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

            showGoogleTranslatorToolStripMenuItem.Enabled = !pTC.EditMode;
            SetGoogleTranslatorEnabled(!pTC.EditMode && showGoogleTranslatorToolStripMenuItem.Checked);

            pTC.SetLayoutMode();

            ProcessEditModeChange(false);

        }

        private void SetGoogleTranslatorEnabled(bool enabled)
        {
            if (googleTranslatorEnabled != enabled)
            {
                googleTranslatorEnabled = enabled;
                splitContainer.Panel2Collapsed = !enabled;

                pTC.ResizeBufferedGraphic();
                pTC.SetSplitterPositionByRatio();
                Recompute();
                
                if (enabled)
                {
                    webBrowser.Navigate(getGoogleTranslateUrl());
                }
            }
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

            if (AskToSaveModified(sender) == DialogResult.Cancel)
                return;

            using (var f = new OpenRecentForm())
            {
                f.AppSettings = appSettings;
                f.ShowDialog();
                if (!string.IsNullOrEmpty(f.Result))
                    LoadFromFile(f.Result);
            }

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (var settingsForm = new SettingsForm())
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
            using (var d = new SaveFileDialog())
            {
                d.Filter = "Text files (*.txt)|*.txt";
                
                d.RestoreDirectory = true;
                
                var dialogResult = d.ShowDialog();

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
                ctrlPressed = false;
        }

        private void reverseContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really reverse book contents?"
                + "\r\nThis will physically reverse the two texts in the file."
                + "\r\nPlease don't forget to change the file name as well.",
                "Reverse book contents",
                MessageBoxButtons.YesNo) == DialogResult.No)
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
            Process.Start(p);
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
                    == DialogResult.No)
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

            var newTp = new TextPair("", "", true, true);

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

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var exportHTMLForm = new ExportHTMLForm(pTC);
            exportHTMLForm.ShowDialog();
        }


        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            pTC.ResizeBufferedGraphic();
            pTC.SetSplitterPositionByRatio();
            Recompute();
        }

        private void showGoogleTranslatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!pTC.EditMode)
                SetGoogleTranslatorEnabled(showGoogleTranslatorToolStripMenuItem.Checked);
        }

        private void splitScreenVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var distanceCoef = splitContainer.Orientation == Orientation.Vertical ?
                splitContainer.Height / (float)splitContainer.Width :
                splitContainer.Width / (float)splitContainer.Height;
            var newSplitterDistance = (int)(splitContainer.SplitterDistance * distanceCoef);

            splitContainer.Orientation = 
                splitScreenVerticallyToolStripMenuItem.Checked ? Orientation.Vertical : Orientation.Horizontal;
            splitContainer.SplitterDistance = newSplitterDistance;
        }

        private string getGoogleTranslateUrl()
        {
            return "https://translate.google.ca/";
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
        public bool ShowGoogleTranslator { get; set; }
    }

    public class AppSettings
    {
        public bool HighlightFragments { get; set; }
        public bool HighlightFirstWords { get; set; }
        public double Brightness { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public float WindowSplitterDistance { get; set; }
        public bool SplitScreenVertically { get; set; }

        public Collection<FileUsageInfo> FileUsages { get; set; }

        public AppSettings()
        {
            HighlightFragments = true;
            HighlightFirstWords = true;
            Brightness = 0.974;
            FontName = "Arial";
            FontSize = 18.0F;
            FileUsages = new Collection<FileUsageInfo>();
            WindowSplitterDistance = 0.66f;
            SplitScreenVertically = true;
        }

    }

}
