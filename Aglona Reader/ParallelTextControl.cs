using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


namespace AglonaReader
{
    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    public partial class ParallelTextControl : UserControl
    {
        public float startingPercent;
        public int startingNumberOfFrags;
        public bool stopwatchStarted;
        internal readonly Stopwatch stopWatch;

        internal const int LayoutModeNormal = 0;
        private const int LayoutModeAlternating = 1;
        internal const int LayoutModeAdvanced = 2;

        private const int PopUpOffsetX = 7;
        private const int PopUpOffsetY = 7;

        public int layoutMode;


        /// <summary>
        /// 0 = not set; 1 = black; 2 = gray
        /// </summary>
        private int currentTextColor;


        private IntPtr secondaryHdc;

        public byte SelectionSide { get; set; }
        public int Selection1Pair { get; set; }
        public int Selection1Position { get; set; }
        public int Selection2Pair { get; set; }
        public int Selection2Position { get; set; }
        public Frame SelectionFrame { get; }
        public bool SelectionFinished { get; set; }

        private readonly Frame advancedHighlightFrame;
        private readonly PopUpInfo popUpInfo;

        public int mouseTextLine = -1;
        private int mouseTextX = -1;
        public ScreenWord mouseTextWord;

        public bool Modified { get; set; }

        public bool HighlightFirstWords { get; set; }
        public bool HighlightFragments { get; set; }

        // Length of a string to be considered a "big block"
        public const int BigTextSize = 1000;

        public const int HorizontalMouseStep = 30;

        private byte numberOfColors;

        // Contains H values of text color table
        private List<double> colorTableH;
        private List<SolidBrush> brushTable;
        private List<Pen> penTable;
        private List<Color> darkColorTable;
        private List<Color> lightColorTable;
        private Color grayColor; // Changes with Brightness


        public TextPair Hp => PText.Number() == 0 ? null : PText[HighlightedPair];

        private double brightness;
        
        public double Brightness
        {
            get => brightness;
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
                foreach (var s in brushTable)
                    s.Dispose();

                brushTable.Clear();

                foreach (var p in penTable)
                    p.Dispose();

                penTable.Clear();

                darkColorTable.Clear();
                lightColorTable.Clear();

            }

            for (byte i = 0; i < numberOfColors; i++)
            {
                Color lightColor = ColorRgb.Hsl2Rgb(colorTableH[i], 1, brightness);
                lightColorTable.Add(lightColor);

                brushTable.Add(new SolidBrush(lightColor));

                Color darkColor = ColorRgb.Hsl2Rgb(colorTableH[i], 1, brightness - 0.1);

                penTable.Add(new Pen(darkColor));
                darkColorTable.Add(darkColor);

            }

            grayColor = ColorRgb.Hsl2Rgb(0, 0, brightness - 0.1);
        }

        public bool EditWhenNipped { get; set; }

        public ScreenWord MouseCurrentWord { get; set; }

        private EditPairForm editPairForm;

        public bool Side1Set { get; set; }
        public bool Side2Set { get; set; }

        private Graphics PanelGraphics { get; }

        public int NaturalDividerPosition1 { get; set; }
        public int NaturalDividerPosition2 { get; set; }

        public ScreenWord NaturalDividerPosition1W { get; set; }
        public ScreenWord NaturalDividerPosition2W { get; set; }

        private readonly SortedList<int, List<ScreenWord>> wordsOnScreen;

        public Font textFont;

        private readonly Brush drawBrush = new SolidBrush(Color.Black);

        public int VMargin { get; set; }

        /// <summary>
        /// The current parallel text that is open
        /// </summary>
        public ParallelText PText { get; set; }


        /// <summary>
        /// Index of current Pair
        /// </summary>
        public int CurrentPair { get; set; }


        public TextPair this[int pairIndex] => PText[pairIndex];


        /// <summary>
        /// Buffered graphics on which we paint frames above rendered text + splitter page from SecondaryBG
        /// </summary>
        public BufferedGraphics PrimaryBg { get; set; }

        /// <summary>
        /// Buffered graphics on which we draw white frame, text and the splitter
        /// </summary>
        public BufferedGraphics SecondaryBg { set; get; }

        public int PanelMargin { get; set; }

        public void SetSplitterPositionByRatio()
        {
            // ReSharper disable once PossibleLossOfFraction
            SplitterPosition = (int)(Width * SplitterRatio - SplitterWidth / 2);
        }

        public void SetSplitterPositionByRatio(float newSplitterRatio)
        {
            SplitterRatio = newSplitterRatio;
            SetSplitterPositionByRatio();
        }

        public void SetSplitterRatioByPosition()
        {
            SplitterRatio = (splitterPosition + (float)SplitterWidth / 2) / Width;
        }

        private int LeftWidth { get; set; }

        private int RightWidth { get; set; }

        public int SplitterMoveOffset { get; set; }

        public int horizontalStartingPosition = 0;

        public int LastMouseX { get; set; }
        public int LastMouseY { get; set; }

        public bool Reversed { get; set; }

        private const byte FrameOffsetX = 5;
        private const int FrameOffsetY = 2;

        /// <summary>
        /// Splitter position in pixels
        /// </summary>
        private int splitterPosition;


        public void ComputeSideCoordinates()
        {
            if (layoutMode == LayoutModeAlternating || layoutMode == LayoutModeAdvanced)
            {
                text1Start = PanelMargin - FrameOffsetX;
                text1End = Width - PanelMargin + FrameOffsetX;

                text2Start = text1Start;
                text2End = text1End;
            }

            else if (Reversed)
            {
                text1Start = splitterPosition + SplitterWidth + PanelMargin - FrameOffsetX;
                text1End = Width - PanelMargin + FrameOffsetX;

                text2Start = PanelMargin - FrameOffsetX;
                text2End = LeftWidth - PanelMargin + FrameOffsetX;
            }
            else
            {
                text1Start = PanelMargin - FrameOffsetX;
                text1End = LeftWidth - PanelMargin + FrameOffsetX;

                text2Start = splitterPosition + SplitterWidth + PanelMargin - FrameOffsetX;
                text2End = Width - PanelMargin + FrameOffsetX;
            }
        }

        public int SplitterPosition
        {
            get => splitterPosition;

            set
            {
                splitterPosition = value;

                LeftWidth = splitterPosition;
                RightWidth = Width - SplitterWidth - LeftWidth;

                ComputeSideCoordinates();

            }
        }

        /// <summary>
        /// Splitter width in pixels
        /// </summary>
        public int SplitterWidth { get; set; }

        public Pen HighlightedPen { get; }
        public Pen AudioPen { get; }
        private Pen SuggestedPen { get; }
        public Pen CorrectedPen { get; }

        public DoubleFrame HighlightedFrame { get; }
        public DoubleFrame NippingFrame { get; }
        public Frame AudioSingleFrame { get; } // Used for highlighting audio in Alternating and Advanced modes

        private readonly Collection<AbstractFrame> frames;

        private int text1Start;
        private int text1End;

        private int text2Start;
        private int text2End;

        public int lineHeight;

        private readonly SortedDictionary<string, int> widthDictionary;

        public int verticalStartingPosition;
        private int indentLength;
        
        private int advancedHighlightedPair;
        
        private readonly Brush popUpBrush;
        
        private readonly Color popUpTextColor;
        private bool advancedModeShowPopups;

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

