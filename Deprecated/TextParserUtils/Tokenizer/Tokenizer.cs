using System.Collections.Generic;
using System.Linq;
using TextParser.Parser.Tokenizer;
using TextParserUtils.Parser.Tokenizer;

namespace TextParser.Tokenizer
{
    public class Tokenizer<T>
    {
        public bool CaseSensitive { get; }

        private readonly IList<TokenInfo<T>> _tokens = new List<TokenInfo<T>>();

        public Tokenizer(bool caseSensitive)
        {
            CaseSensitive = caseSensitive;
        }

        public void AddToken(T type, string regex)
        {
            if (_tokens.Where(t => t.Regex.ToString() == regex && !t.Type.Equals(type)).Any())
            {
                throw new TokenDuplicateException("Token with regex '" + regex + "' already exists!");
            }
            if (_tokens.Where(t => t.Type.Equals(type) && t.Regex.ToString() != regex).Any())
            {
                throw new TokenDuplicateException("Token '" + type + "' already has a regex");
            }
            if (!_tokens.Where(t => t.Type.Equals(type) && t.Regex.ToString() == regex).Any())
            {
                _tokens.Add(new TokenInfo<T>(type, regex, CaseSensitive));
            }
        }

        private TokenMatch<T> FindMatch(string txt)
        {
            foreach (var token in _tokens)
            {
                var match = token.Match(txt);
                if (match.Success)
                {
                    return match;
                }
            }
            return TokenMatch<T>.NoMatch;
        }

        public List<Token<T>> Tokenize(string txt)
        {
            var result = new List<Token<T>>();


            while (txt.Length != 0)
            {
                var match = FindMatch(txt);
                if (match.Success)
                {
                    result.Add(new Token<T>(match.TokenType, match.Value));
                    txt = match.RemainingString;
                }
                else
                {
                    txt = txt.Substring(1);
                }
            }

            return result;
        }
    }
}
