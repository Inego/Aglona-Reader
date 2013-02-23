using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;


namespace AglonaReader
{

    public class WordInfo
    {
        public string Word { get; set; }
        public int Line { get; set; }
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Pos { get; set; }
        public bool Eastern { get; set; }

        public WordInfo(string word, int line, int wordX, int wordX2, int pos, bool eastern)
        {
            this.Word = word;
            this.Line = line;
            this.X1 = wordX;
            this.X2 = wordX2;
            this.Pos = pos;
            this.Eastern = eastern;
        }

    }

    public class RenderedTextInfo
    {
        public bool Valid { get; set; }
        public int Line1 { get; set; }
        public int Line2 { get; set; }
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int X2B { get; set; }
    }

    public class TextPair
    {

        public int aggregateSize = 0;
        public int totalTextSize = 0;

        public string Substring(byte side, int startPosition, int length)
        {
            if (side == 1)
                if (SB1 == null)
                    return Text1.Substring(startPosition, length);
                else
                    return SB1.ToString(startPosition, length);
            else
                if (SB2 == null)
                    return Text2.Substring(startPosition, length);
                else
                    return SB2.ToString(startPosition, length);
        }

        public string Substring(byte side, int startPosition)
        {
            if (side == 1)
                if (SB1 == null)
                    return Text1.Substring(startPosition);
                else
                    return SB1.ToString(startPosition, SB1.Length = startPosition);
            else
                if (SB2 == null)
                    return Text2.Substring(startPosition);
                else
                    return SB2.ToString(startPosition, SB2.Length - startPosition);
        }

        public RenderedTextInfo RenderedInfo(byte side)
        {
            return side == 1 ? RenderedInfo1 : RenderedInfo2;
        }

        public bool IsBig()
        {
            return SB1 != null || SB2 != null;
        }

        public byte StructureLevel { get; set; }

        public string Text1 { get; set; }
        public string Text2 { get; set; }



        // Used if texts are large (typically in aligning mode for the "big block")
        public StringBuilder SB1 { get; set; }
        public StringBuilder SB2 { get; set; }

        /// <summary>
        /// Indicates that Text1 begins a paragraph
        /// </summary>
        public bool StartParagraph1 { get; set; }
        /// <summary>
        /// Indicates that Text2 begins a paragraph
        /// </summary>
        public bool StartParagraph2 { get; set; }

        /// <summary>
        /// Current position for processing in text 1
        /// </summary>
        public int CurrentPos1 { get; set; }

        /// <summary>
        /// Current position for processing in text 2
        /// </summary>
        public int CurrentPos2 { get; set; }

        /// <summary>
        /// Indicates that all lines of text 1 have already been computed
        /// </summary>
        public bool AllLinesComputed1 { get; set; }

        /// <summary>
        /// Indicates that all lines of text 2 have already been computed
        /// </summary>
        public bool AllLinesComputed2 { get; set; }


        /// <summary>
        /// How many lines are required to be added in order to compute the start Line of the Next text Pair.
        /// Zero means that the Next Pair begins at the same Line.
        /// </summary>
        public int Height { get; set; }

        public RenderedTextInfo RenderedInfo1 { get; set; }
        public RenderedTextInfo RenderedInfo2 { get; set; }

        private Collection<WordInfo> computedWords1;
        private Collection<WordInfo> computedWords2;
        public bool ContinueFromNewLine1;
        public bool ContinueFromNewLine2;

        public char GetChar(byte side, int charIndex)
        {
            if (side == 1)
                if (SB1 == null)
                    return Text1[charIndex];
                else
                    return SB1[charIndex];
            else
                if (SB2 == null)
                    return Text2[charIndex];
                else
                    return SB2[charIndex];
        }

        public Collection<WordInfo> ComputedWords(byte side, bool createNew)
        {
            if (side == 1)
            {
                if (createNew && computedWords1 == null) computedWords1 = new Collection<WordInfo>();
                return computedWords1;
            }
            else
            {
                if (createNew && computedWords2 == null) computedWords2 = new Collection<WordInfo>();
                return computedWords2;
            }
        }

        public Collection<WordInfo> ComputedWords(byte side)
        {
            return ComputedWords(side, false);
        }

        public TextPair()
        {
            Height = -1;

            CurrentPos1 = 0;
            CurrentPos2 = 0;

            AllLinesComputed1 = false;
            AllLinesComputed2 = false;

            RenderedInfo1 = new RenderedTextInfo();
            RenderedInfo2 = new RenderedTextInfo();

        }

