using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace AglonaReader
{

    public class WordInfo
    {
        public string word;
        public int line;
        public int x;
        public int x2;
        public int pos;

        public WordInfo(string word, int line, int wordX, int wordX2, int pos)
        {
            this.word = word;
            this.line = line;
            this.x = wordX;
            this.x2 = wordX2;
            this.pos = pos;
        }

    }
    
    public class RenderedTextInfo
    {
        public bool valid;
        public int line1;
        public int line2;
        public int x1;
        public int x2;
        public int x2b;
        //public int x2b;
    }

    public class TextPair
    {

        public RenderedTextInfo RenderedInfo(byte side)
        {
            return side == 1 ? renderedInfo1 : renderedInfo2;
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
            return sb1 != null || sb2 != null;
        }

        public byte structureLevel;

        public string text1;
        public string text2;

        

        // Used if texts are large (typically in aligning mode for the "big block")
        public StringBuilder sb1;
        public StringBuilder sb2;

        /// <summary>
        /// Indicates that text1 begins a paragraph
        /// </summary>
        public bool startParagraph1;
        /// <summary>
        /// Indicates that text2 begins a paragraph
        /// </summary>
        public bool startParagraph2;

        /// <summary>
        /// Current position for processing in text 1
        /// </summary>
        public int currentPos1;

        /// <summary>
        /// Current position for processing in text 2
        /// </summary>
        public int currentPos2;

        /// <summary>
        /// Indicates that all lines of text 1 have already been computed
        /// </summary>
        public bool allLinesComputed1;

        /// <summary>
        /// Indicates that all lines of text 2 have already been computed
        /// </summary>
        public bool allLinesComputed2;

        
        /// <summary>
        /// How many lines are required to be added in order to compute the start line of the next text Pair.
        /// Zero means that the next Pair begins at the same line.
        /// </summary>
        public int height;

        public bool ComputedWordsExist() { return (computedWords1 != null); }

        public RenderedTextInfo renderedInfo1;
        public RenderedTextInfo renderedInfo2;

        private Collection<WordInfo> computedWords1;
        public Collection<WordInfo> computedWords2;

        public char GetChar(byte side, int charIndex)
        {
            if (side == 1)
                if (sb1 == null)
                    return text1[charIndex];
                else
                    return sb1[charIndex];
            else
                if (sb2 == null)
                    return text2[charIndex];
                else
                    return sb2[charIndex];
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
            height = -1;

            currentPos1 = 0;
            currentPos2 = 0;

            allLinesComputed1 = false;
            allLinesComputed2 = false;

            renderedInfo1 = new RenderedTextInfo();
            renderedInfo2 = new RenderedTextInfo();

        }

        public TextPair(string text1, string text2, bool startParagraph1, bool startParagraph2) : this()
        {
            if (text1 != null)
                if (text1.Length >= ParallelTextControl.BigTextSize)
                    this.sb1 = new StringBuilder(text1);
                else
                    this.text1 = text1;

            if (text2 != null)
                if (text2.Length >= ParallelTextControl.BigTextSize)
                    this.sb2 = new StringBuilder(text2);
                else
                    this.text2 = text2;

            this.startParagraph1 = startParagraph1;
            this.startParagraph2 = startParagraph2;

        }

        internal void ClearComputedWords()
        {
            if (computedWords1 != null)
                computedWords1.Clear();
            if (computedWords2 != null)
                computedWords2.Clear();

            height = -1;
                
            allLinesComputed1 = false;
            allLinesComputed2 = false;

        }

        internal int GetLength(byte side)
        {
            if (side == 1)
                if (sb1 == null)
                    return text1.Length;
                else
                    return sb1.Length;
            else
                if (sb2 == null)
                    return text2.Length;
                else
                    return sb2.Length;
        }

        internal void SetStructureLevel(byte p)
        {
            structureLevel = p;
            startParagraph1 = true;
            startParagraph2 = true;
        }


        internal int NaturalDividerPosition(byte side)
        {
            if (side == 1)
                if (sb1 == null)
                    return NaturalDividerPosition(text1, recommended_natural1);
                else
                    return NaturalDividerPosition(sb1, recommended_natural1);
            else
                if (sb2 == null)
                    return NaturalDividerPosition(text2, recommended_natural2);
                else
                    return NaturalDividerPosition(sb2, recommended_natural2);
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
                        if (state == 1)
                            state = 2;
                        else if (state == 3)
                            state = 2;
                        break;

                    case '.':
                    case ':':
                    case ';':
                    case '!':
                    case '?':
                        if (state == 1)
                            state = 3;
                        break;

                    case ' ':
                    case '\t':
                    case '\r':
                        if (state == 3)
                            state = 2;

                        break;

                    default:
                        if ((c == '\'' || c == '\"' || c == '«' || c == '»') && state != 2)
                            // do nothing
                            ;
                        else if (state == 0)
                            state = 1;
                        else if (state == 2 || state == 3)
                        {
                            if (current_iteration == recommended_natural)
                                return pos;
                            current_iteration++;
                            state = 1;
                        }
                        break;
                }

                pos++;

            }

            return -1;
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
                        if (state == 1)
                            state = 2;
                        else if (state == 3)
                            state = 2;
                        break;

                    case '.':
                    case ':':
                    case ';':
                    case '!':
                    case '?':
                        if (state == 1)
                            state = 3;
                        break;

                    case ' ':
                    case '\t':
                    case '\r':
                        if (state == 3)
                            state = 2;

                        break;

                    default:
                        if ((c == '\'' || c == '\"' || c == '«' || c == '»') && state != 2)
                            // do nothing
                            ;
                        else if (state == 0)
                            state = 1;
                        else if (state == 2 || state == 3)
                        {
                            if (current_iteration == recommended_natural)
                                return pos;
                            current_iteration++;
                            state = 1;
                        }
                        break;
                }

                pos++;

            }

            return -1;
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
        public string author1 = "";
        public string title1 = "";
        public string info1 = "";
        public string lang1 = "";

        public string author2 = "";
        public string title2 = "";
        public string info2 = "";
        public string lang2 = "";

        public string info = "";

        public string fileName = "";


        public List<TextPair> textPairs;

        /// <summary>
        /// Contains a list of pairs which are at least partially computed.
        /// It is used for speedy truncating.
        /// </summary>
        public List<TextPair> computedPairs;

        public int Number()
        { return textPairs.Count(); }

        public ParallelText()
        {
            textPairs = new List<TextPair>();
            computedPairs = new List<TextPair>();
        }

        public TextPair this[int pairIndex]
        {
            get { return textPairs[pairIndex]; }
        }

        public void AddPair(string text1, string text2, bool startParagraph1, bool startParagraph2)
        {
            TextPair newPair = textPairs.Count == 0 ?
                new TextPair(text1, text2, true, true) :
                new TextPair(text1, text2, startParagraph1, startParagraph2);

            textPairs.Add(newPair);
            
        }

        public void AddPair(string text1, string text2)
        {
            AddPair(text1, text2, true, true);
        }

        public void Truncate()
        {
            foreach (TextPair p in computedPairs)
                p.ClearComputedWords();
            computedPairs.Clear();
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

                l.Add(new WordInfo(r.word, r.line, r.x + bias, r.x2 + bias, r.pos));

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

                writer.WriteAttributeString("lang1", lang1);
                writer.WriteAttributeString("author1", author1);
                writer.WriteAttributeString("title1", title1);
                writer.WriteAttributeString("info1", info1);

                writer.WriteAttributeString("lang2", lang2);
                writer.WriteAttributeString("author2", author2);
                writer.WriteAttributeString("title2", title2);
                writer.WriteAttributeString("info2", info2);

                writer.WriteAttributeString("info", info);
                
                foreach (TextPair p in textPairs)
                {
                    writer.WriteStartElement("p");

                    level = 0;

                    if (p.structureLevel == 1)
                        level = 4;
                    else if (p.structureLevel == 2)
                        level = 5;
                    else if (p.structureLevel == 3)
                        level = 6;
                    else
                    {
                        if (p.startParagraph1)
                            level = 1;
                        if (p.startParagraph2)
                            level += 2;
                    }

                    if (level != 0)
                    {
                        writer.WriteStartAttribute("l");
                        writer.WriteValue(level);
                        writer.WriteEndAttribute();
                    }

                    if (p.sb1 == null)
                        writer.WriteAttributeString("s", p.text1);
                    else
                        writer.WriteAttributeString("s", p.sb1.ToString());

                    if (p.sb2 == null)
                        writer.WriteAttributeString("t", p.text2);
                    else
                        writer.WriteAttributeString("t", p.sb2.ToString());
                    
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
            }

            this.fileName = newFileName;
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

                lang1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "author1")
                    return false;

                author1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "title1")
                    return false;

                title1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "info1")
                    return false;

                info1 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "lang2")
                    return false;

                lang2 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "author2")
                    return false;

                author2 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "title2")
                    return false;

                title2 = reader.Value;

                if (!reader.MoveToNextAttribute())
                    return false;

                if (reader.Name != "info2")
                    return false;

                info2 = reader.Value;

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
                            p.startParagraph1 = true;
                            p.startParagraph2 = true;
                        }
                        else if (reader.Value == "1")
                            p.startParagraph1 = true;
                        else if (reader.Value == "2")
                            p.startParagraph2 = true;
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
                        p.sb1 = new StringBuilder(reader.Value);
                    else
                        p.text1 = reader.Value;

                    if (!reader.MoveToNextAttribute())
                        return false;

                    if (reader.Name != "t")
                        return false;

                    if (reader.Value.Length >= ParallelTextControl.BigTextSize)
                        p.sb2 = new StringBuilder(reader.Value);
                    else
                        p.text2 = reader.Value;

                    textPairs.Add(p);

                    goto NextPair;

                }


                reader.Close();

                fileName = newFileName;

                return true;

            }

        }
        
    }

}

