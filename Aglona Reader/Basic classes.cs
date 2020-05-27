namespace AglonaReader
{
    public class WordInfo
    {
        public string Word { get; }
        public int Line { get; set; }
        public int X1 { get; }
        public int X2 { get; }
        public int Position { get; }
        public bool Eastern { get; }

        public WordInfo(string word, int line, int wordX, int wordX2, int position, bool eastern)
        {
            Word = word;
            Line = line;
            X1 = wordX;
            X2 = wordX2;
            Position = position;
            Eastern = eastern;
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

    public class CommonWordInfo : WordInfo
    {
        public TextPair TextPair { get; }
        public readonly byte side;

        public CommonWordInfo(TextPair textPair, string word, int line, int wordX, int wordX2, int pos, bool eastern, byte side)
            : base(word, line, wordX, wordX2, pos, eastern)
        {
            TextPair = textPair;
            this.side = side;
        }
    }

}