        public TextPair(string text1, string text2, bool startParagraph1, bool startParagraph2)
            : this()
        {
            if (text1 != null)
                if (text1.Length >= ParallelTextControl.BigTextSize)
                    this.SB1 = new StringBuilder(text1);
                else
                    this.Text1 = text1;

            if (text2 != null)
                if (text2.Length >= ParallelTextControl.BigTextSize)
                    this.SB2 = new StringBuilder(text2);
                else
                    this.Text2 = text2;

            this.StartParagraph1 = startParagraph1;
            this.StartParagraph2 = startParagraph2;

        }

        internal void ClearComputedWords()
        {
            if (computedWords1 != null)
                computedWords1.Clear();
            if (computedWords2 != null)
                computedWords2.Clear();

            Height = -1;

            AllLinesComputed1 = false;
            AllLinesComputed2 = false;

            CurrentPos1 = 0;
            CurrentPos2 = 0;

        }

        internal int GetLength(byte side)
        {
            if (side == 1)
                if (SB1 == null)
                    return Text1.Length;
                else
                    return SB1.Length;
            else
                if (SB2 == null)
                    return Text2.Length;
                else
                    return SB2.Length;
        }

        internal void SetStructureLevel(byte p)
        {
            StructureLevel = p;
            StartParagraph1 = true;
            StartParagraph2 = true;
        }

        private static int NaturalDividerPosition(StringBuilder text, int startingPos, bool forward)
        {
            byte state = 0;

            int currentWordStart = -1;
            int prevNatural = -1;

            int length = text.Length;

            int pos = 0;

            char c;

            while (pos <= length - 1)
            {
                c = text[pos];

                switch (c)
                {
                    case '\n':
                        state = 2;
                        currentWordStart = -1;
                        break;

                    case '—':
                    case '.':
                    case '。':
                    case ',':
                    case '，':
                    case ':':
                    case ';':
                    case '!':
                    case '?':
                    case '…':
                    case '(':
                    case ')':

                        if (currentWordStart == -1)
                            currentWordStart = pos;

                        if (state == 1)
                            state = 3;
                        else if (state == 2)
                        {
                            /////
                            if (forward)
                            {
                                if (currentWordStart > startingPos)
                                    return currentWordStart;
                            }
                            else
                            {
                                if (currentWordStart >= startingPos)
                                    return prevNatural;
                            }

                            prevNatural = currentWordStart;
                            /////
                            
                            state = 3;
                        }


                        break;

                    case ' ':
                    case '\t':
                    case '\r':
                        currentWordStart = -1;
                        break;

                    default:

                        if (currentWordStart == -1)
                            currentWordStart = pos;

                        if ((c == '\'' || c == '\"' || c == '«' || c == '»' || c == '‹' || c == '›'
                            || c == '“' || c == '”') && state != 2)
                            // do nothing
                            ;
                        else
                            if (state == 0)
                                state = 1;
                            else if (state == 2 || state == 3)
                            {
                                /////
                                if (forward)
                                {
                                    if (currentWordStart > startingPos)
                                        return currentWordStart;
                                }
                                else
                                {
                                    if (currentWordStart >= startingPos)
                                        return prevNatural;
                                }

                                prevNatural = currentWordStart;
                                /////


                                state = 1;
                            }

                        

                        break;
                }

                pos++;

            }

            return -1;

        }


        private static int NaturalDividerPosition(string text, int startingPos, bool forward)
        {
            byte state = 0;

            int currentWordStart = -1;
            int prevNatural = -1;

            int length = text.Length;

            int pos = 0;

            char c;

            while (pos <= length - 1)
            {
                c = text[pos];

                switch (c)
                {
                    case '\n':
                        state = 2;
                        currentWordStart = -1;
                        break;

                    case '—':
                    case '.':
                    case '。':
                    case ',':
                    case '，':
                    case ':':
                    case ';':
                    case '!':
                    case '?':
                    case '…':
                    case '(':
                    case ')':

                        if (currentWordStart == -1)
                            currentWordStart = pos;

                        if (state == 1)
                            state = 3;
                        else if (state == 2)
                        {
                            /////
                            if (forward)
                            {
                                if (currentWordStart > startingPos)
                                    return currentWordStart;
                            }
                            else
                            {
                                if (currentWordStart >= startingPos)
                                    return prevNatural;
                            }

                            prevNatural = currentWordStart;
                            /////

                            state = 3;
                        }


                        break;

                    case ' ':
                    case '\t':
                    case '\r':
                        currentWordStart = -1;
                        break;

                    default:

                        if (currentWordStart == -1)
                            currentWordStart = pos;

                        if ((c == '\'' || c == '\"' || c == '«' || c == '»' || c == '‹' || c == '›'
                            || c == '“' || c == '”') && state != 2)
                            // do nothing
                            ;
                        else
                            if (state == 0)
                                state = 1;
                            else if (state == 2 || state == 3)
                            {
                                /////
                                if (forward)
                                {
                                    if (currentWordStart > startingPos)
                                        return currentWordStart;
                                }
                                else
                                {
                                    if (currentWordStart >= startingPos)
                                        return prevNatural;
                                }

                                prevNatural = currentWordStart;
                                /////


                                state = 1;
                            }



                        break;
                }

                pos++;

            }

            return -1;

        }

        

