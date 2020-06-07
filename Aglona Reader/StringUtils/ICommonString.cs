namespace AglonaReader.StringUtils
{
    public interface ICommonString
    {
        int Length();
        char this[int key] { get; }
        
        string Substring(int startIndex, int length);
    }
}