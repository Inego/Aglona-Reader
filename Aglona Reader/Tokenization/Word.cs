using System.Collections.Generic;
using System.Linq;

namespace AglonaReader.Tokenization
{
    public class Word : Token
    {
        public Word(int start, string content) : base(start, content)
        {
        }
        
        private static readonly List<char> StartingBreakers = new List<char>
        {
            '[', 
            '(', 
            '{'
        };
        
        private static readonly List<char> EndingBreakers = new List<char> { 
            '—',
            '.',
            '。',
            ',',
            '，',
            ':',
            '•',
            ';',
            '!',
            '?',
            '…',
            ')',
            ']',
            '}'
        };
        
        private static readonly List<char> Quotes = new List<char> { 
            '\'',
            '\"',
            '«',
            '»',
            '‹',
            '›',
            '“',
            '”'
        };


        protected override bool BreaksBefore()
        {
            return StartingBreakers.Contains(Content[0]);
        }


        protected override bool BreaksAfter()
        {
            var last = Content.Last();
            
            if (EndingBreakers.Contains(last))
            {
                return true;
            }
            
            return Content.Length > 1 
                   && Quotes.Contains(last) 
                   && EndingBreakers.Contains(Content[Content.Length - 2]);
        }
    }
}
