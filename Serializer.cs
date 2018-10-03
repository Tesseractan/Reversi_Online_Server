using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Reversi_Online_Server_1._1
{
    class Serializer
    {
        public byte[] Serialize(object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
        public object Deserialize(byte[] data)
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(data, 0, data.Length);
                    //memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.Position = 0;
                    return binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
