using System;
using System.Collections.Generic;
namespace VT.DCM.Server
{
    public class Helper
    {
        //流水号
        private static byte threadNO = 0x0;

        private static object obj = new object();

        /// <summary>
        /// 取消息流水号
        /// </summary>
        /// <returns>流水号</returns>
        public static byte GetThreadNO()
        {
            lock (obj)
            {
                threadNO++;
            }
            return threadNO;
        }

        /// <summary>
        /// 10进制数据16进制编码
        /// </summary>
        /// <param name="dec">10进制数</param>
        /// <returns>编码之后的16进制数</returns>
        public static byte Hcd(int dec)
        {
            byte b = (byte)0;
            string s = dec.ToString();
            double m = 0.0;
            for (int x = s.Length - 1; x > -1; x--)
            {
                int z = Convert.ToInt32(s[x].ToString());
                b += (byte)(z * Math.Pow(16.0, m));
                m += 1.0;
            }
            return b;
        }

        public List<Message> SplitCommand(byte[] bytes)
        {
            List<Message> cmds = new List<Message>();

            List<byte[]> lst = new List<byte[]>();

            int startIndex = 0;
            //byte[] bytes = { 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 250, 0, 34, 0, 4, 19, 17, 23, 19, 21, 36, 0, 0, 15, 8, 16, 64, 55, 150, 48, 49, 17, 144, 47, 14, 21, 68, 0, 6, 4, 23, 0, 0, 0, 0, 0, 0, 97, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 251, 0, 34, 0, 4, 19, 17, 23, 19, 21, 53, 0, 0, 15, 7, 16, 64, 56, 17, 48, 49, 7, 101, 82, 34, 34, 69, 0, 6, 5, 253, 0, 0, 0, 0, 0, 0, 153, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 252, 0, 56, 0, 8, 19, 17, 23, 19, 21, 80, 60, 158, 2, 0, 71, 91, 7, 181, 0, 120, 54, 0, 0, 39, 0, 0, 0, 0, 0, 0, 2, 238, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 157, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 253, 0, 34, 0, 4, 19, 17, 23, 19, 21, 82, 0, 0, 15, 8, 16, 64, 56, 41, 48, 48, 147, 54, 87, 66, 66, 71, 0, 6, 8, 46, 0, 4, 0, 0, 1, 4, 209, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 254, 0, 34, 0, 4, 19, 17, 23, 19, 22, 7, 0, 0, 15, 9, 16, 64, 56, 70, 48, 48, 128, 2, 86, 51, 52, 73, 0, 6, 3, 47, 0, 5, 0, 0, 2, 5, 204, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 255, 0, 34, 0, 4, 19, 17, 23, 19, 22, 25, 8, 0, 15, 9, 16, 64, 56, 86, 48, 48, 115, 65, 83, 24, 28, 74, 0, 6, 3, 75, 0, 5, 0, 0, 2, 5, 26, 170 };
            for (int i = 0; i < bytes.Length; i++)
            {
                byte bc = bytes[i];
                byte bn = 0;
                if (i + 1 == bytes.Length)
                {
                    bn = 170;
                }
                else
                {
                    bn = bytes[i + 1];
                }

                if (bc == 170)
                {
                    if (bn != 170)
                    {
                        startIndex = i;
                    }

                    if (bn == 170)
                    {
                        byte[] bs = new byte[i - startIndex + 1];
                        Array.Copy(bytes, startIndex, bs, 0, bs.Length);
                        lst.Add(bs);
                    }
                }
            }
            return cmds;
        }
    }
}