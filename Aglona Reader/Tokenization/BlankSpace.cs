using System.Linq;

namespace AglonaReader.Tokenization
{
    public class BlankSpace : Token
    {
        public BlankSpace(int start, string content) : base(start, content)
        {
        }

        protected override bool BreaksBefore()
        {
            return false;
        }

        protected override bool BreaksAfter()
        {
            return Content.Any(c => c != ' ');
        }
    }
}