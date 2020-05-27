namespace AglonaReader
{
    public class FileUsageInfo
    {
        public const int NormalMode = 0;
        public const int AlternatingMode = 1;
        public const int AdvancedMode = 2;

        public const int AlternatingColorSchemeBlackGreen = 0;
        public const int AlternatingColorSchemeGreenBlack = 1;

        public string FileName { get; set; }
        public int Pair { get; set; }
        public int TopPair { get; set; }
        public bool Reversed { get; set; }
        public float SplitterRatio { get; set; }
        public bool EditMode { get; set; }
        public int ReadingMode { get; set; }
        public int AlternatingColorScheme { get; set; }
    }
}