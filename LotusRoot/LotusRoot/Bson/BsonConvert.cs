using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.Bson
{
    public static class BsonConvert
    {
        public static byte[] SerializeObject(Object obj)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (BsonWriter writer = new BsonWriter(memory))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, obj);
                }
                return memory.ToArray();
            }
        }

        public static T DeserializeObject<T>(byte[] bson)
        {
            using (MemoryStream memory = new MemoryStream(bson))
            {
                using (BsonReader reader = new BsonReader(memory, typeof(T).IsArray, DateTimeKind.Local))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(reader);
                };
            }
        }
    }
}
