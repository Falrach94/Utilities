using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MessageUtilities
{
    [Serializable]
    public class Message
    {
        public bool IsResponse => ResponseId != -1;

        public string Type { get; set; }
        public int ResponseId { get; set; } = -1;
        public int Id { get; set; }
        public string Module { get; }

        public SerializableStorage Data { get; }

        public Message(string module, String type, SerializableStorage data)
        {
            Data = data;
            Module = module;
            Type = type.ToString();
        }

        public byte[] Serialize()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }
        public static Message Deserialize(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream(data))
            {
                return (Message)bf.Deserialize(ms);
            }
        }

        public override string ToString()
        {
            return "(" + Module + ", type: " + Type + /*"(" + Type + ")" +*/ ", id: " + Id + " responseId: " + ResponseId + ")";
        }
    }
}