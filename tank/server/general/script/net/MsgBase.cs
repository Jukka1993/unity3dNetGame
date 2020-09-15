using System.Collections;
using System.Collections.Generic;
using System;
//using System.Text.Json;
using Newtonsoft.Json;
using general.script.proto;
//using System.Web.Script.Serialization;
//using System.Runtime.Serialization.Json;
namespace general.script.net
{
    public class ttClass
    {
        public int a = 0;
        public string xx = "";
    }


    public class MsgBase
    {

        public string protoName = "";

        public static byte[] Encode(MsgBase msgBase)
        {
            MsgBase tt = msgBase;
            string s = JsonConvert.SerializeObject(tt);
            return System.Text.Encoding.UTF8.GetBytes(s);
        }
        public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
        {
            MsgSaveText xx = new MsgSaveText();
            string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            string typeFullName = "general.script.proto." + protoName;
            Console.WriteLine("typeFullName " + typeFullName);
            Console.WriteLine("Type.Tostring "+Type.GetType(typeFullName).ToString());
            MsgBase msgBase = null;
            try
            {
                msgBase = (MsgBase)JsonConvert.DeserializeObject(s, Type.GetType(typeFullName));
            } catch(Exception e)
            {
                Console.WriteLine("DeserializeObject error " + e.ToString());
                Console.WriteLine("\n");
            }
            //MsgBase msgBase = (MsgBase)JsonSerializer.Deserialize(s,Type.GetType(protoName));
            ////MsgBase msgBase = (MsgBase)JsonUtility.FromJson(s, Type.GetType(protoName));
            return msgBase;
        }
        public static byte[] EncodeName(MsgBase msgBase)
        {
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
            Int16 len = (Int16)nameBytes.Length;
            byte[] bytes = new byte[len + 2];
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            if (offset + 2 > bytes.Length)
            {
                Console.WriteLine(" Decode Name fail 1");
                return "";
            }
            Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
            if (offset + 2 + len > bytes.Length)
            {
                Console.WriteLine(" Decode Name fail 2");
                //string name1 = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
                string name1 = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                Console.WriteLine("name1 = " + name1);

                return "";
            }
            count = 2 + len;
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
            if(name == "")
            {
                Console.WriteLine(" Decode Name fail 3");
            }
            return name;
        }
    }
}
