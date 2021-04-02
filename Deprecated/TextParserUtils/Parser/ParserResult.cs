using GameServerData;
using MessageUtilities;

namespace TextParser.Parser
{
    public struct ParserResult<T>
    {
        public ParserResult(bool success, T value, SerializableStorage context)
        {
            Success = success;
            Context = context;
            Value = value;
        }

        public bool Success { get; }
        public SerializableStorage Context { get; }
        public T Value { get; }
    }
}
