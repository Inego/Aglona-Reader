namespace AglonaReader.StringUtils
{
    public class WrappedString : ICommonString
    {
        private readonly string stringValue;

        public WrappedString(string stringValue) => this.stringValue = stringValue;

        public int Length() => stringValue.Length;
        public char this[int key] => stringValue[key];
        public string Substring(int startIndex, int length) => stringValue.Substring(startIndex, length);
    }
}