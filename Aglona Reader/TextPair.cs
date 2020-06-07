using System.Collections.ObjectModel;
using System.Text;
using AglonaReader.StringUtils;
using AglonaReader.Tokenization;

namespace AglonaReader
{
    public class TextPair
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }

        // Used if texts are large (typically in aligning mode for the "big block")
        public StringBuilder Sb1 { get; set; }
        public StringBuilder Sb2 { get; set; }

        /// <summary>
        /// Indicates that Text1 begins a paragraph
        /// </summary>
        public bool StartParagraph1 { get; set; }
        /// <summary>
        /// Indicates that Text2 begins a paragraph
        /// </summary>
        public bool StartParagraph2 { get; set; }


        public int aggregateSize;
        public int totalTextSize;

        public string Substring(byte side, int startPosition, int length)
        {
            if (side == 1) return 
                Sb1 == null ? 
                    Text1.Substring(startPosition, length) 
                    : Sb1.ToString(startPosition, length);
            
            return Sb2 == null ? 
                Text2.Substring(startPosition, length) 
                : Sb2.ToString(startPosition, length);
        }

        public string Substring(byte side, int startPosition)
        {
            if (side == 1) return 
                Sb1 == null ? 
                    Text1.Substring(startPosition) 
                    : Sb1.ToString(startPosition, Sb1.Length - startPosition);
            
            return Sb2 == null ? 
                Text2.Substring(startPosition) 
                : Sb2.ToString(startPosition, Sb2.Length - startPosition);
        }

        public RenderedTextInfo RenderedInfo(byte side)
        {
            return side == 1 ? RenderedInfo1 : RenderedInfo2;
        }

        public bool IsBig()
        {
            return Sb1 != null || Sb2 != null;
        }

        public byte StructureLevel { get; set; }

        
        public uint AudioFileNumber { get; set; }
        public uint TimeBeg { get; set; }
        public uint TimeEnd { get; set; }
        

        
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

        public RenderedTextInfo RenderedInfo1 { get; }
        public RenderedTextInfo RenderedInfo2 { get; }

        private Collection<WordInfo> computedWords1;
        private Collection<WordInfo> computedWords2;
        public bool continueFromNewLine1;
        public bool continueFromNewLine2;

        public char GetChar(byte side, int charIndex)
        {
            if (side == 1) return Sb1?[charIndex] ?? Text1[charIndex];

            return Sb2?[charIndex] ?? Text2[charIndex];
        }

        public Collection<WordInfo> ComputedWords(byte side, bool createNew = false)
        {
            if (side == 1)
            {
                if (createNew && computedWords1 == null) computedWords1 = new Collection<WordInfo>();
                return computedWords1;
            }

            if (createNew && computedWords2 == null) computedWords2 = new Collection<WordInfo>();
            return computedWords2;
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
                    Sb1 = new StringBuilder(text1);
                else
                    Text1 = text1;

            if (text2 != null)
                if (text2.Length >= ParallelTextControl.BigTextSize)
                    Sb2 = new StringBuilder(text2);
                else
                    Text2 = text2;

            StartParagraph1 = startParagraph1;
            StartParagraph2 = startParagraph2;
        }
  

        internal void ClearComputedWords()
        {
            computedWords1?.Clear();
            computedWords2?.Clear();

            Height = -1;

            AllLinesComputed1 = false;
            AllLinesComputed2 = false;

            CurrentPos1 = 0;
            CurrentPos2 = 0;

        }

        internal int GetLength(byte side)
        {
            if (side == 1) return Sb1?.Length ?? Text1.Length;
            return Sb2?.Length ?? Text2.Length;
        }

        internal void SetStructureLevel(byte p)
        {
            StructureLevel = p;
            StartParagraph1 = true;
            StartParagraph2 = true;
        }

        private static int NaturalDividerPosition(StringBuilder text, int startingPos, bool forward)
        {
            return Token.NaturalDividerPosition(text.Wrap(), startingPos, forward);
        }


        private static int NaturalDividerPosition(string text, int startingPos, bool forward)
        {
            return Token.NaturalDividerPosition(text.Wrap(), startingPos, forward);
        }


        public int NaturalDividerPosition(byte side, int startingPos, bool forward)
        {
            if (side == 1) return 
                Sb1 == null ? 
                    NaturalDividerPosition(Text1, startingPos, forward) 
                    : NaturalDividerPosition(Sb1, startingPos, forward);

            return Sb2 == null ? 
                NaturalDividerPosition(Text2, startingPos, forward) 
                : NaturalDividerPosition(Sb2, startingPos, forward);
        }

        internal string GetText(byte side)
        {
            if (side == 1) return Sb1 == null ? Text1 : Sb1.ToString();

            return Sb2 == null ? Text2 : Sb2.ToString();
        }

        internal bool StartParagraph(byte side)
        {
            return side == 1 ? StartParagraph1 : StartParagraph2;
        }

        internal void UpdateTotalSize()
        {
            totalTextSize = Sb1?.Length ?? Text1.Length;
            totalTextSize += Sb2?.Length ?? Text2.Length;
        }

        internal bool AllLinesComputed(byte side) => 
            side == 1 ? AllLinesComputed1 : AllLinesComputed2;
    }
}