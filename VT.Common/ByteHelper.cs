using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace VT.Common
{
    public class ByteHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ByteHelper));

        /// <summary>
        /// 异或和
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="ofs">开始位置</param>
        /// <param name="len">数组长度</param>
        /// <returns>异或和</returns>
        public static byte Xor(byte[] buffer, int ofs, int len)
        {
            byte result = buffer[ofs];

            for (int index = ofs + 1; index < ofs + len; index++)
            {
                result = (byte)(result ^ buffer[index]);
            }

            return result;
        }

        /// <summary>
        /// byte数组转16进制
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="len">数组长度</param>
        /// <returns>16进制数据</returns>
        public static string BytesToHexString(byte[] buffer, int len)
        {
            StringBuilder sbHex = new StringBuilder();

            for (int i = 0; i < len; i++)
            {
                int v = buffer[i] & 0xFF;
                string hv = v.ToString("X2");
                sbHex.Append(hv);
                sbHex.Append(" ");
            }

            return sbHex.ToString();
        }

        /// <summary>
        /// byte数组原样字符串输出
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="len">数组长度</param>
        /// <returns>byte数组字符串</returns>
        public static String BytesToString(byte[] buffer, int len)
        {
            StringBuilder sb = new StringBuilder();
            if (buffer.Length > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    sb.Append(buffer[i]);
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将int转化成4字节数组
        /// </summary>
        /// <param name="len">int型数据</param>
        /// <returns>byte数组</returns>
        public static byte[] ConvertMsgLength(int len)
        {
            byte[] seq = BitConverter.GetBytes(len);
            byte[] rev = seq.Reverse().ToArray();
            return rev;

        }

        /// <summary>
        /// 将short转化成4字节数组
        /// </summary>
        /// <param name="len">short型数据</param>
        /// <returns>byte数组</returns>
        public static byte[] ConvertMsgLength(short len)
        {
            byte[] seq = BitConverter.GetBytes(len);
            byte[] rev = seq.Reverse().ToArray();
            return rev;

        }

        /// <summary>
        /// 将两个字节的数组转为short数据
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <returns>short数据</returns>
        public static short GetMsgLength(byte[] buffer)
        {
            return GetShort(buffer);
        }

        public static short GetShort(byte[] buffer)
        {
            return BitConverter.ToInt16(buffer.Reverse().ToArray(), 0);
        }

        /// <summary>
        /// 若消息属性，消息体或者校验码中出现 0x55 0x01 0x55 0x02：
        /// 若数据中遇到 0x55 0x01 转义为 0xaa
        /// 若数据中遇到 0x55 0x02 转义为 0x55
        /// </summary>
        /// <param name="bytes">需要解码的byte数组</param>
        /// <param name="len">数组长度</param>
        /// <returns>解码之后的byte数组</returns>
        public static byte[] DecodeBytes(byte[] bytes, int len)
        {
            log.Debug(string.Format("-> Len : [{0}]", len));
            log.Debug(string.Format("-> Receive buffer : [{0}]", ByteHelper.BytesToString(bytes, len)));
            byte[] buffer = new byte[len];
            Array.Copy(bytes, 0, buffer, 0, len);

            List<byte> lst = new List<byte>(buffer);
            for (int i = 1; i < lst.Count - 1; i++)
            {
                byte b = lst[i];
                if (b == 85)
                {
                    byte n = lst[i + 1];
                    if (n == 1)
                    {
                        lst[i] = 170;
                    }

                    lst.RemoveAt(i + 1);
                }
            }
            return lst.ToArray();
        }

        /// <summary>
        /// 若消息属性，消息体或者校验码中出现 0xaa 0x55：
        /// 若数据中遇到 0xaa 转义为 0x55 0x01
        /// 若数据中遇到 0x55 转义为 0x55 0x02
        /// </summary>
        /// <param name="bytes">需要编码的byte数组</param>
        /// <returns>编码之后的数组</returns>
        public static byte[] EncodeBytes(byte[] bytes)
        {
            //byte[] bytes = { 85, 11, 22, 3, 8, 5, 6, 7, 8, 2, 3, 85, 30 };
            List<byte> lst = new List<byte>(bytes);
            for (int i = 1; i < lst.Count - 1; i++)
            {
                byte b = lst[i];
                if (b == 85)   //0x55
                {
                    lst.Insert(i + 1, 2);
                }
                if (b == 170)  //0xaa
                {
                    lst[i] = 85;
                    lst.Insert(i + 1, 1);
                }
            }
            
            return lst.ToArray();
        }
    }
}