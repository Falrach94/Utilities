using System.Text.RegularExpressions;
using TextParser.Parser.Tokenizer;

namespace TextParserUtils.Parser.Tokenizer
{
    public class TokenInfo<T>
    {
        public Regex Regex { get; }
        public T Type { get; }

        public TokenInfo(T type, string regex, bool caseSensitive)
        {
            if (!caseSensitive)
            {
                Regex = new Regex(regex, RegexOptions.IgnoreCase);
            }
            else
            {
                Regex = new Regex(regex);
            }
            Type = type;
        }

        public TokenMatch<T> Match(string txt)
        {
            var match = Regex.Match(txt);

            if (!match.Success)
            {
                return TokenMatch<T>.NoMatch;
            }


            return new TokenMatch<T>(txt.Substring(match.Length), match.Value, Type);
        }
    }
}
