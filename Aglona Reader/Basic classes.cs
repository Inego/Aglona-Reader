using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Xml;


namespace AglonaReader
{

    public class DB_Row
    {
        public string word;
        public int line;
        public int x;
        public int x2;
        public int pos;

        public DB_Row(string _w, int _l, int _x, int _x2, int _pos)
        {
            word = _w;
            line = _l;
            x = _x;
            x2 = _x2;
            pos = _pos;
        }

    }
    
    public class RenderedTextInfo
    {
        public bool valid;
        public int line1;
        public int line2;
        public int x1;
        public int x2;
    }

    public class TextPair
    {
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

        public byte structureLevel;

        public string text1;
        public string text2;

        // Length of a string to be considered a "big block"
        public static int BigTextSize = 1000;

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
        /// How many lines are required to be added in order to compute the start line of the next text pair.
        /// Zero means that the next pair begins at the same line.
        /// </summary>
        public int height;

        public bool ComputedWordsExist() { return (computedWords1 != null); }

        public RenderedTextInfo renderedInfo1;
        public RenderedTextInfo renderedInfo2;

        private List<DB_Row> computedWords1;
        public List<DB_Row> computedWords2;

        public char GetChar(byte side, int i)
        {
            if (side == 1)
                if (sb1 == null)
                    return text1[i];
                else
                    return sb1[i];
            else
                if (sb2 == null)
                    return text2[i];
                else
                    return sb2[i];
        }

        public List<DB_Row> ComputedWords(byte side, bool createNew = false)
        {
            if (side == 1)
            {
                if (createNew && computedWords1 == null) computedWords1 = new List<DB_Row>();
                return computedWords1;
            }
            else
            {
                if (createNew && computedWords2 == null) computedWords2 = new List<DB_Row>();
                return computedWords2;
            }
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

        public TextPair(string _t1, string _t2, bool _s1, bool _s2) : this()
        {
            if (_t1.Length >= BigTextSize)
                sb1 = new StringBuilder(_t1);
            else
                text1 = _t1;

            if (_t2.Length >= BigTextSize)
                sb2 = new StringBuilder(_t2);
            else
                text2 = _t2;

            startParagraph1 = _s1;
            startParagraph2 = _s2;

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

        private int NaturalDividerPosition(StringBuilder text, int recommended_natural)
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


        private int NaturalDividerPosition(string text, int recommended_natural)
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

        public void AddPair(string _t1, string _t2, bool _e1 = false, bool _e2 = false)
        {
            TextPair newPair = textPairs.Count == 0 ?
                new TextPair(_t1, _t2, true, true) :
                new TextPair(_t1, _t2, _e1, _e2);

            textPairs.Add(newPair);
            
        }


        public void Truncate()
        {
            foreach (TextPair p in computedPairs)
                p.ClearComputedWords();
            computedPairs.Clear();
        }

        public static void InsertWords(List<DB_Common_Row> list, int spaceLeft, byte side)
        {
            List<DB_Row> l = null;
            TextPair prev_p = null;

            int bias = 0;
            int counter = 0;

            foreach (DB_Common_Row r in list)
            {

                if (spaceLeft != 0 && counter > 0)
                {
                    int inc = (spaceLeft / (list.Count - counter));
                    bias += inc;
                    spaceLeft -= inc;
                }

                if (prev_p != r.textPair)
                {
                    prev_p = r.textPair;

                    l = prev_p.ComputedWords(side, true);
                }

                l.Add(new DB_Row(r.word, r.line, r.x + bias, r.x2 + bias, r.pos));

                counter++;
            }

            list.Clear();
        }


        public void Save(string _fileName)
        {

            byte level;

            using (XmlTextWriter writer = new XmlTextWriter(_fileName, Encoding.UTF8))
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

            fileName = _fileName;
        }


        public bool Load(string _fileName)
        {
            using (XmlTextReader reader = new XmlTextReader(_fileName))
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

                    if (reader.Value.Length >= TextPair.BigTextSize)
                        p.sb1 = new StringBuilder(reader.Value);
                    else
                        p.text1 = reader.Value;

                    if (!reader.MoveToNextAttribute())
                        return false;

                    if (reader.Name != "t")
                        return false;

                    if (reader.Value.Length >= TextPair.BigTextSize)
                        p.sb2 = new StringBuilder(reader.Value);
                    else
                        p.text2 = reader.Value;

                    textPairs.Add(p);

                    goto NextPair;

                }


                reader.Close();

                fileName = _fileName;

                return true;

            }


        }


        
    }

}

