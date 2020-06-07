using System.Text;

namespace AglonaReader.StringUtils
{
    public static class Extensions
    {
        public static WrappedStringBuilder Wrap(this StringBuilder sb)
        {
            return new WrappedStringBuilder(sb);
        }
        
        public static WrappedString Wrap(this string sb)
        {
            return new WrappedString(sb);
        }
    }
}