using System.Text;

namespace AglonaReader.StringUtils
{
    public class WrappedStringBuilder : ICommonString
    {
        private readonly StringBuilder stringBuilderValue;

        public WrappedStringBuilder(StringBuilder stringBuilderValue) => this.stringBuilderValue = stringBuilderValue;

        public int Length() => stringBuilderValue.Length;
        public char this[int key] => stringBuilderValue[key];
        public string Substring(int startIndex, int length) => stringBuilderValue.ToString(startIndex, length);
    }
}