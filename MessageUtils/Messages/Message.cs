using MessageUtils;
using NetworkUtils.Endpoint;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageUtilities
{
    [Serializable]
    public class Message
    {
        private static readonly MethodInfo _deserializeMethod;

        public bool IsResponse => ResponseId != -1;

        public string Type { get; set; }
        public int ResponseId { get; set; } = -1;
        public int Id { get; set; }
        public string Module { get; set; }

        public string DataType { get; set; }

        public object Data { get; set; }

        static Message()
        {
            var methods = typeof(JsonSerializer).GetMethods();
            _deserializeMethod = methods.Where(m => m.IsPublic
                                            && m.IsGenericMethod
                                            && m.Name.Equals("Deserialize")
                                            && m.GetParameters().First().ParameterType.Equals(typeof(string))).First();
        }

        public Message() { }
        public Message(Message msg)
        {
            Module = msg.Module;
            Data = msg.Data;
            Id = msg.Id;
            Type = msg.Type;
            ResponseId = msg.ResponseId;
        }

        public Message(string module, string type, object data)
        {
            Data = data;
            Module = module;
            Type = type.ToString();
        }

        public T GetData<T>()
        {
            return (T)Data;
        }

        public override string ToString()
        {
            return "(" + Module + ", type: " + Type + /*"(" + Type + ")" +*/ ", id: " + Id + " responseId: " + ResponseId + ")";
        }

        public async static Task<Message> FromRawMessageAsync(RawMessage rawMessage)
        {
            return await DeserializeAsync(rawMessage.Data);
        }

        public async Task<byte[]> SerializeAsync()
        {
            using var stream = new MemoryStream();

            if (Data != null)
            {
                DataType = Data.GetType().ToString();
            }
            await JsonSerializer.SerializeAsync(stream, this);
            return stream.ToArray();
        }
        public byte[] Serialize()
        {
            var task = SerializeAsync();
            task.Wait();
            return task.Result;
        }
        public async static Task<Message> DeserializeAsync(byte[] ar)
        {
            using var stream = new MemoryStream(ar);
            Message msg = await JsonSerializer.DeserializeAsync<Message>(stream);

            //
            // deserialize msg.Data
            //
            if (msg.Data != null)
            {
                var dataJsonElement = (JsonElement)msg.Data;
                Type type = System.Type.GetType(msg.DataType);
                var genericDeserializeMethod = _deserializeMethod.MakeGenericMethod(type);
                msg.Data = genericDeserializeMethod.Invoke(null, new[] { dataJsonElement.GetRawText(), null });
            }
            
            return msg;
        }

        public override bool Equals(object obj)
        {
            if(obj == this)
            {
                return true;
            }
            if(obj is Message msg)
            {
                if(msg.Id == Id 
                && msg.Type == Type 
                && ResponseId == msg.ResponseId 
                && msg.Module == Module)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + Type.GetHashCode() + ResponseId.GetHashCode() + Module.GetHashCode();
        }
    }
}