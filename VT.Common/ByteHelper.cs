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
        /// ����
        /// </summary>
        /// <param name="buffer">byte����</param>
        /// <param name="ofs">��ʼλ��</param>
        /// <param name="len">���鳤��</param>
        /// <returns>����</returns>
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
        /// byte����ת16����
        /// </summary>
        /// <param name="buffer">byte����</param>
        /// <param name="len">���鳤��</param>
        /// <returns>16��������</returns>
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
        /// byte����ԭ���ַ������
        /// </summary>
        /// <param name="buffer">byte����</param>
        /// <param name="len">���鳤��</param>
        /// <returns>byte�����ַ���</returns>
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
        /// ��intת����4�ֽ�����
        /// </summary>
        /// <param name="len">int������</param>
        /// <returns>byte����</returns>
        public static byte[] ConvertMsgLength(int len)
        {
            byte[] seq = BitConverter.GetBytes(len);
            byte[] rev = seq.Reverse().ToArray();
            return rev;

        }

        /// <summary>
        /// ��shortת����4�ֽ�����
        /// </summary>
        /// <param name="len">short������</param>
        /// <returns>byte����</returns>
        public static byte[] ConvertMsgLength(short len)
        {
            byte[] seq = BitConverter.GetBytes(len);
            byte[] rev = seq.Reverse().ToArray();
            return rev;

        }

        /// <summary>
        /// �������ֽڵ�����תΪshort����
        /// </summary>
        /// <param name="buffer">byte����</param>
        /// <returns>short����</returns>
        public static short GetMsgLength(byte[] buffer)
        {
            return GetShort(buffer);
        }

        public static short GetShort(byte[] buffer)
        {
            return BitConverter.ToInt16(buffer.Reverse().ToArray(), 0);
        }

        /// <summary>
        /// ����Ϣ���ԣ���Ϣ�����У�����г��� 0x55 0x01 0x55 0x02��
        /// ������������ 0x55 0x01 ת��Ϊ 0xaa
        /// ������������ 0x55 0x02 ת��Ϊ 0x55
        /// </summary>
        /// <param name="bytes">��Ҫ�����byte����</param>
        /// <param name="len">���鳤��</param>
        /// <returns>����֮���byte����</returns>
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
        /// ����Ϣ���ԣ���Ϣ�����У�����г��� 0xaa 0x55��
        /// ������������ 0xaa ת��Ϊ 0x55 0x01
        /// ������������ 0x55 ת��Ϊ 0x55 0x02
        /// </summary>
        /// <param name="bytes">��Ҫ�����byte����</param>
        /// <returns>����֮�������</returns>
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