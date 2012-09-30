using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace AglonaReader
{

    

    public partial class ParallelTextControl : UserControl
    {

        public int mouse_text_line = -1;
        private int mouse_text_x = -1;
        public ScreenWord mouse_text_word = null;

        public bool Modified { get; set; }

        public bool HighlightFirstWords { get; set; }
        public bool HighlightFragments { get; set; }

        // Length of a string to be considered a "big block"
        public const int BigTextSize = 1000;

        byte NumberofColors;

        // Contains H values of text color table
        List<double> colorTableH;
        List<SolidBrush> brushTable;
        List<Pen> penTable;
        List<Color> darkColorTable;
        List<Color> lightColorTable;
        Color grayColor; // Changes with Brightness

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


        public Font TextFont { get; set; }
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
            if (Reversed)
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
        public Pen SuggestedPen { get; set; }
        public Pen CorrectedPen { get; set; }

        public DoubleFrame HighlightedFrame { get; set; }
        public DoubleFrame NippingFrame { get; set; }

        private Collection<AbstractFrame> frames;

        private int text1start;
        private int text1end;

        private int text2start;
        private int text2end;

        public int LineHeight { get; set; }

        public StringFormat GT { get; set; } // Generic Typographic

        private SortedDictionary<string, int> widthDictionary;

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

        public int WordWidth(string word, IDeviceContext graphics)
        {

            int result;

            // First, try to use data from the dictionary if it'Word there

            if (widthDictionary.TryGetValue(word, out result)) return result;
            else
            {
                // Measure and store in the dictionary
                result = TextRenderer.MeasureText(graphics, word, TextFont, Size.Empty, TextFormatFlags.NoPadding).Width;
                widthDictionary.Add(word, result);
                return result;
            }

        }

        /// <summary>
        /// Computes the recommended width between words in pixels
        /// </summary>
        /// <param name="graphics">Graphics on which the text is rendered</param>
        public void ComputeSpaceLength(IDeviceContext graphics)
        {
            SpaceLength = WordWidth(" ", graphics);
            LineHeight = TextFont.Height;
        }

        /// <summary>
        /// Calculates NumberOfScreenLines variable
        /// </summary>
        /// <param name="vSize">Vertical size of screen in pixels</param>
        public void ComputeNumberOfScreenLines()
        {
            NumberOfScreenLines = (Height - 2 * VMargin) / LineHeight;

            LastFullScreenLine = NumberOfScreenLines - 1;

            if (LineHeight * NumberOfScreenLines < Height - 2 * VMargin)
                NumberOfScreenLines++;

        }


        public void ResizeBufferedGraphic()
        {
            PrimaryBG = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), ClientRectangle);
            SecondaryBG = BufferedGraphicsManager.Current.Allocate(PrimaryBG.Graphics, ClientRectangle);

            PrimaryBG.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        public ParallelTextControl()
        {
            InitializeComponent();

            CreateNewParallelBook();
            
            wordsOnScreen = new SortedList<int, List<ScreenWord>>();

            VMargin = 3;
            PanelMargin = 10;

            LastMouseX = -1;
            LastMouseY = -1;

            splitterBrush = Brushes.LightGray;

            frames = new Collection<AbstractFrame>();

            HighlightedPen = Frame.CreatePen(Color.LightBlue, DashStyle.Solid, 4.0F);
            HighlightedFrame = new DoubleFrame(HighlightedPen, frames);
            
            CorrectedPen = Frame.CreatePen(Color.Peru, DashStyle.Solid, 2.0F);

            NippingFrame = new DoubleFrame(SuggestedPen, frames);

            GT = (StringFormat)StringFormat.GenericTypographic.Clone();

            widthDictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);

            PanelGraphics = CreateGraphics();

            TextFont = new System.Drawing.Font("times", 18.0F);

            ComputeSpaceLength(PanelGraphics);

            editPairForm = new EditPairForm();

            EditWhenNipped = false;

            InitializeColors();
            
            Brightness = 0.97;

            HighlightFirstWords = true;
            HighlightFragments = true;

            SuggestedPen = Frame.CreatePen(Color.LightBlue, DashStyle.Dash, 2.0F);

            
        }

        public void CreateNewParallelBook()
        {
            PText = new ParallelText();

            CurrentPair = 0;
            HighlightedPair = 0;

            Reversed = false;
        }


        private void InitializeColors()
        {
            colorTableH = new List<double>();

            //double current = 0;

            //for (byte charIndex = 1; charIndex <= 10; charIndex++)
            //{
            //    colorTableH.Add(current);
            //    current += 0.3;
            //    if (current >= 1)
            //        current -= 1;
            //}

            //for (byte charIndex = 1; charIndex < 20; charIndex++)
            //{
            //    colorTableH.Add(current);
            //    current += 0.25;
            //    if (current >= 1)
            //        current -= 0.95;
            //}

            //for (byte charIndex = 1; charIndex < 20; charIndex++)
            //{
            //    colorTableH.Add(current);
            //    current += 0.05;
                
            //}

            
            
            colorTableH.Add(0.162);
            colorTableH.Add(0.34);
            colorTableH.Add(0.492);
            colorTableH.Add(0.68);
            colorTableH.Add(0.83);
            colorTableH.Add(0);
            //colorTableH.Add(0.11); // Orange? Too close to yellow. Disable for now

            

            //colorTableH.Add(0.15);
            //colorTableH.Add(0.49);
            //colorTableH.Add(0.115);
            //colorTableH.Add(0.255);

            NumberofColors = (byte) colorTableH.Count;


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
                    g.DrawRectangle(frame.FramePen, textstart + frame.X1, VMargin + frame.Line1 * LineHeight - frameoffset_y,
                    frame.X2 - frame.X1 + 2 * frameoffset_x, LineHeight + 2 * frameoffset_y);

            else if (frame.Line1 == -1)
                g.DrawLines(frame.FramePen, new Point[]
                {
                    new Point(textstart, 0),
                    new Point(textstart, VMargin + (frame.Line2 + 1) * LineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2 + 1) * LineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + frame.Line2 * LineHeight + frameoffset_y),
                    new Point(textend, VMargin + frame.Line2 * LineHeight + frameoffset_y),
                    new Point(textend, 0)
                });

            else if (frame.Line2 == -1)
                if (frame.X1 == 0) // Top starts at cursorX = 0
                    g.DrawLines(frame.FramePen, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, VMargin + frame.Line1 * LineHeight - frameoffset_y),
                        new Point(textend, VMargin + frame.Line1 * LineHeight - frameoffset_y),
                        new Point(textend, Height - 1)
                    });
                else
                    g.DrawLines(frame.FramePen, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, VMargin + (frame.Line1 + 1) * LineHeight - frameoffset_y),
                        new Point(textstart + frame.X1, VMargin + (frame.Line1 + 1) * LineHeight - frameoffset_y),
                        new Point(textstart + frame.X1, VMargin + frame.Line1 * LineHeight - frameoffset_y),
                        new Point(textend, VMargin + frame.Line1 * LineHeight - frameoffset_y),
                        new Point(textend, Height - 1)
                    });

            else if (frame.X1 == 0)
                g.DrawPolygon(frame.FramePen, new Point[]
                {
                    new Point(textend, VMargin + frame.Line1 * LineHeight - frameoffset_y),   
                    new Point(textstart, VMargin + frame.Line1 * LineHeight - frameoffset_y),
                    new Point(textstart, VMargin + (frame.Line2 + 1) * LineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2 + 1) * LineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + frame.Line2 * LineHeight + frameoffset_y),
                    new Point(textend, VMargin + frame.Line2 * LineHeight + frameoffset_y)
                });
            else
                g.DrawPolygon(frame.FramePen, new Point[]
                {
                    new Point(textend, VMargin + frame.Line1 * LineHeight - frameoffset_y),
                    new Point(textstart + frame.X1, VMargin + frame.Line1 * LineHeight - frameoffset_y),
                    new Point(textstart + frame.X1, VMargin + (frame.Line1 + 1) * LineHeight - frameoffset_y),
                    new Point(textstart, VMargin + (frame.Line1 + 1) * LineHeight - frameoffset_y),
                    new Point(textstart, VMargin + (frame.Line2 + 1) * LineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2 + 1) * LineHeight + frameoffset_y),
                    new Point(textstart + frame.X2 + 2 * frameoffset_x, VMargin + (frame.Line2) * LineHeight + frameoffset_y),
                    new Point(textend, VMargin + (frame.Line2) * LineHeight + frameoffset_y)
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
                TextRenderer.DrawText(g, DebugString, TextFont, new Point(PanelMargin, Height - LineHeight), Color.Red);

            if (EditWhenNipped)
                g.FillEllipse(Brushes.Red, Width - 13, 2, 10, 10);

            HighlightWord(MouseCurrentWord, Color.LightSkyBlue);

            PrimaryBG.Render();

        }

        public void HighlightWord(ScreenWord sw, Color color)
        {
            if (sw == null)
                return;

            Graphics g = PrimaryBG.Graphics;

            TextRenderer.DrawText(g, sw.Word, TextFont, new Point(sw.X1, VMargin + sw.Line * LineHeight),
                Color.Black, color, TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }

        bool NeedToLineBreakFirstWord(TextPair p, byte side, ref int occLength, ref int maxWidth, int sL, bool startParagraph)
        {
            if (occLength == 0) return false;
            if (startParagraph) return true;

            StringBuilder word = new StringBuilder();

            char c;

            int pos = 0;

            int length = p.GetLength(side);

            while (pos < length)
            {
                c = p.GetChar(side, pos);
                if (c == ' ' || c == '\t' || c == '\n') break;
                word.Append(c);
                pos++;
            }

            return (maxWidth - occLength - sL <= WordWidth(word.ToString(), PanelGraphics));

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
                    return;

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
                    ParallelText.InsertWords(words1, 0, 1);
                    occLength1 = 0;
                    height = height2;
                }
                else if (height2 < height1)
                {
                    // Line break 2
                    ParallelText.InsertWords(words2, 0, 2);
                    occLength2 = 0;
                }

                if (p.AllLinesComputed1 && p.AllLinesComputed2
                    && (p.StructureLevel > 0
                    || cPair + 1 < PText.Number() && PText.TextPairs[cPair + 1].StructureLevel > 0))
                    height++;

                p.Height = height;

            }

            if (requiredHeight != -1)
            {
                remainder -= height;

                if (remainder <= 0)
                    return;
            }

            // Are there more text pairs?

            if (cPair + 1 == PText.Number())
            {
                // This was the last Pair, no more coming.
                ParallelText.InsertWords(words1, 0, 1);
                ParallelText.InsertWords(words2, 0, 2);
                return;
            }

            // ...There are.

            cPair++;

            prev_pair = p;

            p = PText.TextPairs[cPair];

            if (NeedToLineBreakFirstWord(p, 1, ref occLength1, ref width1, SpaceLength, p.StartParagraph1)
                    || NeedToLineBreakFirstWord(p, 2, ref occLength2, ref width2, SpaceLength, p.StartParagraph2))
            {
                ParallelText.InsertWords(words1, 0, 1);
                ParallelText.InsertWords(words2, 0, 2);

                prev_pair.Height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        return;
                }

                occLength1 = 0;
                occLength2 = 0;
            }

            if (requiredLines == -1 && cPair > startPair && prev_pair.Height > 0)
                return;

            goto NextPair;

        }

        public void PrepareScreen()
        {
            PrepareScreen(CurrentPair, NumberOfScreenLines);
        }



        private void RenderText(Graphics g, int pairIndex, ref int offset, ref int cLine, byte side)
        {

            TextPair p = PText.TextPairs[pairIndex];

            Collection<WordInfo> list = p.ComputedWords(side);

            RenderedTextInfo renderedInfo = p.RenderedInfo(side);

            if (cLine >= NumberOfScreenLines
                || list.Count == 0)
            {
                renderedInfo.Valid = false;
                return;
            }

            WordInfo last = list[list.Count - 1];

            if (cLine + last.Line < 0)
            {
                renderedInfo.Valid = false;
                return;
            }

            renderedInfo.Valid = true;

            if (cLine < 0)
                renderedInfo.Line1 = -1;
            else
            {
                renderedInfo.Line1 = cLine + list[0].Line;
                renderedInfo.X1 = list[0].X1;
                //renderedInfo.x1b = list[0].cursorX;
            }

            if (cLine + last.Line >= NumberOfScreenLines || !(side == 1 ? p.AllLinesComputed1 : p.AllLinesComputed2))
                renderedInfo.Line2 = -1;
            else
            {
                renderedInfo.Line2 = cLine + last.Line;
                renderedInfo.X2 = last.X2;
                renderedInfo.X2B = last.X2;

                if (pairIndex < PText.Number() - 1 && last.Line == p.Height)
                {
                    TextPair nextPair = PText.TextPairs[pairIndex + 1];
                    Collection<WordInfo> nextList = nextPair.ComputedWords(side);
                    if (nextList.Count > 0)
                        if (nextList[0].X1 > last.X2)
                            renderedInfo.X2B += nextList[0].X1 - last.X2;
                }

            }

            int x;
            int y = -1;

            ScreenWord prev_screen_word = null;
            ScreenWord s = null;
            List<ScreenWord> l = null;

            int prev_y = -1;

            bool big = ((side == 1 ? p.SB1 : p.SB2) != null);

            // Before drawing text we must draw colored background
                // Colored 
            if (!big && HighlightFragments)
                DrawBackground(side, renderedInfo.Line1, renderedInfo.X1, renderedInfo.Line2, renderedInfo.X2B, SecondaryBG.Graphics,
                    brushTable[pairIndex % NumberofColors]);

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

                    if (HighlightFirstWords && i == 0 && !(big && HighlightFragments))
                    {
                        Rectangle wordRect = new Rectangle(x, VMargin + y * LineHeight, r.X2 - r.X1 + 1, LineHeight);
                        
                        using (LinearGradientBrush brush = new LinearGradientBrush(
                            wordRect,
                            big ? grayColor : darkColorTable[pairIndex % NumberofColors],
                            HighlightFragments && !big ? lightColorTable[pairIndex % NumberofColors] : Color.White,
                            LinearGradientMode.Horizontal))

                            g.FillRectangle(brush, wordRect);
                    }
                        
                    TextRenderer.DrawText(g, wrd, TextFont, new Point(x, VMargin + y * LineHeight),
                         Color.Black, TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

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


        /// <summary>
        /// Renders a newSide
        /// </summary>
        /// <param name="newSide">Number of newSide in SCREEN terms, not in Pair terms</param>
        private void RenderPairSide(byte side, int startPair, int negHeight)
        {

            Graphics g = SecondaryBG.Graphics;

            int cPair = startPair;
            int cLine = -negHeight;

            int offset;
            byte textSide;

            if (side == 1)
                offset = PanelMargin;
            else
                offset = SplitterPosition + SplitterWidth + PanelMargin;

            if (Reversed == (side == 1))
                textSide = 2;
            else
                textSide = 1;
            
        NextPair:

            TextPair p = PText.TextPairs[cPair];

            RenderText(g, cPair, ref offset, ref cLine, textSide);
            
            cLine += p.Height;

            if (cLine >= NumberOfScreenLines)
                return;

            if (cPair < PText.Number() - 1)
            {
                cPair++;
                goto NextPair;
            }

        }


        public void RenderPairs()
        {
            DrawSecondary();
            wordsOnScreen.Clear();

            if (PText.Number() == 0)
                return;

            

            TextPair p;

            int negHeight = 0;

            int startPair = CurrentPair;

            p = PText.TextPairs[startPair];

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

            FirstRenderedPair = -1;
            LastRenderedPair = -1;

            // NOTE. Pairs are run twice (instead of only once, rendering both pairs of text from them simultaneously),
            // because we want to fill wordsOnScreen in such a way that every Line has words with strictly increasing cursorX.
            // If we render Text1 and Text2 from the second Pair, then Text1 and Text2 from the first Pair
            // and so on, when more than one Pair is on one Line, the corresponding List in
            // wordsOnScreen will have these coordinates (let ai be cursorX coords of the second text,
            // bi be cursorX coords of the first text of the Pair):
            // p1a1 p1a2 ... p1an p1b1 p1b2 ... p1bn p2a1 p2a2 ... p2an p2b1 p2b2 ... p2bn
            // ...which is incorrect (because p1bn is greater than p2a1).
            // Correct sequence is
            // p1a1 p1a2 ... p1an p2a1 p2a2 ... p2an p1b1 p1b2 ... p1bn p2b1 p2b2 ... p2bn

            RenderPairSide(1, startPair, negHeight);
            RenderPairSide(2, startPair, negHeight);

        }

        public void FindNaturalDividersScreen(byte side)
        {
            if (PText.Number() > 0)
            {
                TextPair h = PText.TextPairs[HighlightedPair];

                if (side == 0)
                {
                    SetFramesByPair(h, HighlightedFrame);
                }

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
        }

        private void ProcessCurrentWord(StringBuilder word, ref int occLength, Collection<CommonWordInfo> words, ref int Height, TextPair p, byte side, ref int MaxWidth, ref int wordPosition)
        {

            // Current Word complete, let'Word get its length
            int wordLength = WordWidth(word.ToString(), PanelGraphics);

            int newStart = occLength + (occLength == 0 ? 0 : SpaceLength);

            if (newStart + wordLength > MaxWidth)
            {
                // Move this Word to the Next Line.
                // Before that we need to flush words to the DB

                ParallelText.InsertWords(words, MaxWidth - occLength, side);

                Height++;

                newStart = 0;

                occLength = 0;

            }

            // Add this Word to the current Line
            words.Add(new CommonWordInfo(p, word.ToString(), Height, newStart, newStart + wordLength - 1, wordPosition));
            occLength = newStart + wordLength;

            word.Clear();
            
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
            }
            else
                pos = (side == 1) ? p.CurrentPos1 : p.CurrentPos2;

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

                    ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos);

                    if (requiredHeight != -1 && requiredHeight == height)
                        goto CommonExit;
                    wordPos = -1;

                }
                else if (c == '\n')
                {
                    if (word.Length > 0)
                    {
                        ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos);
                        if (requiredHeight != -1 && requiredHeight == height)
                        {
                            wordPos = pos;
                            goto CommonExit;
                        }
                        wordPos = -1;
                    }

                    ParallelText.InsertWords(words, 0, side);

                    height++;
                    occLength = 0;

                    if (requiredHeight != -1 && requiredHeight == height)
                    {
                        wordPos = ++pos;
                        goto CommonExit;
                    }


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
                ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos);
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
                p.CurrentPos1 = wordPos;
            else
                p.CurrentPos2 = wordPos;
            
        }

        public void ProcessLayoutChange()
        {

            // erase both tables
            PText.Truncate();

            ComputeNumberOfScreenLines();

            UpdateScreen();

        }

        private ScreenWord FindScreenWordByPosition(int pairIndex, int pos, byte side)
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

            return (pos1 >= lastPos1 || pos2 >= lastPos2);

        }


        public void SetNippingFrameByScreenWord(byte side, ScreenWord sw)
        {
            Frame f = (Frame) NippingFrame.Frame(side);

            if (sw == null || sw.Prev == null)
                f.Visible = false;
            else
            {
                Frame hf = (Frame) HighlightedFrame.Frame(side);
                f.Visible = true;
                f.FramePen = SuggestedPen;
                f.Line1 = hf.Line1;
                f.X1 = hf.X1;
                f.Line2 = sw.Prev.Line;
                f.X2 = sw.Prev.FX2;
            }
        }

        public ScreenWord WordAfterCursor(int line, int cursorX)
        {
            List<ScreenWord> listOfWords;

            if (wordsOnScreen.TryGetValue(line, out listOfWords))
            {
                // let's see...

                foreach (ScreenWord s in listOfWords)
                {
                    if (cursorX > s.X2) continue;
                    return s;
                }
            }

            return null;
        }



        internal void FindNaturalDividers(byte side)
        {

            if (PText.Number() == 0)
                return;

            // Look for natural dividers in the current (highlighted) Pair

            TextPair p = PText.TextPairs[HighlightedPair];

            if (side == 0 || side == 1)
                NaturalDividerPosition1 = p.NaturalDividerPosition(1);
            if (side == 0 || side == 2)
                NaturalDividerPosition2 = p.NaturalDividerPosition(2);

        }

        internal bool NipHighlightedPair()
        {
            if (NaturalDividerPosition1W == null
                    || NaturalDividerPosition2W == null)

                return false;

            TextPair np = new TextPair();

            TextPair hp = PText.TextPairs[HighlightedPair];

            np.SetRecommendedNaturals(hp);
            hp.ClearRecommendedNaturals();

            np.StartParagraph1 = hp.StartParagraph1;
            np.StartParagraph2 = hp.StartParagraph2;

            NipASide(hp, np, 1);
            NipASide(hp, np, 2);

            PText.TextPairs.Insert(HighlightedPair, np);

            if (EditWhenNipped)
            {
                editPairForm.ParallelTextControl = this;
                editPairForm.PairIndex = HighlightedPair;
                editPairForm.ShowDialog();
                EditWhenNipped = false;
            }

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

            PrepareScreen();
            RenderPairs();

            FindNaturalDividers(0);

            if (CurrentPair != HighlightedPair
                && PosIsOnOrAfterLastScreenWord(HighlightedPair, NaturalDividerPosition1, NaturalDividerPosition2))
            {
                CurrentPair = HighlightedPair;
                PrepareScreen();
                RenderPairs();
            }

            FindNaturalDividersScreen(0);
            ProcessMousePosition(true);
            Render();

            Side1Set = false;
            Side2Set = false;

            Modified = true;

            return true;

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

            if (side == 1 && !Reversed || side == 2 && Reversed)
            {
                textstart = PanelMargin;
                textend = leftWidth - PanelMargin;
                width = leftWidth - 2 * PanelMargin;
            }
            else
            {
                textstart = splitterPosition + splitterWidth + PanelMargin;
                textend = Width - PanelMargin;
                width = rightWidth - 2 * PanelMargin;
            }

            if (line1 == line2)
                if (line1 == -1)
                {
                    // The frame begins and ends beyond the screen
                    // We draw two parallel, unconnected lines on both sides
                    g.FillRectangle(brush, textstart, 0, width, Height);
                }
                else
                    // A piece of text
                    g.FillRectangle(brush, textstart + x1, VMargin + line1 * LineHeight,
                    x2 - x1, LineHeight);

            else if (line1 == -1)
                g.FillPolygon(brush, new Point[]
                {
                    new Point(textstart, 0),
                    new Point(textstart, VMargin + (line2 + 1) * LineHeight),
                    new Point(textstart + x2, VMargin + (line2 + 1) * LineHeight),
                    new Point(textstart + x2, VMargin + line2 * LineHeight),
                    new Point(textend, VMargin + line2 * LineHeight),
                    new Point(textend, 0)
                });

            else if (line2 == -1)
                if (x1 == 0) // Top starts at cursorX = 0
                    g.FillPolygon(brush, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, VMargin + line1 * LineHeight),
                        new Point(textend, VMargin + line1 * LineHeight),
                        new Point(textend, Height - 1)
                    });
                else
                    g.FillPolygon(brush, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, VMargin + (line1 + 1) * LineHeight),
                        new Point(textstart + x1, VMargin + (line1 + 1) * LineHeight),
                        new Point(textstart + x1, VMargin + line1 * LineHeight),
                        new Point(textend, VMargin + line1 * LineHeight),
                        new Point(textend, Height - 1)
                    });

            else if (x1 == 0)
                g.FillPolygon(brush, new Point[]
                {
                    new Point(textend, VMargin + line1 * LineHeight),   
                    new Point(textstart, VMargin + line1 * LineHeight),
                    new Point(textstart, VMargin + (line2 + 1) * LineHeight),
                    new Point(textstart + x2, VMargin + (line2 + 1) * LineHeight),
                    new Point(textstart + x2, VMargin + line2 * LineHeight),
                    new Point(textend, VMargin + line2 * LineHeight)
                });
            else
                g.FillPolygon(brush, new Point[]
                {
                    new Point(textend, VMargin + line1 * LineHeight),
                    new Point(textstart + x1, VMargin + line1 * LineHeight),
                    new Point(textstart + x1, VMargin + (line1 + 1) * LineHeight),
                    new Point(textstart, VMargin + (line1 + 1) * LineHeight),
                    new Point(textstart, VMargin + (line2 + 1) * LineHeight),
                    new Point(textstart + x2, VMargin + (line2 + 1) * LineHeight),
                    new Point(textstart + x2, VMargin + (line2) * LineHeight),
                    new Point(textend, VMargin + (line2) * LineHeight)
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
            if (PText.Number() == 0)
                return;

            EditPair(HighlightedPair);

        }


        public void PairChanged(int pairIndex)
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

            UpdateScreen();
        }

        public void UpdateScreen()
        {
            PrepareScreen();
            RenderPairs();
            FindNaturalDividers(0);
            FindNaturalDividersScreen(0);
            ProcessMousePosition(true);
            Render();
        }

        public void ProcessMousePosition(bool forced)
        {
            // Let's check whether the cursor points to a Word

            // Compute current Line

            int line = (LastMouseY - VMargin) / LineHeight;

            // Let's see what we've got on this Line

            int word_x = -1;

            ScreenWord found_word = WordAfterCursor(line, LastMouseX);

            if (found_word != null)
                word_x = found_word.X1;

            if (forced || mouse_text_line != line || mouse_text_x != word_x)
            {
                if (found_word == null
                    || HighlightedPair != -1 && found_word.PairIndex != HighlightedPair)
                    MouseCurrentWord = null;
                else
                    MouseCurrentWord = found_word;

                Render();

                mouse_text_word = found_word;
                mouse_text_line = line;
                mouse_text_x = word_x;

            }
        }


        private void EditPair(int pairIndex)
        {
            TextPair p = PText.TextPairs[pairIndex];

            if (p.GetLength(1) > BigTextSize || p.GetLength(2) > BigTextSize)
            {
                EditWhenNipped = !EditWhenNipped;
                //MessageBox.Show("The text is too long. Use Edit command on shorter pairs.");
                Render();
                return;
            }

            editPairForm.ParallelTextControl = this;
            editPairForm.PairIndex = pairIndex;
            editPairForm.ShowDialog();

            if (editPairForm.Result)
            {
                PairChanged(pairIndex);
                Modified = true;
            }
        }


        internal void MergePairs(int firstPair)
        {

            TextPair first = PText.TextPairs[firstPair];
            TextPair second = PText.TextPairs[firstPair + 1];

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
            else
                second.SB1.Insert(0, ' ');

            if (second.StartParagraph2)
            {
                second.SB2.Insert(0, '\n');
                second.SB2.Insert(0, '\r');
            }
            else
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

            Modified = true;


        }

        public float SplitterRatio { get; set; }

        public int Number { get { return PText.Number(); } }

        public bool MousePressed { get; set; }
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

        /// <summary>
        /// Previous screen Word from the same Pair
        /// </summary>
        public ScreenWord Prev { get; set; }
        public ScreenWord Next { get; set; }

    }
}
