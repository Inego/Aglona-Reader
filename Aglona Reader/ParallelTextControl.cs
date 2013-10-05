using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace AglonaReader
{

    public partial class ParallelTextControl : UserControl
    {

        public float startingPercent = 0.0F;
        public int startingNumberOfFrags = 0;
        public bool stopwatchStarted = false;
        public Stopwatch stopWatch;
        


        public const int LayoutMode_Normal = 0;
        public const int LayoutMode_Alternating = 1;
        public const int LayoutMode_Advanced = 2;

        public const int popUpOffsetX = 7;
        public const int popUpOffsetY = 7;

        public int LayoutMode;


        /// <summary>
        /// 0 = not set; 1 = black; 2 = gray
        /// </summary>
        int currentTextColor;


        IntPtr secondaryHDC;

        public byte SelectionSide { get; set; }
        public int Selection1Pair { get; set; }
        public int Selection1Position { get; set; }
        public int Selection2Pair { get; set; }
        public int Selection2Position { get; set; }
        public Frame SelectionFrame { get; set; }
        public bool SelectionFinished { get; set; }

        public Frame AdvancedHighlightFrame;
        private PopUpInfo popUpInfo;


        public int mouse_text_line = -1;
        private int mouse_text_x = -1;
        public ScreenWord mouse_text_word = null;

        public bool Modified { get; set; }

        public bool HighlightFirstWords { get; set; }
        public bool HighlightFragments { get; set; }

        // Length of a string to be considered a "big block"
        public const int BigTextSize = 1000;

        public const int horizontalMouseStep = 30;

        byte NumberofColors;

        // Contains H values of text color table
        List<double> colorTableH;
        List<SolidBrush> brushTable;
        List<Pen> penTable;
        List<Color> darkColorTable;
        List<Color> lightColorTable;
        Color grayColor; // Changes with Brightness


        public TextPair hp {
            get
            {
                if (PText.Number() == 0)
                    return null;
                return PText[HighlightedPair];
            }
        }

        private double brightness;
        public double Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                SetColorsByBrightness();
            }

        }

        private void SetColorsByBrightness()
        {

            if (brushTable.Count > 0)
            {
                foreach (SolidBrush s in brushTable)
                    s.Dispose();

                brushTable.Clear();

                foreach (Pen p in penTable)
                    p.Dispose();

                penTable.Clear();

                darkColorTable.Clear();
                lightColorTable.Clear();

            }

            Color lightColor;
            Color darkColor;

            for (byte i = 0; i < NumberofColors; i++)
            {

                lightColor = ColorRGB.HSL2RGB(colorTableH[i], 1, brightness);
                lightColorTable.Add(lightColor);

                brushTable.Add(new SolidBrush(lightColor));

                darkColor = ColorRGB.HSL2RGB(colorTableH[i], 1, brightness - 0.1);

                penTable.Add(new Pen(darkColor));
                darkColorTable.Add(darkColor);

            }

            grayColor = ColorRGB.HSL2RGB(0, 0, brightness - 0.1);
        }

        public bool EditWhenNipped { get; set; }

        public ScreenWord MouseCurrentWord { get; set; }

        EditPairForm editPairForm;

        public bool Side1Set { get; set; }
        public bool Side2Set { get; set; }

        Graphics PanelGraphics { get; set; }

        public int NaturalDividerPosition1 { get; set; }
        public int NaturalDividerPosition2 { get; set; }

        public ScreenWord NaturalDividerPosition1W { get; set; }
        public ScreenWord NaturalDividerPosition2W { get; set; }

        private SortedList<int, List<ScreenWord>> wordsOnScreen;

        public Font textFont;

        private Brush drawBrush = new SolidBrush(Color.Black);

        public int VMargin { get; set; }

        /// <summary>
        /// The current parallel text that is open
        /// </summary>
        public ParallelText PText { get; set; }


        /// <summary>
        /// Index of current Pair
        /// </summary>
        public int CurrentPair { get; set; }


        public TextPair this[int pairIndex]
        {
            get { return PText[pairIndex]; }
        }


        /// <summary>
        /// Buffered graphics on which we paint frames above rendered text + splitter page from SecondaryBG
        /// </summary>
        public BufferedGraphics PrimaryBG { get; set; }

        /// <summary>
        /// Buffered graphics on which we draw white frame, text and the splitter
        /// </summary>
        public BufferedGraphics SecondaryBG { set; get; }

        public int PanelMargin { get; set; }

        public void SetSplitterPositionByRatio()
        {
            SplitterPosition = (int)((Width * SplitterRatio) - splitterWidth / 2);
        }

        public void SetSplitterPositionByRatio(float newSplitterRatio)
        {
            SplitterRatio = newSplitterRatio;
            SetSplitterPositionByRatio();
        }

        public void SetSplitterRatioByPosition()
        {
            SplitterRatio = (splitterPosition + (float)splitterWidth / 2) / Width;
        }

        private int leftWidth;

        public int LeftWidth
        {
            get { return leftWidth; }

        }

        private int rightWidth;

        public int RightWidth
        {
            get { return rightWidth; }
        }

        /// <summary>
        /// X1 position of the right newSide
        /// </summary>
        public int RightPosition { get; set; }

        public byte MouseStatus { get; set; }

        public int SplitterMoveOffset { get; set; }

        public int horizontalStartingPosition = 0;

        public int LastMouseX { get; set; }
        public int LastMouseY { get; set; }

        public bool Reversed { get; set; }

        private byte frameoffset_x = 5;
        private int frameoffset_y = 2;



        /// <summary>
        /// Splitter position in pixels
        /// </summary>
        private int splitterPosition;


        public void ComputeSideCoordinates()
        {
            if (LayoutMode == LayoutMode_Alternating || LayoutMode == LayoutMode_Advanced)
            {
                text1start = PanelMargin - frameoffset_x;
                text1end = Width - PanelMargin + frameoffset_x;

                text2start = text1start;
                text2end = text1end;
            }

            else if (Reversed)
            {
                text1start = splitterPosition + splitterWidth + PanelMargin - frameoffset_x;
                text1end = Width - PanelMargin + frameoffset_x;

                text2start = PanelMargin - frameoffset_x;
                text2end = leftWidth - PanelMargin + frameoffset_x;
            }
            else
            {
                text1start = PanelMargin - frameoffset_x;
                text1end = leftWidth - PanelMargin + frameoffset_x;

                text2start = splitterPosition + splitterWidth + PanelMargin - frameoffset_x;
                text2end = Width - PanelMargin + frameoffset_x;
            }
        }

        public int SplitterPosition
        {
            get { return splitterPosition; }

            set
            {
                splitterPosition = value;

                leftWidth = splitterPosition;
                rightWidth = this.Width - splitterWidth - leftWidth;
                RightPosition = splitterPosition + splitterWidth;

                ComputeSideCoordinates();

            }
        }

        /// <summary>
        /// Splitter width in pixels
        /// </summary>
        private int splitterWidth;

        public int SplitterWidth
        {
            get { return splitterWidth; }

            set
            {
                splitterWidth = value;
            }
        }

        /// <summary>
        /// BackgroundBrush of the splitter
        /// </summary>
        Brush splitterBrush;

        public Pen HighlightedPen { get; set; }
        public Pen AudioPen { get; set; }
        public Pen SuggestedPen { get; set; }
        public Pen CorrectedPen { get; set; }

        public DoubleFrame HighlightedFrame { get; set; }
        public DoubleFrame NippingFrame { get; set; }
        public Frame AudioSingleFrame { get; set; } // Used for highlighting audio in Alternating and Advanced modes

        private Collection<AbstractFrame> frames;

        private int text1start;
        private int text1end;

        private int text2start;
        private int text2end;

        public int lineHeight;

        public StringFormat GT { get; set; } // Generic Typographic

        private SortedDictionary<string, int> widthDictionary;

        public int verticalStartingPosition;
        private int indentLength;
        
        private int Advanced_HighlightedPair;
        
        private Brush popUpBrush;
        
        private Color popUpTextColor;
        private bool AdvancedMode_ShowPopups;

        public int SpaceLength { get; set; }

        /// <summary>
        /// Number of lines that fit on screen according to the current TextFont and vertical size of the screen
        /// </summary>
        public int NumberOfScreenLines { get; set; }

        public int LastFullScreenLine { get; set; }

        public string DebugString { get; set; }

        public int HighlightedPair { get; set; }

        public int FirstRenderedPair { get; set; }
        public int LastRenderedPair { get; set; }

        public static void SetFramesByPair(TextPair textPair, DoubleFrame df)
        {
            if (df == null)
                return;

            if (textPair == null)
            {
                df.F1.Visible = false;
                df.F2.Visible = false;
            }
            else
            {
                df.F1.FillByRenderInfo(textPair.RenderedInfo1, 1);
                df.F2.FillByRenderInfo(textPair.RenderedInfo2, 2);
            }
        }


        public void SetFramesByPair(int pairIndex, DoubleFrame df)
        {
            SetFramesByPair(PText.TextPairs[pairIndex], df);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);

        public int WordWidth(string word, IDeviceContext graphics)
        {

            int result;

            // First, try to use data from the dictionary if it's there

            if (widthDictionary.TryGetValue(word, out result)) return result;
            else
            {
                // Measure and store in the dictionary

                Size sz;

                //result = TextRenderer.MeasureText(graphics, word, textFont, Size.Empty, TextFormatFlags.NoPadding).Width;

                GetTextExtentPoint32(secondaryHDC, word, word.Length, out sz);

                widthDictionary.Add(word, sz.Width);
                return sz.Width;
            }

        }

        /// <summary>
        /// Computes the recommended width between words in pixels
        /// </summary>
        /// <param name="graphics">Graphics on which the text is rendered</param>
        public void ComputeSpaceLength(IDeviceContext graphics)
        {
            widthDictionary.Clear();
            
            secondaryHDC = PanelGraphics.GetHdc();
            IntPtr last_font = SelectObject(secondaryHDC, textFont.ToHfont());

            SpaceLength = WordWidth(" ", graphics);

            ComputeIndent();

            DeleteObject(last_font);
            PanelGraphics.ReleaseHdc(secondaryHDC);

            lineHeight = textFont.Height;

            ComputeNumberOfScreenLines();
        }

        /// <summary>
        /// Calculates NumberOfScreenLines variable
        /// </summary>
        /// <param name="vSize">Vertical size of screen in pixels</param>
        public void ComputeNumberOfScreenLines()
        {
            NumberOfScreenLines = (Height - 2 * VMargin) / lineHeight;

            LastFullScreenLine = NumberOfScreenLines - 1;

            if (lineHeight * NumberOfScreenLines < Height - 2 * VMargin)
                NumberOfScreenLines++;

        }


        public void ResizeBufferedGraphic()
        {
            Graphics controlGraphics = CreateGraphics();

            PrimaryBG = BufferedGraphicsManager.Current.Allocate(controlGraphics, ClientRectangle);
            SecondaryBG = BufferedGraphicsManager.Current.Allocate(PrimaryBG.Graphics, ClientRectangle);

            PrimaryBG.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        public ParallelTextControl()
        {
            InitializeComponent();

            stopWatch = new Stopwatch();

            CreateNewParallelBook();

            SelectionFinished = true;

            wordsOnScreen = new SortedList<int, List<ScreenWord>>();

            VMargin = 3;
            PanelMargin = 10;

            LastMouseX = -1;
            LastMouseY = -1;

            splitterBrush = Brushes.LightGray;

            frames = new Collection<AbstractFrame>();

            HighlightedPen = Frame.CreatePen(Color.LightBlue, DashStyle.Solid, 4.0F);
            
            AudioPen = Frame.CreatePen(Color.Gray, DashStyle.Dot, 2.0F);

            HighlightedFrame = new DoubleFrame(HighlightedPen, frames);

            AudioSingleFrame = new Frame(AudioPen, frames);

            CorrectedPen = Frame.CreatePen(Color.Peru, DashStyle.Solid, 2.0F);

            NippingFrame = new DoubleFrame(SuggestedPen, frames);

            Pen selectionPen = Frame.CreatePen(Color.Black, DashStyle.Solid, 2.0F);

            SelectionFrame = new Frame(selectionPen, frames);
            

            GT = (StringFormat)StringFormat.GenericTypographic.Clone();

            widthDictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);

            // ADVANCED MODE POPUP
            popUpInfo = new PopUpInfo();
            Pen AdvancedHighlightPen = Frame.CreatePen(Color.SteelBlue, DashStyle.Solid, 4.0F);
            AdvancedHighlightFrame = new Frame(AdvancedHighlightPen, frames);
            int popUpOpacity = 210;
            popUpBrush = new SolidBrush(Color.FromArgb(popUpOpacity, Color.Black));
            popUpTextColor = Color.White;
            AdvancedMode_ShowPopups = false;
            
            PanelGraphics = CreateGraphics();

            textFont = new System.Drawing.Font("Arial", 18.0F);
            
            ComputeSpaceLength(PanelGraphics);

            EditWhenNipped = false;

            InitializeColors();

            Brightness = 0.97;

            HighlightFirstWords = true;
            HighlightFragments = true;

            SuggestedPen = Frame.CreatePen(Color.SteelBlue, DashStyle.Dash, 2.0F);

        }

        public void CreateNewParallelBook()
        {
            PText = new ParallelText();

            CurrentPair = 0;
            HighlightedPair = 0;

            Reversed = false;
            EditMode = true;

        }


        private void InitializeColors()
        {
            colorTableH = new List<double>();

            // CLASSIC (RAINBOW)

            //colorTableH.Add(0.162); // Yellow
            //colorTableH.Add(0.34);  // Green
            //colorTableH.Add(0.492); // Cyan
            //colorTableH.Add(0.68);  // Blue
            //colorTableH.Add(0.83);  // Pink
            //colorTableH.Add(0);     // Red
            ////colorTableH.Add(0.11); // Orange? Too close to yellow. Disable for now

            colorTableH.Add(0.162); // Yellow
            colorTableH.Add(0.83);  // Pink
            colorTableH.Add(0.34);  // Green
            colorTableH.Add(0.492); // Cyan
            colorTableH.Add(0);     // Red
            colorTableH.Add(0.68);  // Blue
            
            NumberofColors = (byte)colorTableH.Count;

            brushTable = new List<SolidBrush>();
            penTable = new List<Pen>();
            darkColorTable = new List<Color>();
            lightColorTable = new List<Color>();

        }


        public void DrawSecondary()
        {
            Graphics g = SecondaryBG.Graphics;

            g.Clear(Color.White);

            //graphics.FillRectangle(splitterBrush, splitterPosition, VMargin, splitterWidth, Height - 2 * VMargin);
        }

        public void DrawFrame(Frame frame)
        {
            if (frame == null)
                return;

            if (!frame.Visible)
                return;

            int textstart;
            int textend;

            if (frame.Side == 1)
            {
                textstart = text1start;
                textend = text1end;
            }
            else
            {
                textstart = text2start;
                textend = text2end;
            }

            Graphics g = PrimaryBG.Graphics;

            if (frame.Line1 == frame.Line2)
                if (frame.Line1 == -1)
                {
                    // The frame begins and ends beyond the screen
                    // We draw two parallel, unconnected lines on both sides
                    g.DrawLine(frame.FramePen, textstart, 0, textstart, Height - 1);
                    g.DrawLine(frame.FramePen, textend, 0, textend, Height - 1);
                }
                else
                    // A piece of text
                    g.DrawRectangle(frame.FramePen, textstart + frame.X1, VMargin + frame.Line1 * lineHeight - frameoffset_y,
                    frame.X2 - frame.X1 + 2 * frameoffset_x, lineHeight + 2 * frameoffset_y);

            else if (frame.Line1 == -1)
                g.DrawLines(frame.FramePen, new Point[]
                {
                    new Point(textstart, 0),
                    new Point(textstart, VMargin + (frame.Line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + frame.Line2 * lineHeight + frameoffset_y),
                    new Point(textend, VMargin + frame.Line2 * lineHeight + frameoffset_y),
                    new Point(textend, 0)
                });

            else if (frame.Line2 == -1)
                if (frame.X1 == 0) // Top starts at cursorX = 0
                    g.DrawLines(frame.FramePen, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, VMargin + frame.Line1 * lineHeight - frameoffset_y),
                        new Point(textend, VMargin + frame.Line1 * lineHeight - frameoffset_y),
                        new Point(textend, Height - 1)
                    });
                else
                    g.DrawLines(frame.FramePen, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, VMargin + (frame.Line1 + 1) * lineHeight - frameoffset_y),
                        new Point(textstart + frame.X1, VMargin + (frame.Line1 + 1) * lineHeight - frameoffset_y),
                        new Point(textstart + frame.X1, VMargin + frame.Line1 * lineHeight - frameoffset_y),
                        new Point(textend, VMargin + frame.Line1 * lineHeight - frameoffset_y),
                        new Point(textend, Height - 1)
                    });

            else if (frame.X1 == 0)
                g.DrawPolygon(frame.FramePen, new Point[]
                {
                    new Point(textend, VMargin + frame.Line1 * lineHeight - frameoffset_y),   
                    new Point(textstart, VMargin + frame.Line1 * lineHeight - frameoffset_y),
                    new Point(textstart, VMargin + (frame.Line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + frame.Line2 * lineHeight + frameoffset_y),
                    new Point(textend, VMargin + frame.Line2 * lineHeight + frameoffset_y)
                });
            else
                g.DrawPolygon(frame.FramePen, new Point[]
                {
                    new Point(textend, VMargin + frame.Line1 * lineHeight - frameoffset_y),
                    new Point(textstart + frame.X1, VMargin + frame.Line1 * lineHeight - frameoffset_y),
                    new Point(textstart + frame.X1, VMargin + (frame.Line1 + 1) * lineHeight - frameoffset_y),
                    new Point(textstart, VMargin + (frame.Line1 + 1) * lineHeight - frameoffset_y),
                    new Point(textstart, VMargin + (frame.Line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2) * lineHeight + frameoffset_y),
                    new Point(textend, VMargin + (frame.Line2) * lineHeight + frameoffset_y)
                });

        }


        public void Render()
        {
            SecondaryBG.Render();

            // Draw frames
            foreach (AbstractFrame f in frames)
                f.Draw(this);

            Graphics g = PrimaryBG.Graphics;

            if (!string.IsNullOrEmpty(DebugString))
                TextRenderer.DrawText(g, DebugString, textFont, new Point(PanelMargin, Height - lineHeight), Color.Red);

            if (EditWhenNipped)
                g.FillEllipse(Brushes.Red, Width - 13, 2, 10, 10);

            HighlightWord(MouseCurrentWord, Color.LightSkyBlue);

            RenderAdvancedPopup(g);

            PrimaryBG.Render();

        }

        private void RenderAdvancedPopup(Graphics g)
        {

            if (!(LayoutMode == LayoutMode_Advanced && popUpInfo.visible))
                return;

            DrawBackground(0, popUpInfo.Y, popUpInfo.X, popUpInfo.Y2, popUpInfo.X2, g, popUpBrush);

            secondaryHDC = PrimaryBG.Graphics.GetHdc();

            SetTextColor(secondaryHDC, ColorTranslator.ToWin32(popUpTextColor));
            SetBkMode(secondaryHDC, 1);


            IntPtr last_font = SelectObject(secondaryHDC, textFont.ToHfont());

            foreach(WordInfo sw in popUpInfo.words)
                TextOut(secondaryHDC, sw.X1 + PanelMargin + popUpInfo.X, VMargin + popUpInfo.offsetY * popUpOffsetY + (popUpInfo.Y + sw.Line) * lineHeight, sw.Word, sw.Word.Length);

            DeleteObject(last_font);

            PrimaryBG.Graphics.ReleaseHdc(secondaryHDC);

        }

        [DllImport("gdi32.dll")]
        static extern uint SetBkColor(IntPtr hdc, int crColor);

        public void HighlightWord(ScreenWord sw, Color color)
        {
            if (sw == null)
                return;

            //Graphics g = PrimaryBG.Graphics;

            secondaryHDC = PrimaryBG.Graphics.GetHdc();

            SetBkColor(secondaryHDC, ColorTranslator.ToWin32(color));
            SetBkMode(secondaryHDC, 2);
            SetTextColor(secondaryHDC, 0);

            IntPtr last_font = SelectObject(secondaryHDC, textFont.ToHfont());

            TextOut(secondaryHDC, sw.X1, VMargin + sw.Line * lineHeight, sw.Word, sw.Word.Length);

            DeleteObject(last_font);

            PrimaryBG.Graphics.ReleaseHdc(secondaryHDC);

            //TextRenderer.DrawText(g, sw.Word, textFont, new Point(sw.X1, VMargin + sw.Line * lineHeight),
            //    Color.Black, color, TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }

        bool NeedToLineBreakFirstWord(TextPair p, byte side, ref int occLength, ref int maxWidth, int sL, bool startParagraph)
        {
            if (occLength == 0) return false;
            if (startParagraph) return true;

            return (maxWidth - occLength - sL <= WordWidth(GetWord(p, side, 0), PanelGraphics));

        }

        public static string GetWord(TextPair p, byte side, int pos)
        {
            char c;

            StringBuilder word = new StringBuilder();

            int length = p.GetLength(side);

            while (pos < length)
            {
                c = p.GetChar(side, pos);
                if (c == ' ' || c == '\t')
                    if (word.Length == 0)
                    {
                        pos++;
                        continue;
                    }
                    else
                        break;

                if (c == '\n' || c == '\r')
                    break;

                if (IsEasternCharacter(c))
                    return c.ToString();

                word.Append(c);

                pos++;
            }

            return word.ToString();
        }

        /// <summary>
        /// Determines for the current Pair whether calculations are required
        /// and runs them if that'Word the case
        /// </summary>
        /// <param name="startPair">Index of the start Pair</param>
        /// <param name="limit">Number of lines</param>
        public void PrepareScreen(int startPair, int requiredLines)
        {

            if (PText.Number() == 0)
                return;

            if (LayoutMode == LayoutMode_Normal)
                PrepareScreen_Normal(startPair, requiredLines);

            else if (LayoutMode == LayoutMode_Alternating)
                PrepareScreen_Alternating(startPair, requiredLines);

            else if (LayoutMode == LayoutMode_Advanced)
                PrepareScreen_Advanced(startPair, requiredLines);

        }

        private void PrepareScreen_Advanced(int startPair, int requiredLines)
        {
            secondaryHDC = SecondaryBG.Graphics.GetHdc();

            // Required number of lines that we want to compute for the current Pair.
            // -1 means we want to compute ALL lines
            int requiredHeight;

            int remainder = requiredLines;

            int txtWidth = Width - 2 * PanelMargin;

            TextPair p;

            // If the startPair is not starting from a new Line on both texts (i. e. it is not a true-true Pair)
            // then we must ensure that all of the preceding pairs starting from the previous true-true pairs are computed,
            // because we need to know where exactly in the Line our Pair starts on both sides.
            // Actually, it is sufficient to stop at the closest partially-computed Pair (because if it is partially
            // computed we can safely compute it to the end)

            int cPair = startPair;

            byte side = (Reversed ? (byte) 2 : (byte) 1);

        Upstairs2:

            p = PText.TextPairs[cPair];

            // Look for the closest true-true or partially computed Pair
            if (!(p.StartParagraph(side)) && p.Height == -1)
            {
                cPair--;
                goto Upstairs2;
            }

            Collection<CommonWordInfo> words = new Collection<CommonWordInfo>();

            int occLength = 0; // Occupied length in the current Line

            int height;

            TextPair prev_pair = null;

            IntPtr last_font = SelectObject(secondaryHDC, textFont.ToHfont());

        NextPair2:

            if (cPair < startPair || requiredLines == -1)
                requiredHeight = -1;
            else
            {

                if (p.Height != -1 && remainder <= p.Height)
                    // cool
                    goto CommonExit2;

                requiredHeight = remainder;

            }

            height = p.Height;

            if (!(p.AllLinesComputed(side)))
            {

                if (p.Height == -1)
                {
                    PText.ComputedPairs.Add(p);
                    if (p.StartParagraph(side))
                        occLength = indentLength;
                }

                ProcessTextFromPair(p, side, ref occLength, words, ref height, ref txtWidth, requiredHeight);

                if (p.AllLinesComputed(side) && (p.StructureLevel > 0
                    || cPair + 1 < PText.Number() && PText.TextPairs[cPair + 1].StructureLevel > 0))
                {
                    ParallelText.InsertWords(words, 0);
                    height += 2;
                }

                p.Height = height;

            }

            if (requiredHeight != -1)
            {
                remainder -= height;

                if (remainder <= 0)
                    goto CommonExit2;

            }

            // Are there more text pairs?

            if (cPair + 1 == PText.Number())
            {
                ParallelText.InsertWords(words, 0);
                goto CommonExit2;
            }

            // ...There are.

            cPair++;

            prev_pair = p;

            p = PText.TextPairs[cPair];

            if (
                words.Count > 0 &&
                NeedToLineBreakFirstWord(p, side, ref occLength, ref txtWidth, SpaceLength, p.StartParagraph(side)))
            {
                ParallelText.InsertWords(words, (p.StartParagraph(side) ? 0 : txtWidth - occLength));

                prev_pair.Height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        goto CommonExit2;
                }

                occLength = (p.StartParagraph(side) ? indentLength : 0);

            }

            if (requiredLines == -1 && cPair > startPair && prev_pair.Height > 0)
                goto CommonExit2;

            goto NextPair2;

        CommonExit2:

            

            DeleteObject(last_font);
            SecondaryBG.Graphics.ReleaseHdc(secondaryHDC);
        }


        private void PrepareScreen_Alternating(int startPair, int requiredLines)
        {
            secondaryHDC = SecondaryBG.Graphics.GetHdc();

            // Required number of lines that we want to compute for the current Pair.
            // -1 means we want to compute ALL lines
            int requiredHeight;

            int remainder = requiredLines;

            int txtWidth = Width - 2 * PanelMargin;

            TextPair p;

            // If the startPair is not starting from a new Line on both texts (i. e. it is not a true-true Pair)
            // then we must ensure that all of the preceding pairs starting from the previous true-true pairs are computed,
            // because we need to know where exactly in the Line our Pair starts on both sides.
            // Actually, it is sufficient to stop at the closest partially-computed Pair (because if it is partially
            // computed we can safely compute it to the end)

            int cPair = startPair;

        Upstairs1:

            p = PText.TextPairs[cPair];

            // Look for the closest true-true or partially computed Pair
            if (!(p.StartParagraph1 || p.StartParagraph2) && p.Height == -1)
            {
                cPair--;
                goto Upstairs1;
            }

            Collection<CommonWordInfo> words = new Collection<CommonWordInfo>();

            int occLength = 0; // Occupied length in the current Line

            int height;

            TextPair prev_pair = null;

            IntPtr last_font = SelectObject(secondaryHDC, textFont.ToHfont());

            byte side1;
            byte side2;

            if (Reversed)
            {
                side1 = 2;
                side2 = 1;
            }
            else
            {
                side1 = 1;
                side2 = 2;
            }


        NextPair1:

            if (cPair < startPair || requiredLines == -1)
                requiredHeight = -1;
            else
            {

                if (p.Height != -1 && remainder <= p.Height)
                    // cool
                    goto CommonExit1;

                requiredHeight = remainder;

            }

            height = p.Height;

            if (!(p.AllLinesComputed1 && p.AllLinesComputed2))
            {

                if (p.Height == -1)
                {
                    PText.ComputedPairs.Add(p);
                    if (p.StartParagraph1 || p.StartParagraph2)
                        occLength = indentLength;
                }
                //else occLength = 0;

                ProcessTextFromPair(p, side1, ref occLength, words, ref height, ref txtWidth, requiredHeight);
                ProcessTextFromPair(p, side2, ref occLength, words, ref height, ref txtWidth, requiredHeight);


                if (p.AllLinesComputed1 && p.AllLinesComputed2
                    && (p.StructureLevel > 0
                    || cPair + 1 < PText.Number() && PText.TextPairs[cPair + 1].StructureLevel > 0))
                {
                    ParallelText.InsertWords(words, 0);

                    height += 2;
                }

                p.Height = height;

            }

            if (requiredHeight != -1)
            {
                remainder -= height;

                if (remainder <= 0)
                    goto CommonExit1;
            }

            // Are there more text pairs?

            if (cPair + 1 == PText.Number())
            {
                // This was the last Pair, no more coming.
                ParallelText.InsertWords(words, 0);
                goto CommonExit1;
            }

            // ...There are.

            cPair++;

            prev_pair = p;

            p = PText.TextPairs[cPair];

            if (words.Count > 0 && NeedToLineBreakFirstWord(p, side1, ref occLength, ref txtWidth, SpaceLength, p.StartParagraph1 || p.StartParagraph2))
            {
                ParallelText.InsertWords(words, (p.StartParagraph1 || p.StartParagraph2 ? 0 : txtWidth - occLength));

                prev_pair.Height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        goto CommonExit1;
                }

                occLength = (p.StartParagraph1 || p.StartParagraph2 ? indentLength : 0);

            }

            if (requiredLines == -1 && cPair > startPair && prev_pair.Height > 0)
                goto CommonExit1;

            goto NextPair1;

        CommonExit1:

            DeleteObject(last_font);
            SecondaryBG.Graphics.ReleaseHdc(secondaryHDC);
        }

        private void PrepareScreen_Normal(int startPair, int requiredLines)
        {
            secondaryHDC = SecondaryBG.Graphics.GetHdc();

            IntPtr last_font = SelectObject(secondaryHDC, textFont.ToHfont());

            // Required number of lines that we want to compute for the current Pair.
            // -1 means we want to compute ALL lines
            int requiredHeight;

            int remainder = requiredLines;

            TextPair p;

            // If the startPair is not starting from a new Line on both texts (i. e. it is not a true-true Pair)
            // then we must ensure that all of the preceding pairs starting from the previous true-true pairs are computed,
            // because we need to know where exactly in the Line our Pair starts on both sides.
            // Actually, it is sufficient to stop at the closest partially-computed Pair (because if it is partially
            // computed we can safely compute it to the end)

            int cPair = startPair;

        Upstairs:

            p = PText.TextPairs[cPair];

            // Look for the closest true-true or partially computed Pair
            if (!(p.StartParagraph1 && p.StartParagraph2) && p.Height == -1)
            {
                cPair--;
                goto Upstairs;
            }

            Collection<CommonWordInfo> words1 = new Collection<CommonWordInfo>();
            Collection<CommonWordInfo> words2 = new Collection<CommonWordInfo>();

            int occLength1 = 0; // Occupied length in the current Line
            int occLength2 = 0;

            int height1;
            int height2;
            int height;

            TextPair prev_pair = null;

            int width1 = (Reversed ? RightWidth : LeftWidth) - 2 * PanelMargin;
            int width2 = (Reversed ? LeftWidth : RightWidth) - 2 * PanelMargin;

        NextPair:

            if (cPair < startPair || requiredLines == -1)
                requiredHeight = -1;
            else
            {

                if (p.Height != -1 && remainder <= p.Height)
                    // cool
                    goto CommonExit;

                requiredHeight = remainder;

            }

            if (p.AllLinesComputed1 && p.AllLinesComputed2)
                height = p.Height;

            else
            {

                height1 = p.Height;
                height2 = p.Height;

                if (p.Height == -1)
                    PText.ComputedPairs.Add(p);

                ProcessTextFromPair(p, 1, ref occLength1, words1, ref height1, ref width1, requiredHeight);
                ProcessTextFromPair(p, 2, ref occLength2, words2, ref height2, ref width2, requiredHeight);

                // Now we must check whether one of the heights is smaller than the other

                height = height1;

                if (height1 < height2)
                {
                    // Line break 1
                    ParallelText.InsertWords(words1, 0);
                    occLength1 = 0;
                    height = height2;
                }
                else if (height2 < height1)
                {
                    // Line break 2
                    ParallelText.InsertWords(words2, 0);
                    occLength2 = 0;
                }

                if (p.AllLinesComputed1 && p.AllLinesComputed2
                    && (p.StructureLevel > 0
                    || cPair + 1 < PText.Number() && PText.TextPairs[cPair + 1].StructureLevel > 0))
                {
                    ParallelText.InsertWords(words1, 0);
                    ParallelText.InsertWords(words2, 0);
                    occLength1 = 0;
                    occLength2 = 0;
                    height += 2;
                }

                p.Height = height;

            }

            if (requiredHeight != -1)
            {
                remainder -= height;

                if (remainder <= 0)
                    goto CommonExit;
            }

            // Are there more text pairs?

            if (cPair + 1 == PText.Number())
            {
                // This was the last Pair, no more coming.
                ParallelText.InsertWords(words1, 0);
                ParallelText.InsertWords(words2, 0);
                goto CommonExit;
            }

            // ...There are.

            cPair++;

            prev_pair = p;

            p = PText.TextPairs[cPair];

            if (NeedToLineBreakFirstWord(p, 1, ref occLength1, ref width1, SpaceLength, p.StartParagraph1)
                    || NeedToLineBreakFirstWord(p, 2, ref occLength2, ref width2, SpaceLength, p.StartParagraph2))
            {
                ParallelText.InsertWords(words1, 0);
                ParallelText.InsertWords(words2, 0);

                prev_pair.Height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        goto CommonExit;
                }

                occLength1 = 0;
                occLength2 = 0;
            }

            if (requiredLines == -1 && cPair > startPair && prev_pair.Height > 0)
                goto CommonExit;

            goto NextPair;

        CommonExit:

            DeleteObject(last_font);
            SecondaryBG.Graphics.ReleaseHdc(secondaryHDC);
        }

        public void PrepareScreen()
        {
            PrepareScreen(CurrentPair, NumberOfScreenLines);
        }


        private void RenderBackground(Graphics g, int pairIndex, ref int offset, ref int cLine, byte side)
        {

            TextPair p = PText.TextPairs[pairIndex];

            Collection<WordInfo> list = p.ComputedWords(side);

            RenderedTextInfo renderedInfo = p.RenderedInfo(side);

            if (cLine >= NumberOfScreenLines
                || list == null
                || list.Count == 0)
            {
                renderedInfo.Valid = false;
                return;
            }

            WordInfo first = list[0];

            WordInfo last = list[list.Count - 1];

            if (cLine + last.Line < 0)
            {
                renderedInfo.Valid = false;
                return;
            }

            renderedInfo.Valid = true;

            if (cLine + first.Line < 0)
                renderedInfo.Line1 = -1;
            else
            {
                renderedInfo.Line1 = cLine + first.Line;
                renderedInfo.X1 = first.X1;
            }

            //bool alternating = (LayoutMode == LayoutMode_Alternating);

            if (cLine + last.Line >= NumberOfScreenLines || !(side == 1 ? p.AllLinesComputed1 : p.AllLinesComputed2))
                renderedInfo.Line2 = -1;
            else
            {
                renderedInfo.Line2 = cLine + last.Line;
                renderedInfo.X2 = last.X2;
                renderedInfo.X2B = last.X2;

                Collection<WordInfo> nextList = null;
                TextPair nextPair = null;

                if (LayoutMode == LayoutMode_Alternating)
                {

                    if (Reversed == (side == 1))
                        nextPair = p;
                    else if (pairIndex < PText.Number() - 1 && last.Line == p.Height)
                        nextPair = PText.TextPairs[pairIndex + 1];

                    if (nextPair != null)
                        nextList = nextPair.ComputedWords((byte)(3 - side));
                    
                }
                else
                {
                    // NORMAL and ADVANCED mode
                    if (pairIndex < PText.Number() - 1 && last.Line == p.Height)
                    {
                        nextPair = PText.TextPairs[pairIndex + 1];
                        nextList = nextPair.ComputedWords(side);
                    }
                }

                if (nextList != null && nextList.Count > 0)
                    if (nextList[0].X1 > last.X2)
                        renderedInfo.X2B += nextList[0].X1 - last.X2;

            }

            // Alternating and advanced mode don't use colored backgrounds
            if (LayoutMode != LayoutMode_Normal)
                return;

            bool big = ((side == 1 ? p.SB1 : p.SB2) != null);

            // Before drawing text we must draw colored background
            // Colored 
            if (!big && HighlightFragments)
                DrawBackground(side, renderedInfo.Line1, renderedInfo.X1, renderedInfo.Line2, renderedInfo.X2B, SecondaryBG.Graphics,
                    brushTable[pairIndex % NumberofColors]);

            if (HighlightFirstWords
                && !(big && HighlightFragments)
                && list != null
                && list.Count > 0
                && first.X2 >= first.X1)
            {
                Rectangle wordRect = new Rectangle(offset + first.X1, VMargin + (cLine + first.Line) * lineHeight, first.X2 - first.X1 + 1, lineHeight);

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    wordRect,
                    big ? grayColor : darkColorTable[pairIndex % NumberofColors],
                    HighlightFragments && !big ? lightColorTable[pairIndex % NumberofColors] : Color.White,
                    LinearGradientMode.Horizontal))

                    g.FillRectangle(brush, wordRect);

            }

        }

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
            string lpString, int cbString);

        [DllImport("gdi32.dll")]
        static extern uint SetTextColor(IntPtr hdc, int crColor);


        public bool NotFitOnScreenBase(int pairIndex)
        {
            if (pairIndex < CurrentPair)
                return true;

            TextPair p = PText[pairIndex];

            if (LayoutMode == LayoutMode_Advanced)
            {
                if (Reversed)
                    return (!p.RenderedInfo2.Valid
                        || p.RenderedInfo2.Line2 == -1
                        || p.RenderedInfo2.Line2 > LastFullScreenLine)
                        && !p.IsBig()
                        || p.RenderedInfo2.Line1 == -1;
                else
                    return (!p.RenderedInfo1.Valid
                        || p.RenderedInfo1.Line2 == -1
                        || p.RenderedInfo1.Line2 > LastFullScreenLine)
                        && !p.IsBig()
                        || p.RenderedInfo1.Line1 == -1;

            }
            else

                return (p.RenderedInfo1.Line2 == -1
                        || p.RenderedInfo1.Line2 > LastFullScreenLine
                        || p.RenderedInfo2.Line2 == -1
                        || p.RenderedInfo2.Line2 > LastFullScreenLine)
                        && !p.IsBig()
                        || !p.RenderedInfo1.Valid
                        || !p.RenderedInfo2.Valid
                        || p.RenderedInfo1.Line1 == -1
                        || p.RenderedInfo2.Line1 == -1;
        }

        public bool NotFitOnScreen(int pairIndex)
        {
            if (LastRenderedPair != 1 && pairIndex > LastRenderedPair)
                return true;

            return NotFitOnScreenBase(pairIndex);
        }

        private void RenderText(Graphics g, int pairIndex, ref int offset, ref int cLine, byte side)
        {

            TextPair p = PText.TextPairs[pairIndex];

            RenderedTextInfo renderedInfo = p.RenderedInfo(side);

            if (!renderedInfo.Valid)
                return;

            int newColor;

            if (!EditMode && (NotFitOnScreenBase(pairIndex)))
                newColor = 2;
            else if ((LayoutMode == LayoutMode_Alternating))
            {
                switch (AlternatingColorScheme)
                {
                    case FileUsageInfo.AlternatingColorScheme_GreenBlack:
                        if (Reversed == (side == 2))
                            newColor = 3;
                        else
                            newColor = 1;
                        break;
                    case FileUsageInfo.AlternatingColorScheme_BlackGreen:
                        if (Reversed == (side == 2))
                            newColor = 1;
                        else
                            newColor = 3;
                        break;
                    default:
                        newColor = 1;
                        break;
                }

            }
            else
                newColor = 1;

            if (newColor != currentTextColor)
            {
                currentTextColor = newColor;

                switch(newColor)
                {
                    case 1:
                        SetTextColor(secondaryHDC, ColorTranslator.ToWin32(Color.Black));
                        break;
                    case 2:
                        SetTextColor(secondaryHDC, ColorTranslator.ToWin32(Color.Gray));
                        break;
                    case 3:
                        SetTextColor(secondaryHDC, ColorTranslator.ToWin32(Color.ForestGreen));
                        break;
                }
                
            }

            Collection<WordInfo> list = p.ComputedWords(side);

            int x;
            int y = -1;

            ScreenWord prev_screen_word = null;
            ScreenWord s = null;
            List<ScreenWord> l = null;

            int prev_y = -1;

            if (list != null)
                for (int i = 0; i < list.Count; i++)
                {
                    WordInfo r = list[i];

                    y = cLine + r.Line;

                    if (y < 0)
                        continue;

                    if (y >= NumberOfScreenLines)
                    {
                        renderedInfo.Line2 = -1;
                        renderedInfo.X2 = 0;
                        return;
                    }

                    s = new ScreenWord();

                    if (prev_screen_word != null)
                    {
                        s.Prev = prev_screen_word;
                        prev_screen_word.Next = s;
                    }

                    prev_screen_word = s;

                    s.FX1 = r.X1;

                    x = s.FX1 + offset;

                    if (y != prev_y)
                    {
                        if (!wordsOnScreen.TryGetValue(y, out l))
                        {
                            l = new List<ScreenWord>();
                            wordsOnScreen.Add(y, l);
                        }
                        prev_y = y;

                        if (FirstRenderedPair == -1)
                            FirstRenderedPair = pairIndex;

                        if (pairIndex > LastRenderedPair)
                            LastRenderedPair = pairIndex;

                    }

                    string wrd = r.Word;

                    TextOut(secondaryHDC, x, VMargin + y * lineHeight, wrd, wrd.Length);

                    s.PairIndex = pairIndex;
                    s.Pos = r.Pos;
                    s.Side = side;
                    s.X1 = x;
                    s.FX2 = r.X2;
                    s.X2 = s.FX2 + offset;
                    s.Line = y;
                    s.Word = wrd;

                    l.Add(s);

                }
        }


        [DllImport("gdi32.dll")]
        static extern int SetBkMode(IntPtr hdc, int iBkMode);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr objectHandle);

        /// <summary>
        /// Renders a newSide
        /// </summary>
        /// <param name="newSide">Number of newSide in SCREEN terms, not in Pair terms</param>
        private void RenderPairText(byte side, int startPair, int negHeight)
        {

            Graphics g = SecondaryBG.Graphics;

            int offset;
            byte textSide;

            DefineVarsByLayoutMode(side, out offset, out textSide);

            int cPair = startPair;
            int cLine = -negHeight;

            secondaryHDC = SecondaryBG.Graphics.GetHdc();

            SetBkMode(secondaryHDC, 1);

            IntPtr last_font = SelectObject(secondaryHDC, textFont.ToHfont());

            // Text itself

            while (true)
            {

                TextPair p = PText.TextPairs[cPair];

                RenderText(g, cPair, ref offset, ref cLine, textSide);

                cLine += p.Height;

                if (cLine >= NumberOfScreenLines)
                    break;

                if (cPair < PText.Number() - 1)
                    cPair++;
                else
                    break;
            }

            DeleteObject(last_font);

            SecondaryBG.Graphics.ReleaseHdc(secondaryHDC);

        }


        public void RenderPairs(bool instantRender)
        {
            Advanced_HighlightedPair = -2;

            DrawSecondary();

            wordsOnScreen.Clear();

            if (PText.Number() == 0)
                return;

            TextPair p;

            int negHeight = 0;

            int startPair = CurrentPair;

            p = PText.TextPairs[startPair];

            FirstRenderedPair = -1;
            LastRenderedPair = -1;

            // REWIND

            if (LayoutMode == LayoutMode_Advanced)
            {

                byte side = (Reversed ? (byte) 2 : (byte) 1);

                // Special rewinding algorithm for advanced mode
                if (!(p.StartParagraph(side)))
                {
                    do
                    {
                        startPair--;
                        p = PText.TextPairs[startPair];
                    }
                    while (!(p.StartParagraph(side)) && p.Height == 0);

                    negHeight = p.Height;

                }

                currentTextColor = -1;

                RenderBackgroundSide(side, startPair, negHeight);
                RenderPairText(side, startPair, negHeight);

                if (instantRender)
                    Render();

                return;
                
            }
            else if (LayoutMode == LayoutMode_Alternating)
            {
                // Special rewinding algorithm for alternating mode    
                if (!(p.StartParagraph1 || p.StartParagraph2))
                {
                    do
                    {
                        startPair--;
                        p = PText.TextPairs[startPair];
                    }
                    while (!(p.StartParagraph1 || p.StartParagraph2) && p.Height == 0);

                    negHeight = p.Height;

                }

            }

            else // NORMAL mode
            {
                // Standard rewinding algorithm
                if (!(p.StartParagraph1 && p.StartParagraph2))
                {
                    // Must rewind back for the closest (from above) Pair that is either true-true or
                    // multi-Line (charIndex. e. with Height > 0)

                    do
                    {
                        startPair--;
                        p = PText.TextPairs[startPair];
                    }
                    while (!(p.StartParagraph1 && p.StartParagraph2) && p.Height == 0);

                    negHeight = p.Height;

                }

            }

            RenderBackgroundSide(1, startPair, negHeight);
            RenderBackgroundSide(2, startPair, negHeight);

            currentTextColor = -1;

            RenderPairText(1, startPair, negHeight);
            RenderPairText(2, startPair, negHeight);

            if (instantRender)
                Render();

        }

        private void RenderBackgroundSide(int side, int startPair, int negHeight)
        {

            Graphics g = SecondaryBG.Graphics;

            int offset;
            byte textSide;

            DefineVarsByLayoutMode(side, out offset, out textSide);

            
            int cPair = startPair;
            int cLine = -negHeight;

            // RenderedInfo and Backgrounds

            while (true)
            {

                TextPair p = PText.TextPairs[cPair];

                RenderBackground(g, cPair, ref offset, ref cLine, textSide);

                cLine += p.Height;

                if (cLine >= NumberOfScreenLines)
                    break;

                if (cPair < PText.Number() - 1)
                    cPair++;
                else
                    break;

            }

        }

        private void DefineVarsByLayoutMode(int side, out int offset, out byte textSide)
        {
            switch (LayoutMode)
            {
                case LayoutMode_Normal:

                    if (side == 1)
                        offset = PanelMargin;
                    else
                        offset = SplitterPosition + SplitterWidth + PanelMargin;

                    if (Reversed == (side == 1))
                        textSide = 2;
                    else
                        textSide = 1;

                    break;

                case LayoutMode_Alternating:

                    if (Reversed == (side == 1))
                        textSide = 1;
                    else
                        textSide = 2;

                    offset = PanelMargin;

                    break;

                case LayoutMode_Advanced:

                    offset = PanelMargin;

                    textSide = (byte)side;

                    break;

                default:
                    textSide = 0;
                    offset = 0;
                    break;
            }
        }

        public void UpdateFramesOnScreen(byte side)
        {
            if (PText.Number() == 0)
                return;

            if (EditMode)
            {
                TextPair h = PText.TextPairs[HighlightedPair];

                if (side == 0)
                    SetFramesByPair(h, HighlightedFrame);

                if (side == 0 || side == 1)
                {
                    NaturalDividerPosition1W = FindScreenWordByPosition(HighlightedPair, NaturalDividerPosition1, 1);
                    SetNippingFrameByScreenWord(1, NaturalDividerPosition1W);
                }

                if (side == 0 || side == 2)
                {
                    NaturalDividerPosition2W = FindScreenWordByPosition(HighlightedPair, NaturalDividerPosition2, 2);
                    SetNippingFrameByScreenWord(2, NaturalDividerPosition2W);
                }

                MouseCurrentWord = null;
            }

            else
            {                
                if (PText.WithAudio)
                {
                    TextPair h = PText.TextPairs[HighlightedPair];

                    if (side == 0)
                    {
                        if (LayoutMode == LayoutMode_Normal)
                            SetFramesByPair(h, HighlightedFrame);
                        else if (LayoutMode == LayoutMode_Alternating)
                        {

                            RenderedTextInfo _r1 = Reversed ? h.RenderedInfo2 : h.RenderedInfo1;
                            RenderedTextInfo _r2 = Reversed ? h.RenderedInfo1 : h.RenderedInfo2;

                            AudioSingleFrame.Line1 = _r1.Line1;
                            AudioSingleFrame.Line2 = _r2.Line2;
                            AudioSingleFrame.X1 = _r1.X1;
                            AudioSingleFrame.X2 = _r2.X2;

                        }
                        else // Advanced
                            AudioSingleFrame.FillByRenderInfo(Reversed ? h.RenderedInfo2 : h.RenderedInfo1, 0);
                    }

                }

                UpdateSelectionFrame();
                
            }

        }


        void ComputeIndent()
        {
            indentLength = (LayoutMode == LayoutMode_Normal ? 0 : SpaceLength * 8);
        }


        private void ProcessCurrentWord(StringBuilder word, ref int occLength, Collection<CommonWordInfo> words, ref int Height, TextPair p, byte side, ref int MaxWidth, ref int wordPosition, bool eastern)
        {

            // Current Word complete, let's get its length

            int wordLength;

            wordLength = WordWidth(word.ToString(), PanelGraphics);

            int newStart = occLength + (words.Count == 0 || eastern && words.Count > 0 && words[words.Count - 1].Eastern ? 0 : SpaceLength);

            if (newStart + wordLength > MaxWidth && words.Count != 0)
            {
                // Move this Word to the Next Line.
                // Before that we need to flush words to the DB

                ParallelText.InsertWords(words, MaxWidth - occLength);

                Height++;

                newStart = 0;

                occLength = 0;

            }

            // Add this Word to the current Line
            words.Add(new CommonWordInfo(p, word.ToString(), Height, newStart, newStart + wordLength - 1, wordPosition, eastern, side));
            occLength = newStart + wordLength;

            word.Length = 0;

        }

        void ProcessTextFromPair(TextPair p, byte side, ref int occLength, Collection<CommonWordInfo> words, ref int height, ref int MaxWidth, int requiredHeight)
        {
            if ((side == 1) ? p.AllLinesComputed1 : p.AllLinesComputed2)
                return;

            int pos;
            int wordPos;

            if (height == -1)
            {
                pos = 0;
                height = 0;
                if (side == 1)
                    p.ContinueFromNewLine1 = false;
                else
                    p.ContinueFromNewLine2 = false;
            }
            else
            {
                if (side == 1)
                {
                    pos = p.CurrentPos1;
                    if (p.ContinueFromNewLine1)
                    {
                        occLength = indentLength;
                        p.ContinueFromNewLine1 = false;
                    }
                    
                }
                else
                {
                    pos = p.CurrentPos2;
                    if (p.ContinueFromNewLine2)
                    {
                        occLength = indentLength;
                        p.ContinueFromNewLine2 = false;
                    }
                }
                
            }

            wordPos = -1;

            char c;

            StringBuilder word = new StringBuilder();

            int textLength = p.GetLength(side);

            while (pos < textLength)
            {
                // Must be slow
                c = p.GetChar(side, pos);

                if (c == ' ' || c == '\t' || c == '\r')
                {

                    if (word.Length == 0)
                    {
                        pos++;
                        continue;
                    }

                    ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos, false);

                    if (requiredHeight != -1 && requiredHeight == height)
                        goto CommonExit;

                    wordPos = -1;

                }
                else if (c == '\n')
                {
                    if (word.Length > 0)
                    {
                        ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos, false);
                        if (requiredHeight != -1 && requiredHeight == height)
                        {
                            wordPos = pos;
                            goto CommonExit;
                        }
                        wordPos = -1;
                    }

                    ParallelText.InsertWords(words, 0);

                    height++;
                    occLength = indentLength;

                    if (requiredHeight != -1 && requiredHeight == height)
                    {
                        //height--;
                        wordPos = ++pos;
                        if (side == 1)
                            p.ContinueFromNewLine1 = true;
                        else
                            p.ContinueFromNewLine2 = true;
                        goto CommonExit;
                    }


                }

                else if (IsEasternCharacter(c))
                {
                    if (word.Length != 0)
                    {
                        ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos, false);
                        if (requiredHeight != -1 && requiredHeight == height)
                            goto CommonExit;
                    }

                    word.Append(c);

                    ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref pos, true);

                    if (requiredHeight != -1 && requiredHeight == height)
                    {
                        wordPos = pos;
                        goto CommonExit;
                    }

                    wordPos = -1;

                }

                else
                {
                    if (wordPos == -1)
                        wordPos = pos;

                    word.Append(c);
                }

                pos++;

            }

            // Reached the end, process current Word (if there is any)
            if (word.Length > 0)
            {
                ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos, false);
                if (requiredHeight != -1 && requiredHeight == height)
                    goto CommonExit;
            }

            if (side == 1)
                p.AllLinesComputed1 = true;
            else
                p.AllLinesComputed2 = true;

            return;

            // Get here when the Height is reached
        CommonExit:

            if (side == 1)
            {
                p.CurrentPos1 = wordPos;
            }
            else
            {
                p.CurrentPos2 = wordPos;
            }

        }

        public static bool IsEasternCharacter(char c)
        {
            return (c >= (char)0x2e80
                && !(c >= (char)0xac00 && c <= (char)0xd7a3)); // Hangul
        }

        public void ProcessLayoutChange(bool updateScreen)
        {
            // erase both tables
            PText.Truncate();
            ComputeNumberOfScreenLines();
            UpdateScreen();
        }

        public ScreenWord FindScreenWordByPosition(int pairIndex, int pos, byte side)
        {
            if (pos != -1)
                foreach (KeyValuePair<int, List<ScreenWord>> kv in wordsOnScreen)
                    foreach (ScreenWord sw in kv.Value)
                        if (sw.PairIndex == pairIndex && sw.Pos == pos && sw.Side == side)
                            return sw;

            return null;

        }

        private bool PosIsOnOrAfterLastScreenWord(int pairIndex, int pos1, int pos2)
        {
            int lastPos1 = -1;
            int lastPos2 = -1;


            foreach (KeyValuePair<int, List<ScreenWord>> kv in wordsOnScreen)
                foreach (ScreenWord sw in kv.Value)
                    if (sw.PairIndex == pairIndex)
                    {
                        if (sw.Side == 1)
                        {
                            lastPos1 = sw.Pos;
                            if (pos1 == lastPos1 && sw.Line >= LastFullScreenLine)
                                return true;
                        }
                        else
                        {
                            lastPos2 = sw.Pos;
                            if (pos2 == lastPos2 && sw.Line >= LastFullScreenLine)
                                return true;
                        }
                    }

            return (pos1 > lastPos1 || pos2 > lastPos2);

        }


        public void SetNippingFrameByScreenWord(byte side, ScreenWord sw)
        {
            Frame f = (Frame)NippingFrame.Frame(side);

            if (sw == null || sw.Prev == null)
                f.Visible = false;
            else
            {
                Frame hf = (Frame)HighlightedFrame.Frame(side);
                f.Visible = true;
                f.FramePen = SuggestedPen;
                f.Line1 = hf.Line1;
                f.X1 = hf.X1;
                f.Line2 = sw.Prev.Line;
                f.X2 = sw.Prev.FX2;
            }
        }

        public ScreenWord WordAfterCursor(int line, int cursorX, int side)
        {
            List<ScreenWord> listOfWords;

            ScreenWord lastWord = null;

            if (wordsOnScreen.TryGetValue(line, out listOfWords))
            {
                // let's see...

                foreach (ScreenWord s in listOfWords)
                {
                    if (side != -1 && s.Side != side)
                        continue;
                    if (cursorX >= s.X1 &&(lastWord == null || lastWord.X1 < s.X1))
                        lastWord = s;
                }
            }

            return lastWord;
        }


        internal bool NipHighlightedPair()
        {
            if (NaturalDividerPosition1W == null
                    || NaturalDividerPosition2W == null)

                return false;

            TextPair np = new TextPair();

            TextPair hp = PText.TextPairs[HighlightedPair];

            np.StartParagraph1 = hp.StartParagraph1;
            np.StartParagraph2 = hp.StartParagraph2;

            NipASide(hp, np, 1);
            NipASide(hp, np, 2);

            np.AudioFileNumber = hp.AudioFileNumber;
            np.TimeBeg = hp.TimeBeg;
            np.TimeEnd = hp.TimeEnd;

            hp.AudioFileNumber = 0;
            hp.TimeBeg = 0;
            hp.TimeEnd = 0;

            PText.TextPairs.Insert(HighlightedPair, np);

            if (EditWhenNipped)
            {
                PrepareEditForm();

                editPairForm.ParallelTextControl = this;
                editPairForm.PairIndex = HighlightedPair;
                editPairForm.ShowDialog();
                EditWhenNipped = false;
            }

            PText[HighlightedPair].UpdateTotalSize();
            PText[HighlightedPair + 1].UpdateTotalSize();
            PText.UpdateAggregates(HighlightedPair);

            // Truncate all preceding pairs until true-true

            TextPair _p;
            int i = HighlightedPair;

            do
            {
                i--;
                if (i < 0)
                    break;
                _p = PText.TextPairs[i];
                _p.ClearComputedWords();
            }

            while (!_p.StartParagraph1 || !_p.StartParagraph2);

            hp.ClearComputedWords();


            // Truncate all following pairs until end or true-true

            HighlightedPair++;

            int j = HighlightedPair;

            TextPair _q;

            while (j < PText.Number() - 1)
            {
                j++;
                _q = PText[j];
                if (_q.StartParagraph1 && _q.StartParagraph2)
                    break;
                _q.ClearComputedWords();
            }

            if (!stopwatchStarted)
                ResetStopwatch(1);
            else
                if (!stopWatch.IsRunning)
                    stopWatch.Start();

            PrepareScreen();
            RenderPairs(false);

            FindFirstNaturalDividers();

            if (CurrentPair != HighlightedPair
                && PosIsOnOrAfterLastScreenWord(HighlightedPair, NaturalDividerPosition1, NaturalDividerPosition2))
            {
                CurrentPair = HighlightedPair;
                PrepareScreen();
                RenderPairs(false);
            }

            UpdateFramesOnScreen(0);
            ProcessMousePosition(true, false);
            Render();

            Side1Set = false;
            Side2Set = false;

            Modified = true;

            return true;

        }

        // mode
        //  0: don't change
        //  1: force start
        //  2: force stop
        //
        public void ResetStopwatch(int mode)
        {

            int whatToDo = mode == 0 ? (stopWatch.IsRunning ? 1 : 2) : mode;

            stopwatchStarted = true;
            
            stopWatch.Reset();
            
            if (whatToDo == 1)
                stopWatch.Start();
            
            startingNumberOfFrags = Number;

            if (Number > 1)
                startingPercent = (float)this[Number - 2].aggregateSize / this[Number - 1].aggregateSize;
            else
                startingPercent = 0;
        }



        private void NipASide(TextPair source_pair, TextPair target_pair, byte side)
        {
            int final_pos;

            StringBuilder source_sb = null;

            if (side == 1)
            {
                final_pos = NaturalDividerPosition1;
                if (source_pair.SB1 == null)
                {
                    source_pair.SB1 = new StringBuilder(source_pair.Text1);
                    source_pair.Text1 = null;
                }
                source_sb = source_pair.SB1;
            }
            else
            {
                final_pos = NaturalDividerPosition2;
                if (source_pair.SB2 == null)
                {
                    source_pair.SB2 = new StringBuilder(source_pair.Text2);
                    source_pair.Text2 = null;
                }
                source_sb = source_pair.SB2;
            }


            StringBuilder sb = new StringBuilder();

            int state = 0;
            char c;

            int pos = 0;

            while (pos < final_pos)
            {
                c = source_sb[pos];

                switch (c)
                {
                    case ' ':
                    case '\r':
                    case '\t':
                        if (state == 1)
                            state = 2;
                        break;
                    case '\n':
                        if (state > 0)
                            state = 3;
                        break;
                    default:
                        if (state == 2)
                            sb.Append(' ');
                        else if (state == 3)
                        {
                            sb.Append('\r');
                            sb.Append('\n');
                        }
                        sb.Append(c);
                        state = 1;
                        break;
                }

                pos++;

            }

            if (side == 1)
                target_pair.Text1 = sb.ToString();
            else
                target_pair.Text2 = sb.ToString();



            bool startParagraph = (state == 3);
            if (side == 1)
                source_pair.StartParagraph1 = startParagraph;
            else
                source_pair.StartParagraph2 = startParagraph;


            // Cut everything before final_pos in the source text

            source_sb.Remove(0, final_pos);

            if (source_sb.Length < BigTextSize)
            {
                if (side == 1)
                {
                    source_pair.Text1 = source_sb.ToString();
                    source_pair.SB1 = null;
                }
                else
                {
                    source_pair.Text2 = source_sb.ToString();
                    source_pair.SB2 = null;
                }
            }

        }


        void DrawBackground(byte side, int line1, int x1, int line2, int x2, Graphics g, Brush brush)
        {
            int textstart;
            int textend;
            int width;
            int oY; // vertical offset
            int mX; // Background's own X margin (used only in Advanced mode for popup background)

            if (LayoutMode == LayoutMode_Advanced)
            {
                textstart = PanelMargin;
                textend = Width - PanelMargin;
                width = Width - 2 * PanelMargin;
                oY = popUpInfo.offsetY * popUpOffsetY;
                mX = 5;
            }
            else if (side == 1 && !Reversed || side == 2 && Reversed)
            {
                textstart = PanelMargin;
                textend = leftWidth - PanelMargin;
                width = leftWidth - 2 * PanelMargin;
                oY = 0;
                mX = 0;
            }
            else
            {
                textstart = splitterPosition + splitterWidth + PanelMargin;
                textend = Width - PanelMargin;
                width = rightWidth - 2 * PanelMargin;
                oY = 0;
                mX = 0;
            }

            if (line1 == line2)
                if (line1 == -1)
                {
                    // The frame begins and ends beyond the screen
                    // We draw two parallel, unconnected lines on both sides
                    g.FillRectangle(brush, textstart - mX, oY, width + 2 * mX, Height);
                }
                else
                    // A piece of text
                    g.FillRectangle(brush, textstart + x1 - mX, VMargin + line1 * lineHeight + oY,
                    x2 - x1 + 2 * mX, lineHeight);

            else if (line1 == -1)
                g.FillPolygon(brush, new Point[]
                {
                    new Point(textstart - mX, oY),
                    new Point(textstart - mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textstart + x2 + mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textstart + x2 + mX, VMargin + line2 * lineHeight + oY),
                    new Point(textend + mX, VMargin + line2 * lineHeight + oY),
                    new Point(textend + mX,  oY)
                });

            else if (line2 == -1)
                if (x1 == 0) // Top starts at cursorX = 0
                    g.FillPolygon(brush, new Point[]
                    {
                        new Point(textstart - mX, Height - 1 + oY),
                        new Point(textstart - mX, VMargin + line1 * lineHeight + oY),
                        new Point(textend + mX, VMargin + line1 * lineHeight + oY),
                        new Point(textend + mX, Height - 1 + oY)
                    });
                else
                    g.FillPolygon(brush, new Point[]
                    {
                        new Point(textstart - mX, Height - 1 + oY),
                        new Point(textstart - mX, VMargin + (line1 + 1) * lineHeight + oY),
                        new Point(textstart + x1 - mX, VMargin + (line1 + 1) * lineHeight + oY),
                        new Point(textstart + x1 - mX, VMargin + line1 * lineHeight + oY),
                        new Point(textend + mX, VMargin + line1 * lineHeight + oY),
                        new Point(textend + mX, Height - 1 + oY)
                    });

            else if (x1 == 0)
                g.FillPolygon(brush, new Point[]
                {
                    new Point(textend + mX, VMargin + line1 * lineHeight + oY),   
                    new Point(textstart - mX, VMargin + line1 * lineHeight + oY),
                    new Point(textstart - mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textstart + x2 + mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textstart + x2 + mX, VMargin + line2 * lineHeight + oY),
                    new Point(textend + mX, VMargin + line2 * lineHeight + oY)
                });
            else
                g.FillPolygon(brush, new Point[]
                {
                    new Point(textend + mX, VMargin + line1 * lineHeight + oY),
                    new Point(textstart + x1 - mX, VMargin + line1 * lineHeight + oY),
                    new Point(textstart + x1 - mX, VMargin + (line1 + 1) * lineHeight + oY),
                    new Point(textstart - mX, VMargin + (line1 + 1) * lineHeight + oY),
                    new Point(textstart - mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textstart + x2 + mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textstart + x2 + mX, VMargin + (line2) * lineHeight + oY),
                    new Point(textend + mX, VMargin + (line2) * lineHeight + oY)
                });
        }


        internal void DrawBackground(Background f)
        {
            if (!f.Visible)
                return;

            DrawBackground(f.Side, f.Line1, f.X1, f.Line2, f.X2, PrimaryBG.Graphics, f.BackgroundBrush);

        }

        public void EditCurrentPair()
        {
            if (!EditMode || PText.Number() == 0)
                return;

            EditPair(HighlightedPair);

        }


        public void PairChanged(int pairIndex, bool forceUpdate)
        {
            TextPair p = PText.TextPairs[pairIndex];

            p.ClearComputedWords();

            // Truncate all preceding pairs until true-true

            TextPair _p;
            int i = pairIndex;

            do
            {
                i--;
                if (i < 0)
                    break;
                _p = PText.TextPairs[i];
                _p.ClearComputedWords();
            }

            while (!_p.StartParagraph1 || !_p.StartParagraph2);

            // Truncate all following pairs until end or true-true

            int j = pairIndex;

            TextPair _q;

            while (j < PText.Number() - 1)
            {
                j++;
                _q = PText.TextPairs[j];
                if (_q.StartParagraph1 && _q.StartParagraph2)
                    break;
                _q.ClearComputedWords();
            }

            if (forceUpdate)
            {
                FindFirstNaturalDividers();
                UpdateScreen();
            }
        }

        public void UpdateScreen()
        {
            PrepareScreen();
            RenderPairs(false);
            UpdateFramesOnScreen(0);
            ProcessMousePosition(true, false);
            Render();
        }

        public void ProcessMousePosition(bool forced, bool renderRequired)
        {
            // Let's check whether the cursor points to a Word

            // Compute current Line

            int side;

            bool needToRender = false;


            if (!EditMode && !SelectionFinished)
                // When selection started in one side, look always for words on that side
                side = SelectionSide;
            else
                side = -1;


            int line = (LastMouseY - VMargin) / lineHeight;

            // Let's see what we've got on this Line

            int word_x = -1;

            ScreenWord found_word = WordAfterCursor(line, LastMouseX, side);

            if (found_word != null)
                word_x = found_word.X1;

            if (forced || mouse_text_line != line || mouse_text_x != word_x)
            {

                mouse_text_word = found_word;
                mouse_text_line = line;
                mouse_text_x = word_x;

                if (EditMode)
                {
                    if (found_word == null
                        || HighlightedPair != -1 && found_word.PairIndex != HighlightedPair)
                        MouseCurrentWord = null;
                    else
                        MouseCurrentWord = found_word;

                    if (renderRequired)
                        needToRender = true;
                }
                else
                {

                    if (LayoutMode == LayoutMode_Advanced && AdvancedMode_ShowPopups)
                    {

                        int newPair = (mouse_text_word == null ? -1 : mouse_text_word.PairIndex);

                        if (newPair != Advanced_HighlightedPair)
                        {
                            Advanced_HighlightedPair = newPair;

                            popUpInfo.visible = false;

                            if (newPair == -1)
                            {
                                AdvancedHighlightFrame.Visible = false;
                            }
                            else
                            {

                                TextPair p = PText[newPair];

                                RenderedTextInfo r = (Reversed ? p.RenderedInfo2 : p.RenderedInfo1);

                                if (r.Valid)
                                {

                                    AdvancedHighlightFrame.Visible = true;
                                    AdvancedHighlightFrame.Side = mouse_text_word.Side;
                                    AdvancedHighlightFrame.Line1 = r.Line1;
                                    AdvancedHighlightFrame.Line2 = r.Line2;
                                    AdvancedHighlightFrame.X1 = r.X1;
                                    AdvancedHighlightFrame.X2 = r.X2;

                                    byte trSide = (Reversed ? (byte)1 : (byte)2);

                                    Collection<CommonWordInfo> words = new Collection<CommonWordInfo>();

                                    int occLength = 0;

                                    int maxWidth = Width - 2 * PanelMargin;

                                    int height = 0;

                                    Collection<WordInfo> c = p.ComputedWords(trSide);

                                    if (c == null || c.Count == 0)
                                    {
                                        ProcessTextFromPair(p, trSide, ref occLength, words, ref height, ref maxWidth, NumberOfScreenLines);
                                        ParallelText.InsertWords(words, 0);
                                        c = p.ComputedWords(trSide);

                                    }

                                    if (c != null && c.Count != 0)
                                    {
                                        
                                        if (c[0].Line > 0)
                                        {
                                            int linesToDec = c[0].Line;

                                            foreach (WordInfo wi in c)
                                                wi.Line -= linesToDec;
                                        }

                                        DeterminePopupPosition(c, r, maxWidth);

                                    }

                                }

                            }

                            needToRender = true;
                        }

                    }

                    if (!SelectionFinished && mouse_text_word != null)
                    {
                        // Update second part of the selection
                        if (Selection2Pair != mouse_text_word.PairIndex
                            || Selection2Position != mouse_text_word.Pos)
                        {
                            Selection2Pair = mouse_text_word.PairIndex;
                            Selection2Position = mouse_text_word.Pos;

                            UpdateSelectionFrame();

                            if (renderRequired)
                                needToRender = true;
                        }       
                                
                    }

                }

            }

            if (needToRender)
                Render();
        }

        private void DeterminePopupPosition(Collection<WordInfo> c, RenderedTextInfo r, int maxWidth)
        {
            
            popUpInfo.visible = true;
            popUpInfo.words = c;

            WordInfo last = c[c.Count - 1];

            // h: height

            int h1;
            int up;
            int down;


            if (r.Line1 == -1)
                up = 0;
            else
                up = r.Line1;

            if (r.Line2 == -1)
                down = 0;
            else
                down = LastFullScreenLine - r.Line2;

            if (r.Line1 == -1 || r.Line2 == -1)
                h1 = LastFullScreenLine + 1;
            else
                h1 = r.Line2 - r.Line1 + 1;
            
            int h2 = last.Line + 1;

            // s: single
            bool s1 = (h1 == 1);
            bool s2 = (h2 == 1);

            int length2 = last.X2;


            popUpInfo.Y = -1;

            if (s1 && s2)
            {
                if (down > 0)
                {
                    if (r.X1 + length2 <= maxWidth)
                        SetPopUpCoordinates(r.Line2 + 1, r.X1, 1, 0);
                    else
                        SetPopUpCoordinates(r.Line2 + 1, maxWidth - length2, 1, 0);
                }
                else if (up > 0)
                {
                    if (r.X1 + length2 <= maxWidth)
                        SetPopUpCoordinates(r.Line1 - 1, r.X1, -1, 0);
                    else
                        SetPopUpCoordinates(r.Line1 - 1, maxWidth - length2, -1, 0);
                }
                else if (popUpOffsetX + r.X2 + length2 <= maxWidth)
                    SetPopUpCoordinates(r.Line1, r.X2, 0, 1);
                else if (r.X1 >= popUpOffsetX + length2)
                    SetPopUpCoordinates(r.Line1, r.X1 - length2, 0, -1);
                
            }
            else
            {
                if (h2 <= down)
                    SetPopUpCoordinates(r.Line2 + 1, 0, 1, 0);
                else if (s2 && r.Line2 != -1 && r.Line2 <= LastFullScreenLine && r.X2 + popUpOffsetX + length2 <= maxWidth)
                    SetPopUpCoordinates(r.Line2, r.X2, 0, 1);
                else if (h2 <= up)
                    if (h2 == 1)
                        SetPopUpCoordinates(r.Line1 - 1, maxWidth - length2, -1, 0);
                    else
                        if (r.Line1 != -1 && r.X1 >= length2 + popUpOffsetX)
                            SetPopUpCoordinates(r.Line1 - h2 + 1, 0, -1, 0);
                        else
                            SetPopUpCoordinates(r.Line1 - h2, 0, -1, 0);
                else if (s2 && r.Line1 != -1 && r.X1 >= length2 + popUpOffsetX)
                    SetPopUpCoordinates(r.Line1, r.X1 - length2, 0, -1);
            }

            if (popUpInfo.Y == -1)
                // The worst case: draw over
                SetPopUpCoordinates((NumberOfScreenLines - h2 + 1) / 2, 0, 0, 0);

            popUpInfo.Y2 = popUpInfo.Y + h2 - 1;
            popUpInfo.X2 = popUpInfo.X + last.X2;

            //DebugString = "[" + h1.ToString() + ']'
            //    + r.Line1.ToString() + ':' + r.X1.ToString() + '—'
            //    + r.Line2.ToString() + ':' + r.X2.ToString()
            //    + " -- > [" + h2.ToString() + ']' + popUpInfo.Y.ToString() + ':' + popUpInfo.X.ToString();

        }

        private void SetPopUpCoordinates(int Y, int X, int offY, int offX)
        {
            popUpInfo.Y = Y;
            popUpInfo.X = X + offX * popUpOffsetX;
            popUpInfo.offsetY = offY;
            
        }


        private void EditPair(int pairIndex)
        {
            TextPair p = PText.TextPairs[pairIndex];

            if (p.GetLength(1) > BigTextSize || p.GetLength(2) > BigTextSize)
            {
                EditWhenNipped = !EditWhenNipped;
                Render();
                return;
            }

            PrepareEditForm();

            editPairForm.ParallelTextControl = this;
            editPairForm.PairIndex = pairIndex;
            editPairForm.ShowDialog();

            if (editPairForm.Result)
            {
                if (PText[pairIndex].StructureLevel > 0
                    && pairIndex < PText.Number() - 1
                    && !(PText[pairIndex + 1].StartParagraph1 && PText[pairIndex + 1].StartParagraph2))
                {
                    PText[pairIndex + 1].StartParagraph1 = true;
                    PText[pairIndex + 1].StartParagraph2 = true;
                    PairChanged(pairIndex + 1, false);
                }
                PairChanged(pairIndex, true);
                PText[pairIndex].UpdateTotalSize();
                PText.UpdateAggregates(pairIndex);
                Modified = true;
            }
        }

        private void PrepareEditForm()
        {
            if (editPairForm == null)
            {
                editPairForm = new EditPairForm();

                Form parentForm = FindForm();

                editPairForm.Left = parentForm.Left + (parentForm.Width - editPairForm.Width) / 2;
                editPairForm.Top = parentForm.Top + (parentForm.Height - editPairForm.Height) / 2;

            }
        }

        internal bool WesternJoint(int firstPair, byte side)
        {

            TextPair first = PText[firstPair];
            TextPair second = PText[firstPair + 1];

            int firstLength = first.GetLength(side);

            if (firstLength == 0 || second.GetLength(side) == 0)
                return false; // no space needed between texts since one or both are empty

            return !IsEasternCharacter(first.GetChar(side, firstLength - 1)) && !IsEasternCharacter(second.GetChar(side, 0));

        }


        internal void MergePairs(int firstPair)
        {

            TextPair first = PText[firstPair];
            TextPair second = PText[firstPair + 1];

            if (second.SB1 == null)
            {
                second.SB1 = new StringBuilder(second.Text1);
                second.Text1 = null;
            }

            if (second.SB2 == null)
            {
                second.SB2 = new StringBuilder(second.Text2);
                second.Text2 = null;
            }

            if (second.StartParagraph1)
            {
                second.SB1.Insert(0, '\n');
                second.SB1.Insert(0, '\r');
            }
            else if (WesternJoint(firstPair, 1))
                second.SB1.Insert(0, ' ');

            if (second.StartParagraph2)
            {
                second.SB2.Insert(0, '\n');
                second.SB2.Insert(0, '\r');
            }
            else if (WesternJoint(firstPair, 2))
                second.SB2.Insert(0, ' ');

            second.SB1.Insert(0, first.SB1 == null ? first.Text1 : first.SB1.ToString());
            second.SB2.Insert(0, first.SB2 == null ? first.Text2 : first.SB2.ToString());

            second.StartParagraph1 = first.StartParagraph1;
            second.StartParagraph2 = first.StartParagraph2;

            PText.TextPairs.Remove(first);

            if (second.SB1.Length < BigTextSize)
            {
                second.Text1 = second.SB1.ToString();
                second.SB1 = null;
            }

            if (second.SB2.Length < BigTextSize)
            {
                second.Text2 = second.SB2.ToString();
                second.SB2 = null;
            }

            if (first.AudioFileNumber == second.AudioFileNumber)
                second.TimeBeg = first.TimeBeg;

            Modified = true;

        }

        public float SplitterRatio { get; set; }

        public int Number { get { return PText.Number(); } }

        public bool MousePressed { get; set; }

        public bool EditMode { get; set; }


        internal void UpdateSelectionFrame()
        {

            if (SelectionFrame.Side == 0)
                return;

            int Y1;
            int Y2;
            int X1;
            int X2;

            AssignProperSelectionOrder(out Y1, out Y2, out X1, out X2);

            ScreenWord word1 = FindScreenWordByPosition(Y1, X1, SelectionSide);
            ScreenWord word2 = FindScreenWordByPosition(Y2, X2, SelectionSide);

            SelectionFrame.Visible = true;

            if (word1 == null)
                if (Y1 >= LastRenderedPair)
                    SelectionFrame.Visible = false;
                else
                    SelectionFrame.Line1 = -1;
            else
            {
                SelectionFrame.Line1 = word1.Line;
                SelectionFrame.X1 = word1.FX1;
            }

            if (word2 == null)
                if (Y2 >= LastRenderedPair)
                    SelectionFrame.Line2 = -1;
                else
                    SelectionFrame.Visible = false;
            else
            {
                SelectionFrame.Line2 = word2.Line;
                SelectionFrame.X2 = word2.FX2;
            }

        }

        public void AssignProperSelectionOrder(out int Y1, out int Y2, out int X1, out int X2)
        {
            if (Selection1Pair < Selection2Pair || Selection1Pair == Selection2Pair && Selection1Position <= Selection2Position)
            {
                Y1 = Selection1Pair;
                X1 = Selection1Position;
                Y2 = Selection2Pair;
                X2 = Selection2Position;
            }
            else
            {
                Y1 = Selection2Pair;
                X1 = Selection2Position;
                Y2 = Selection1Pair;
                X2 = Selection1Position;
            }
        }

        internal void SetFont(System.Drawing.Font font, System.Drawing.Font font2)
        {
            if (textFont != null)
                textFont.Dispose();
            
            textFont = font;
            
            ComputeSpaceLength(PanelGraphics);
            
            ProcessLayoutChange(true);

        }

        public void SetHighlightedFrameColor()
        {
            TextPair p = PText[HighlightedPair];
        }

        public int ReadingMode { get; set; }

        public int AlternatingColorScheme { get; set; }

        public void SetLayoutMode()
        {

            if (EditMode || ReadingMode == FileUsageInfo.NormalMode)
            {
                LayoutMode = LayoutMode_Normal;
            }
            else if (ReadingMode == FileUsageInfo.AlternatingMode)
                LayoutMode = LayoutMode_Alternating;
            else
                LayoutMode = LayoutMode_Advanced;

            AudioSingleFrame.Visible = (WithAudio() && !EditMode && LayoutMode != LayoutMode_Normal);
            HighlightedFrame.SetVisibility(!AudioSingleFrame.Visible);

            ComputeIndent();

            ComputeSideCoordinates();

            Advanced_HighlightedPair = -1;
            AdvancedHighlightFrame.Visible = false;

        }


        internal void SwitchAdvancedShowPopups()
        {
            if (AdvancedMode_ShowPopups)
            {
                AdvancedMode_ShowPopups = false;
                AdvancedHighlightFrame.Visible = false;
                popUpInfo.visible = false;
                Advanced_HighlightedPair = -3;
                Render();
            }
            else
            {
                AdvancedMode_ShowPopups = true;
                ProcessMousePosition(true, true);
            }
        }

        internal ScreenWord FindFirstScreenWord(int pairIndex, byte side)
        {
            foreach (KeyValuePair<int, List<ScreenWord>> kv in wordsOnScreen)
                    foreach (ScreenWord sw in kv.Value)
                        if (sw.PairIndex == pairIndex && sw.Side == side)
                            return sw;

            return null;
        }

        internal ScreenWord FindLastScreenWord(int pairIndex, byte side)
        {
            ScreenWord lastFound = null;

            foreach (KeyValuePair<int, List<ScreenWord>> kv in wordsOnScreen)
                foreach (ScreenWord sw in kv.Value)
                    if (sw.PairIndex == pairIndex && sw.Side == side)
                        lastFound = sw;

            return lastFound;
        }



        internal bool NextRecommended(byte side, bool forward)
        {

            int divPos;

            divPos = side == 1 ? NaturalDividerPosition1 : NaturalDividerPosition2;

            int newPos = PText[HighlightedPair].NaturalDividerPosition(side, divPos, forward);

            if (newPos == -1)
                return false;
            else
            {
                if (side == 1)
                    NaturalDividerPosition1 = newPos;
                else
                    NaturalDividerPosition2 = newPos;

                return true;
            }


        }

        internal void FindFirstNaturalDividers()
        {
            Side1Set = false;
            Side2Set = false;

            NaturalDividerPosition1 = -1;
            NaturalDividerPosition2 = -1;

            if (Number == 0)
                return;

            NextRecommended(1, true);
            NextRecommended(2, true);

        }

        internal void ExportText(string fileName, bool leftSide)
        {

            int sideToExport = (leftSide ? 1 : 2);

            if (Reversed)
                sideToExport = 3 - sideToExport;

            PText.ExportText(fileName, sideToExport);
        }

        public bool WithAudio()
        {
            return PText.WithAudio;
        }
    }

    public class ScreenWord
    {
        public string Word { get; set; }
        public int X1 { get; set; } // start of the Word -- real point on screen
        public int X2 { get; set; } // end of the Word
        public int PairIndex { get; set; } // index of Pair
        public byte Side { get; set; } // 1 or 2 -- the second or first text
        public int Pos { get; set; } // position of the Word in the Pair
        public int FX1 { get; set; }
        public int FX2 { get; set; }
        public int Line { get; set; }

        public ScreenWord Prev { get; set; }
        public ScreenWord Next { get; set; }

    }

    class PopUpInfo
    {
        public int X;
        public int Y;
        public bool visible;
        public int offsetY;
        public int X2;
        public int Y2;
        public Collection<WordInfo> words;
        
        public PopUpInfo()
        {
            X = -1;
            Y = -1;
            offsetY = 0;

            visible = false;
        }
    }


}
