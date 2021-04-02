using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MessageUtilities
{
    [Serializable]
    public class SerializableStorage
    {
        readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public void Add(string name, object data)
        {
            _data.Add(name, data);
        }

        public bool Contains(string name)
        {
            return _data.ContainsKey(name);
        }

        public T Get<T>(string name)
        {
            if (!Contains(name))
            {
                throw new ItemNotFoundException(name);
            }

            var obj = _data[name];

            if (!(obj is T))
            {
                throw new InvalidItemCastException(name, "Item '" + name + "' can not be cast to " + typeof(T));
            }

            return (T)_data[name];
        }

        public byte[] ToByteArray()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }
        public static SerializableStorage CreateFromByteArray(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (SerializableStorage)bf.Deserialize(ms);
            }
        }
    }
}
