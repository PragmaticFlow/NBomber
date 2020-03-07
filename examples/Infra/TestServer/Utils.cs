using System;
using System.Text;
using Newtonsoft.Json;

namespace TestServer.Utils
{
    public static class MsgConverter
    {
        public static ArraySegment<byte> ToJsonByteArray(object msg)
        {
            var json = JsonConvert.SerializeObject(msg);
            return new ArraySegment<byte>(Encoding.ASCII.GetBytes(json));
        }

        public static T FromJsonByteArray<T>(byte[] msg)
        {
            var json = Encoding.ASCII.GetString(msg);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