        public int NaturalDividerPosition(byte side, int startingPos, bool forward)
        {
            if (side == 1)
                if (SB1 == null)
                    return NaturalDividerPosition(Text1, startingPos, forward);
                else
                    return NaturalDividerPosition(SB1, startingPos, forward);
            else
                if (SB2 == null)
                    return NaturalDividerPosition(Text2, startingPos, forward);
                else
                    return NaturalDividerPosition(SB2, startingPos, forward);

        }
        
        internal string GetText(byte side)
        {
            if (side == 1)
                if (SB1 == null)
                    return Text1;
                else
                    return SB1.ToString();
            else
                if (SB2 == null)
                    return Text2;
                else
                    return SB2.ToString();
        }

        internal bool StartParagraph(byte side)
        {
            return side == 1 ? StartParagraph1 : StartParagraph2;
        }

        internal void UpdateTotalSize()
        {
            if (SB1 == null)
                totalTextSize = Text1.Length;
            else
                totalTextSize = SB1.Length;

            if (SB2 == null)
                totalTextSize += Text2.Length;
            else
                totalTextSize += SB2.Length;
        }

        internal bool AllLinesComputed(byte side)
        {
            if (side == 1)
                return AllLinesComputed1;
            else
                return AllLinesComputed2;

        }
    }

    public class ParallelText
    {
        public string Author1 { get; set; }
        public string Title1 { get; set; }
        public string Info1 { get; set; }
        public string Lang1 { get; set; }

        public string Author2 { get; set; }
        public string Title2 { get; set; }
        public string Info2 { get; set; }
        public string Lang2 { get; set; }

        public string Info { get; set; }

        public string FileName { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TextPair> TextPairs { get; set; }

        /// <summary>
        /// Contains a list of pairs which are at least partially computed.
        /// It is used for speedy truncating.
        /// </summary>
        /// 
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TextPair> ComputedPairs { get; set; }

        public int Number()
        { return TextPairs.Count; }

        public ParallelText()
        {
            TextPairs = new List<TextPair>();
            ComputedPairs = new List<TextPair>();
        }

        public TextPair this[int pairIndex]
        {
            get { return TextPairs[pairIndex]; }
        }

        public void AddPair(string text1, string text2, bool startParagraph1, bool startParagraph2)
        {
            TextPair newPair = TextPairs.Count == 0 ?
                new TextPair(text1, text2, true, true) :
                new TextPair(text1, text2, startParagraph1, startParagraph2);

            TextPairs.Add(newPair);

        }

        public void AddPair(string text1, string text2)
        {
            AddPair(text1, text2, true, true);
        }

        public void Truncate()
        {
            foreach (TextPair p in ComputedPairs)
                p.ClearComputedWords();
            ComputedPairs.Clear();
        }

        public static void InsertWords(Collection<CommonWordInfo> list, int spaceLeft)
        {

            if (list == null)
                return;

            Collection<WordInfo> l = null;
            TextPair prev_p = null;
            byte prev_side = 0;
            

            int bias = 0;


            // Spaces can be only in cases like W E or E W or W W,
            // where W is a "western" word and E is an eastern character
            // they can't be between EE

            CommonWordInfo previousWord = null;

            int numberOfSpacesLeft = 0;

            // So before extending spaces we must know their number.
            foreach (CommonWordInfo r in list)
            {
                if (previousWord != null)
                    if (!(r.Eastern && previousWord.Eastern))
                        numberOfSpacesLeft++;
                previousWord = r;
            }

            previousWord = null;

            foreach (CommonWordInfo r in list)
            {

                if (spaceLeft != 0 && previousWord != null && !(r.Eastern && previousWord.Eastern))
                {
                    int inc = (spaceLeft / numberOfSpacesLeft);
                    bias += inc;
                    spaceLeft -= inc;
                    numberOfSpacesLeft--;
                }

                if (prev_p != r.TextPair)
                {
                    prev_p = r.TextPair;
                    prev_side = 0;
                }

                if (r.side != prev_side)
                {
                    prev_side = r.side;
                    l = prev_p.ComputedWords(r.side, true);
                }

                l.Add(new WordInfo(r.Word, r.Line, r.X1 + bias, r.X2 + bias, r.Pos, r.Eastern));

                previousWord = r;
            }

            list.Clear();
        }


        public void Save(string newFileName)
        {

            byte level;

            using (XmlTextWriter writer = new XmlTextWriter(newFileName, Encoding.UTF8))
            {

                writer.WriteStartElement("ParallelBook");

                writer.WriteAttributeString("lang1", Lang1);
                writer.WriteAttributeString("author1", Author1);
                writer.WriteAttributeString("title1", Title1);
                writer.WriteAttributeString("info1", Info1);

                writer.WriteAttributeString("lang2", Lang2);
                writer.WriteAttributeString("author2", Author2);
                writer.WriteAttributeString("title2", Title2);
                writer.WriteAttributeString("info2", Info2);

                writer.WriteAttributeString("info", Info);

                foreach (TextPair p in TextPairs)
                {
                    writer.WriteStartElement("p");

                    level = 0;

                    if (p.StructureLevel == 1)
                        level = 4;
                    else if (p.StructureLevel == 2)
                        level = 5;
                    else if (p.StructureLevel == 3)
                        level = 6;
                    else
                    {
                        if (p.StartParagraph1)
                            level = 1;
                        if (p.StartParagraph2)
                            level += 2;
                    }

                    if (level != 0)
                    {
                        writer.WriteStartAttribute("l");
                        writer.WriteValue(level);
                        writer.WriteEndAttribute();
                    }

                    if (p.SB1 == null)
                        writer.WriteAttributeString("s", p.Text1);
                    else
                        writer.WriteAttributeString("s", p.SB1.ToString());

                    if (p.SB2 == null)
                        writer.WriteAttributeString("t", p.Text2);
                    else
                        writer.WriteAttributeString("t", p.SB2.ToString());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
            }

            this.FileName = newFileName;
        }


        public bool Load(string newFileName)
        {


            using (XmlTextReader reader = new XmlTextReader(newFileName))
            {
                try
                {
                    reader.Read();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("File not found or unavailable: " + newFileName);
                    return false;
                }

                if (reader.NodeType != XmlNodeType.Element)
                    return false;

                if (reader.Name != "ParallelBook")
                    return false;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "lang1")
                    return false;

                Lang1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "author1")
                    return false;

                Author1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "title1")
                    return false;

                Title1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "info1")
                    return false;

