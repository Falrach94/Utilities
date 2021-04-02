using GameServerData;
using MessageUtilities;
using System.Collections.Generic;
using TextParser.Parser.Exceptions;
using TextParser.Parser.Tokenizer;

namespace TextParser.Parser
{
    public class ParserTreeNode<T>
    {
        private T _command;

        public ParserSequence SequenceItem { get; set; }

        public int Rank { get; }
        public bool IsValidEnd => _command != null;
        public bool IsLeaf => _childrenDic.Count == 0;

        private readonly Dictionary<string, ParserTreeNode<T>> _childrenDic = new Dictionary<string, ParserTreeNode<T>>();

        public ParserTreeNode(int rank)
        {
            Rank = rank;
        }

        public void FillCommandList(string command, List<string> list)
        {
            if (Rank > 1)
            {
                command += " ";
            }
            if (Rank != 0)
            {
                if (SequenceItem.Name != null)
                {
                    command += "{" + SequenceItem.Name + "}";
                }
                else
                {
                    command += SequenceItem.InnerToken.ToLower();
                }
            }

            if (IsValidEnd)
            {
                list.Add(command);
            }

            foreach (var c in _childrenDic.Values)
            {
                c.FillCommandList(command, list);
            }
        }

        public void InsertSequence(ParserSequence sequence, T command)
        {
            if (sequence == null)
            {
                if (_command != null)
                {
                    throw new SequenceAlreadyPresentException();
                }

                _command = command;
            }
            else
            {
                if (!_childrenDic.ContainsKey(sequence.InnerToken))
                {
                    _childrenDic.Add(sequence.InnerToken, new ParserTreeNode<T>(Rank + 1)
                    {
                        SequenceItem = sequence
                    });
                }
                try
                {
                    _childrenDic[sequence.InnerToken].InsertSequence(sequence.Next, command);
                }
                catch (SequenceAlreadyPresentException)
                {
                    throw new SequenceAlreadyPresentException(sequence.First);
                }
            }
        }

        public T ParseSequence(List<Token<string>> sequence, SerializableStorage context)
        {
            if (Rank == sequence.Count)
            {
                if (!IsValidEnd)
                {
                    throw new NoSuchSequenceFoundException();
                }
                return _command;
            }
            else
            {
                if (!_childrenDic.ContainsKey(sequence[Rank].Type))
                {
                    throw new NoSuchSequenceFoundException();
                }

                var nextNode = _childrenDic[sequence[Rank].Type];

                if (sequence[Rank].Type == "String")
                {
                    string txt = sequence[Rank].Value;
                    txt = txt.Substring(1, txt.Length - 2);
                    context.Add(nextNode.SequenceItem.Name, txt);
                }
                else if (sequence[Rank].Type == "Number")
                {
                    context.Add(nextNode.SequenceItem.Name, int.Parse(sequence[Rank].Value));
                }
                else if (sequence[Rank].Type == "Boolean")
                {
                    context.Add(nextNode.SequenceItem.Name, bool.Parse(sequence[Rank].Value));
                }

                return _childrenDic[sequence[Rank].Type].ParseSequence(sequence, context);
            }
        }
    }

}
