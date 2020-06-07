using System.Collections.Generic;
using AglonaReader.StringUtils;

namespace AglonaReader.Tokenization
{
    public abstract class Token
    {
        public int Start { get; }
        
        public string Content { get; }

        protected Token(int start, string content)
        {
            Content = content;
            Start = start;
            
        }

        protected abstract bool BreaksBefore();

        protected abstract bool BreaksAfter();

        private static readonly List<char> BlankChars = new List<char> {' ', '\t', '\n', '\r'};
        
        public static IEnumerable<Token> Tokenize(ICommonString input)
        {
            if (input == null) yield break;

            Token ComputeToken(int start, int currentPos, bool isBlank)
            {
                var tokenContent = input.Substring(start, currentPos - start);
                return isBlank
                    ? (Token) new BlankSpace(start, tokenContent)
                    : new Word(start, tokenContent);
            }

            var sourceLength = input.Length();

            var currentStart = 0;

            var currentIsBlank = true;

            for (var i = 0; i < sourceLength; i++)
            {
                var isBlank = BlankChars.Contains(input[i]);

                if (isBlank == currentIsBlank) continue;

                if (i > currentStart)
                    yield return ComputeToken(currentStart, i, currentIsBlank);

                currentIsBlank = isBlank;
                currentStart = i;
            }

            if (sourceLength > currentStart)
                yield return ComputeToken(currentStart, sourceLength, currentIsBlank);
        }
        
        public static int NaturalDividerPosition(ICommonString text, int startingPos, bool forward)
        {
            var prevNatural = -1;
            var breakAtNext = false;
            
            foreach (var token in Token.Tokenize(text))
            {
                if (token is Word)
                {
                    if ((breakAtNext || token.BreaksBefore()) && token.Start > 0)
                    {
                        if (forward)
                        {
                            if (token.Start > startingPos)
                            {
                                return token.Start;
                            }                            
                        }
                        else
                        {
                            if (token.Start >= startingPos)
                            {
                                return prevNatural;
                            }                            
                        }

                        prevNatural = token.Start;
                    }

                    breakAtNext = token.BreaksAfter();
                }
                else
                {
                    if (token.BreaksAfter())
                    {
                        breakAtNext = true;
                    }
                }
            }

            return -1;
        }
    }
}