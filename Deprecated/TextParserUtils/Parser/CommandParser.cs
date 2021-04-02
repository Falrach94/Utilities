using GameServerData;
using MessageUtilities;
using System.Collections.Generic;
using TextParser.Parser;
using TextParser.Tokenizer;

namespace TextParserUtils.Parser
{
    public delegate string CommandDelegate(SerializableStorage context);

    public class CommandParser
    {
        public Tokenizer<string> Tokenizer { get; }

        private readonly List<string> _examples = new List<string>();

        private readonly ParserTreeNode<CommandDelegate> _root = new ParserTreeNode<CommandDelegate>(0);

        public string[] Examples
        {
            get
            {
                List<string> commands = new List<string>();
                _root.FillCommandList("", commands);
                return commands.ToArray();
            }
        }
        // => _examples.ToArray();

        public CommandParser(Tokenizer<string> tokenizer)
        {
            Tokenizer = tokenizer;

            Tokenizer.AddToken("Number", "^[0-9]+");
            Tokenizer.AddToken("String", "^\"(.*?)\"");
            Tokenizer.AddToken("Boolean", "^(true)|(false)");
        }


        public void AddRule(ParserRule<CommandDelegate> rule)
        {
            if (rule.Example != null)
            {
                _examples.Add(rule.Example);
            }
            _root.InsertSequence(rule.Sequence, rule.Command);
        }

        public ParserResult<CommandDelegate> ParseCommand(string txt)
        {
            var tokens = Tokenizer.Tokenize(txt);
            try
            {
                var context = new SerializableStorage();
                var result = _root.ParseSequence(tokens, context);
                return new ParserResult<CommandDelegate>(true, result, context);
            }
            catch
            {
                return new ParserResult<CommandDelegate>(false, null, null);
            }
        }
    }
}
