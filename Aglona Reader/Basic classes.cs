using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
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

        public WordInfo(string word, int line, int wordX, int wordX2, int pos)
        {
            this.Word = word;
            this.Line = line;
            this.X1 = wordX;
            this.X2 = wordX2;
            this.Pos = pos;
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

        public RenderedTextInfo RenderedInfo(byte side)
        {
            return side == 1 ? RenderedInfo1 : RenderedInfo2;
        }

        private int recommended_natural1;
        private int recommended_natural2;

        public void IncRecommendedNatural(byte side)
        {
            if (side == 1)
                recommended_natural1++;
            else
                recommended_natural2++;
        }

        public void DecRecommendedNatural(byte side)
        {
            if (side == 1)
                recommended_natural1--;
            else
                recommended_natural2--;
        }

        public int GetRecommendedNatural(byte side)
        {
            if (side == 1)
                return recommended_natural1;
            else
                return recommended_natural2;
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

        public TextPair(string text1, string text2, bool startParagraph1, bool startParagraph2) : this()
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


        internal int NaturalDividerPosition(byte side)
        {
            if (side == 1)
                if (SB1 == null)
                    return NaturalDividerPosition(Text1, recommended_natural1);
                else
                    return NaturalDividerPosition(SB1, recommended_natural1);
            else
                if (SB2 == null)
                    return NaturalDividerPosition(Text2, recommended_natural2);
                else
                    return NaturalDividerPosition(SB2, recommended_natural2);
        }


        private static int NaturalDividerPosition(StringBuilder text, int recommended_natural)
        {
            int current_iteration = 0;

            byte state = 0;

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
                        break;

                    case '—':
                    case '.':
                    case ',':
                    case ':':
                    case ';':
                    case '!':
                    case '?':
                        if (state == 1)
                            state = 3;
                        else if (state == 2)
                        {
                            if (current_iteration == recommended_natural)
                                goto CorrectAndReturn;
                            current_iteration++;
                            state = 3;
                        }


                        break;

                    case ' ':
                    case '\t':
                    case '\r':
                        break;

                    default:
                        if ((c == '\'' || c == '\"' || c == '«' || c == '»') && state != 2)
                            // do nothing
                            ;
                        else
                            if (state == 0)
                                state = 1;
                            else if (state == 2 || state == 3)
                            {
                                if (current_iteration == recommended_natural)
                                    goto CorrectAndReturn;
                                current_iteration++;
                                state = 1;
                            }
                        break;
                }

                pos++;

            }

            return -1;

        CorrectAndReturn:

            char prev;

            if (pos > 0)
            {
                prev = text[pos - 1];
                if (!(prev == ' '
                    || prev == '\n'
                    || prev == '\t'
                    || prev == '\r'))
                {
                    pos--;
                    goto CorrectAndReturn;
                }
            }

            return pos;


        }

        

        private static int NaturalDividerPosition(string text, int recommended_natural)
        {
            int current_iteration = 0;

            byte state = 0;

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
                        break;

                    case '—':
                    case '.':
                    case ',':
                    case ':':
                    case ';':
                    case '!':
                    case '?':
                        if (state == 1)
                            state = 3;
                        else if (state == 2)
                        {
                            if (current_iteration == recommended_natural)
                                goto CorrectAndReturn;
                            current_iteration++;
                            state = 3;
                        }
                       

                        break;

                    case ' ':
                    case '\t':
                    case '\r':
                        break;

                    default:
                        if ((c == '\'' || c == '\"' || c == '«' || c == '»') && state != 2)
                            // do nothing
                            ;
                        else
                            if (state == 0)
                                state = 1;
                            else if (state == 2 || state == 3)
                            {
                                if (current_iteration == recommended_natural)
                                    goto CorrectAndReturn;
                                current_iteration++;
                                state = 1;
                            }
                        break;
                }

                pos++;

            }

            return -1;

        CorrectAndReturn:

            char prev;

            if (pos > 0)
            {
                prev = text[pos - 1];
                if (!(prev == ' '
                    || prev == '\n'
                    || prev == '\t'
                    || prev == '\r'))
                {
                    pos--;
                    goto CorrectAndReturn;
                }
            }

            return pos;

            
        }


        internal void SetRecommendedNaturals(TextPair hp)
        {
            recommended_natural1 = hp.GetRecommendedNatural(1);
            recommended_natural2 = hp.GetRecommendedNatural(2);
        }

        internal void ClearRecommendedNaturals()
        {
            recommended_natural1 = 0;
            recommended_natural2 = 0;
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
        { return TextPairs.Count(); }

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

        public static void InsertWords(Collection<CommonWordInfo> list, int spaceLeft, byte side)
        {

            if (list == null)
                return;

            Collection<WordInfo> l = null;
            TextPair prev_p = null;

            int bias = 0;
            int counter = 0;

            foreach (CommonWordInfo r in list)
            {

                if (spaceLeft != 0 && counter > 0)
                {
                    int inc = (spaceLeft / (list.Count - counter));
                    bias += inc;
                    spaceLeft -= inc;
                }

                if (prev_p != r.TextPair)
                {
                    prev_p = r.TextPair;

                    l = prev_p.ComputedWords(side, true);
                }

                l.Add(new WordInfo(r.Word, r.Line, r.X1 + bias, r.X2 + bias, r.Pos));

                counter++;
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
                reader.Read();

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

                    if (!reader.MoveToNextAttribute())
                        return false;

                    if (reader.Name != "t")
                        return false;

                    if (reader.Value.Length >= ParallelTextControl.BigTextSize)
                        p.SB2 = new StringBuilder(reader.Value);
                    else
                        p.Text2 = reader.Value;

                    TextPairs.Add(p);

                    goto NextPair;

                }


                reader.Close();

                FileName = newFileName;

                return true;

            }

        }
        
    }

}