        private static void SetFramesByPair(TextPair textPair, DoubleFrame df)
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
        
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);

        private int WordWidth(string word)
        {
            // First, try to use data from the dictionary if it's there

            if (widthDictionary.TryGetValue(word, out var result)) return result;
            // Measure and store in the dictionary

            GetTextExtentPoint32(secondaryHdc, word, word.Length, out var sz);

            widthDictionary.Add(word, sz.Width);
            return sz.Width;

        }

        /// <summary>
        /// Computes the recommended width between words in pixels
        /// </summary>
        private void ComputeSpaceLength()
        {
            widthDictionary.Clear();
            
            secondaryHdc = PanelGraphics.GetHdc();
            var lastFont = SelectObject(secondaryHdc, textFont.ToHfont());

            SpaceLength = WordWidth(" ");

            ComputeIndent();

            DeleteObject(lastFont);
            PanelGraphics.ReleaseHdc(secondaryHdc);

            lineHeight = textFont.Height;

            ComputeNumberOfScreenLines();
        }

        private void ComputeNumberOfScreenLines()
        {
            NumberOfScreenLines = (Height - 2 * VMargin) / lineHeight;

            LastFullScreenLine = NumberOfScreenLines - 1;

            if (lineHeight * NumberOfScreenLines < Height - 2 * VMargin)
                NumberOfScreenLines++;

        }


        public void ResizeBufferedGraphic()
        {
            var controlGraphics = CreateGraphics();

            PrimaryBg = BufferedGraphicsManager.Current.Allocate(controlGraphics, ClientRectangle);
            SecondaryBg = BufferedGraphicsManager.Current.Allocate(PrimaryBg.Graphics, ClientRectangle);

            PrimaryBg.Graphics.SmoothingMode = SmoothingMode.HighQuality;
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

            frames = new Collection<AbstractFrame>();

            HighlightedPen = Frame.CreatePen(Color.LightBlue, DashStyle.Solid, 4.0F);
            
            AudioPen = Frame.CreatePen(Color.Gray, DashStyle.Dot, 2.0F);

            HighlightedFrame = new DoubleFrame(HighlightedPen, frames);

            AudioSingleFrame = new Frame(AudioPen, frames);

            CorrectedPen = Frame.CreatePen(Color.Peru, DashStyle.Solid, 2.0F);

            NippingFrame = new DoubleFrame(SuggestedPen, frames);

            var selectionPen = Frame.CreatePen(Color.Black, DashStyle.Solid, 2.0F);

            SelectionFrame = new Frame(selectionPen, frames);


            widthDictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);

            // ADVANCED MODE POPUP
            popUpInfo = new PopUpInfo();
            var advancedHighlightPen = Frame.CreatePen(Color.SteelBlue, DashStyle.Solid, 4.0F);
            advancedHighlightFrame = new Frame(advancedHighlightPen, frames);
            const int popUpOpacity = 210;
            popUpBrush = new SolidBrush(Color.FromArgb(popUpOpacity, Color.Black));
            popUpTextColor = Color.White;
            advancedModeShowPopups = false;
            
            PanelGraphics = CreateGraphics();

            textFont = new Font("Arial", 18.0F);
            
            ComputeSpaceLength();

            EditWhenNipped = false;

            InitializeColors();

            Brightness = 0.97;

            HighlightFirstWords = true;
            HighlightFragments = true;

            SuggestedPen = Frame.CreatePen(Color.SteelBlue, DashStyle.Dash, 2.0F);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0)
            {
                RegisterTouchWindow(Handle, 0);
            }
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


            double c = 0;

            for (var i = 1; i < 13; i++)
            {
                colorTableH.Add(c);
                c += 5.0 / 12;
                if (c >= 1.0) c -= 1;
            }

            numberOfColors = (byte)colorTableH.Count;

            brushTable = new List<SolidBrush>();
            penTable = new List<Pen>();
            darkColorTable = new List<Color>();
            lightColorTable = new List<Color>();

        }


        private void DrawSecondary()
        {
            var g = SecondaryBg.Graphics;
            g.Clear(Color.White);
        }

        public void DrawFrame(Frame frame)
        {
            if (frame == null)
                return;

            if (!frame.Visible)
                return;

            int textStart;
            int textEnd;

            if (frame.Side == 1)
            {
                textStart = text1Start;
                textEnd = text1End;
            }
            else
            {
                textStart = text2Start;
                textEnd = text2End;
            }

            var g = PrimaryBg.Graphics;

            if (frame.Line1 == frame.Line2)
                if (frame.Line1 == -1)
                {
                    // The frame begins and ends beyond the screen
                    // We draw two parallel, unconnected lines on both sides
                    g.DrawLine(frame.framePen, textStart, 0, textStart, Height - 1);
                    g.DrawLine(frame.framePen, textEnd, 0, textEnd, Height - 1);
                }
                else
                    // A piece of text
                    g.DrawRectangle(frame.framePen, textStart + frame.X1, VMargin + frame.Line1 * lineHeight - FrameOffsetY,
                    frame.X2 - frame.X1 + 2 * FrameOffsetX, lineHeight + 2 * FrameOffsetY);

            else if (frame.Line1 == -1)
                g.DrawLines(frame.framePen, new[]
                {
                    new Point(textStart, 0),
                    new Point(textStart, VMargin + (frame.Line2 + 1) * lineHeight + FrameOffsetY),
                    new Point(textStart + frame.X2 + 2 * FrameOffsetX, VMargin + (frame.Line2 + 1) * lineHeight + FrameOffsetY),
                    new Point(textStart + frame.X2 + 2 * FrameOffsetX, VMargin + frame.Line2 * lineHeight + FrameOffsetY),
                    new Point(textEnd, VMargin + frame.Line2 * lineHeight + FrameOffsetY),
                    new Point(textEnd, 0)
                });

            else if (frame.Line2 == -1)
                if (frame.X1 == 0) // Top starts at cursorX = 0
                    g.DrawLines(frame.framePen, new[]
                    {
                        new Point(textStart, Height - 1),
                        new Point(textStart, VMargin + frame.Line1 * lineHeight - FrameOffsetY),
                        new Point(textEnd, VMargin + frame.Line1 * lineHeight - FrameOffsetY),
                        new Point(textEnd, Height - 1)
                    });
                else
                    g.DrawLines(frame.framePen, new[]
                    {
                        new Point(textStart, Height - 1),
                        new Point(textStart, VMargin + (frame.Line1 + 1) * lineHeight - FrameOffsetY),
                        new Point(textStart + frame.X1, VMargin + (frame.Line1 + 1) * lineHeight - FrameOffsetY),
                        new Point(textStart + frame.X1, VMargin + frame.Line1 * lineHeight - FrameOffsetY),
                        new Point(textEnd, VMargin + frame.Line1 * lineHeight - FrameOffsetY),
                        new Point(textEnd, Height - 1)
                    });

            else if (frame.X1 == 0)
                g.DrawPolygon(frame.framePen, new[]
                {
                    new Point(textEnd, VMargin + frame.Line1 * lineHeight - FrameOffsetY),   
                    new Point(textStart, VMargin + frame.Line1 * lineHeight - FrameOffsetY),
                    new Point(textStart, VMargin + (frame.Line2 + 1) * lineHeight + FrameOffsetY),
                    new Point(textStart + frame.X2 + 2 * FrameOffsetX, VMargin + (frame.Line2 + 1) * lineHeight + FrameOffsetY),
                    new Point(textStart + frame.X2 + 2 * FrameOffsetX, VMargin + frame.Line2 * lineHeight + FrameOffsetY),
                    new Point(textEnd, VMargin + frame.Line2 * lineHeight + FrameOffsetY)
                });
            else
                g.DrawPolygon(frame.framePen, new[]
                {
                    new Point(textEnd, VMargin + frame.Line1 * lineHeight - FrameOffsetY),
                    new Point(textStart + frame.X1, VMargin + frame.Line1 * lineHeight - FrameOffsetY),
                    new Point(textStart + frame.X1, VMargin + (frame.Line1 + 1) * lineHeight - FrameOffsetY),
                    new Point(textStart, VMargin + (frame.Line1 + 1) * lineHeight - FrameOffsetY),
                    new Point(textStart, VMargin + (frame.Line2 + 1) * lineHeight + FrameOffsetY),
                    new Point(textStart + frame.X2 + 2 * FrameOffsetX, VMargin + (frame.Line2 + 1) * lineHeight + FrameOffsetY),
                    new Point(textStart + frame.X2 + 2 * FrameOffsetX, VMargin + frame.Line2 * lineHeight + FrameOffsetY),
                    new Point(textEnd, VMargin + frame.Line2 * lineHeight + FrameOffsetY)
                });

        }


        public void Render()
        {
            SecondaryBg.Render();

            // Draw frames
            foreach (var f in frames)
                f.Draw(this);

            var g = PrimaryBg.Graphics;

            if (!string.IsNullOrEmpty(DebugString))
                TextRenderer.DrawText(g, DebugString, textFont, new Point(PanelMargin, Height - lineHeight), Color.Red);

            if (EditWhenNipped)
                g.FillEllipse(Brushes.Red, Width - 13, 2, 10, 10);

            HighlightWord(MouseCurrentWord, Color.LightSkyBlue);

            RenderAdvancedPopup(g);

            PrimaryBg.Render();

        }

        private void RenderAdvancedPopup(Graphics g)
        {

            if (!(layoutMode == LayoutModeAdvanced && popUpInfo.visible))
                return;

            DrawBackground(0, popUpInfo.y, popUpInfo.x, popUpInfo.y2, popUpInfo.x2, g, popUpBrush);

            secondaryHdc = PrimaryBg.Graphics.GetHdc();

            SetTextColor(secondaryHdc, ColorTranslator.ToWin32(popUpTextColor));
            SetBkMode(secondaryHdc, 1);


            var lastFont = SelectObject(secondaryHdc, textFont.ToHfont());

            foreach(var sw in popUpInfo.words)
                TextOut(secondaryHdc, sw.X1 + PanelMargin + popUpInfo.x, VMargin + popUpInfo.offsetY * PopUpOffsetY + (popUpInfo.y + sw.Line) * lineHeight, sw.Word, sw.Word.Length);

            DeleteObject(lastFont);

            PrimaryBg.Graphics.ReleaseHdc(secondaryHdc);

        }

        [DllImport("gdi32.dll")]
        private static extern uint SetBkColor(IntPtr hdc, int crColor);

        private void HighlightWord(ScreenWord sw, Color color)
        {
            if (sw == null)
                return;

            //Graphics g = PrimaryBG.Graphics;

            secondaryHdc = PrimaryBg.Graphics.GetHdc();

            SetBkColor(secondaryHdc, ColorTranslator.ToWin32(color));
            SetBkMode(secondaryHdc, 2);
            SetTextColor(secondaryHdc, 0);

            var lastFont = SelectObject(secondaryHdc, textFont.ToHfont());

            TextOut(secondaryHdc, sw.X1, VMargin + sw.Line * lineHeight, sw.Word, sw.Word.Length);

            DeleteObject(lastFont);

            PrimaryBg.Graphics.ReleaseHdc(secondaryHdc);

            //TextRenderer.DrawText(g, sw.Word, textFont, new Point(sw.X1, VMargin + sw.Line * lineHeight),
            //    Color.Black, color, TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }

        private bool NeedToLineBreakFirstWord(TextPair p, byte side, ref int occLength, ref int maxWidth, int sL, bool startParagraph)
        {
            if (occLength == 0) return false;
            if (startParagraph) return true;

            return maxWidth - occLength - sL <= WordWidth(GetWord(p, side, 0));

        }

        public static string GetWord(TextPair p, byte side, int pos)
        {
            var word = new StringBuilder();

            var length = p.GetLength(side);

            while (pos < length)
            {
                var c = p.GetChar(side, pos);
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
        /// and runs them if that's the case
        /// </summary>
        /// <param name="startPair">Index of the start Pair</param>
        /// <param name="requiredLines">Number of lines required to be shown</param>
        public void PrepareScreen(int startPair, int requiredLines)
        {
            if (PText.Number() == 0)
                return;

            switch (layoutMode)
            {
                case LayoutModeNormal:
                    PrepareScreen_Normal(startPair, requiredLines);
                    break;
                case LayoutModeAlternating:
                    PrepareScreen_Alternating(startPair, requiredLines);
                    break;
                case LayoutModeAdvanced:
                    PrepareScreen_Advanced(startPair, requiredLines);
                    break;
            }
        }

        private void PrepareScreen_Advanced(int startPair, int requiredLines)
        {
            secondaryHdc = SecondaryBg.Graphics.GetHdc();

            // Required number of lines that we want to compute for the current Pair.
            // -1 means we want to compute ALL lines
            int requiredHeight;

            var remainder = requiredLines;

            var txtWidth = Width - 2 * PanelMargin;

            // If the startPair is not starting from a new Line on both texts (i. e. it is not a true-true Pair)
            // then we must ensure that all of the preceding pairs starting from the previous true-true pairs are computed,
            // because we need to know where exactly in the Line our Pair starts on both sides.
            // Actually, it is sufficient to stop at the closest partially-computed Pair (because if it is partially
            // computed we can safely compute it to the end)

            var cPair = startPair;

            var side = Reversed ? (byte) 2 : (byte) 1;

        Upstairs2:

            var p = PText.TextPairs[cPair];

            // Look for the closest true-true or partially computed Pair
            if (!p.StartParagraph(side) && p.Height == -1)
            {
                cPair--;
                goto Upstairs2;
            }

            var words = new Collection<CommonWordInfo>();

            var occLength = 0; // Occupied length in the current Line

            var lastFont = SelectObject(secondaryHdc, textFont.ToHfont());

        NextPair2:

            if (cPair < startPair || requiredLines == -1)
                requiredHeight = -1;
            else
            {
                if (p.Height != -1 && remainder <= p.Height)
                    goto CommonExit2;

                requiredHeight = remainder;
            }

            var height = p.Height;

            if (!p.AllLinesComputed(side))
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

            var prevPair = p;

            p = PText.TextPairs[cPair];

            if (
                words.Count > 0 &&
                NeedToLineBreakFirstWord(p, side, ref occLength, ref txtWidth, SpaceLength, p.StartParagraph(side)))
            {
                ParallelText.InsertWords(words, p.StartParagraph(side) ? 0 : txtWidth - occLength);

                prevPair.Height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        goto CommonExit2;
                }

                occLength = p.StartParagraph(side) ? indentLength : 0;

            }

            if (requiredLines == -1 && cPair > startPair && prevPair.Height > 0)
                goto CommonExit2;

            goto NextPair2;

        CommonExit2:

            

            DeleteObject(lastFont);
            SecondaryBg.Graphics.ReleaseHdc(secondaryHdc);
        }


        private void PrepareScreen_Alternating(int startPair, int requiredLines)
        {
            secondaryHdc = SecondaryBg.Graphics.GetHdc();

            // Required number of lines that we want to compute for the current Pair.
            // -1 means we want to compute ALL lines
            int requiredHeight;

            var remainder = requiredLines;

            var txtWidth = Width - 2 * PanelMargin;

            // If the startPair is not starting from a new Line on both texts (i. e. it is not a true-true Pair)
            // then we must ensure that all of the preceding pairs starting from the previous true-true pairs are computed,
            // because we need to know where exactly in the Line our Pair starts on both sides.
            // Actually, it is sufficient to stop at the closest partially-computed Pair (because if it is partially
            // computed we can safely compute it to the end)

            var cPair = startPair;

        Upstairs1:

            var p = PText.TextPairs[cPair];

            // Look for the closest true-true or partially computed Pair
            if (!(p.StartParagraph1 || p.StartParagraph2) && p.Height == -1)
            {
                cPair--;
                goto Upstairs1;
            }

            var words = new Collection<CommonWordInfo>();

            var occLength = 0; // Occupied length in the current Line

            var lastFont = SelectObject(secondaryHdc, textFont.ToHfont());

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

            var height = p.Height;

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

            var prevPair = p;

            p = PText.TextPairs[cPair];

            if (words.Count > 0 && NeedToLineBreakFirstWord(p, side1, ref occLength, ref txtWidth, SpaceLength, p.StartParagraph1 || p.StartParagraph2))
            {
                ParallelText.InsertWords(words, p.StartParagraph1 || p.StartParagraph2 ? 0 : txtWidth - occLength);

                prevPair.Height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        goto CommonExit1;
                }

                occLength = p.StartParagraph1 || p.StartParagraph2 ? indentLength : 0;

            }

            if (requiredLines == -1 && cPair > startPair && prevPair.Height > 0)
                goto CommonExit1;

            goto NextPair1;

        CommonExit1:

            DeleteObject(lastFont);
            SecondaryBg.Graphics.ReleaseHdc(secondaryHdc);
        }

        private void PrepareScreen_Normal(int startPair, int requiredLines)
        {
            secondaryHdc = SecondaryBg.Graphics.GetHdc();

            var lastFont = SelectObject(secondaryHdc, textFont.ToHfont());

            // Required number of lines that we want to compute for the current Pair.
            // -1 means we want to compute ALL lines
            int requiredHeight;

            var remainder = requiredLines;

            // If the startPair is not starting from a new Line on both texts (i. e. it is not a true-true Pair)
            // then we must ensure that all of the preceding pairs starting from the previous true-true pairs are computed,
            // because we need to know where exactly in the Line our Pair starts on both sides.
            // Actually, it is sufficient to stop at the closest partially-computed Pair (because if it is partially
            // computed we can safely compute it to the end)

            var cPair = startPair;

        Upstairs:

            var p = PText.TextPairs[cPair];

            // Look for the closest true-true or partially computed Pair
            if (!(p.StartParagraph1 && p.StartParagraph2) && p.Height == -1)
            {
                cPair--;
                goto Upstairs;
            }

            var words1 = new Collection<CommonWordInfo>();
            var words2 = new Collection<CommonWordInfo>();

            var occLength1 = 0; // Occupied length in the current Line
            var occLength2 = 0;

            int height;

            var width1 = (Reversed ? RightWidth : LeftWidth) - 2 * PanelMargin;
            var width2 = (Reversed ? LeftWidth : RightWidth) - 2 * PanelMargin;

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
                var height1 = p.Height;
                var height2 = p.Height;

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

            var prevPair = p;

            p = PText.TextPairs[cPair];

            if (NeedToLineBreakFirstWord(p, 1, ref occLength1, ref width1, SpaceLength, p.StartParagraph1)
                    || NeedToLineBreakFirstWord(p, 2, ref occLength2, ref width2, SpaceLength, p.StartParagraph2))
            {
                ParallelText.InsertWords(words1, 0);
                ParallelText.InsertWords(words2, 0);

                prevPair.Height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        goto CommonExit;
                }

                occLength1 = 0;
                occLength2 = 0;
            }

            if (requiredLines == -1 && cPair > startPair && prevPair.Height > 0)
                goto CommonExit;

            goto NextPair;

        CommonExit:

            DeleteObject(lastFont);
            SecondaryBg.Graphics.ReleaseHdc(secondaryHdc);
        }

        public void PrepareScreen()
        {
            PrepareScreen(CurrentPair, NumberOfScreenLines);
        }


        private void RenderBackground(Graphics g, int pairIndex, ref int offset, ref int cLine, byte side)
        {

            var p = PText.TextPairs[pairIndex];

            var list = p.ComputedWords(side);

            var renderedInfo = p.RenderedInfo(side);

            if (cLine >= NumberOfScreenLines
                || list == null
                || list.Count == 0)
            {
                renderedInfo.Valid = false;
                return;
            }

            var first = list[0];

            var last = list[list.Count - 1];

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

                if (layoutMode == LayoutModeAlternating)
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
            if (layoutMode != LayoutModeNormal)
                return;

            var big = (side == 1 ? p.Sb1 : p.Sb2) != null;

            // Before drawing text we must draw colored background
            // Colored 
            if (!big && HighlightFragments)
                DrawBackground(side, renderedInfo.Line1, renderedInfo.X1, renderedInfo.Line2, renderedInfo.X2B, SecondaryBg.Graphics,
                    brushTable[pairIndex % numberOfColors]);

            if (!HighlightFirstWords || big && HighlightFragments || list.Count <= 0 ||
                first.X2 < first.X1) return;
            var wordRect = new Rectangle(offset + first.X1, VMargin + (cLine + first.Line) * lineHeight, first.X2 - first.X1 + 1, lineHeight);

            using (var brush = new LinearGradientBrush(
                wordRect,
                big ? grayColor : darkColorTable[pairIndex % numberOfColors],
                HighlightFragments && !big ? lightColorTable[pairIndex % numberOfColors] : Color.White,
                LinearGradientMode.Horizontal))

                g.FillRectangle(brush, wordRect);

        }

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
            string lpString, int cbString);

        [DllImport("gdi32.dll")]
        private static extern uint SetTextColor(IntPtr hdc, int crColor);


        private bool NotFitOnScreenBase(int pairIndex)
        {
            if (pairIndex < CurrentPair)
                return true;

            var p = PText[pairIndex];

            // ReSharper disable once InvertIf
            if (layoutMode == LayoutModeAdvanced)
            {
                if (Reversed)
                    return (!p.RenderedInfo2.Valid
                        || p.RenderedInfo2.Line2 == -1
                        || p.RenderedInfo2.Line2 > LastFullScreenLine)
                        && !p.IsBig()
                        || p.RenderedInfo2.Line1 == -1;
                return (!p.RenderedInfo1.Valid
                        || p.RenderedInfo1.Line2 == -1
                        || p.RenderedInfo1.Line2 > LastFullScreenLine)
                       && !p.IsBig()
                       || p.RenderedInfo1.Line1 == -1;

            }

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

        private void RenderText(int pairIndex, ref int offset, ref int cLine, byte side)
        {

            var p = PText.TextPairs[pairIndex];

            var renderedInfo = p.RenderedInfo(side);

            if (!renderedInfo.Valid)
                return;

            int newColor;

            if (!EditMode && NotFitOnScreenBase(pairIndex))
                newColor = 2;
            else if (layoutMode == LayoutModeAlternating)
            {
                switch (AlternatingColorScheme)
                {
                    case FileUsageInfo.AlternatingColorSchemeGreenBlack:
                        newColor = Reversed == (side == 2) ? 3 : 1;
                        break;
                    case FileUsageInfo.AlternatingColorSchemeBlackGreen:
                        newColor = Reversed == (side == 2) ? 1 : 3;
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
                        SetTextColor(secondaryHdc, ColorTranslator.ToWin32(Color.Black));
                        break;
                    case 2:
                        SetTextColor(secondaryHdc, ColorTranslator.ToWin32(Color.Gray));
                        break;
                    case 3:
                        SetTextColor(secondaryHdc, ColorTranslator.ToWin32(Color.ForestGreen));
                        break;
                }
                
            }

            var list = p.ComputedWords(side);

            ScreenWord prevScreenWord = null;
            List<ScreenWord> l = null;

            var prevY = -1;

            if (list == null) return;
            
            foreach (var r in list)
            {
                var y = cLine + r.Line;

                if (y < 0)
                    continue;

                if (y >= NumberOfScreenLines)
                {
                    renderedInfo.Line2 = -1;
                    renderedInfo.X2 = 0;
                    return;
                }

                var s = new ScreenWord();

                if (prevScreenWord != null)
                {
                    s.Prev = prevScreenWord;
                    prevScreenWord.Next = s;
                }

                prevScreenWord = s;

                s.Fx1 = r.X1;

                var x = s.Fx1 + offset;

                if (y != prevY)
                {
                    if (!wordsOnScreen.TryGetValue(y, out l))
                    {
                        l = new List<ScreenWord>();
                        wordsOnScreen.Add(y, l);
                    }
                    prevY = y;

                    if (FirstRenderedPair == -1)
                        FirstRenderedPair = pairIndex;

                    if (pairIndex > LastRenderedPair)
                        LastRenderedPair = pairIndex;

                }

                var wrd = r.Word;

                TextOut(secondaryHdc, x, VMargin + y * lineHeight, wrd, wrd.Length);

                s.PairIndex = pairIndex;
                s.Pos = r.Position;
                s.Side = side;
                s.X1 = x;
                s.Fx2 = r.X2;
                s.Line = y;
                s.Word = wrd;

                l.Add(s);
            }
        }


        [DllImport("gdi32.dll")]
        private static extern int SetBkMode(IntPtr hdc, int iBkMode);

        [DllImport("gdi32.dll")]
        // ReSharper disable once IdentifierTypo
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr objectHandle);

        private void RenderPairText(byte side, int startPair, int negHeight)
        {
            DefineVarsByLayoutMode(side, out var offset, out var textSide);

            var cPair = startPair;
            var cLine = -negHeight;

            secondaryHdc = SecondaryBg.Graphics.GetHdc();

            SetBkMode(secondaryHdc, 1);

            var lastFont = SelectObject(secondaryHdc, textFont.ToHfont());

            // Text itself

            while (true)
            {

                var p = PText.TextPairs[cPair];

                RenderText(cPair, ref offset, ref cLine, textSide);

                cLine += p.Height;

                if (cLine >= NumberOfScreenLines)
                    break;

                if (cPair < PText.Number() - 1)
                    cPair++;
                else
                    break;
            }

            DeleteObject(lastFont);

            SecondaryBg.Graphics.ReleaseHdc(secondaryHdc);

        }


        public void RenderPairs(bool instantRender)
        {
            advancedHighlightedPair = -2;

            DrawSecondary();

            wordsOnScreen.Clear();

            if (PText.Number() == 0)
                return;

            var negHeight = 0;

            var startPair = CurrentPair;

            var p = PText.TextPairs[startPair];

            FirstRenderedPair = -1;
            LastRenderedPair = -1;

            // REWIND

            if (layoutMode == LayoutModeAdvanced)
            {

                var side = Reversed ? (byte) 2 : (byte) 1;

                // Special rewinding algorithm for advanced mode
                if (!p.StartParagraph(side))
                {
                    do
                    {
                        startPair--;
                        p = PText.TextPairs[startPair];
                    }
                    while (!p.StartParagraph(side) && p.Height == 0);

                    negHeight = p.Height;

                }

                currentTextColor = -1;

                RenderBackgroundSide(side, startPair, negHeight);
                RenderPairText(side, startPair, negHeight);

                if (instantRender)
                    Render();

                return;
                
            }

            if (layoutMode == LayoutModeAlternating)
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

            var g = SecondaryBg.Graphics;

            DefineVarsByLayoutMode(side, out var offset, out var textSide);

            
            var cPair = startPair;
            var cLine = -negHeight;

            // RenderedInfo and Backgrounds

            while (true)
            {

                var p = PText.TextPairs[cPair];

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
            switch (layoutMode)
            {
                case LayoutModeNormal:

                    if (side == 1)
                        offset = PanelMargin;
                    else
                        offset = SplitterPosition + SplitterWidth + PanelMargin;

                    textSide = Reversed == (side == 1) ? (byte) 2 : (byte) 1;

                    break;

                case LayoutModeAlternating:
                    textSide = Reversed == (side == 1) ? (byte) 1 : (byte) 2;
                    offset = PanelMargin;
                    break;

                case LayoutModeAdvanced:
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
                var h = PText.TextPairs[HighlightedPair];

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
                    var h = PText.TextPairs[HighlightedPair];

                    if (side == 0)
                    {
                        if (layoutMode == LayoutModeNormal)
                            SetFramesByPair(h, HighlightedFrame);
                        else if (layoutMode == LayoutModeAlternating)
                        {

                            var r1 = Reversed ? h.RenderedInfo2 : h.RenderedInfo1;
                            var r2 = Reversed ? h.RenderedInfo1 : h.RenderedInfo2;

                            AudioSingleFrame.Line1 = r1.Line1;
                            AudioSingleFrame.Line2 = r2.Line2;
                            AudioSingleFrame.X1 = r1.X1;
                            AudioSingleFrame.X2 = r2.X2;

                        }
                        else // Advanced
                            AudioSingleFrame.FillByRenderInfo(Reversed ? h.RenderedInfo2 : h.RenderedInfo1, 0);
                    }

                }

                UpdateSelectionFrame();
                
            }

        }


        private void ComputeIndent()
        {
            indentLength = layoutMode == LayoutModeNormal ? 0 : SpaceLength * 8;
        }


        private void ProcessCurrentWord(StringBuilder word, ref int occLength, Collection<CommonWordInfo> words, ref int height, TextPair p, byte side, ref int maxWidth, ref int wordPosition, bool eastern)
        {

            // Current Word complete, let's get its length

            var wordLength = WordWidth(word.ToString());

            var newStart = occLength + (words.Count == 0 || eastern && words.Count > 0 && words[words.Count - 1].Eastern ? 0 : SpaceLength);

            if (newStart + wordLength > maxWidth && words.Count != 0)
            {
                // Move this Word to the Next Line.
                // Before that we need to flush words to the DB

                ParallelText.InsertWords(words, maxWidth - occLength);

                height++;

                newStart = 0;

                occLength = 0;

            }

            // Add this Word to the current Line
            words.Add(new CommonWordInfo(p, word.ToString(), height, newStart, newStart + wordLength - 1, wordPosition, eastern, side));
            occLength = newStart + wordLength;

            word.Length = 0;

        }

        private void ProcessTextFromPair(TextPair p, byte side, ref int occLength, Collection<CommonWordInfo> words, ref int height, ref int maxWidth, int requiredHeight)
        {
            if (side == 1 ? p.AllLinesComputed1 : p.AllLinesComputed2)
                return;

            int pos;

            if (height == -1)
            {
                pos = 0;
                height = 0;
                if (side == 1)
                    p.continueFromNewLine1 = false;
                else
                    p.continueFromNewLine2 = false;
            }
            else
            {
                if (side == 1)
                {
                    pos = p.CurrentPos1;
                    if (p.continueFromNewLine1)
                    {
                        occLength = indentLength;
                        p.continueFromNewLine1 = false;
                    }
                    
                }
                else
                {
                    pos = p.CurrentPos2;
                    if (p.continueFromNewLine2)
                    {
                        occLength = indentLength;
                        p.continueFromNewLine2 = false;
                    }
                }
                
            }

            var wordPos = -1;

            var word = new StringBuilder();

            var textLength = p.GetLength(side);

            while (pos < textLength)
            {
                // Must be slow
                var c = p.GetChar(side, pos);

                if (c == ' ' || c == '\t' || c == '\r')
                {
                    if (word.Length == 0)
                    {
                        pos++;
                        continue;
                    }

                    ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref maxWidth, ref wordPos, false);

                    if (requiredHeight != -1 && requiredHeight == height)
                        goto CommonExit;

                    wordPos = -1;

                }
                else if (c == '\n')
                {
                    if (word.Length > 0)
                    {
                        ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref maxWidth, ref wordPos, false);
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
                            p.continueFromNewLine1 = true;
                        else
                            p.continueFromNewLine2 = true;
                        goto CommonExit;
                    }
                }

                else if (IsEasternCharacter(c))
                {
                    if (word.Length != 0)
                    {
                        ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref maxWidth, ref wordPos, false);
                        if (requiredHeight != -1 && requiredHeight == height)
                            goto CommonExit;
                    }

                    word.Append(c);

                    ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref maxWidth, ref pos, true);

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
                ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref maxWidth, ref wordPos, false);
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

        private static bool IsEasternCharacter(char c)
        {
            return c >= (char)0x2e80
                   && !(c >= (char)0xac00 && c <= (char)0xd7a3); // Hangul
        }

        public void ProcessLayoutChange()
        {
            // erase both tables
            PText.Truncate();
            ComputeNumberOfScreenLines();
            UpdateScreen();
        }

        private ScreenWord FindScreenWordByPosition(int pairIndex, int pos, byte side) =>
            pos == -1
                ? null
                : wordsOnScreen.SelectMany(kv => kv.Value)
                    .FirstOrDefault(sw => sw.PairIndex == pairIndex && sw.Pos == pos && sw.Side == side);

        private bool PosIsOnOrAfterLastScreenWord(int pairIndex, int pos1, int pos2)
        {
            var lastPos1 = -1;
            var lastPos2 = -1;

            foreach (var kv in wordsOnScreen)
                foreach (var sw in kv.Value)
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

            return pos1 > lastPos1 || pos2 > lastPos2;
        }


        public void SetNippingFrameByScreenWord(byte side, ScreenWord sw)
        {
            var f = (Frame)NippingFrame.Frame(side);

            if (sw?.Prev == null)
                f.Visible = false;
            else
            {
                var hf = (Frame)HighlightedFrame.Frame(side);
                f.Visible = true;
                f.framePen = SuggestedPen;
                f.Line1 = hf.Line1;
                f.X1 = hf.X1;
                f.Line2 = sw.Prev.Line;
                f.X2 = sw.Prev.Fx2;
            }
        }

        internal bool NipHighlightedPair()
        {
            if (NaturalDividerPosition1W == null
                    || NaturalDividerPosition2W == null)

                return false;

            var np = new TextPair();

            var hp = PText.TextPairs[HighlightedPair];

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

            TextPair p2;
            var i = HighlightedPair;

            do
            {
                i--;
                if (i < 0)
                    break;
                p2 = PText.TextPairs[i];
                p2.ClearComputedWords();
            }

            while (!p2.StartParagraph1 || !p2.StartParagraph2);

            hp.ClearComputedWords();


            // Truncate all following pairs until end or true-true

            HighlightedPair++;

            var j = HighlightedPair;

            while (j < PText.Number() - 1)
            {
                j++;
                var q = PText[j];
                if (q.StartParagraph1 && q.StartParagraph2)
                    break;
                q.ClearComputedWords();
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
            var whatToDo = mode == 0 ? stopWatch.IsRunning ? 1 : 2 : mode;

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


        private void NipASide(TextPair sourcePair, TextPair targetPair, byte side)
        {
            int finalPos;

            StringBuilder sourceSb;

            if (side == 1)
            {
                finalPos = NaturalDividerPosition1;
                if (sourcePair.Sb1 == null)
                {
                    sourcePair.Sb1 = new StringBuilder(sourcePair.Text1);
                    sourcePair.Text1 = null;
                }
                sourceSb = sourcePair.Sb1;
            }
            else
            {
                finalPos = NaturalDividerPosition2;
                if (sourcePair.Sb2 == null)
                {
                    sourcePair.Sb2 = new StringBuilder(sourcePair.Text2);
                    sourcePair.Text2 = null;
                }
                sourceSb = sourcePair.Sb2;
            }


            var sb = new StringBuilder();

            var state = 0;

            var pos = 0;

            while (pos < finalPos)
            {
                var c = sourceSb[pos];

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
                targetPair.Text1 = sb.ToString();
            else
                targetPair.Text2 = sb.ToString();



            var startParagraph = state == 3;
            if (side == 1)
                sourcePair.StartParagraph1 = startParagraph;
            else
                sourcePair.StartParagraph2 = startParagraph;


            // Cut everything before final_pos in the source text

            sourceSb.Remove(0, finalPos);

            if (sourceSb.Length >= BigTextSize) return;
            
            if (side == 1)
            {
                sourcePair.Text1 = sourceSb.ToString();
                sourcePair.Sb1 = null;
            }
            else
            {
                sourcePair.Text2 = sourceSb.ToString();
                sourcePair.Sb2 = null;
            }

        }


        private void DrawBackground(byte side, int line1, int x1, int line2, int x2, Graphics g, Brush brush)
        {
            int textStart;
            int textEnd;
            int width;
            int oY; // vertical offset
            int mX; // Background's own X margin (used only in Advanced mode for popup background)

            if (layoutMode == LayoutModeAdvanced)
            {
                textStart = PanelMargin;
                textEnd = Width - PanelMargin;
                width = Width - 2 * PanelMargin;
                oY = popUpInfo.offsetY * PopUpOffsetY;
                mX = 5;
            }
            else if (side == 1 && !Reversed || side == 2 && Reversed)
            {
                textStart = PanelMargin;
                textEnd = LeftWidth - PanelMargin;
                width = LeftWidth - 2 * PanelMargin;
                oY = 0;
                mX = 0;
            }
            else
            {
                textStart = splitterPosition + SplitterWidth + PanelMargin;
                textEnd = Width - PanelMargin;
                width = RightWidth - 2 * PanelMargin;
                oY = 0;
                mX = 0;
            }

            if (line1 == line2)
                if (line1 == -1)
                {
                    // The frame begins and ends beyond the screen
                    // We draw two parallel, unconnected lines on both sides
                    g.FillRectangle(brush, textStart - mX, oY, width + 2 * mX, Height);
                }
                else
                    // A piece of text
                    g.FillRectangle(brush, textStart + x1 - mX, VMargin + line1 * lineHeight + oY,
                    x2 - x1 + 2 * mX, lineHeight);

            else if (line1 == -1)
                g.FillPolygon(brush, new[]
                {
                    new Point(textStart - mX, oY),
                    new Point(textStart - mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textStart + x2 + mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textStart + x2 + mX, VMargin + line2 * lineHeight + oY),
                    new Point(textEnd + mX, VMargin + line2 * lineHeight + oY),
                    new Point(textEnd + mX,  oY)
                });

            else if (line2 == -1)
                if (x1 == 0) // Top starts at cursorX = 0
                    g.FillPolygon(brush, new[]
                    {
                        new Point(textStart - mX, Height - 1 + oY),
                        new Point(textStart - mX, VMargin + line1 * lineHeight + oY),
                        new Point(textEnd + mX, VMargin + line1 * lineHeight + oY),
                        new Point(textEnd + mX, Height - 1 + oY)
                    });
                else
                    g.FillPolygon(brush, new[]
                    {
                        new Point(textStart - mX, Height - 1 + oY),
                        new Point(textStart - mX, VMargin + (line1 + 1) * lineHeight + oY),
                        new Point(textStart + x1 - mX, VMargin + (line1 + 1) * lineHeight + oY),
                        new Point(textStart + x1 - mX, VMargin + line1 * lineHeight + oY),
                        new Point(textEnd + mX, VMargin + line1 * lineHeight + oY),
                        new Point(textEnd + mX, Height - 1 + oY)
                    });

            else if (x1 == 0)
                g.FillPolygon(brush, new[]
                {
                    new Point(textEnd + mX, VMargin + line1 * lineHeight + oY),   
                    new Point(textStart - mX, VMargin + line1 * lineHeight + oY),
                    new Point(textStart - mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textStart + x2 + mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textStart + x2 + mX, VMargin + line2 * lineHeight + oY),
                    new Point(textEnd + mX, VMargin + line2 * lineHeight + oY)
                });
            else
                g.FillPolygon(brush, new[]
                {
                    new Point(textEnd + mX, VMargin + line1 * lineHeight + oY),
                    new Point(textStart + x1 - mX, VMargin + line1 * lineHeight + oY),
                    new Point(textStart + x1 - mX, VMargin + (line1 + 1) * lineHeight + oY),
                    new Point(textStart - mX, VMargin + (line1 + 1) * lineHeight + oY),
                    new Point(textStart - mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textStart + x2 + mX, VMargin + (line2 + 1) * lineHeight + oY),
                    new Point(textStart + x2 + mX, VMargin + line2 * lineHeight + oY),
                    new Point(textEnd + mX, VMargin + line2 * lineHeight + oY)
                });
        }


        internal void DrawBackground(Background f)
        {
            if (!f.Visible)
                return;

            DrawBackground(f.Side, f.Line1, f.X1, f.Line2, f.X2, PrimaryBg.Graphics, f.BackgroundBrush);

        }

        public bool EditCurrentPair()
        {
            if (!EditMode || PText.Number() == 0)
                return false;

            return EditPair(HighlightedPair, false);

        }


        public void PairChanged(int pairIndex, bool forceUpdate)
        {
            if (pairIndex < 0)  return;
            
            var p = PText.TextPairs[pairIndex];

            p.ClearComputedWords();

            // Truncate all preceding pairs until true-true

            TextPair p2;
            var i = pairIndex;

            do
            {
                i--;
                if (i < 0)
                    break;
                p2 = PText.TextPairs[i];
                p2.ClearComputedWords();
            }
            while (!p2.StartParagraph1 || !p2.StartParagraph2);

            // Truncate all following pairs until end or true-true

            var j = pairIndex;

            while (j < PText.Number() - 1)
            {
                j++;
                var q = PText.TextPairs[j];
                if (q.StartParagraph1 && q.StartParagraph2)
                    break;
                q.ClearComputedWords();
            }

            if (!forceUpdate) return;
            
            FindFirstNaturalDividers();
            UpdateScreen();
        }

        public void UpdateScreen()
        {
            PrepareScreen();
            RenderPairs(false);
            UpdateFramesOnScreen(0);
            ProcessMousePosition(true, false);
            Render();
        }

        private int GetLineNumberAtPosition(int y)
        {
            return (y - VMargin) / lineHeight;
        }

        public ScreenWord GetWordAtPosition(int x, int y)
        {
            // When selection started in one side, look always for words on that side
            var side = !EditMode && !SelectionFinished ? SelectionSide : -1;
            
            // Let's see what we've got on this Line

            ScreenWord lastWord = null;

            if (!wordsOnScreen.TryGetValue(GetLineNumberAtPosition(y), out var listOfWords)) return null;
            
            foreach (var s in listOfWords)
            {
                if (side != -1 && s.Side != side)
                    continue;
                if (x >= s.X1 && (lastWord == null || lastWord.X1 < s.X1))
                    lastWord = s;
            }

            return lastWord;
        }

        public void ProcessMousePosition(bool forced, bool renderRequired)
        {
            // Let's check whether the cursor points to a Word

            var needToRender = false;

            var wordX = -1;
            var line = GetLineNumberAtPosition(LastMouseY);

            var foundWord = GetWordAtPosition(LastMouseX, LastMouseY);

            if (foundWord != null)
                wordX = foundWord.X1;

            if (forced || mouseTextLine != line || mouseTextX != wordX)
            {

                mouseTextWord = foundWord;
                mouseTextLine = line;
                mouseTextX = wordX;

                if (EditMode)
                {
                    if (foundWord == null
                        || HighlightedPair != -1 && foundWord.PairIndex != HighlightedPair)
                        MouseCurrentWord = null;
                    else
                        MouseCurrentWord = foundWord;

                    if (renderRequired)
                        needToRender = true;
                }
                else
                {

                    if (layoutMode == LayoutModeAdvanced && advancedModeShowPopups)
                    {

                        var newPair = mouseTextWord?.PairIndex ?? -1;

                        if (newPair != advancedHighlightedPair)
                        {
                            advancedHighlightedPair = newPair;

                            popUpInfo.visible = false;

                            if (newPair == -1)
                            {
                                advancedHighlightFrame.Visible = false;
                            }
                            else
                            {

                                var p = PText[newPair];

                                var r = Reversed ? p.RenderedInfo2 : p.RenderedInfo1;

                                if (r.Valid)
                                {
                                    advancedHighlightFrame.Visible = true;
                                    advancedHighlightFrame.Side = mouseTextWord.Side;
                                    advancedHighlightFrame.Line1 = r.Line1;
                                    advancedHighlightFrame.Line2 = r.Line2;
                                    advancedHighlightFrame.X1 = r.X1;
                                    advancedHighlightFrame.X2 = r.X2;

                                    var trSide = Reversed ? (byte)1 : (byte)2;

                                    var words = new Collection<CommonWordInfo>();

                                    var occLength = 0;

                                    var maxWidth = Width - 2 * PanelMargin;

                                    var height = 0;

                                    var c = p.ComputedWords(trSide);

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
                                            var linesToDec = c[0].Line;

                                            foreach (var wi in c)
                                                wi.Line -= linesToDec;
                                        }

                                        DeterminePopupPosition(c, r, maxWidth);

                                    }

                                }

                            }

                            needToRender = true;
                        }

                    }

                    if (!SelectionFinished && mouseTextWord != null)
                    {
                        // Update second part of the selection
                        if (Selection2Pair != mouseTextWord.PairIndex
                            || Selection2Position != mouseTextWord.Pos)
                        {
                            Selection2Pair = mouseTextWord.PairIndex;
                            Selection2Position = mouseTextWord.Pos;

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

            var last = c[c.Count - 1];

            // h: height

            var up = r.Line1 == -1 ? 0 : r.Line1;
            var down = r.Line2 == -1 ? 0 : LastFullScreenLine - r.Line2;

            var h1 = r.Line1 == -1 || r.Line2 == -1 ? LastFullScreenLine + 1 : r.Line2 - r.Line1 + 1;
            var h2 = last.Line + 1;

            // s: single
            var s1 = h1 == 1;
            var s2 = h2 == 1;

            var length2 = last.X2;


            popUpInfo.y = -1;

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
                else if (PopUpOffsetX + r.X2 + length2 <= maxWidth)
                    SetPopUpCoordinates(r.Line1, r.X2, 0, 1);
                else if (r.X1 >= PopUpOffsetX + length2)
                    SetPopUpCoordinates(r.Line1, r.X1 - length2, 0, -1);
                
            }
            else
            {
                if (h2 <= down)
                    SetPopUpCoordinates(r.Line2 + 1, 0, 1, 0);
                else if (s2 && r.Line2 != -1 && r.Line2 <= LastFullScreenLine && r.X2 + PopUpOffsetX + length2 <= maxWidth)
                    SetPopUpCoordinates(r.Line2, r.X2, 0, 1);
                else if (h2 <= up)
                    if (h2 == 1)
                        SetPopUpCoordinates(r.Line1 - 1, maxWidth - length2, -1, 0);
                    else
                        if (r.Line1 != -1 && r.X1 >= length2 + PopUpOffsetX)
                            SetPopUpCoordinates(r.Line1 - h2 + 1, 0, -1, 0);
                        else
                            SetPopUpCoordinates(r.Line1 - h2, 0, -1, 0);
                else if (s2 && r.Line1 != -1 && r.X1 >= length2 + PopUpOffsetX)
                    SetPopUpCoordinates(r.Line1, r.X1 - length2, 0, -1);
            }

            if (popUpInfo.y == -1)
                // The worst case: draw over
                SetPopUpCoordinates((NumberOfScreenLines - h2 + 1) / 2, 0, 0, 0);

            popUpInfo.y2 = popUpInfo.y + h2 - 1;
            popUpInfo.x2 = popUpInfo.x + last.X2;

            //DebugString = "[" + h1.ToString() + ']'
            //    + r.Line1.ToString() + ':' + r.X1.ToString() + '—'
            //    + r.Line2.ToString() + ':' + r.X2.ToString()
            //    + " -- > [" + h2.ToString() + ']' + popUpInfo.Y.ToString() + ':' + popUpInfo.X.ToString();

        }

        private void SetPopUpCoordinates(int y, int x, int offY, int offX)
        {
            popUpInfo.y = y;
            popUpInfo.x = x + offX * PopUpOffsetX;
            popUpInfo.offsetY = offY;
            
        }


        public bool EditPair(int pairIndex, bool newPair)
        {
            var p = PText.TextPairs[pairIndex];

            if (p.GetLength(1) > BigTextSize || p.GetLength(2) > BigTextSize)
            {
                EditWhenNipped = !EditWhenNipped;
                Render();
                return false;
            }

            PrepareEditForm();

            editPairForm.ParallelTextControl = this;
            editPairForm.PairIndex = pairIndex;
            if (newPair)
                editPairForm.SetFocusNewPair();
            editPairForm.ShowDialog();

            if (!editPairForm.Result) return false;
            
            if (pairIndex == 0)
            {
                PText[pairIndex].StartParagraph1 = true;
                PText[pairIndex].StartParagraph2 = true;
            }

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
            return true;

        }

        private void PrepareEditForm()
        {
            if (editPairForm != null) return;
            
            editPairForm = new EditPairForm();

            var parentForm = FindForm();

            editPairForm.Left = parentForm.Left + (parentForm.Width - editPairForm.Width) / 2;
            editPairForm.Top = parentForm.Top + (parentForm.Height - editPairForm.Height) / 2;
        }

        internal bool WesternJoint(int firstPair, byte side)
        {

            var first = PText[firstPair];
            var second = PText[firstPair + 1];

            var firstLength = first.GetLength(side);

            if (firstLength == 0 || second.GetLength(side) == 0)
                return false; // no space needed between texts since one or both are empty

            return !IsEasternCharacter(first.GetChar(side, firstLength - 1)) && !IsEasternCharacter(second.GetChar(side, 0));

        }


        internal void MergePairs(int firstPair)
        {

            var first = PText[firstPair];
            var second = PText[firstPair + 1];

            if (second.Sb1 == null)
            {
                second.Sb1 = new StringBuilder(second.Text1);
                second.Text1 = null;
            }

            if (second.Sb2 == null)
            {
                second.Sb2 = new StringBuilder(second.Text2);
                second.Text2 = null;
            }

            if (second.StartParagraph1)
            {
                second.Sb1.Insert(0, '\n');
                second.Sb1.Insert(0, '\r');
            }
            else if (WesternJoint(firstPair, 1))
                second.Sb1.Insert(0, ' ');

            if (second.StartParagraph2)
            {
                second.Sb2.Insert(0, '\n');
                second.Sb2.Insert(0, '\r');
            }
            else if (WesternJoint(firstPair, 2))
                second.Sb2.Insert(0, ' ');

            second.Sb1.Insert(0, first.Sb1 == null ? first.Text1 : first.Sb1.ToString());
            second.Sb2.Insert(0, first.Sb2 == null ? first.Text2 : first.Sb2.ToString());

            second.StartParagraph1 = first.StartParagraph1;
            second.StartParagraph2 = first.StartParagraph2;

            PText.TextPairs.Remove(first);

            if (second.Sb1.Length < BigTextSize)
            {
                second.Text1 = second.Sb1.ToString();
                second.Sb1 = null;
            }

            if (second.Sb2.Length < BigTextSize)
            {
                second.Text2 = second.Sb2.ToString();
                second.Sb2 = null;
            }

            if (first.AudioFileNumber == second.AudioFileNumber)
                second.TimeBeg = first.TimeBeg;

            Modified = true;

        }

        public float SplitterRatio { get; set; }

        public int Number => PText.Number();

        public bool MousePressed { get; set; }

        public bool EditMode { get; set; }


        internal void UpdateSelectionFrame()
        {

            if (SelectionFrame.Side == 0)
                return;

            AssignProperSelectionOrder(out var y1, out var y2, out var x1, out var x2);

            var word1 = FindScreenWordByPosition(y1, x1, SelectionSide);
            var word2 = FindScreenWordByPosition(y2, x2, SelectionSide);

            SelectionFrame.Visible = true;

            if (word1 == null)
                if (y1 >= LastRenderedPair)
                    SelectionFrame.Visible = false;
                else
                    SelectionFrame.Line1 = -1;
            else
            {
                SelectionFrame.Line1 = word1.Line;
                SelectionFrame.X1 = word1.Fx1;
            }

            if (word2 == null)
                if (y2 >= LastRenderedPair)
                    SelectionFrame.Line2 = -1;
                else
                    SelectionFrame.Visible = false;
            else
            {
                SelectionFrame.Line2 = word2.Line;
                SelectionFrame.X2 = word2.Fx2;
            }

        }

        public void AssignProperSelectionOrder(out int y1, out int y2, out int x1, out int x2)
        {
            if (Selection1Pair < Selection2Pair || Selection1Pair == Selection2Pair && Selection1Position <= Selection2Position)
            {
                y1 = Selection1Pair;
                x1 = Selection1Position;
                y2 = Selection2Pair;
                x2 = Selection2Position;
            }
            else
            {
                y1 = Selection2Pair;
                x1 = Selection2Position;
                y2 = Selection1Pair;
                x2 = Selection1Position;
            }
        }

        internal void SetFont(Font font)
        {
            textFont?.Dispose();

            textFont = font;
            
            ComputeSpaceLength();
            
            ProcessLayoutChange();
        }

        public int ReadingMode { get; set; }

        public int AlternatingColorScheme { get; set; }

        public void SetLayoutMode()
        {

            if (EditMode || ReadingMode == FileUsageInfo.NormalMode)
            {
                layoutMode = LayoutModeNormal;
            }
            else if (ReadingMode == FileUsageInfo.AlternatingMode)
                layoutMode = LayoutModeAlternating;
            else
                layoutMode = LayoutModeAdvanced;

            AudioSingleFrame.Visible = WithAudio() && !EditMode && layoutMode != LayoutModeNormal;
            HighlightedFrame.SetVisibility(EditMode);

            ComputeIndent();

            ComputeSideCoordinates();

            advancedHighlightedPair = -1;
            advancedHighlightFrame.Visible = false;

        }


        internal void SwitchAdvancedShowPopups()
        {
            if (advancedModeShowPopups)
            {
                advancedModeShowPopups = false;
                advancedHighlightFrame.Visible = false;
                popUpInfo.visible = false;
                advancedHighlightedPair = -3;
                Render();
            }
            else
            {
                advancedModeShowPopups = true;
                ProcessMousePosition(true, true);
            }
        }

        internal ScreenWord FindFirstScreenWord(int pairIndex, byte side)
        {
            return wordsOnScreen.SelectMany(kv => kv.Value)
                .FirstOrDefault(sw => sw.PairIndex == pairIndex && sw.Side == side);
        }

        internal ScreenWord FindLastScreenWord(int pairIndex, byte side)
        {
            ScreenWord lastFound = null;

            foreach (var kv in wordsOnScreen)
                foreach (var sw in kv.Value)
                    if (sw.PairIndex == pairIndex && sw.Side == side)
                        lastFound = sw;

            return lastFound;
        }



        internal bool NextRecommended(byte side, bool forward)
        {
            var divPos = side == 1 ? NaturalDividerPosition1 : NaturalDividerPosition2;

            var newPos = PText[HighlightedPair].NaturalDividerPosition(side, divPos, forward);

            if (newPos == -1)
                return false;
            if (side == 1)
                NaturalDividerPosition1 = newPos;
            else
                NaturalDividerPosition2 = newPos;

            return true;


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

            var sideToExport = leftSide ? 1 : 2;

            if (Reversed)
                sideToExport = 3 - sideToExport;

            PText.ExportText(fileName, sideToExport);
        }

        public bool WithAudio()
        {
            return PText.WithAudio;
        }

        public void ChangeReadingMode(int modeIndex)
        {

            var newReadingMode = 0;

            switch (modeIndex)
            {
                case 0:
                    newReadingMode = FileUsageInfo.NormalMode;
                    break;
                case 1:
                    newReadingMode = FileUsageInfo.AlternatingMode;
                    break;
                case 2:
                    newReadingMode = FileUsageInfo.AdvancedMode;
                    break;
            }

            if (newReadingMode == ReadingMode)
                return;

            ReadingMode = newReadingMode;

            SetLayoutMode();
            
            if (EditMode)
            {
                SelectionSide = 0;
                SelectionFrame.Visible = false;
            }
            else
            {
                HighlightedFrame.SetVisibility(false);
                NippingFrame.SetVisibility(false);
                MouseCurrentWord = null;
            }
            
            ProcessLayoutChange();

        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RegisterTouchWindow(IntPtr hWnd, uint ulFlags);
    }

    public class ScreenWord
    {
        public string Word { get; set; }
        public int X1 { get; set; } // start of the Word -- real point on screen
        public int PairIndex { get; set; } // index of Pair
        public byte Side { get; set; } // 1 or 2 -- the second or first text
        public int Pos { get; set; } // position of the Word in the Pair
        public int Fx1 { get; set; }
        public int Fx2 { get; set; }
        public int Line { get; set; }

        public ScreenWord Prev { get; set; }
        public ScreenWord Next { get; set; }

    }

    internal class PopUpInfo
    {
        public int x;
        public int y;
        public bool visible;
        public int offsetY;
        public int x2;
        public int y2;
        public Collection<WordInfo> words;
        
        public PopUpInfo()
        {
            x = -1;
            y = -1;
            offsetY = 0;

            visible = false;
        }
    }


}