                Info1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "lang2")
                    return false;

                Lang2 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "author2")
                    return false;

                Author2 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "title2")
                    return false;

                Title2 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "info2")
                    return false;

                Info2 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "info")
                    return false;

                Info = reader.Value;


            NextPair:

                if (!reader.Read())
                    return false;

                if (reader.Name == "p" && reader.NodeType == XmlNodeType.Element)
                {
                    if (!reader.MoveToNextAttribute())
                        return false;

                    TextPair p = new TextPair();

                    if (reader.Name == "l")
                    {
                        if (reader.Value == "3")
                        {
                            p.StartParagraph1 = true;
                            p.StartParagraph2 = true;
                        }
                        else if (reader.Value == "1")
                            p.StartParagraph1 = true;
                        else if (reader.Value == "2")
                            p.StartParagraph2 = true;
                        else if (reader.Value == "4")
                            p.SetStructureLevel(1);
                        else if (reader.Value == "5")
                            p.SetStructureLevel(2);
                        else if (reader.Value == "6")
                            p.SetStructureLevel(3);

                        if (!reader.MoveToNextAttribute())
                            return false;

                    }

                    if (reader.Name != "s")
                        return false;

                    if (reader.Value.Length >= ParallelTextControl.BigTextSize)
                        p.SB1 = new StringBuilder(reader.Value);
                    else
                        p.Text1 = reader.Value;

                    p.totalTextSize = reader.Value.Length;

                    if (!reader.MoveToNextAttribute())
                        return false;

                    if (reader.Name != "t")
                        return false;

                    if (reader.Value.Length >= ParallelTextControl.BigTextSize)
                        p.SB2 = new StringBuilder(reader.Value);
                    else
                        p.Text2 = reader.Value;

                    p.totalTextSize += reader.Value.Length;

                    TextPairs.Add(p);

                    goto NextPair;

                }


                reader.Close();

                FileName = newFileName;

                if (TextPairs.Count > 0)
                    UpdateAggregates(0);

                return true;



            }

        }


        internal void UpdateAggregates(int pairIndex)
        {
            int accLength;

            if (pairIndex == 0)
                accLength = -2;
            else
                accLength = TextPairs[pairIndex - 1].aggregateSize;

            TextPair tp;

            for (int i = pairIndex; i < Number(); i++)
            {
                tp = TextPairs[i];
                accLength += 2 + tp.totalTextSize;
                tp.aggregateSize = accLength;
            }
        }

        

    }

}

