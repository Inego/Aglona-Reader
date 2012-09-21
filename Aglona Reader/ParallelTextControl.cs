using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace AglonaReader
{
    public partial class ParallelTextControl : UserControl
    {
        public bool editWhenNipped;

        public ScreenWord mouse_text_currentword = null;

        EditPairForm editPairForm;

        public TextPair nippedPair;

        public bool side1Set = false;
        public bool side2Set = false;

        Graphics panelGraphics;

        public int naturalDividerPosition1;
        public int naturalDividerPosition2;

        public ScreenWord naturalDividerPosition1_w;
        public ScreenWord naturalDividerPosition2_w;

        private SortedList<int, List<ScreenWord>> wordsOnScreen;


        public Font font = new System.Drawing.Font("times", 18.0F);
        private Brush drawBrush = new SolidBrush(Color.Black);

        public int vMargin = 2;

        /// <summary>
        /// The current parallel text that is open
        /// </summary>
        public ParallelText pText;


        /// <summary>
        /// Index of current pair
        /// </summary>
        public int currentPair;


        /// <summary>
        /// Buffered graphics on which we paint frames above rendered text + splitter page from secondaryBG
        /// </summary>
        public BufferedGraphics primaryBG;

        /// <summary>
        /// Buffered graphics on which we draw white f, text and the splitter
        /// </summary>
        public BufferedGraphics secondaryBG;

        public int panelMargin = 10;

        /// <summary>
        /// Splitter position relative to component width
        /// </summary>
        private float splitterRatio;

        public void SetSplitterPositionByRatio()
        {
            SplitterPosition = (int)((Width * splitterRatio) - splitterWidth / 2);
        }

        public void SetSplitterPositionByRatio(float newSplitterRatio)
        {
            splitterRatio = newSplitterRatio;
            SetSplitterPositionByRatio();
        }

        public void SetSplitterRatioByPosition()
        {
            splitterRatio = (splitterPosition + (float)splitterWidth / 2) / Width;
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
        /// X position of the right side
        /// </summary>
        public int rightPosition;

        public byte mouseStatus;

        public int splitterMoveOffset;

        public int lastMouseX;
        public int lastMouseY;

        public bool reversed;

        private byte frameoffset_x = 5;
        private int frameoffset_y = 2;



        /// <summary>
        /// Splitter position in pixels
        /// </summary>
        private int splitterPosition;


        public void ComputeSideCoordinates()
        {
            if (reversed)
            {
                text1start = splitterPosition + splitterWidth + panelMargin - frameoffset_x;
                text1end = Width - panelMargin + frameoffset_x;

                text2start = panelMargin - frameoffset_x;
                text2end = leftWidth - panelMargin + frameoffset_x;
            }
            else
            {
                text1start = panelMargin - frameoffset_x;
                text1end = leftWidth - panelMargin + frameoffset_x;

                text2start = splitterPosition + splitterWidth + panelMargin - frameoffset_x;
                text2end = Width - panelMargin + frameoffset_x;
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
                rightPosition = splitterPosition + splitterWidth;

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
        /// Brush of the splitter
        /// </summary>
        Brush splitterBrush;

        public Pen highlightedPen;
        public Pen suggestedPen = Frame.CreatePen(Color.LightBlue, DashStyle.Dash, 2.0F);
        public Pen correctedPen;
        public Pen previousPen = Frame.CreatePen(Color.YellowGreen, DashStyle.Solid, 1.0F);

        public DoubleFrame highlightedFrame;
        public DoubleFrame nippingFrame;
        public DoubleFrame nippedFrame;
        

        private List<AbstractFrame> frames;

        private int text1start;
        private int text1end;

        private int text2start;
        private int text2end;

        public int lineHeight;

        public StringFormat gt; // Generic Typographic

        private SortedDictionary<string, int> widthDictionary;

        public int spaceLength;

        /// <summary>
        /// Number of lines that fit on screen according to the current font and vertical size of the screen
        /// </summary>
        public int linesOnScreen;

        public string debugString = "";

        public int HighlightedPair;
        
        public int firstRenderedPair;
        public int lastRenderedPair;


        public void SetFramesByPair(TextPair p, DoubleFrame df)
        {
            if (p == null)
            {
                df.f1.visible = false;
                df.f2.visible = false;
            }
            else
            {
                df.f1.FillByRenderInfo(p.renderedInfo1, 1);
                df.f2.FillByRenderInfo(p.renderedInfo2, 2);
            }
        }


        public void SetFramesByPair(int pairIndex, DoubleFrame df)
        {
            SetFramesByPair(pText.textPairs[pairIndex], df);
        }

        public int WordWidth(string s, Graphics g)
        {

            int result;

            // First, try to use data from the dictionary if it's there

            if (widthDictionary.TryGetValue(s, out result)) return result;
            else
            {
                // Measure and store in the dictionary
                result = TextRenderer.MeasureText(g, s, font, Size.Empty, TextFormatFlags.NoPadding).Width;
                widthDictionary.Add(s, result);
                return result;
            }

        }

        /// <summary>
        /// Computes the recommended width between words in pixels
        /// </summary>
        /// <param name="g">Graphics on which the text is rendered</param>
        public void ComputeSpaceLength(Graphics g)
        {
            spaceLength = WordWidth(" ", g);
            lineHeight = font.Height;
        }

        /// <summary>
        /// Calculates linesOnScreen variable
        /// </summary>
        /// <param name="vSize">Vertical size of screen in pixels</param>
        public void CalculateLinesOnScreen()
        {
            linesOnScreen = (Height - 2 * vMargin) / lineHeight + 1;
        }


        public void ResizeBufferedGraphic()
        {
            primaryBG = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), ClientRectangle);
            secondaryBG = BufferedGraphicsManager.Current.Allocate(primaryBG.Graphics, ClientRectangle);

            primaryBG.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }


        private void InitializeText()
        {

            pText = new ParallelText();

            //pText.AddPair("......", "wergweg");
            //pText.AddPair("the Aglona Reader 2012", "wergweg");
            //pText.AddPair("........", "wergweg");

            //pText.AddPair("длинное слово", "long word");

            //pText.AddPair("длинное слово", "long word");
            //pText.AddPair("в", "v");

            //pText.AddPair("a\ne", "aa\nee");
            //pText.AddPair("b\nf", "bb\nff", true, true);
            //pText.AddPair("c\ng", "cc\ngg", true, true);
            //pText.AddPair("d\nh", "dd\nhh", true, true);

            //pText.AddPair("Это - длинное предложение, вперед.", "This is a very long sentence, go ahead!");
            //pText.AddPair("Одно слово.", "One word.");
            //pText.AddPair("Два слова.", "Two words.");

            //pText.AddPair("Однажды весною, в час небывало жаркого заката, в Москве, на Патриарших прудах, появились два гражданина.",
            //    "Reiz pavasari, neredzēti karsta saules rieta laikā, Maskavā, pie Patriarhu dīķiem, parādījās divi pilsoņi.");
            //pText.AddPair("Первый из них, одетый в летнюю серенькую пару, был маленького роста, упитан, лыс,",
            //    "Viens no viņiem, pelēkā vasaras uzvalkā ģērbies, bija mazs augumā, apaļīgs, ar pliku pauri,");
            //pText.AddPair("свою приличную шляпу пирожком нес в руке,",
            //    "pieklājīga izskata platmali nesa rokā, saņēmis aiz ielocījuma,");
            //pText.AddPair("а на хорошо выбритом лице его помещались сверхъестественных размеров очки в черной роговой оправе.",
            //    "bet uz viņa gludi skūtās sejas bija novietotas neiedomājami lielas brilles melnos raga rāmjos.");
            //pText.AddPair("Второй – плечистый, рыжеватый, вихрастый молодой человек в заломленной на затылок клетчатой кепке –",
            //    "Otrs — plecīgs, rūsganmatains, čirkains jauns cilvēks ar pakausī atbīdītu rūtainu naģeni —");
            //pText.AddPair("был в ковбойке, жеваных белых брюках и в черных тапочках.",
            //    "bija kovboju kreklā, saņurcītās, baltās biksēs un melnās čībiņās.");
            //pText.AddPair("Первый был не кто иной, как Михаил Александрович Берлиоз, председатель правления одной из крупнейших московских литературных ассоциаций, сокращенно именуемой МАССОЛИТ, и редактор толстого художественного журнала,",
            //    "Pirmais nebija neviens cits kā Mihails Aleksandrovičs Berliozs, priekšsēdētājs vienā no lielākajām Maskavas literātu asociācijām, saīsināti — MASLITASS, un bieza literārā žurnāla redaktors,");
            //pText.AddPair("а молодой спутник его – поэт Иван Николаевич Понырев, пишущий под псевдонимом Бездомный.",
            //    "bet viņa jaunais pavadonis — dzejnieks Ivans Nikolajevičs Ponirevs, pazīstams ar pseidonīmu Bezpajumtnieks.");
            //pText.AddPair("Попав в тень чуть зеленеющих лип, писатели первым долгом бросились к пестро раскрашенной будочке с надписью «Пиво и воды».",
            //    "Nonākuši tik tikko zaļot sākušo liepu paēnā, rakstnieki vispirms metās pie raibi izkrāsotās būdas ar uzrakstu «Alus un atspirdzinoši dzērieni».", true, true);
            //pText.AddPair("Да, следует отметить первую странность этого страшного майского вечера.",
            //    "Jā, jāatzīmē šī baismīgā maija vakara pirmā dīvainība.", true, true);
            //pText.AddPair("Не только у будочки, но и во всей аллее, параллельной Малой Бронной улице, не оказалось ни одного человека.",
            //    "Ne tikai pie būdas, bet visā Mazās Bronnajas ielai paralēlajā alejā nebija neviena cilvēka.");
            //pText.AddPair("В тот час, когда уж, кажется, и сил не было дышать,",
            //    "Tajā stundā, kad, šķita, vairs nav ko elpot,");
            //pText.AddPair("когда солнце, раскалив Москву, в сухом тумане валилось куда то за Садовое кольцо,",
            //    "kad saule, Maskavu nokveldējusi, sausā dūmakā gāzās lejā kaut kur aiz Dārzu loka,");
            //pText.AddPair("– никто не пришел под липы, никто не сел на скамейку, пуста была аллея.",
            //    "neviens ne nāca zem liepām, ne sēdās uz sola, aleja bija tukša.");
            //pText.AddPair("– Дайте нарзану, – попросил Берлиоз.",
            //    "—Dodiet narzānu, — Berliozs palūdza.", true, true);
            //pText.AddPair("– Нарзану нету, – ответила женщина в будочке и почему то обиделась.",
            //    "—Narzāna nava, — sieviete būdā atbildēja un nez kāpēc apvainojās.", true, true);
            //pText.AddPair("– Пиво есть? – сиплым голосом осведомился Бездомный.",
            //    "— Alus ir? — čerkstošā balsī painteresējās Bezpajumtnieks.", true, true);
            //pText.AddPair("– Пиво привезут к вечеру, – ответила женщина.",
            //    "— Alu atvedīs pret vakaru, — sieviete atbildēja.", true, true);
            //pText.AddPair("– А что есть? – спросил Берлиоз.",
            //    "— Bet kas ir? — jautāja Berliozs.", true, true);
            //pText.AddPair("– Абрикосовая, только теплая, – сказала женщина.",
            //    "— Aprikožu dzēriens, tikai silts, — teica sieviete.", true, true);
            //pText.AddPair("– Ну, давайте, давайте, давайте!..",
            //    "— Dodiet, dodiet, dodiet šurp!…", true, true);
            //pText.AddPair("Абрикосовая дала обильную желтую пену, и в воздухе запахло парикмахерской.",
            //    "Aprikožu dzēriens stipri putoja, un visapkārt iesmaržojās pēc frizētavas.", true, true);
            //pText.AddPair("Напившись, литераторы немедленно начали икать, расплатились и уселись на скамейке лицом к пруду и спиной к Бронной.",
            //    "Padzērušies literāti uzreiz sāka žagoties, samaksāja un apsēdās uz sola — ar sejām pret dīķi un mugurām pret Bronnajas ielu.");
            

            //pText.AddPair("","");
            //pText.AddPair("","");
        }


        public ParallelTextControl()
        {
            InitializeComponent();

            InitializeText();

            wordsOnScreen = new SortedList<int, List<ScreenWord>>();

            currentPair = 0;
            HighlightedPair = 0;

            reversed = false;

            vMargin = 3;

            lastMouseX = -1;
            lastMouseY = -1;

            splitterBrush = Brushes.LightGray;

            frames = new List<AbstractFrame>();

            Brush nipped = new SolidBrush(Color.FromArgb(30, 128, 255, 0));

            nippedFrame = new DoubleFrame(nipped, frames);

            highlightedPen = Frame.CreatePen(Color.LightBlue, DashStyle.Solid, 4.0F);
            highlightedFrame = new DoubleFrame(highlightedPen, frames);

            
            correctedPen = Frame.CreatePen(Color.Peru, DashStyle.Solid, 2.0F);
            

            nippingFrame = new DoubleFrame(suggestedPen, frames);

            gt = (StringFormat)StringFormat.GenericTypographic.Clone();

            widthDictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);

            panelGraphics = CreateGraphics();

            ComputeSpaceLength(panelGraphics);

            editPairForm = new EditPairForm();

            editWhenNipped = false;
        }


        public void DrawSecondary()
        {
            Graphics g = secondaryBG.Graphics;

            g.Clear(Color.White);

            //g.FillRectangle(splitterBrush, 

            //g.FillRectangle(splitterBrush, splitterPosition, vMargin, splitterWidth, Height - 2 * vMargin);
        }

        


        public void DrawFrame(Frame f)
        {

            if (!f.visible)
                return;

            int textstart;
            int textend;

            if (f.side == 1)
            {
                textstart = text1start;
                textend = text1end;
            }
            else
            {
                textstart = text2start;
                textend = text2end;
            }

            Graphics g = primaryBG.Graphics;

            if (f.line1 == f.line2)
                if (f.line1 == -1)
                {
                    // The frame begins and ends beyond the screen
                    // We draw two parallel, unconnected lines on both sides
                    g.DrawLine(f.pen, textstart, 0, textstart, Height - 1);
                    g.DrawLine(f.pen, textend, 0, textend, Height - 1);
                }
                else
                    // A piece of text
                    g.DrawRectangle(f.pen, textstart + f.x1, vMargin + f.line1 * lineHeight - frameoffset_y,
                    f.x2 - f.x1 + 2 * frameoffset_x, lineHeight + 2 * frameoffset_y);

            else if (f.line1 == -1)
                g.DrawLines(f.pen, new Point[]
                {
                    new Point(textstart, 0),
                    new Point(textstart, vMargin + (f.line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + f.x2 + 2 * frameoffset_x, vMargin + (f.line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + f.x2 + 2 * frameoffset_x, vMargin + f.line2 * lineHeight + frameoffset_y),
                    new Point(textend, vMargin + f.line2 * lineHeight + frameoffset_y),
                    new Point(textend, 0)
                });

            else if (f.line2 == -1)
                if (f.x1 == 0) // Top starts at x = 0
                    g.DrawLines(f.pen, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, vMargin + f.line1 * lineHeight - frameoffset_y),
                        new Point(textend, vMargin + f.line1 * lineHeight - frameoffset_y),
                        new Point(textend, Height - 1)
                    });
                else
                    g.DrawLines(f.pen, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, vMargin + (f.line1 + 1) * lineHeight - frameoffset_y),
                        new Point(textstart + f.x1, vMargin + (f.line1 + 1) * lineHeight - frameoffset_y),
                        new Point(textstart + f.x1, vMargin + f.line1 * lineHeight - frameoffset_y),
                        new Point(textend, vMargin + f.line1 * lineHeight - frameoffset_y),
                        new Point(textend, Height - 1)
                    });

            else if (f.x1 == 0)
                g.DrawPolygon(f.pen, new Point[]
                {
                    new Point(textend, vMargin + f.line1 * lineHeight - frameoffset_y),   
                    new Point(textstart, vMargin + f.line1 * lineHeight - frameoffset_y),
                    new Point(textstart, vMargin + (f.line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + f.x2 + 2 * frameoffset_x, vMargin + (f.line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + f.x2 + 2 * frameoffset_x, vMargin + f.line2 * lineHeight + frameoffset_y),
                    new Point(textend, vMargin + f.line2 * lineHeight + frameoffset_y)
                });
            else
                g.DrawPolygon(f.pen, new Point[]
                {
                    new Point(textend, vMargin + f.line1 * lineHeight - frameoffset_y),
                    new Point(textstart + f.x1, vMargin + f.line1 * lineHeight - frameoffset_y),
                    new Point(textstart + f.x1, vMargin + (f.line1 + 1) * lineHeight - frameoffset_y),
                    new Point(textstart, vMargin + (f.line1 + 1) * lineHeight - frameoffset_y),
                    new Point(textstart, vMargin + (f.line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + f.x2 + 2 * frameoffset_x, vMargin + (f.line2 + 1) * lineHeight + frameoffset_y),
                    new Point(textstart + f.x2 + 2 * frameoffset_x, vMargin + (f.line2) * lineHeight + frameoffset_y),
                    new Point(textend, vMargin + (f.line2) * lineHeight + frameoffset_y)
                });

        }


        public void Render()
        {
            secondaryBG.Render();

            // Draw frames
            foreach (AbstractFrame f in frames)
                f.Draw(this);

            Graphics g = primaryBG.Graphics;

            if (debugString != "")
                TextRenderer.DrawText(g, debugString, font, new Point(panelMargin, Height - lineHeight), Color.Red);

            if (editWhenNipped)
                g.FillEllipse(Brushes.Red, Width - 13, 2, 10, 10);

            HighlightWord(mouse_text_currentword, Color.LightSkyBlue);

            primaryBG.Render();

        }

        public void HighlightWord(ScreenWord sw, Color color)
        {
            if (sw == null)
                return;

            Graphics g = primaryBG.Graphics;

            TextRenderer.DrawText(g, sw.word, font, new Point(sw.x, vMargin + sw.line * lineHeight),
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
                if (c == ' ' || c == '\n') break;
                word.Append(c);
                pos++;
            }

            return (occLength + sL + WordWidth(word.ToString(), panelGraphics) > maxWidth);

        }

        /// <summary>
        /// Determines for the current pair whether calculations are required
        /// and runs them if that's the case
        /// </summary>
        /// <param name="startPair">Index of the start pair</param>
        /// <param name="limit">Number of lines</param>
        public void PrepareScreen(int startPair, int requiredLines)
        {

            if (pText.Number() == 0)
                return;


            // Required number of lines that we want to compute for the current pair.
            // -1 means we want to compute ALL lines
            int requiredHeight;

            int remainder = requiredLines;

            TextPair p;

            // If the startPair is not starting from a new line on both texts (i. e. it is not a true-true pair)
            // then we must ensure that all of the preceding pairs starting from the previous true-true pairs are computed,
            // because we need to know where exactly in the line our pair starts on both sides.
            // Actually, it is sufficient to stop at the closest partially-computed pair (because if it is partially
            // computed we can safely compute it to the end)

            int cPair = startPair;

        Upstairs:

            p = pText.textPairs[cPair];

            // Look for the closest true-true or partially computed pair
            if (!(p.startParagraph1 && p.startParagraph2) && p.height == -1)
            {
                cPair--;
                goto Upstairs;
            }

            List<DB_Common_Row> words1 = new List<DB_Common_Row>();
            List<DB_Common_Row> words2 = new List<DB_Common_Row>();

            int occLength1 = 0; // Occupied length in the current line
            int occLength2 = 0;

            int height1;
            int height2;
            int height;

            TextPair prev_pair = null;
            

            int width1 = (reversed ? RightWidth : LeftWidth) - 2 * panelMargin;
            int width2 = (reversed ? LeftWidth : RightWidth) - 2 * panelMargin;

        NextPair:

            if (cPair < startPair)
                requiredHeight = -1;
            else
            {
                
                if (p.height != -1 && remainder <= p.height)
                    // cool
                    return;

                requiredHeight = remainder;

            }

            if (p.allLinesComputed1 && p.allLinesComputed2)
            {
                height = p.height;                 
            }

            else
            {

                height1 = p.height;
                height2 = p.height;

                if (p.height == -1)
                    pText.computedPairs.Add(p);

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

                p.height = height;

            }

            if (requiredHeight != -1)
            {
                remainder -= height;

                if (remainder <= 0)
                    return;
            }

            // Are there more text pairs?

            if (cPair + 1 == pText.Number())
            {
                // This was the last pair, no more coming.
                ParallelText.InsertWords(words1, 0, 1);
                ParallelText.InsertWords(words2, 0, 2);
                return;
            }

            // ...There are.

            cPair++;

            prev_pair = p;

            p = pText.textPairs[cPair];

            if (NeedToLineBreakFirstWord(p, 1, ref occLength1, ref width1, spaceLength, p.startParagraph1)
                    || NeedToLineBreakFirstWord(p, 2, ref occLength2, ref width2, spaceLength, p.startParagraph2))
            {
                ParallelText.InsertWords(words1, 0, 1);
                ParallelText.InsertWords(words2, 0, 2);

                prev_pair.height++;

                if (requiredHeight != -1)
                {
                    remainder--;

                    if (remainder <= 0)
                        return;
                }

                occLength1 = 0;
                occLength2 = 0;
            }

            goto NextPair;

        }

        public void PrepareScreen()
        {
            PrepareScreen(currentPair, linesOnScreen);
        }



        private void RenderText(Graphics g, int pairIndex, ref int offset, ref int cLine, byte side)
        {

            TextPair p = pText.textPairs[pairIndex];

            int x;
            int y = -1;

            ScreenWord prev_screen_word = null;
            ScreenWord s = null;
            List<ScreenWord> l = null;

            int prev_y = -1;

            bool first = true;

            RenderedTextInfo renderedInfo = side == 1 ? p.renderedInfo1 : p.renderedInfo2;
            List<DB_Row> list = p.ComputedWords(side);

            if (list != null)
            foreach (DB_Row r in list)
            {

                y = cLine + r.line;

                if (y < 0)
                    continue;

                if (y >= linesOnScreen)
                {
                    renderedInfo.line2 = -1;
                    renderedInfo.x2 = 0;
                    return;
                }

                

                s = new ScreenWord();

                if (prev_screen_word != null)
                {
                    s.prev = prev_screen_word;
                    prev_screen_word.next = s;
                }

                prev_screen_word = s;

                s.f_x = r.x;

                x = s.f_x + offset;

                if (y != prev_y)
                {
                    if (!wordsOnScreen.TryGetValue(y, out l))
                    {
                        l = new List<ScreenWord>();
                        wordsOnScreen.Add(y, l);
                    }
                    prev_y = y;

                    if (firstRenderedPair == -1)
                        firstRenderedPair = pairIndex;

                    lastRenderedPair = pairIndex;
                }

                string wrd = r.word;

                // Draw next word
                TextRenderer.DrawText(g, wrd, font, new Point(x, vMargin + y * lineHeight),
                    Color.Black, TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

                s.pair = p;
                s.pos = r.pos;
                s.side = side;
                s.x = x;
                s.f_x2 = r.x2;
                s.x2 = s.f_x2 + offset;
                s.line = y;
                s.word = wrd;

                l.Add(s);

                if (first)
                {
                    first = false;
                    renderedInfo.valid = true;
                    if (cLine < 0)
                    {
                        // If the current pair starts above the screen,
                        // put line1 as -1 for a special frame
                        renderedInfo.line1 = -1;
                        renderedInfo.x1 = 0;
                    }
                    else
                    {
                        renderedInfo.line1 = y;
                        renderedInfo.x1 = s.f_x;
                    }
                }

            }

            if (s == null)
                renderedInfo.valid = false;
            else if (!(side == 1 ? p.allLinesComputed1 : p.allLinesComputed2))
            {
                renderedInfo.line2 = -1;
                renderedInfo.x2 = 0;
            }
            else
            {
                renderedInfo.line2 = y;
                renderedInfo.x2 = s.f_x2;
            }
        }


        /// <summary>
        /// Renders a side
        /// </summary>
        /// <param name="side">Number of side in SCREEN terms, not in pair terms</param>
        private void RenderPairSide(byte side, int startPair, int negHeight)
        {

            Graphics g = secondaryBG.Graphics;

            int cPair = startPair;
            int cLine = -negHeight;

            int offset;
            byte textSide;

            if (side == 1)
                offset = panelMargin;
            else
                offset = SplitterPosition + SplitterWidth + panelMargin;

            if (reversed == (side == 1))
                textSide = 2;
            else
                textSide = 1;
            
        NextPair:

            TextPair p = pText.textPairs[cPair];

            RenderText(g, cPair, ref offset, ref cLine, textSide);
            
            cLine += p.height;

            if (cLine >= linesOnScreen)
                return;

            if (cPair < pText.Number() - 1)
            {
                cPair++;
                goto NextPair;
            }

        }
          


        public void RenderPairs()
        {
            DrawSecondary();
            wordsOnScreen.Clear();

            if (pText.Number() == 0)
                return;

            

            TextPair p;

            int negHeight = 0;

            int startPair = currentPair;

            p = pText.textPairs[startPair];

            if (!(p.startParagraph1 && p.startParagraph2))
            {
                // Must rewind back for the closest (from above) pair that is either true-true or
                // multi-line (i. e. with height > 0)

                do
                {
                    startPair--;
                    p = pText.textPairs[startPair];
                }
                while (!(p.startParagraph1 && p.startParagraph2) && p.height == 0);

                negHeight = p.height;

            }

            firstRenderedPair = -1;
            lastRenderedPair = -1;

            // NOTE. Pairs are run twice (instead of only once, rendering both pairs of text from them simultaneously),
            // because we want to fill wordsOnScreen in such a way that every line has words with strictly increasing x.
            // If we render text1 and text2 from the second pair, then text1 and text2 from the first pair
            // and so on, when more than one pair is on one line, the corresponding List in
            // wordsOnScreen will have these coordinates (let ai be x coords of the second text,
            // bi be x coords of the first text of the pair):
            // p1a1 p1a2 ... p1an p1b1 p1b2 ... p1bn p2a1 p2a2 ... p2an p2b1 p2b2 ... p2bn
            // ...which is incorrect (because p1bn is greater than p2a1).
            // Correct sequence is
            // p1a1 p1a2 ... p1an p2a1 p2a2 ... p2an p1b1 p1b2 ... p1bn p2b1 p2b2 ... p2bn

            RenderPairSide(1, startPair, negHeight);
            RenderPairSide(2, startPair, negHeight);

        }

        public void FindNaturalDividersOnScreen(byte side)
        {
            if (pText.Number() > 0)
            {
                TextPair h = pText.textPairs[HighlightedPair];

                if (side == 0)
                {
                    SetFramesByPair(h, highlightedFrame);
                    SetFramesByPair(nippedPair, nippedFrame);
                }

                if (side == 0 || side == 1)
                {
                    naturalDividerPosition1_w = FindScreenWordByPosition(h, naturalDividerPosition1, 1);
                    SetNippingFrameByScreenWord(1, naturalDividerPosition1_w);
                }

                if (side == 0 || side == 2)
                {
                    naturalDividerPosition2_w = FindScreenWordByPosition(h, naturalDividerPosition2, 2);
                    SetNippingFrameByScreenWord(2, naturalDividerPosition2_w);
                }
            }
        }

        private void ProcessCurrentWord(StringBuilder word, ref int occLength, List<DB_Common_Row> words, ref int Height, TextPair p, byte side, ref int MaxWidth, ref int wordPosition)
        {

            // Current word complete, let's get its length
            int wordLength = WordWidth(word.ToString(), panelGraphics);

            int newStart = occLength + (occLength == 0 ? 0 : spaceLength);

            if (newStart + wordLength > MaxWidth)
            {
                // Move this word to the next line.
                // Before that we need to flush words to the DB

                ParallelText.InsertWords(words, MaxWidth - occLength, side);

                Height++;

                newStart = 0;

                occLength = 0;

            }

            // Add this word to the current line
            words.Add(new DB_Common_Row(p, word.ToString(), Height, newStart, newStart + wordLength - 1, wordPosition));
            occLength = newStart + wordLength;

            word.Clear();
            
        }

        void ProcessTextFromPair(TextPair p, byte side, ref int occLength, List<DB_Common_Row> words, ref int height, ref int MaxWidth, int requiredHeight)
        {
            if ((side == 1) ? p.allLinesComputed1 : p.allLinesComputed2)
                return;
           
            int pos;
            int wordPos;

            if (height == -1)
            {
                pos = 0;
                height = 0;
            }
            else
            {
                pos = (side == 1) ? p.currentPos1 : p.currentPos2;
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

            // Reached the end, process current word (if there is any)
            if (word.Length > 0)
            {
                ProcessCurrentWord(word, ref occLength, words, ref height, p, side, ref MaxWidth, ref wordPos);
                if (requiredHeight != -1 && requiredHeight == height)
                    goto CommonExit;
            }

            if (side == 1)
                p.allLinesComputed1 = true;
            else
                p.allLinesComputed2 = true;

            return;

            // Get here when the Height is reached
        CommonExit:
            
            if (side == 1)
                p.currentPos1 = wordPos;
            else
                p.currentPos2 = wordPos;
            
        }

        public void Recompute()
        {

            // erase both tables
            pText.Truncate();

            CalculateLinesOnScreen();

            PrepareScreen(currentPair, linesOnScreen);
            //PrepareScreen(0, 1);
            //PrepareScreen(0, 2);

            ResizeBufferedGraphic();
            RenderPairs();
            FindNaturalDividersOnScreen(0);
            Render();

        }

        private ScreenWord FindScreenWordByPosition(TextPair p, int pos, byte side)
        {
            if (pos != -1)
                foreach (KeyValuePair<int, List<ScreenWord>> kv in wordsOnScreen)
                    foreach (ScreenWord sw in kv.Value)
                        if (sw.pair == p && sw.pos == pos && sw.side == side)
                        {
                            return sw;
                        }
            
            return null;

        }

        public void SetNippingFrameByScreenWord(byte side, ScreenWord sw)
        {
            Frame f = (Frame) nippingFrame.Frame(side);

            if (sw == null || sw.prev == null)
                f.visible = false;
            else
            {
                Frame hf = (Frame) highlightedFrame.Frame(side);
                f.visible = true;
                f.pen = suggestedPen;
                f.line1 = hf.line1;
                f.x1 = hf.x1;
                f.line2 = sw.prev.line;
                f.x2 = sw.prev.f_x2;
            }
        }

        public ScreenWord WordAfterCursor(int line, int x)
        {

            List<ScreenWord> listOfWords;

            if (wordsOnScreen.TryGetValue(line, out listOfWords))
            {
                // let's see...

                foreach (ScreenWord s in listOfWords)
                {
                    //if (e.X < s.x || e.X > s.x2) continue;
                    if (x > s.x2) continue;
                    return s;
                }
            }

            return null;
        }



        internal void FindNaturalDividers(byte side)
        {

            if (pText.Number() == 0)
                return;

            // Look for natural dividers in the current (highlighted) pair

            TextPair p = pText.textPairs[HighlightedPair];

            if (side == 0 || side == 1)
                naturalDividerPosition1 = p.NaturalDividerPosition(1);
            if (side == 0 || side == 2)
                naturalDividerPosition2 = p.NaturalDividerPosition(2);

        }

        internal void NipHighlightedPair()
        {
            if (naturalDividerPosition1_w == null
                    || naturalDividerPosition2_w == null)

                return;

            TextPair np = new TextPair();

            TextPair hp = pText.textPairs[HighlightedPair];

            np.SetRecommendedNaturals(hp);
            hp.ClearRecommendedNaturals();

            np.startParagraph1 = hp.startParagraph1;
            np.startParagraph2 = hp.startParagraph2;

            NipASide(hp, np, 1);
            NipASide(hp, np, 2);

            pText.textPairs.Insert(HighlightedPair, np);

            if (editWhenNipped)
            {
                editPairForm.pTC = this;
                editPairForm.pairIndex = HighlightedPair;
                editPairForm.ShowDialog();
                editWhenNipped = false;
            }

            // Truncate all preceding pairs until true-true

            if (!(np.startParagraph1 && np.startParagraph2))
            {
                
                TextPair _p;
                int i = HighlightedPair;

                do
                {
                    i--;
                    _p = pText.textPairs[i];
                    _p.ClearComputedWords();
                }

                while (!_p.startParagraph1 || !_p.startParagraph2);
            }

            hp.ClearComputedWords();
            

            // Truncate all following pairs until end or true-true

            HighlightedPair++;

            int j = HighlightedPair;

            TextPair _q;

            while (j < pText.Number() - 1)
            {
                j++;
                _q = pText.textPairs[j];
                if (_q.startParagraph1 && _q.startParagraph2)
                    break;
                _q.ClearComputedWords();
            }

            PrepareScreen();
            RenderPairs();

            FindNaturalDividers(0);

            FindNaturalDividersOnScreen(0);
            SetFramesByPair(np, nippedFrame);
            Render();

            nippedPair = np;

            side1Set = false;
            side2Set = false;

        }

        

        private void NipASide(TextPair source_pair, TextPair target_pair, byte side)
        {
            int final_pos;

            StringBuilder source_sb = null;
            
            if (side == 1)
            {
                final_pos = naturalDividerPosition1;
                if (source_pair.sb1 == null)
                {
                    source_pair.sb1 = new StringBuilder(source_pair.text1);
                    source_pair.text1 = null;
                }
                source_sb = source_pair.sb1;
            }
            else
            {
                final_pos = naturalDividerPosition2;
                if (source_pair.sb2 == null)
                {
                    source_pair.sb2 = new StringBuilder(source_pair.text2);
                    source_pair.text2 = null;
                }
                source_sb = source_pair.sb2;
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
                target_pair.text1 = sb.ToString();
            else
                target_pair.text2 = sb.ToString();



            bool startParagraph = (state == 3);
            if (side == 1)
                source_pair.startParagraph1 = startParagraph;
            else
                source_pair.startParagraph2 = startParagraph;
            

            // Cut everything before final_pos in the source text

            source_sb.Remove(0, final_pos);
    
        }

        internal void DrawDoubleFrame(DoubleFrame doubleFrame)
        {
            doubleFrame.f1.Draw(this);
            doubleFrame.f2.Draw(this);
        }


        internal void DrawBackground(Background f)
        {
            if (!f.visible)
                return;

            int textstart;
            int textend;
            int width;

            if (f.side == 1 && !reversed || f.side == 2 && reversed)
            {
                textstart = panelMargin;
                textend = leftWidth - panelMargin;
                width = leftWidth - 2 * panelMargin;
            }
            else
            {
                textstart = splitterPosition + splitterWidth + panelMargin;
                textend = Width - panelMargin;
                width = rightWidth - 2 * panelMargin;
            }

            Graphics g = primaryBG.Graphics;

            if (f.line1 == f.line2)
                if (f.line1 == -1)
                {
                    // The frame begins and ends beyond the screen
                    // We draw two parallel, unconnected lines on both sides
                    g.FillRectangle(f.brush, textstart, 0, width, Height);
                }
                else
                    // A piece of text
                    g.FillRectangle(f.brush, textstart + f.x1, vMargin + f.line1 * lineHeight,
                    f.x2 - f.x1, lineHeight);

            else if (f.line1 == -1)
                g.FillPolygon(f.brush, new Point[]
                {
                    new Point(textstart, 0),
                    new Point(textstart, vMargin + (f.line2 + 1) * lineHeight),
                    new Point(textstart + f.x2, vMargin + (f.line2 + 1) * lineHeight),
                    new Point(textstart + f.x2, vMargin + f.line2 * lineHeight),
                    new Point(textend, vMargin + f.line2 * lineHeight),
                    new Point(textend, 0)
                });

            else if (f.line2 == -1)
                if (f.x1 == 0) // Top starts at x = 0
                    g.FillPolygon(f.brush, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, vMargin + f.line1 * lineHeight),
                        new Point(textend, vMargin + f.line1 * lineHeight),
                        new Point(textend, Height - 1)
                    });
                else
                    g.FillPolygon(f.brush, new Point[]
                    {
                        new Point(textstart, Height - 1),
                        new Point(textstart, vMargin + (f.line1 + 1) * lineHeight),
                        new Point(textstart + f.x1, vMargin + (f.line1 + 1) * lineHeight),
                        new Point(textstart + f.x1, vMargin + f.line1 * lineHeight),
                        new Point(textend, vMargin + f.line1 * lineHeight),
                        new Point(textend, Height - 1)
                    });

            else if (f.x1 == 0)
                g.FillPolygon(f.brush, new Point[]
                {
                    new Point(textend, vMargin + f.line1 * lineHeight),   
                    new Point(textstart, vMargin + f.line1 * lineHeight),
                    new Point(textstart, vMargin + (f.line2 + 1) * lineHeight),
                    new Point(textstart + f.x2, vMargin + (f.line2 + 1) * lineHeight),
                    new Point(textstart + f.x2, vMargin + f.line2 * lineHeight),
                    new Point(textend, vMargin + f.line2 * lineHeight)
                });
            else
                g.FillPolygon(f.brush, new Point[]
                {
                    new Point(textend, vMargin + f.line1 * lineHeight),
                    new Point(textstart + f.x1, vMargin + f.line1 * lineHeight),
                    new Point(textstart + f.x1, vMargin + (f.line1 + 1) * lineHeight),
                    new Point(textstart, vMargin + (f.line1 + 1) * lineHeight),
                    new Point(textstart, vMargin + (f.line2 + 1) * lineHeight),
                    new Point(textstart + f.x2, vMargin + (f.line2 + 1) * lineHeight),
                    new Point(textstart + f.x2, vMargin + (f.line2) * lineHeight),
                    new Point(textend, vMargin + (f.line2) * lineHeight)
                });
        }

        public void EditCurrentPair()
        {
            if (pText.Number() == 0)
                return;

            EditPair(HighlightedPair);

        }


        public void PairChanged(int pairIndex)
        {
            TextPair p = pText.textPairs[pairIndex];

            p.ClearComputedWords();

            // Truncate all preceding pairs until true-true

            if (!(p.startParagraph1 && p.startParagraph2))
            {

                TextPair _p;
                int i = pairIndex;

                do
                {
                    i--;
                    _p = pText.textPairs[i];
                    _p.ClearComputedWords();
                }

                while (!_p.startParagraph1 || !_p.startParagraph2);
            }


            // Truncate all following pairs until end or true-true

            int j = pairIndex;

            TextPair _q;

            while (j < pText.Number() - 1)
            {
                j++;
                _q = pText.textPairs[j];
                if (_q.startParagraph1 && _q.startParagraph2)
                    break;
                _q.ClearComputedWords();
            }

            PrepareScreen();
            RenderPairs();
            FindNaturalDividers(0);
            FindNaturalDividersOnScreen(0);
            Render();
        }

        private void EditPair(int pairIndex)
        {
            TextPair p = pText.textPairs[pairIndex];

            if (p.GetLength(1) > 10000 || p.GetLength(2) > 10000)
            {
                editWhenNipped = !editWhenNipped;
                //MessageBox.Show("The text is too long. Use Edit command on shorter pairs.");
                Render();
                return;
            }

            editPairForm.pTC = this;
            editPairForm.pairIndex = pairIndex;
            editPairForm.ShowDialog();

            if (editPairForm.result)
            {
                PairChanged(pairIndex);
            }
        }


        internal void MergePairs(int firstPair)
        {

            TextPair first = pText.textPairs[firstPair];
            TextPair second = pText.textPairs[firstPair + 1];

            if (second.sb1 == null)
            {
                second.sb1 = new StringBuilder(second.text1);
                second.text1 = null;
            }

            if (second.sb2 == null)
            {
                second.sb2 = new StringBuilder(second.text2);
                second.text2 = null;
            }

            if (second.startParagraph1)
            {
                second.sb1.Insert(0, '\n');
                second.sb1.Insert(0, '\r');
            }
            else
                second.sb1.Insert(0, ' ');

            if (second.startParagraph2)
            {
                second.sb2.Insert(0, '\n');
                second.sb2.Insert(0, '\r');
            }
            else
                second.sb2.Insert(0, ' ');

            second.sb1.Insert(0, first.sb1 == null ? first.text1 : first.sb1.ToString());
            second.sb2.Insert(0, first.sb2 == null ? first.text2 : first.sb2.ToString());

            second.startParagraph1 = first.startParagraph1;
            second.startParagraph2 = first.startParagraph2;

            pText.textPairs.Remove(first);



            //if (first.sb1 == null)
            //{
            //    first.sb1 = new StringBuilder(first.text1);
            //    first.text1 = null;
            //}

            //if (first.sb2 == null)
            //{
            //    first.sb2 = new StringBuilder(first.text2);
            //    first.text2 = null;
            //}

            //if (second.startParagraph1)
            //{
            //    first.sb1.Append('\r');
            //    first.sb1.Append('\n');
            //}
            //else
            //    first.sb1.Append(' ');

            //if (second.startParagraph2)
            //{
            //    first.sb2.Append('\r');
            //    first.sb2.Append('\n');
            //}
            //else
            //    first.sb2.Append(' ');

            //first.sb1.Append(second.sb1 == null ? second.text1 : second.sb1.ToString());
            //first.sb2.Append(second.sb2 == null ? second.text2 : second.sb2.ToString());

            //pText.textPairs.Remove(second);

        }
    }

    public class ScreenWord

    {
        public string word;
        public int x; // start of the word -- real point on screen
        public int x2; // end of the word
        public TextPair pair; // index of pair
        public byte side; // 1 or 2 -- the second or first text
        public int pos; // position of the word in the pair
        public int f_x;
        public int f_x2;
        public int line;

        /// <summary>
        /// Previous screen word from the same pair
        /// </summary>
        public ScreenWord prev;

        public ScreenWord next;
    }
}
